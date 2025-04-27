
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using ClinicPatient.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace ClinicPatient.Areas.Identity.Pages.Account
{
    public class DoctorRegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public DoctorRegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "الاسم الكامل مطلوب")]
            [Display(Name = "الاسم الكامل")]
            public string FullName { get; set; }

            [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
            [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صالح")]
            [Display(Name = "البريد الإلكتروني")]
            public string Email { get; set; }

            [Required(ErrorMessage = "رقم الهاتف مطلوب")]
            [Phone(ErrorMessage = "رقم الهاتف غير صالح")]
            [Display(Name = "رقم الهاتف")]
            public string PhoneNumber { get; set; }

            [Required(ErrorMessage = "تاريخ الميلاد مطلوب")]
            [DataType(DataType.Date)]
            [Display(Name = "تاريخ الميلاد")]
            public DateTime BirthDate { get; set; }

            [Required(ErrorMessage = "الجنس مطلوب")]
            [Display(Name = "الجنس")]
            public string Gender { get; set; }

            [Required(ErrorMessage = "المدينة مطلوبة")]
            [Display(Name = "المدينة")]
            public string City { get; set; }

            [Required(ErrorMessage = "كلمة المرور مطلوبة")]
            [StringLength(100, ErrorMessage = "يجب أن تحتوي {0} على الأقل {2} وبحد أقصى {1} حرفًا.", MinimumLength = 8)]
            [DataType(DataType.Password)]
            [Display(Name = "كلمة المرور")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "تأكيد كلمة المرور")]
            [Compare("Password", ErrorMessage = "كلمة المرور وتأكيدها غير متطابقين.")]
            public string ConfirmPassword { get; set; }

            [Display(Name = "نوع المستخدم")]
            public string UserType { get; set; } = "Patient"; // القيمة الافتراضية هي "مريض"

            // حقول خاصة بالطبيب
            [Display(Name = "التخصص")]
            public string Specialty { get; set; }

            [Display(Name = "سنوات الخبرة")]
            [Range(0, 100, ErrorMessage = "يرجى إدخال قيمة صحيحة لسنوات الخبرة")]
            public int? Experience { get; set; }

            [Display(Name = "رقم الترخيص الطبي")]
            public string License { get; set; }

            [Display(Name = "العيادة/المستشفى")]
            public string Workplace { get; set; }

            [Display(Name = "عنوان العيادة")]
            public string? Location { get; set; }

            [Display(Name = "نبذة مختصرة")]
            public string Bio { get; set; }

            [Required(ErrorMessage = "يجب الموافقة على شروط الاستخدام وسياسة الخصوصية")]
            [Display(Name = "أوافق على شروط الاستخدام وسياسة الخصوصية")]
            public bool AgreeTerms { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            // التحقق من صحة النموذج وتطبيق القواعد المخصصة
            if (Input.UserType == "Doctor")
            {
                // التحقق من الحقول الإلزامية للطبيب
                if (string.IsNullOrEmpty(Input.Specialty))
                {
                    ModelState.AddModelError("Input.Specialty", "التخصص مطلوب للطبيب");
                }

                if (Input.Experience == null || Input.Experience < 1)
                {
                    ModelState.AddModelError("Input.Experience", "سنوات الخبرة مطلوبة وأكبر من صفر");
                }

                if (string.IsNullOrEmpty(Input.License))
                {
                    ModelState.AddModelError("Input.License", "رقم الترخيص الطبي مطلوب");
                }

                if (string.IsNullOrEmpty(Input.Location))
                {
                    ModelState.AddModelError("Input.Location", "مكان العمل مطلوب");
                }
            }

            if (ModelState.IsValid)
            {
                var user = CreateUser();

                // تعيين البيانات الأساسية للمستخدم
                user.FullName = Input.FullName;
                user.PhoneNumber = Input.PhoneNumber;
                user.BirthDate = Input.BirthDate;
                user.Gender = Input.Gender;
                user.City = Input.City;
                user.UserType = Input.UserType;

                // إذا كان المستخدم طبيب، قم بتعيين البيانات الخاصة به
                if (Input.UserType == "Doctor")
                {
                    user.Specialty = Input.Specialty;
                    user.Experience = Input.Experience;
                    user.License = Input.License;
                    user.Workplace = Input.Workplace;
                    user.Location = Input.Location;
                    user.Bio = Input.Bio;
                }

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                user.ImageUrl = "~/images/default-user.png";
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("تم إنشاء مستخدم جديد بكلمة مرور.");

                    // تعيين دور المستخدم (طبيب أو مريض)
                    await _userManager.AddToRoleAsync(user, Input.UserType);

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    //await _emailSender.SendEmailAsync(Input.Email, "تأكيد بريدك الإلكتروني",
                    //    $"الرجاء تأكيد حسابك من خلال <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>الضغط هنا</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        //return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                        //await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                    else
                    {
                        //await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"لا يمكن إنشاء مثيل من '{nameof(ApplicationUser)}'. " +
                    $"تأكد من أن '{nameof(ApplicationUser)}' ليس فئة مجردة ولديه منشئ بدون معلمات، أو " +
                    $"قم بتجاوز صفحة التسجيل في /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("واجهة المستخدم الافتراضية تتطلب مخزن مستخدم بدعم البريد الإلكتروني.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}