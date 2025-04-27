using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ClinicPatient.Models;
using Microsoft.CodeAnalysis;
using ClinicPatient.ViewModels;

namespace ClinicPatient.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<DoctorAvailability> DoctorAvailabilities { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<MedicalReport> MedicalReports { get; set; }
        public DbSet<ContactUsMessage> ContactUsMessages { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationSettings> NotificationSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure relationships and constraints
            builder.Entity<Rating>()
                .HasIndex(r => new { r.DoctorId, r.PatientId })
                .IsUnique();

            // Configure one-to-one relationship between Doctor and User
            builder.Entity<Doctor>()
                .HasOne(d => d.User)
                .WithOne(u => u.Doctor)
                .HasForeignKey<Doctor>(d => d.UserId);

            // Configure one-to-one relationship between Patient and User
            builder.Entity<Patient>()
                .HasOne(p => p.User)
                .WithOne(u => u.Patient)
                .HasForeignKey<Patient>(p => p.UserId);

            // Configure one-to-many relationship between Doctor and DoctorAvailability
            builder.Entity<DoctorAvailability>()
                .HasOne(a => a.Doctor)
                .WithMany(d => d.Availabilities)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Fix for cascade delete cycles
            builder.Entity<MedicalReport>()
                .HasOne(m => m.Doctor)
                .WithMany(d => d.MedicalReports)
                .HasForeignKey(m => m.DoctorId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<MedicalReport>()
                .HasOne(m => m.Appointment)
                .WithOne(a => a.MedicalReport)
                .HasForeignKey<MedicalReport>(m => m.AppointmentId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Rating>()
                .HasOne(r => r.Doctor)
                .WithMany(d => d.Ratings)
                .HasForeignKey(r => r.DoctorId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Doctor>().HasData(
                new Doctor
                {
                    Id = 1,
                    UserId = "83670e60-4703-4748-8f56-3433db72cead",
                    Specialty = "قلب وأوعية دموية",
                    Experience = 15,
                    Rating = (decimal?)4.5,
                    Bio = "استشاري أمراض القلب والأوعية الدموية بخبرة أكثر من 15 عاماً في علاج أمراض القلب والشرايين",
                    Location = "غزة - السرايا",
                    City = "غزة",
                    FullName = "أحمد محمد",
                    Workplace = "عيادة أحمد",
                    CreatedAt = new DateTime(2025, 4, 26),
                    UpdatedAt = new DateTime(2025, 4, 26),
                    BloodType = "O+"
                },                
                new Doctor
                {
                    Id = 2,
                    UserId = "4c3734ae-0661-45b2-985c-4142bf3fd57e",
                    Specialty = "جلدية",
                    Experience = 13,
                    Rating = (decimal?)5,
                    Bio = "استشارية الأمراض الجلدية والتجميل، متخصصة في علاج مشاكل البشرة والجلد والليزر التجميلي",
                    Location = "غزة - الشفا",
                    City = "غزة",
                    FullName = "سارة علي",
                    Workplace = "عيادة سارة",
                    CreatedAt = new DateTime(2025, 4, 26),
                    UpdatedAt = new DateTime(2025, 4, 26),
                    BloodType = "O+"
                },                
                new Doctor
                {
                    Id = 3,
                    UserId = "43e7b2b8-91ab-4b37-823a-0668332d073a",
                    Specialty = "جراحة عظام",
                    Experience = 20,
                    Rating = (decimal?)4,
                    Bio = "استشاري جراحة العظام والمفاصل، متخصص في جراحات استبدال المفاصل وإصابات الملاعب",
                    Location = "خانيونس - البلد",
                    City = "خانيونس",
                    FullName = "خالد العمري",
                    Workplace = "عيادة خالد",
                    CreatedAt = new DateTime(2025, 4, 26),
                    UpdatedAt = new DateTime(2025, 4, 26),
                    BloodType = "O+"
                }
            );
        }
    }
}