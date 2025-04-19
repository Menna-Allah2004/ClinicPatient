namespace ClinicPatient.ViewModels
{
    public class DoctorSearchViewModel
    {
        public string? SearchTerm { get; set; }
        public string? Specialty { get; set; }
        public string? Location { get; set; }
        public string? Rating { get; set; }
        public string? Price { get; set; }
        public bool AvailableToday { get; set; }

        // قائمة الأطباء المعروضة
        public List<DoctorViewModel> Doctors { get; set; } = new();
    }
}
