using System;
using System.ComponentModel.DataAnnotations;

namespace ClinicPatient.ViewModels
{
    public class RegisterViewModel
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

        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [StringLength(100, ErrorMessage = "يجب أن تكون كلمة المرور على الأقل {2} حرفًا.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة المرور")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "تأكيد كلمة المرور")]
        [Compare("Password", ErrorMessage = "كلمة المرور وتأكيدها غير متطابقين.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "نوع المستخدم مطلوب")]
        [Display(Name = "نوع المستخدم")]
        public string Role { get; set; } // Patient, Doctor

        // حقول إضافية للطبيب
        [Display(Name = "التخصص")]
        public string Specialty { get; set; }

        [Display(Name = "السيرة الذاتية")]
        public string Bio { get; set; }

        [Display(Name = "التعليم")]
        public string Education { get; set; }

        [Display(Name = "سنوات الخبرة")]
        public int? Experience { get; set; }

        [Display(Name = "رسوم الاستشارة")]
        public decimal? ConsultationFee { get; set; }

        // حقول إضافية للمريض
        [Display(Name = "تاريخ الميلاد")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "الجنس")]
        public string Gender { get; set; }

        [Display(Name = "فصيلة الدم")]
        public string BloodType { get; set; }

        [Required]
        [Display(Name = "User Type")]
        public string UserType { get; set; } // Patient or Doctor

        // Additional fields for Doctor
        [Display(Name = "Specialization")]
        public string Specialization { get; set; }

        [Display(Name = "Years of Experience")]
        public int? ExperienceYears { get; set; }

        // باقي الخصائص مثل Email، Password، FullName...
        public string ReturnUrl { get; set; }
    }
}