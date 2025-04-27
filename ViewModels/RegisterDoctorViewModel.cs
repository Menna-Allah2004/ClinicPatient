using System.ComponentModel.DataAnnotations;

namespace ClinicPatient.ViewModels
{
    public class RegisterDoctorViewModel
    {
        [Required(ErrorMessage = "الاسم الكامل مطلوب")]
        [Display(Name = "الاسم الكامل")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صالح")]
        [Display(Name = "البريد الإلكتروني")]
        public string Email { get; set; }

        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [Display(Name = "رقم الهاتف")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [StringLength(100, ErrorMessage = "يجب أن تكون {0} على الأقل {2} حرفًا وبحد أقصى {1} حرفًا.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة المرور")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "تأكيد كلمة المرور")]
        [Compare("Password", ErrorMessage = "كلمة المرور وتأكيدها غير متطابقين.")]
        public string ConfirmPassword { get; set; }

        // معلومات الطبيب
        [Required(ErrorMessage = "التخصص مطلوب")]
        [Display(Name = "التخصص")]
        public string Specialization { get; set; }

        [Required(ErrorMessage = "سنوات الخبرة مطلوبة")]
        [Display(Name = "سنوات الخبرة")]
        public int? ExperienceYears { get; set; }

        [Display(Name = "نبذة شخصية")]
        public string Bio { get; set; }

        [Display(Name = "المؤهلات العلمية")]
        public string Education { get; set; }

        [Display(Name = "رسوم الاستشارة")]
        public decimal ConsultationFee { get; set; }

        [Required]
        [Display(Name = "User Type")]
        public string UserType { get; set; }

        // URL العودة بعد التسجيل
        public string ReturnUrl { get; set; }
    }
}