using ClinicPatient.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace ClinicPatient.ViewModels
{
    public class ReportViewModel
    {
        public int Id { get; set; }

        [Display(Name = "عنوان التقرير")]
        public string Title { get; set; }

        [Display(Name = "نوع التقرير")]
        public string Type { get; set; }

        [Display(Name = "تاريخ الإنشاء")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "حالة التقرير")]
        public string Status { get; set; }

        [Display(Name = "رابط التحميل")]
        public string DownloadUrl { get; set; }

        [Display(Name = "حجم الملف")]
        public string FileSize { get; set; }

        [Display(Name = "وصف التقرير")]
        public string Description { get; set; }
    }

    public class DoctorReportsViewModel
    {
        [Display(Name = "إجمالي الزيارات")]
        public int TotalVisits { get; set; }

        //[Display(Name = "الزيارات هذا الشهر")]
        //public int MonthlyVisits { get; set; }

        [Display(Name = "الزيارات هذا الأسبوع")]
        public int WeeklyVisits { get; set; }

        [Display(Name = "متوسط الزيارات اليومية")]
        public double DailyAverage { get; set; }

        [Display(Name = "التقارير الأخيرة")]
        public List<MedicalReport> RecentReports { get; set; }
        public bool HasReports => RecentReports != null && RecentReports.Count > 0;

        [Display(Name = "بيانات الرسم البياني")]
        public ChartDataViewModel ChartData { get; set; }

        public List<MonthlyVisitData> MonthlyVisits { get; set; }

        public DoctorReportsViewModel()
        {
            RecentReports = new List<MedicalReport>();
            ChartData = new ChartDataViewModel();
        }
    }

    public class MonthlyVisitData
    {
        public string Month { get; set; }
        public int VisitCount { get; set; }
    }

    public class ChartDataViewModel
    {
        public List<string> Labels { get; set; }
        public List<int> Data { get; set; }
        public string ChartType { get; set; }

        public ChartDataViewModel()
        {
            Labels = new List<string>();
            Data = new List<int>();
            ChartType = "line";
        }
    }
}