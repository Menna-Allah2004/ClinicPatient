using System;
using System.ComponentModel.DataAnnotations;

namespace ClinicPatient.ViewModels
{
    public class MedicalReportViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "الطبيب مطلوب")]
        [Display(Name = "الطبيب")]
        public int DoctorId { get; set; }

        public string DoctorName { get; set; }

        [Required(ErrorMessage = "المريض مطلوب")]
        [Display(Name = "المريض")]
        public int PatientId { get; set; }

        public string PatientName { get; set; }

        [Display(Name = "موعد")]
        public int? AppointmentId { get; set; }

        public DateTime? AppointmentDate { get; set; }

        [Required(ErrorMessage = "التشخيص مطلوب")]
        [Display(Name = "التشخيص")]
        public string Diagnosis { get; set; }

        [Display(Name = "العلاج")]
        public string Treatment { get; set; }

        [Display(Name = "الوصفة الطبية")]
        public string Prescription { get; set; }

        [Display(Name = "ملاحظات")]
        public string Notes { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
