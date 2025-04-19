using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClinicPatient.ViewModels
{
    public class AppointmentViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "الطبيب مطلوب")]
        [Display(Name = "الطبيب")]
        public int DoctorId { get; set; }

        public string DoctorName { get; set; }
        public string DoctorSpecialization { get; set; }
        public string DoctorImageUrl { get; set; }

        [Required(ErrorMessage = "المريض مطلوب")]
        [Display(Name = "المريض")]
        public int PatientId { get; set; }

        public string PatientName { get; set; }

        [Required(ErrorMessage = "تاريخ الموعد مطلوب")]
        [Display(Name = "تاريخ الموعد")]
        [DataType(DataType.Date)]
        public DateTime AppointmentDate { get; set; }

        [Required(ErrorMessage = "وقت بدء الموعد مطلوب")]
        [Display(Name = "وقت بدء الموعد")]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "وقت انتهاء الموعد مطلوب")]
        [Display(Name = "وقت انتهاء الموعد")]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }

        [Display(Name = "حالة الموعد")]
        public string Status { get; set; }

        [Display(Name = "ملاحظات")]
        public string Notes { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    public class BookAppointmentViewModel
    {
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string DoctorSpecialization { get; set; }
        public string DoctorImageUrl { get; set; }
        public decimal? ConsultationFee { get; set; }
        public List<DateTime> AvailableDates { get; set; }
        public List<TimeSlotViewModel> AvailableTimeSlots { get; set; }
        public AppointmentViewModel Appointment { get; set; }
    }

    public class TimeSlotViewModel
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsAvailable { get; set; }
    }
}