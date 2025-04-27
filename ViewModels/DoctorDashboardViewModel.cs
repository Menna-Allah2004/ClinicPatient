using System;
using System.Collections.Generic;

namespace ClinicPatient.ViewModels
{
    public class DoctorDashboardViewModel
    {
        public DoctorViewModel Doctor { get; set; }
        public RegisterDoctorViewModel Doctor1 { get; set; }
        public string? WorkingHours { get; set; }
        public int ScheduledMeetings { get; set; }
        public int ScheduledLabTests { get; set; }
        public int ScheduledConsultations { get; set; }
        public int InPatientsCountChange { get; set; }
        public int InPatientsCount { get; set; }
        public string DoctorLocation { get; set; }
        public string DoctorBloodType { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime CurrentCalendarDate { get; set; }
        public int CompletedMeetings { get; set; }
        public int CompletedLabTests { get; set; }
        public int CompletedConsultations { get; set; }
        public IEnumerable<CalendarDayViewModel> CalendarDays { get; set; }
        public List<PatientListViewModel> Patients { get; set; }

        // إحصائيات الطبيب
        public string DoctorName { get; set; }
        public int PatientsCount { get; set; }
        public int OnlineConsultationsCount { get; set; }
        public int LabTestsCount { get; set; }

        // معلومات التقدم في المهام
        public int ConsultationsCount { get; set; }
        public int ConsultationsCompleted { get; set; }
        public int LabAnalysisCount { get; set; }
        public int LabAnalysisCompleted { get; set; }
        public int MeetingsCount { get; set; }
        public int MeetingsCompleted { get; set; }

        // معلومات المواعيد
        public List<AppointmentViewModel> TodayAppointments { get; set; }
        public List<AppointmentViewModel> UpcomingAppointments { get; set; }
        public List<AppointmentViewModel> CompletedAppointments { get; set; }

        // التاريخ الحالي
        public DateTime CurrentDate { get; set; }
        public string Greeting { get; set; }

        // حساب النسب المئوية للمهام
        public int ConsultationsPercentage => ConsultationsCount > 0 ? (int)(ConsultationsCompleted * 100 / ConsultationsCount) : 0;
        public int LabAnalysisPercentage => LabAnalysisCount > 0 ? (int)(LabAnalysisCompleted * 100 / LabAnalysisCount) : 0;
        public int MeetingsPercentage => MeetingsCount > 0 ? (int)(MeetingsCompleted * 100 / MeetingsCount) : 0;

        // النسبة المئوية الإجمالية للأحداث المجدولة
        public int TotalEventsPercentage
        {
            get
            {
                int total = ConsultationsCount + LabAnalysisCount + MeetingsCount;
                int completed = ConsultationsCompleted + LabAnalysisCompleted + MeetingsCompleted;
                return total > 0 ? (int)(completed * 100 / total) : 0;
            }
        }

        // نسبة إكمال المهام اليومية
        public int CompletedTasksPercentage { get; set; }

        // اتجاهات الإحصائيات (زيادة أو نقصان)
        public int PatientsCountChange { get; set; } // نسبة التغيير عن الأسبوع الماضي
        public int OnlineConsultationsChange { get; set; }
        public int LabTestsChange { get; set; }

        public class PatientDetailsViewModel
        {
            public PatientViewModel Patient { get; set; }
            public List<AppointmentViewModel> Appointments { get; set; }
        }
    }
}
