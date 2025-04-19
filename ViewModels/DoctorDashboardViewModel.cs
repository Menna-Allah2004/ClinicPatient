using System;
using System.Collections.Generic;

namespace ClinicPatient.ViewModels
{
    public class DoctorDashboardViewModel
    {
        public string DoctorName { get; set; }
        public string Greeting { get; set; }
        public DateTime CurrentDate { get; set; }
        public int PatientCount { get; set; }
        public int ConsultationCount { get; set; }
        public int CompletedTasksPercentage { get; set; }
        public List<AppointmentViewModel> TodayAppointments { get; set; }
        public List<AppointmentViewModel> UpcomingAppointments { get; set; }
        public List<AppointmentViewModel> CompletedAppointments { get; set; }
        public List<PatientViewModel> Patients { get; set; }
    }
}
