using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicPatient.Models
{
    public class NotificationSettings
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        public bool AppointmentNotifications { get; set; } = true;

        public bool TaskReminders { get; set; } = true;

        public bool EmailNotifications { get; set; } = false;

        // Navigation property
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}