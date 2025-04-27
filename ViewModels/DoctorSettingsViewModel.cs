using ClinicPatient.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace ClinicPatient.ViewModels
{
    public class DoctorSettingsViewModel
    {
        public DoctorViewModel ProfileSettings { get; set; }

        public NotificationSettingsViewModel NotificationSettings { get; set; }

        public SecuritySettingsViewModel SecuritySettings { get; set; }

        public DoctorSettingsViewModel()
        {
            ProfileSettings = new DoctorViewModel();
            NotificationSettings = new NotificationSettingsViewModel();
            SecuritySettings = new SecuritySettingsViewModel();
        }
    }

    public class NotificationSettingsViewModel
    {
        [Display(Name = "إشعارات المواعيد")]
        public bool AppointmentNotifications { get; set; } = true;

        [Display(Name = "تذكيرات المهام")]
        public bool TaskReminders { get; set; } = true;

        [Display(Name = "إشعارات البريد الإلكتروني")]
        public bool EmailNotifications { get; set; } = false;
    }

    public class SecuritySettingsViewModel
    {
        [Required(ErrorMessage = "كلمة المرور الحالية مطلوبة")]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة المرور الحالية")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "كلمة المرور الجديدة مطلوبة")]
        [StringLength(100, ErrorMessage = "يجب أن تكون كلمة المرور {0} على الأقل {2} وبحد أقصى {1} حرفًا.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة المرور الجديدة")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "تأكيد كلمة المرور")]
        [Compare("NewPassword", ErrorMessage = "كلمة المرور الجديدة وتأكيد كلمة المرور غير متطابقين.")]
        public string ConfirmPassword { get; set; }
    }
}
