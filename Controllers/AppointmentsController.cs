using ClinicPatient.Data;
using ClinicPatient.Models;
using ClinicPatient.Services;
using ClinicPatient.Repositories;
using ClinicPatient.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicPatient.Controllers
{
    [Authorize]
    public class AppointmentsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificationService _notificationService;
        private readonly ApplicationDbContext _context;

        public AppointmentsController(
            IUnitOfWork unitOfWork, 
            UserManager<ApplicationUser> userManager,
            INotificationService notificationService,
            ApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _context = context;
            _notificationService = notificationService;
        }

        // GET: Appointments
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            IEnumerable<Appointment> appointments;

            if (user.UserType == "Patient")
            {
                var patient = await _unitOfWork.Patients.GetFirstOrDefaultAsync(p => p.UserId == user.Id);
                if (patient == null)
                {
                    return NotFound();
                }

                appointments = await _unitOfWork.Appointments.GetAppointmentsByPatientIdAsync(patient.Id);
            }
            else if (user.UserType == "Doctor")
            {
                var doctor = await _unitOfWork.Doctors.GetFirstOrDefaultAsync(d => d.UserId == user.Id);
                if (doctor == null)
                {
                    return NotFound();
                }

                appointments = await _unitOfWork.Appointments.GetAppointmentsByDoctorIdAsync(doctor.Id);
            }
            else // Admin
            {
                appointments = await _unitOfWork.Appointments.GetAsync(includeProperties: "Doctor,Patient,Doctor.User,Patient.User");
            }

            var viewModels = appointments.Select(a => new AppointmentViewModel
            {
                Id = a.Id,
                PatientId = a.PatientId,
                PatientName = a.Patient.User.FullName,
                DoctorId = a.DoctorId,
                DoctorName = a.Doctor.User.FullName,
                DoctorSpecialization = a.Doctor.Specialization,
                AppointmentDate = a.AppointmentDate,
                Status = a.Status,
                Notes = a.Notes
            }).ToList();

            return View(viewModels);
        }

        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var appointment = await _unitOfWork.Appointments.GetFirstOrDefaultAsync(
                a => a.Id == id,
                includeProperties: "Doctor,Patient,Doctor.User,Patient.User");

            if (appointment == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Check if the user has permission to view this appointment
            if (user.UserType == "Patient")
            {
                var patient = await _unitOfWork.Patients.GetFirstOrDefaultAsync(p => p.UserId == user.Id);
                if (patient == null || patient.Id != appointment.PatientId)
                {
                    return Forbid();
                }
            }
            else if (user.UserType == "Doctor")
            {
                var doctor = await _unitOfWork.Doctors.GetFirstOrDefaultAsync(d => d.UserId == user.Id);
                if (doctor == null || doctor.Id != appointment.DoctorId)
                {
                    return Forbid();
                }
            }

            var viewModel = new AppointmentViewModel
            {
                Id = appointment.Id,
                PatientId = appointment.PatientId,
                PatientName = appointment.Patient.User.FullName,
                DoctorId = appointment.DoctorId,
                DoctorName = appointment.Doctor.User.FullName,
                DoctorSpecialization = appointment.Doctor.Specialization,
                AppointmentDate = appointment.AppointmentDate,
                Status = appointment.Status,
                Notes = appointment.Notes
            };

            return View(viewModel);
        }

        // GET: Appointments/Create
        [Authorize(Roles = "Patient,Admin")]
        public async Task<IActionResult> Create()
        {
            var doctors = await _unitOfWork.Doctors.GetAsync(includeProperties: "User");
            ViewBag.Doctors = new SelectList(doctors, "Id", "User.FullName");

            var user = await _userManager.GetUserAsync(User);
            if (user != null && user.UserType == "Patient")
            {
                var patient = await _unitOfWork.Patients.GetFirstOrDefaultAsync(p => p.UserId == user.Id);
                if (patient != null)
                {
                    ViewBag.PatientId = patient.Id;
                }
            }
            else if (user != null && user.UserType == "Admin")
            {
                var patients = await _unitOfWork.Patients.GetAsync(includeProperties: "User");
                ViewBag.Patients = new SelectList(patients, "Id", "User.FullName");
            }

            return View();
        }

        // POST: Appointments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Patient,Admin")]
        public async Task<IActionResult> Create(AppointmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if the time slot is available
                bool isAvailable = await _unitOfWork.Appointments.IsTimeSlotAvailableAsync(
                    model.DoctorId, model.AppointmentDate);

                if (!isAvailable)
                {
                    ModelState.AddModelError(string.Empty, "The selected time slot is not available. Please choose another time.");

                    var doctors = await _unitOfWork.Doctors.GetAsync(includeProperties: "User");
                    ViewBag.Doctors = new SelectList(doctors, "Id", "User.FullName", model.DoctorId);

                    var user = await _userManager.GetUserAsync(User);
                    if (user != null && user.UserType == "Patient")
                    {
                        var patient = await _unitOfWork.Patients.GetFirstOrDefaultAsync(p => p.UserId == user.Id);
                        if (patient != null)
                        {
                            ViewBag.PatientId = patient.Id;
                        }
                    }
                    else if (user != null && user.UserType == "Admin")
                    {
                        var patients = await _unitOfWork.Patients.GetAsync(includeProperties: "User");
                        ViewBag.Patients = new SelectList(patients, "Id", "User.FullName", model.PatientId);
                    }

                    return View(model);
                }

                var appointment = new Appointment
                {
                    PatientId = model.PatientId,
                    DoctorId = model.DoctorId,
                    AppointmentDate = model.AppointmentDate,
                    Status = "Pending",
                    Notes = model.Notes
                };

                await _unitOfWork.Appointments.AddAsync(appointment);

                TempData["Success"] = "Appointment created successfully";
                return RedirectToAction(nameof(Index));
            }

            var doctorsForView = await _unitOfWork.Doctors.GetAsync(includeProperties: "User");
            ViewBag.Doctors = new SelectList(doctorsForView, "Id", "User.FullName", model.DoctorId);

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null && currentUser.UserType == "Patient")
            {
                var patient = await _unitOfWork.Patients.GetFirstOrDefaultAsync(p => p.UserId == currentUser.Id);
                if (patient != null)
                {
                    ViewBag.PatientId = patient.Id;
                }
            }
            else if (currentUser != null && currentUser.UserType == "Admin")
            {
                var patients = await _unitOfWork.Patients.GetAsync(includeProperties: "User");
                ViewBag.Patients = new SelectList(patients, "Id", "User.FullName", model.PatientId);
            }

            return View(model);
        }

        // GET: Appointments/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var appointment = await _unitOfWork.Appointments.GetFirstOrDefaultAsync(
                a => a.Id == id,
                includeProperties: "Doctor,Patient,Doctor.User,Patient.User");

            if (appointment == null)
            {
                return NotFound();
            }

            var viewModel = new AppointmentViewModel
            {
                Id = appointment.Id,
                PatientId = appointment.PatientId,
                PatientName = appointment.Patient.User.FullName,
                DoctorId = appointment.DoctorId,
                DoctorName = appointment.Doctor.User.FullName,
                DoctorSpecialization = appointment.Doctor.Specialization,
                AppointmentDate = appointment.AppointmentDate,
                Status = appointment.Status,
                Notes = appointment.Notes
            };

            var doctors = await _unitOfWork.Doctors.GetAsync(includeProperties: "User");
            ViewBag.Doctors = new SelectList(doctors, "Id", "User.FullName", appointment.DoctorId);

            var patients = await _unitOfWork.Patients.GetAsync(includeProperties: "User");
            ViewBag.Patients = new SelectList(patients, "Id", "User.FullName", appointment.PatientId);

            ViewBag.Statuses = new SelectList(new[] { "Pending", "Confirmed", "Completed", "Cancelled" }, appointment.Status);

            return View(viewModel);
        }

        // POST: Appointments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, AppointmentViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var appointment = await _unitOfWork.Appointments.GetByIdAsync(id);
                if (appointment == null)
                {
                    return NotFound();
                }

                // Check if the time slot is available (only if date/doctor changed)
                if (appointment.AppointmentDate != model.AppointmentDate || appointment.DoctorId != model.DoctorId)
                {
                    bool isAvailable = await _unitOfWork.Appointments.IsTimeSlotAvailableAsync(
                        model.DoctorId, model.AppointmentDate);

                    if (!isAvailable)
                    {
                        ModelState.AddModelError(string.Empty, "The selected time slot is not available. Please choose another time.");

                        var doctors = await _unitOfWork.Doctors.GetAsync(includeProperties: "User");
                        ViewBag.Doctors = new SelectList(doctors, "Id", "User.FullName", model.DoctorId);

                        var patients = await _unitOfWork.Patients.GetAsync(includeProperties: "User");
                        ViewBag.Patients = new SelectList(patients, "Id", "User.FullName", model.PatientId);

                        ViewBag.Statuses = new SelectList(new[] { "Pending", "Confirmed", "Completed", "Cancelled" }, model.Status);

                        return View(model);
                    }
                }

                appointment.PatientId = model.PatientId;
                appointment.DoctorId = model.DoctorId;
                appointment.AppointmentDate = model.AppointmentDate;
                appointment.Status = model.Status;
                appointment.Notes = model.Notes;

                await _unitOfWork.Appointments.UpdateAsync(appointment);

                TempData["Success"] = "Appointment updated successfully";
                return RedirectToAction(nameof(Index));
            }

            var doctorsForView = await _unitOfWork.Doctors.GetAsync(includeProperties: "User");
            ViewBag.Doctors = new SelectList(doctorsForView, "Id", "User.FullName", model.DoctorId);

            var patientsForView = await _unitOfWork.Patients.GetAsync(includeProperties: "User");
            ViewBag.Patients = new SelectList(patientsForView, "Id", "User.FullName", model.PatientId);

            ViewBag.Statuses = new SelectList(new[] { "Pending", "Confirmed", "Completed", "Cancelled" }, model.Status);

            return View(model);
        }

        // POST: Appointments/UpdateStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var appointment = await _unitOfWork.Appointments.GetFirstOrDefaultAsync(
                a => a.Id == id,
                includeProperties: "Doctor,Patient,Doctor.User,Patient.User");

            if (appointment == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Check if the user has permission to update this appointment
            if (user.UserType == "Patient")
            {
                var patient = await _unitOfWork.Patients.GetFirstOrDefaultAsync(p => p.UserId == user.Id);
                if (patient == null || patient.Id != appointment.PatientId)
                {
                    return Forbid();
                }

                // Patients can only cancel their appointments
                if (status != "Cancelled")
                {
                    return Forbid();
                }
            }
            else if (user.UserType == "Doctor")
            {
                var doctor = await _unitOfWork.Doctors.GetFirstOrDefaultAsync(d => d.UserId == user.Id);
                if (doctor == null || doctor.Id != appointment.DoctorId)
                {
                    return Forbid();
                }

                // Doctors can confirm, complete, or cancel appointments
                if (status != "Confirmed" && status != "Completed" && status != "Cancelled")
                {
                    return Forbid();
                }
            }

            appointment.Status = status;
            await _unitOfWork.Appointments.UpdateAsync(appointment);

            TempData["Success"] = $"Appointment status updated to {status}";
            return RedirectToAction(nameof(Index));
        }

        // GET: Appointments/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var appointment = await _unitOfWork.Appointments.GetFirstOrDefaultAsync(
                a => a.Id == id,
                includeProperties: "Doctor,Patient,Doctor.User,Patient.User");

            if (appointment == null)
            {
                return NotFound();
            }

            var viewModel = new AppointmentViewModel
            {
                Id = appointment.Id,
                PatientId = appointment.PatientId,
                PatientName = appointment.Patient.User.FullName,
                DoctorId = appointment.DoctorId,
                DoctorName = appointment.Doctor.User.FullName,
                DoctorSpecialization = appointment.Doctor.Specialization,
                AppointmentDate = appointment.AppointmentDate,
                Status = appointment.Status,
                Notes = appointment.Notes
            };

            return View(viewModel);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _unitOfWork.Appointments.RemoveAsync(id);
            TempData["Success"] = "Appointment deleted successfully";
            return RedirectToAction(nameof(Index));
        }

        // GET: Appointments/GetAvailableTimeSlots
        [HttpGet]
        public async Task<IActionResult> GetAvailableTimeSlots(int doctorId, DateTime date)
        {
            var availableSlots = await _unitOfWork.DoctorAvailabilities.GetAvailableTimeSlotsAsync(doctorId, date);
            return Json(availableSlots);
        }

        // GET: Appointments/SearchByDate
        public async Task<IActionResult> SearchByDate(DateTime date)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            IEnumerable<Appointment> appointments;

            if (user.UserType == "Patient")
            {
                var patient = await _unitOfWork.Patients.GetFirstOrDefaultAsync(p => p.UserId == user.Id);
                if (patient == null)
                {
                    return NotFound();
                }

                appointments = await _unitOfWork.Appointments.GetAsync(
                    a => a.PatientId == patient.Id && a.AppointmentDate.Date == date.Date,
                    includeProperties: "Doctor,Patient,Doctor.User,Patient.User");
            }
            else if (user.UserType == "Doctor")
            {
                var doctor = await _unitOfWork.Doctors.GetFirstOrDefaultAsync(d => d.UserId == user.Id);
                if (doctor == null)
                {
                    return NotFound();
                }

                appointments = await _unitOfWork.Appointments.GetAsync(
                    a => a.DoctorId == doctor.Id && a.AppointmentDate.Date == date.Date,
                    includeProperties: "Doctor,Patient,Doctor.User,Patient.User");
            }
            else // Admin
            {
                appointments = await _unitOfWork.Appointments.GetAppointmentsByDateAsync(date);
            }

            var viewModels = appointments.Select(a => new AppointmentViewModel
            {
                Id = a.Id,
                PatientId = a.PatientId,
                PatientName = a.Patient.User.FullName,
                DoctorId = a.DoctorId,
                DoctorName = a.Doctor.User.FullName,
                DoctorSpecialization = a.Doctor.Specialization,
                AppointmentDate = a.AppointmentDate,
                Status = a.Status,
                Notes = a.Notes
            }).ToList();

            ViewData["SearchDate"] = date.ToString("yyyy-MM-dd");
            return View("Index", viewModels);
        }
    }
}