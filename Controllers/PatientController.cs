using ClinicPatient.Models;
using ClinicPatient.Repositories;
using ClinicPatient.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicPatient.Controllers
{
    [Authorize(Roles = "Patient")]
    public class PatientController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public PatientController(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        // GET: /Patient/Profile
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            var patient = await _unitOfWork.Patients.GetFirstOrDefaultAsync(p => p.UserId == user.Id, "User");

            if (patient == null)
                return NotFound();

            var model = new PatientViewModel
            {
                Id = patient.Id,
                FullName = patient.User.FullName,
                Email = patient.User.Email,
                PhoneNumber = patient.User.PhoneNumber,
                DateOfBirth = patient.DateOfBirth ?? DateTime.MinValue,
                MedicalHistory = patient.MedicalHistory
            };

            return View(model); // Views/Patient/Profile.cshtml
        }

        // POST: /Patient/Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(PatientViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            var patient = await _unitOfWork.Patients.GetFirstOrDefaultAsync(p => p.UserId == user.Id, "User");

            if (patient == null)
                return NotFound();

            // تحديث البيانات
            patient.User.FullName = model.FullName;
            patient.User.PhoneNumber = model.PhoneNumber;
            patient.DateOfBirth = model.DateOfBirth;
            patient.MedicalHistory = model.MedicalHistory;

            await _userManager.UpdateAsync(patient.User);
            await _unitOfWork.Patients.UpdateAsync(patient);

            TempData["Success"] = "تم تحديث الملف الشخصي بنجاح.";
            return RedirectToAction(nameof(Profile));
        }

        // GET: /Patient/Appointments
        public async Task<IActionResult> Appointments()
        {
            var user = await _userManager.GetUserAsync(User);
            var patient = await _unitOfWork.Patients.GetFirstOrDefaultAsync(p => p.UserId == user.Id);

            if (patient == null)
                return NotFound();

            var appointments = await _unitOfWork.Appointments.GetAsync(
                a => a.PatientId == patient.Id,
                q => q.OrderByDescending(a => a.AppointmentDate).ThenBy(a => a.StartTime),
                "Doctor.User");

            var viewModels = appointments.Select(a => new AppointmentViewModel
            {
                Id = a.Id,
                DoctorId = a.DoctorId,
                DoctorName = a.Doctor.User.FullName,
                AppointmentDate = a.AppointmentDate,
                StartTime = a.StartTime,
                EndTime = a.EndTime,
                Status = a.Status,
                Notes = a.Notes
            }).ToList();

            return View(viewModels); // Views/Patient/Appointments.cshtml
        }
    }
}
