using ClinicPatient.Models;
using ClinicPatient.Repositories;
using ClinicPatient.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicPatient.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PatientsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public PatientsController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        // GET: Patients
        public async Task<IActionResult> Index()
        {
            var patients = await _unitOfWork.Patients.GetAsync(includeProperties: "User");
            var viewModels = patients.Select(p => new PatientViewModel
            {
                Id = p.Id,
                FullName = p.User.FullName,
                Email = p.User.Email,
                PhoneNumber = p.User.PhoneNumber,
                DateOfBirth = p.DateOfBirth ?? DateTime.MinValue,
                MedicalHistory = p.MedicalHistory
            }).ToList();

            return View(viewModels);
        }

        // GET: Patients/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var patient = await _unitOfWork.Patients.GetFirstOrDefaultAsync(
                p => p.Id == id,
                includeProperties: "User");

            if (patient == null)
            {
                return NotFound();
            }

            var viewModel = new PatientViewModel
            {
                Id = patient.Id,
                FullName = patient.User.FullName,
                Email = patient.User.Email,
                PhoneNumber = patient.User.PhoneNumber,
                DateOfBirth = patient.DateOfBirth ?? DateTime.MinValue,
                MedicalHistory = patient.MedicalHistory
            };

            return View(viewModel);
        }

        // GET: Patients/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var patient = await _unitOfWork.Patients.GetFirstOrDefaultAsync(
                p => p.Id == id,
                includeProperties: "User");

            if (patient == null)
            {
                return NotFound();
            }

            var viewModel = new PatientViewModel
            {
                Id = patient.Id,
                FullName = patient.User.FullName,
                Email = patient.User.Email,
                PhoneNumber = patient.User.PhoneNumber,
                DateOfBirth = patient.DateOfBirth ?? DateTime.MinValue,
                MedicalHistory = patient.MedicalHistory
            };

            return View(viewModel);
        }

        // POST: Patients/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PatientViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var patient = await _unitOfWork.Patients.GetFirstOrDefaultAsync(
                    p => p.Id == id,
                    includeProperties: "User");

                if (patient == null)
                {
                    return NotFound();
                }

                // Update user information
                patient.User.FullName = model.FullName;
                patient.User.PhoneNumber = model.PhoneNumber;

                var userResult = await _userManager.UpdateAsync(patient.User);
                if (!userResult.Succeeded)
                {
                    foreach (var error in userResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }

                // Update patient information
                patient.DateOfBirth = model.DateOfBirth;
                patient.MedicalHistory = model.MedicalHistory;

                await _unitOfWork.Patients.UpdateAsync(patient);

                TempData["Success"] = "Patient updated successfully";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Patients/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var patient = await _unitOfWork.Patients.GetFirstOrDefaultAsync(
                p => p.Id == id,
                includeProperties: "User");

            if (patient == null)
            {
                return NotFound();
            }

            var viewModel = new PatientViewModel
            {
                Id = patient.Id,
                FullName = patient.User.FullName,
                Email = patient.User.Email,
                PhoneNumber = patient.User.PhoneNumber,
                DateOfBirth = patient.DateOfBirth ?? DateTime.MinValue,
                MedicalHistory = patient.MedicalHistory
            };

            return View(viewModel);
        }

        // POST: Patients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patient = await _unitOfWork.Patients.GetFirstOrDefaultAsync(
                p => p.Id == id,
                includeProperties: "User");

            if (patient == null)
            {
                return NotFound();
            }

            // Delete the patient profile
            await _unitOfWork.Patients.RemoveAsync(patient);

            // Delete the user account
            var user = await _userManager.FindByIdAsync(patient.UserId);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }

            TempData["Success"] = "Patient deleted successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}