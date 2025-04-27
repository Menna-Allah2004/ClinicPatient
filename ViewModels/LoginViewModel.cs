using System.ComponentModel.DataAnnotations;

namespace ClinicPatient.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صالح")]
        [Display(Name = "البريد الإلكتروني")]
        public string Email { get; set; }

        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة المرور")]
        public string Password { get; set; }

        // إضافة حقل لنوع المستخدم (طبيب أو مريض)
        [Display(Name = "نوع المستخدم")]
        public string UserType { get; set; }

        [Display(Name = "تذكرني؟")]
        public bool RememberMe { get; set; }
    }
}