using ClinicPatient.Models;
using System.ComponentModel.DataAnnotations;

namespace ClinicPatient.ViewModels
{
    public class DoctorPatientsViewModel
    {
        public int TotalPatients { get; set; }
        public List<PatientViewModel> Patients { get; set; }
        public bool HasPatients => Patients != null && Patients.Count > 0;
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
    }
}