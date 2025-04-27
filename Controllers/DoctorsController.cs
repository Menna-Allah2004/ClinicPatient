using ClinicPatient.Data;
using ClinicPatient.Models;
using ClinicPatient.Repositories;
using ClinicPatient.Services;
using ClinicPatient.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicPatient.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificationService _notificationService;

        public DoctorsController(
            IUnitOfWork unitOfWork, 
            UserManager<ApplicationUser> userManager, 
            INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _notificationService = notificationService;
        }

        // GET: Doctors
        public async Task<IActionResult> Index(string searchTerm, string specialization)
        {
            var doctors = await _unitOfWork.Doctors.GetAsync(d => d.User.IsApproved, null, "User");

            if (!string.IsNullOrEmpty(searchTerm))
            {
                doctors = doctors.Where(d => d.User.FullName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(specialization))
            {
                doctors = doctors.Where(d => d.Specialty == specialization);
            }

            var viewModels = doctors.Select(d => new DoctorViewModel
            {
                Id = d.Id,
                FullName = d.User.FullName,
                Email = d.User.Email,
                PhoneNumber = d.User.PhoneNumber,
                Specialization = d.Specialty,
                ExperienceYears = d.Experience,
                Bio = d.Bio,
                Rating = d.Rating ?? 0m,
                RatingCount = d.RatingCount,
                ConsultationFee = d.ConsultationFee,
                ImageUrl = d.ImageUrl ?? "/images/default-doctor.png"
            }).ToList();

            var specializations = await _unitOfWork.Doctors.GetAsync();
            var specializationList = specializations.Select(d => d.Specialty).Distinct().ToList();

            var listViewModel = new DoctorsListViewModel
            {
                Doctors = viewModels,
                SearchTerm = searchTerm,
                Specialization = specialization,
                Specializations = specializationList
            };

            return View(listViewModel);
        }

        // GET: Doctors/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var doctor = await _unitOfWork.Doctors.GetFirstOrDefaultAsync(
                d => d.Id == id && d.User.IsApproved,
                includeProperties: "User,AvailableSlots");

            if (doctor == null)
            {
                return NotFound();
            }

            var viewModel = new DoctorViewModel
            {
                Id = doctor.Id,
                FullName = doctor.User.FullName,
                Email = doctor.User.Email,
                PhoneNumber = doctor.User.PhoneNumber,
                Bio = doctor.Bio,
                Education = doctor.Education,
                Specialization = doctor.Specialty,
                ExperienceYears = doctor.Experience,
                Rating = doctor.Rating ?? 0m,
                RatingCount = doctor.RatingCount,
                ConsultationFee = doctor.ConsultationFee,
                ImageUrl = doctor.ImageUrl ?? "/images/default-doctor.png",
                Availabilities = doctor.Availabilities?.ToList()
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var doctor = await _unitOfWork.Doctors.GetFirstOrDefaultAsync(d => d.UserId == user.Id, "Appointments.Patient.User");

            if (doctor == null)
            {
                return NotFound();
            }

            var greeting = GetGreeting();
            var currentDate = DateTime.Now;

            // Get patient count (distinct patients)
            var patientCount = doctor.Appointments
            .Where(a => a.Status == "Completed")
            .Select(a => a.PatientId)
            .Distinct()
            .Count();

            // Get consultation count
            var consultationCount = doctor.Appointments
            .Count(a => a.Status == "Completed");

            // Get today's appointments
            var todayAppointments = doctor.Appointments
            .Where(a => a.AppointmentDate.Date == currentDate.Date)
            .OrderBy(a => a.StartTime)
            .Select(a => new AppointmentViewModel
            {
                Id = a.Id,
                PatientId = a.PatientId,
                PatientName = a.Patient.User.FullName,
                AppointmentDate = a.AppointmentDate,
                StartTime = a.StartTime,
                EndTime = a.EndTime,
                Status = a.Status,
                Notes = a.Notes
            })
            .ToList();

            // Get upcoming appointments
            var upcomingAppointments = doctor.Appointments
            .Where(a =>
            (a.AppointmentDate.Date > currentDate.Date ||
            (a.AppointmentDate.Date == currentDate.Date && a.StartTime > currentDate.TimeOfDay)) &&
            a.Status != "Cancelled")
            .OrderBy(a => a.AppointmentDate)
            .ThenBy(a => a.StartTime)
            .Select(a => new AppointmentViewModel
            {
                Id = a.Id,
                PatientId = a.PatientId,
                PatientName = a.Patient.User.FullName,
                AppointmentDate = a.AppointmentDate,
                StartTime = a.StartTime,
                EndTime = a.EndTime,
                Status = a.Status,
                Notes = a.Notes
            })
            .Take(10)
            .ToList();

            // Get completed appointments
            var completedAppointments = doctor.Appointments
            .Where(a => a.Status == "Completed")
            .OrderByDescending(a => a.AppointmentDate)
            .ThenBy(a => a.StartTime)
            .Select(a => new AppointmentViewModel
            {
                Id = a.Id,
                PatientId = a.PatientId,
                PatientName = a.Patient.User.FullName,
                AppointmentDate = a.AppointmentDate,
                StartTime = a.StartTime,
                EndTime = a.EndTime,
                Status = a.Status,
                Notes = a.Notes
            })
            .Take(10)
            .ToList();

            // Get patients
            var patients = await _unitOfWork.Patients.GetAsync(
            p => doctor.Appointments.Any(a => a.PatientId == p.Id && a.Status == "Completed"), null, "User");

            var patientViewModels = patients.Select(p => new PatientViewModel
            {
                Id = p.Id,
                FullName = p.User.FullName,
                Gender = p.Gender,
                PhoneNumber = p.User.PhoneNumber,
                Email = p.User.Email,
                // Get last visit date
                DateOfBirth = doctor.Appointments
            .Where(a => a.PatientId == p.Id && a.Status == "Completed")
            .OrderByDescending(a => a.AppointmentDate)
            .Select(a => a.AppointmentDate)
            .FirstOrDefault()
            })
            .OrderByDescending(p => p.DateOfBirth)
            .Take(10)
            .ToList();

            // Calculate completed tasks percentage
            var totalTodayTasks = todayAppointments.Count;
            var completedTodayTasks = todayAppointments.Count(a => a.Status == "Completed");
            var completedTasksPercentage = totalTodayTasks > 0
            ? (int)Math.Round((double)completedTodayTasks / totalTodayTasks * 100)
            : 0;

            var viewModel = new DoctorDashboardViewModel
            {
                DoctorName = user.FullName,
                Greeting = greeting,
                CurrentDate = currentDate,
                PatientCount = patientCount,
                ConsultationCount = consultationCount,
                CompletedTasksPercentage = completedTasksPercentage,
                TodayAppointments = todayAppointments,
                UpcomingAppointments = upcomingAppointments,
                CompletedAppointments = completedAppointments,
                Patients = patientViewModels
            };

            return View(viewModel);
        }

        private string GetGreeting()
        {
            var hour = DateTime.Now.Hour;
            if (hour < 12)
                return "Good Morning";
            else if (hour < 18)
                return "Good Afternoon";
            else
                return "Good Evening";
        }

        [Authorize(Roles = "Doctor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAppointmentStatus(int id, string status)
        {
            var user = await _userManager.GetUserAsync(User);
            var doctor = await _unitOfWork.Doctors.GetFirstOrDefaultAsync(d => d.UserId == user.Id);

            if (doctor == null)
            {
                return NotFound();
            }

            var appointment = await _unitOfWork.Appointments.GetFirstOrDefaultAsync(
            a => a.Id == id && a.DoctorId == doctor.Id,
            "Patient.User");

            if (appointment == null)
            {
                return NotFound();
            }

            appointment.Status = status;
            appointment.UpdatedAt = DateTime.Now;
            await _unitOfWork.Appointments.UpdateAsync(appointment);

            // Send notification to patient
            string message = status == "Confirmed"
            ? $"Your appointment with Dr. {user.FullName} on {appointment.AppointmentDate.ToShortDateString()} at {appointment.StartTime} has been confirmed."
            : status == "Completed"
            ? $"Your appointment with Dr. {user.FullName} has been completed. You can now view your medical report."
            : $"Your appointment with Dr. {user.FullName} on {appointment.AppointmentDate.ToShortDateString()} has been cancelled.";

            await _notificationService.CreateNotificationAsync(
            appointment.Patient.UserId,
            $"Appointment Status Update: {status}",
            message,
            "Appointment",
            appointment.Id
            );

            TempData["Success"] = $"Appointment status updated to {status}";
            return RedirectToAction(nameof(Dashboard));
        }

        // GET: Doctors/Search
        public async Task<IActionResult> Search(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return RedirectToAction(nameof(Index));
            }

            var doctors = await _unitOfWork.Doctors.SearchDoctorsAsync(searchTerm);
            var viewModels = doctors.Select(d => new DoctorViewModel
            {
                Id = d.Id,
                FullName = d.User.FullName,
                Email = d.User.Email,
                PhoneNumber = d.User.PhoneNumber,
                Specialization = d.Specialty,
                ExperienceYears = d.Experience,
                Rating = d.Rating ?? 0m,
                ImageUrl = d.ImageUrl
            }).ToList();

            ViewData["SearchTerm"] = searchTerm;
            return View("Index", viewModels);
        }

        // GET: Doctors/BySpecialization
        public async Task<IActionResult> BySpecialization(string specialization)
        {
            if (string.IsNullOrEmpty(specialization))
            {
                return RedirectToAction(nameof(Index));
            }

            var doctors = await _unitOfWork.Doctors.GetDoctorsBySpecializationAsync(specialization);
            var viewModels = doctors.Select(d => new DoctorViewModel
            {
                Id = d.Id,
                FullName = d.User.FullName,
                Email = d.User.Email,
                PhoneNumber = d.User.PhoneNumber,
                Specialization = d.Specialty,
                ExperienceYears = d.Experience,
                Rating = d.Rating ?? 0m,
                ImageUrl = d.ImageUrl
            }).ToList();

            ViewData["Specialization"] = specialization;
            return View("Index", viewModels);
        }

        [Authorize(Roles = "Admin")]
        // GET: Doctors/Create
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        // POST: Doctors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DoctorViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Create user account
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    PhoneNumber = model.PhoneNumber,
                    UserType = "Doctor"
                };

                // Generate a random password (you might want to send this via email)
                string password = GenerateRandomPassword();
                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Doctor");

                    // Create doctor profile
                    var doctor = new Doctor
                    {
                        UserId = user.Id,
                        Specialty = model.Specialization,
                        Experience = model.ExperienceYears,
                        Rating = 0
                    };

                    // Handle profile image upload
                    if (model.ProfileImage != null)
                    {
                        // Implementation for file upload
                        // This is a placeholder - you would need to implement file storage
                        string uniqueFileName = System.Guid.NewGuid().ToString() + "_" + model.ProfileImage.FileName;
                        // Save file to wwwroot/images/doctors
                        // string filePath = Path.Combine(webHostEnvironment.WebRootPath, "images", "doctors", uniqueFileName);
                        // using (var fileStream = new FileStream(filePath, FileMode.Create))
                        // {
                        //     await model.ProfileImage.CopyToAsync(fileStream);
                        // }

                        doctor.ImageUrl = "/images/doctors/" + uniqueFileName;
                    }

                    await _unitOfWork.Doctors.AddAsync(doctor);

                    // TODO: Send email with login credentials

                    TempData["Success"] = "Doctor created successfully. Password: " + password;
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        // GET: Doctors/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var doctor = await _unitOfWork.Doctors.GetFirstOrDefaultAsync(
                d => d.Id == id,
                includeProperties: "User");

            if (doctor == null)
            {
                return NotFound();
            }

            var viewModel = new DoctorViewModel
            {
                Id = doctor.Id,
                FullName = doctor.User.FullName,
                Email = doctor.User.Email,
                PhoneNumber = doctor.User.PhoneNumber,
                Specialization = doctor.Specialty,
                ExperienceYears = doctor.Experience,
                Rating = doctor.Rating ?? 0m,
                ImageUrl = doctor.ImageUrl
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Admin")]
        // POST: Doctors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DoctorViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var doctor = await _unitOfWork.Doctors.GetFirstOrDefaultAsync(
                    d => d.Id == id,
                    includeProperties: "User");

                if (doctor == null)
                {
                    return NotFound();
                }

                // Update user information
                doctor.User.FullName = model.FullName;
                doctor.User.PhoneNumber = model.PhoneNumber;

                var userResult = await _userManager.UpdateAsync(doctor.User);
                if (!userResult.Succeeded)
                {
                    foreach (var error in userResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }

                // Update doctor information
                doctor.Specialty = model.Specialization;
                doctor.Experience = model.ExperienceYears;

                // Handle profile image upload
                if (model.ProfileImage != null)
                {
                    // Implementation for file upload
                    // This is a placeholder - you would need to implement file storage
                    string uniqueFileName = System.Guid.NewGuid().ToString() + "_" + model.ProfileImage.FileName;
                    // Save file to wwwroot/images/doctors
                    // string filePath = Path.Combine(webHostEnvironment.WebRootPath, "images", "doctors", uniqueFileName);
                    // using (var fileStream = new FileStream(filePath, FileMode.Create))
                    // {
                    //     await model.ProfileImage.CopyToAsync(fileStream);
                    // }

                    doctor.ImageUrl = "/images/doctors/" + uniqueFileName;
                }

                await _unitOfWork.Doctors.UpdateAsync(doctor);

                TempData["Success"] = "Doctor updated successfully";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        // GET: Doctors/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var doctor = await _unitOfWork.Doctors.GetFirstOrDefaultAsync(
                d => d.Id == id,
                includeProperties: "User");

            if (doctor == null)
            {
                return NotFound();
            }

            var viewModel = new DoctorViewModel
            {
                Id = doctor.Id,
                FullName = doctor.User.FullName,
                Email = doctor.User.Email,
                PhoneNumber = doctor.User.PhoneNumber,
                Specialization = doctor.Specialty,
                ExperienceYears = doctor.Experience,
                Rating = doctor.Rating ?? 0m,
                ImageUrl = doctor.ImageUrl
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Admin")]
        // POST: Doctors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var doctor = await _unitOfWork.Doctors.GetFirstOrDefaultAsync(
                d => d.Id == id,
                includeProperties: "User");

            if (doctor == null)
            {
                return NotFound();
            }

            // Delete the doctor profile
            await _unitOfWork.Doctors.RemoveAsync(doctor);

            // Delete the user account
            var user = await _userManager.FindByIdAsync(doctor.UserId);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }

            TempData["Success"] = "Doctor deleted successfully";
            return RedirectToAction(nameof(Index));
        }

        private string GenerateRandomPassword()
        {
            // Generate a random password that meets the requirements
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()";
            var random = new System.Random();
            var password = new string(Enumerable.Repeat(chars, 10)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            return password;
        }
    }
}