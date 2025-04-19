using Microsoft.AspNetCore.Http;
using ClinicPatient.Models;
using ClinicPatient.Repositories;
using ClinicPatient.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using ClinicPatient.Data;
using ClinicPatient.Services;

namespace ClinicPatient.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly INotificationService _notificationService;
        private readonly IUnitOfWork _unitOfWork;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IUnitOfWork unitOfWork,
            ApplicationDbContext context,
            IEmailService emailService,
            INotificationService notificationService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _emailService = emailService;
            _notificationService = notificationService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Register(string returnUrl = null)
        {
            var model = new RegisterViewModel
            {
                ReturnUrl = returnUrl
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.FullName,
                    Email = model.Email,
                    FullName = model.FullName,
                    PhoneNumber = model.PhoneNumber,
                    //Role = model.Role,
                    UserType = model.UserType,
                    IsApproved = model.UserType == "Patient" // Patients are approved automatically
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Add user to role based on UserType
                    await _userManager.AddToRoleAsync(user, model.UserType);

                    // Create additional profile based on user type
                    if (model.UserType == "Patient")
                    {
                        var patient = new Patient
                        {
                            UserId = user.Id,
                            Gender = model.Gender,
                            BloodType = model.BloodType,
                            DateOfBirth = model.DateOfBirth ?? DateTime.Now,
                            MedicalHistory = ""
                        };
                        await _unitOfWork.Patients.AddAsync(patient);
                        await _unitOfWork.SaveAsync();

                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToAction("Index", "Home");
                    }
                    else if (model.UserType == "Doctor")
                    {
                        var doctor = new Doctor
                        {
                            UserId = user.Id,
                            Specialization = model.Specialization ?? "",
                            Bio = model.Bio,
                            ExperienceYears = model.ExperienceYears ?? 0,
                            Rating = 0,                            
                            Education = model.Education,
                            ConsultationFee = model.ConsultationFee
                        };
                        await _unitOfWork.Doctors.AddAsync(doctor);
                        await _unitOfWork.SaveAsync();

                        // إرسال إشعار للمسؤول
                        var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
                        foreach (var admin in adminUsers)
                        {
                            await _notificationService.CreateNotificationAsync(
                                admin.Id,
                                "طلب تسجيل طبيب جديد",
                                $"طلب {user.FullName} التسجيل كطبيب. يرجى مراجعة الطلب.",
                                "System"
                            );
                        }

                        TempData["SuccessMessage"] = "تم تسجيل حسابك بنجاح. سيتم مراجعة معلوماتك والرد عليك قريبًا.";
                        return RedirectToAction("Login");
                    }

                    // Optional: Send email confirmation
                    // var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                    // await _emailSender.SendEmailAsync(model.Email, "Confirm your account", $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a>");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToAction("RegisterConfirmation");
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                        {
                            return Redirect(model.ReturnUrl); // ✅ ترجع المستخدم للمكان اللي كان فيه
                        }
                        return RedirectToAction("Index", "Home");
                    }
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);

                    if (user.UserType == "Doctor" && !user.IsApproved)
                    {
                        await _signInManager.SignOutAsync();
                        ModelState.AddModelError(string.Empty, "حسابك قيد المراجعة. سيتم إعلامك عند الموافقة عليه.");
                        return View(model);
                    }

                    // ✅ التوجيه بعد التحقق من تسجيل الدخول
                    if (await _userManager.IsInRoleAsync(user, "Admin"))
                        return RedirectToAction("Dashboard", "Admin");

                    if (await _userManager.IsInRoleAsync(user, "Doctor"))
                        return RedirectToAction("Dashboard", "Doctors");

                    if (await _userManager.IsInRoleAsync(user, "Patient"))
                        return RedirectToAction("Profile", "Patients");

                    return LocalRedirect(returnUrl);
                }
                if (result.IsLockedOut)
                {
                    return RedirectToAction("Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "محاولة تسجيل دخول غير صالحة.");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            if (user.UserType == "Patient")
            {
                return RedirectToAction("Profile", "Patients");
            }
            else if (user.UserType == "Doctor")
            {
                return RedirectToAction("Dashboard", "Doctors");
            }
            else if (user.UserType == "Admin")
            {
                return RedirectToAction("Dashboard", "Admin");
            }
            else
            {
                // Admin profile
                return View("AdminProfile", user);
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePatientProfile(PatientViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("PatientProfile", model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View("PatientProfile", model);
            }

            var patient = await _unitOfWork.Patients.GetByIdAsync(model.Id);
            if (patient == null)
            {
                return NotFound();
            }

            patient.DateOfBirth = model.DateOfBirth;
            patient.MedicalHistory = model.MedicalHistory;

            await _unitOfWork.Patients.UpdateAsync(patient);

            TempData["StatusMessage"] = "Your profile has been updated";
            return RedirectToAction(nameof(Profile));
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateDoctorProfile(DoctorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("DoctorProfile", model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View("DoctorProfile", model);
            }

            var doctor = await _unitOfWork.Doctors.GetByIdAsync(model.Id);
            if (doctor == null)
            {
                return NotFound();
            }

            doctor.Specialization = model.Specialization;
            doctor.ExperienceYears = model.ExperienceYears;

            // Handle profile image upload
            if (model.ProfileImage != null)
            {
                // Implementation for file upload
                // This is a placeholder - you would need to implement file storage
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProfileImage.FileName;
                // Save file to wwwroot/images/doctors
                // string filePath = Path.Combine(webHostEnvironment.WebRootPath, "images", "doctors", uniqueFileName);
                // using (var fileStream = new FileStream(filePath, FileMode.Create))
                // {
                //     await model.ProfileImage.CopyToAsync(fileStream);
                // }

                doctor.ImageUrl = "/images/doctors/" + uniqueFileName;
            }
            await _unitOfWork.Doctors.UpdateAsync(doctor);

            TempData["StatusMessage"] = "Your profile has been updated";
            return RedirectToAction(nameof(Profile));
        }
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
    }
}