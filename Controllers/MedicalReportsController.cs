using ClinicPatient.Models;
using ClinicPatient.Repositories;
using ClinicPatient.Services;
using ClinicPatient.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicPatient.Controllers
{
    [Authorize]
    public class MedicalReportsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificationService _notificationService;

        public MedicalReportsController(
        IUnitOfWork unitOfWork,
        UserManager<ApplicationUser> userManager,
        INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _notificationService = notificationService;
        }

        // GET: MedicalReports
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            if (user.UserType == "Patient")
            {
                var patient = await _unitOfWork.Patients.GetFirstOrDefaultAsync(p => p.UserId == user.Id);
                if (patient == null)
                {
                    return NotFound();
                }

                var reports = await _unitOfWork.MedicalReports.GetAsync(
                m => m.PatientId == patient.Id,
                q => q.OrderByDescending(m => m.CreatedAt),
                "Doctor.User");

                var viewModels = reports.Select(m => new MedicalReportViewModel
                {
                    Id = m.Id,
                    DoctorId = m.DoctorId,
                    DoctorName = m.Doctor.User.FullName,
                    PatientId = m.PatientId,
                    AppointmentId = m.AppointmentId,
                    Diagnosis = m.Diagnosis,
                    Treatment = m.Treatment,
                    Prescription = m.Prescription,
                    Notes = m.Notes,
                    CreatedAt = m.CreatedAt
                }).ToList();

                return View(viewModels);
            }
            else if (user.UserType == "Doctor")
            {
                var doctor = await _unitOfWork.Doctors.GetFirstOrDefaultAsync(d => d.UserId == user.Id);
                if (doctor == null)
                {
                    return NotFound();
                }

                var reports = await _unitOfWork.MedicalReports.GetAsync(
                m => m.DoctorId == doctor.Id,
                q => q.OrderByDescending(m => m.CreatedAt),
                "Patient.User");

                var viewModels = reports.Select(m => new MedicalReportViewModel
                {
                    Id = m.Id,
                    DoctorId = m.DoctorId,
                    PatientId = m.PatientId,
                    PatientName = m.Patient.User.FullName,
                    AppointmentId = m.AppointmentId,
                    Diagnosis = m.Diagnosis,
                    Treatment = m.Treatment,
                    Prescription = m.Prescription,
                    Notes = m.Notes,
                    CreatedAt = m.CreatedAt
                }).ToList();

                return View("DoctorReports", viewModels);
            }

            return RedirectToAction("Index", "Home");
        }

        // GET: MedicalReports/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            MedicalReportViewModel viewModel;

            if (user.UserType == "Patient")
            {
                var patient = await _unitOfWork.Patients.GetFirstOrDefaultAsync(p => p.UserId == user.Id);
                if (patient == null)
                {
                    return NotFound();
                }

                var report = await _unitOfWork.MedicalReports.GetFirstOrDefaultAsync(
                m => m.Id == id && m.PatientId == patient.Id,
                "Doctor.User,Appointment");

                if (report == null)
                {
                    return NotFound();
                }

                viewModel = new MedicalReportViewModel
                {
                    Id = report.Id,
                    DoctorId = report.DoctorId,
                    DoctorName = report.Doctor.User.FullName,
                    PatientId = report.PatientId,
                    AppointmentId = report.AppointmentId,
                    AppointmentDate = report.Appointment?.AppointmentDate,
                    Diagnosis = report.Diagnosis,
                    Treatment = report.Treatment,
                    Prescription = report.Prescription,
                    Notes = report.Notes,
                    CreatedAt = report.CreatedAt
                };
            }
            else if (user.UserType == "Doctor")
            {
                var doctor = await _unitOfWork.Doctors.GetFirstOrDefaultAsync(d => d.UserId == user.Id);
                if (doctor == null)
                {
                    return NotFound();
                }

                var report = await _unitOfWork.MedicalReports.GetFirstOrDefaultAsync(
                m => m.Id == id && m.DoctorId == doctor.Id,
                "Patient.User,Appointment");

                if (report == null)
                {
                    return NotFound();
                }

                viewModel = new MedicalReportViewModel
                {
                    Id = report.Id,
                    DoctorId = report.DoctorId,
                    PatientId = report.PatientId,
                    PatientName = report.Patient.User.FullName,
                    AppointmentId = report.AppointmentId,
                    AppointmentDate = report.Appointment?.AppointmentDate,
                    Diagnosis = report.Diagnosis,
                    Treatment = report.Treatment,
                    Prescription = report.Prescription,
                    Notes = report.Notes,
                    CreatedAt = report.CreatedAt
                };
            }
            else // Admin
            {
                var report = await _unitOfWork.MedicalReports.GetFirstOrDefaultAsync(
                m => m.Id == id,
                "Doctor.User,Patient.User,Appointment");

                if (report == null)
                {
                    return NotFound();
                }

                viewModel = new MedicalReportViewModel
                {
                    Id = report.Id,
                    DoctorId = report.DoctorId,
                    DoctorName = report.Doctor.User.FullName,
                    PatientId = report.PatientId,
                    PatientName = report.Patient.User.FullName,
                    AppointmentId = report.AppointmentId,
                    AppointmentDate = report.Appointment?.AppointmentDate,
                    Diagnosis = report.Diagnosis,
                    Treatment = report.Treatment,
                    Prescription = report.Prescription,
                    Notes = report.Notes,
                    CreatedAt = report.CreatedAt
                };
            }

            return View(viewModel);
        }

        // GET: MedicalReports/Create
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Create(int? appointmentId)
        {
            var user = await _userManager.GetUserAsync(User);
            var doctor = await _unitOfWork.Doctors.GetFirstOrDefaultAsync(d => d.UserId == user.Id);

            if (doctor == null)
            {
                return NotFound();
            }

            MedicalReportViewModel viewModel = new MedicalReportViewModel
            {
                DoctorId = doctor.Id,
                DoctorName = user.FullName
            };

            if (appointmentId.HasValue)
            {
                var appointment = await _unitOfWork.Appointments.GetFirstOrDefaultAsync(
                a => a.Id == appointmentId && a.DoctorId == doctor.Id,
                "Patient.User");

                if (appointment == null)
                {
                    return NotFound();
                }

                viewModel.PatientId = appointment.PatientId;
                viewModel.PatientName = appointment.Patient.User.FullName;
                viewModel.AppointmentId = appointment.Id;
                viewModel.AppointmentDate = appointment.AppointmentDate;
            }

            return View(viewModel);
        }

        // POST: MedicalReports/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Create(MedicalReportViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var doctor = await _unitOfWork.Doctors.GetFirstOrDefaultAsync(d => d.UserId == user.Id);

                if (doctor == null || doctor.Id != model.DoctorId)
                {
                    return NotFound();
                }

                var patient = await _unitOfWork.Patients.GetFirstOrDefaultAsync(
                p => p.Id == model.PatientId,
                "User");

                if (patient == null)
                {
                    return NotFound();
                }

                var report = new MedicalReport
                {
                    DoctorId = model.DoctorId,
                    PatientId = model.PatientId,
                    AppointmentId = model.AppointmentId,
                    Diagnosis = model.Diagnosis,
                    Treatment = model.Treatment,
                    Prescription = model.Prescription,
                    Notes = model.Notes,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                await _unitOfWork.MedicalReports.AddAsync(report);

                // If there's an associated appointment, update its status to "Completed"
                if (model.AppointmentId.HasValue)
                {
                    var appointment = await _unitOfWork.Appointments.GetByIdAsync(model.AppointmentId.Value);
                    if (appointment != null)
                    {
                        appointment.Status = "Completed";
                        appointment.UpdatedAt = DateTime.Now;
                        await _unitOfWork.Appointments.UpdateAsync(appointment);
                    }
                }

                // Send notification to patient
                await _notificationService.CreateNotificationAsync(
                patient.UserId,
                "New Medical Report",
                $"Dr. {user.FullName} has created a new medical report for you. You can view it now.",
                "Report",
                report.Id
                );

                TempData["Success"] = "Medical report created successfully.";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: MedicalReports/Edit/5
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var doctor = await _unitOfWork.Doctors.GetFirstOrDefaultAsync(d => d.UserId == user.Id);

            if (doctor == null)
            {
                return NotFound();
            }

            var report = await _unitOfWork.MedicalReports.GetFirstOrDefaultAsync(
            m => m.Id == id && m.DoctorId == doctor.Id,
            "Patient.User,Appointment");

            if (report == null)
            {
                return NotFound();
            }

            var viewModel = new MedicalReportViewModel
            {
                Id = report.Id,
                DoctorId = report.DoctorId,
                DoctorName = user.FullName,
                PatientId = report.PatientId,
                PatientName = report.Patient.User.FullName,
                AppointmentId = report.AppointmentId,
                AppointmentDate = report.Appointment?.AppointmentDate,
                Diagnosis = report.Diagnosis,
                Treatment = report.Treatment,
                Prescription = report.Prescription,
                Notes = report.Notes,
                CreatedAt = report.CreatedAt
            };

            return View(viewModel);
        }

        // POST: MedicalReports/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Edit(int id, MedicalReportViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var doctor = await _unitOfWork.Doctors.GetFirstOrDefaultAsync(d => d.UserId == user.Id);

                if (doctor == null || doctor.Id != model.DoctorId)
                {
                    return NotFound();
                }

                var report = await _unitOfWork.MedicalReports.GetFirstOrDefaultAsync(
                m => m.Id == id && m.DoctorId == doctor.Id,
                "Patient.User");

                if (report == null)
                {
                    return NotFound();
                }

                report.Diagnosis = model.Diagnosis;
                report.Treatment = model.Treatment;
                report.Prescription = model.Prescription;
                report.Notes = model.Notes;
                report.UpdatedAt = DateTime.Now;

                await _unitOfWork.MedicalReports.UpdateAsync(report);

                // Send notification to patient
                await _notificationService.CreateNotificationAsync(
                report.Patient.UserId,
                "Medical Report Updated",
                $"Dr. {user.FullName} has updated your medical report. You can view it now.",
                "Report",
                report.Id
                );

                TempData["Success"] = "Medical report updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }
    }
}