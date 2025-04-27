using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace ClinicPatient.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Appointment Date and Time")]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        [Required]
        [Display(Name = "Status")]
        [StringLength(20)]
        public string Status { get; set; } = "Pending";

        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public string Title { get; set; }  

        public string Type { get; set; }   

        public bool IsVirtual { get; set; }  

        public string? MeetingLink { get; set; }

        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; }

        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; }

        public virtual MedicalReport MedicalReport { get; set; }
    }
}