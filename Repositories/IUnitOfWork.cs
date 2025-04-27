using ClinicPatient.Models;
using System;
using System.Threading.Tasks;

namespace ClinicPatient.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IDoctorRepository Doctors { get; }
        IPatientRepository Patients { get; }
        IAppointmentRepository Appointments { get; }
        IMedicalReportRepository MedicalReports { get; }
        IDoctorAvailabilityRepository DoctorAvailabilities { get; }
        IContactUsMessageRepository ContactUsMessages { get; }
        IRatingRepository Ratings { get; }
        INotificationRepository Notifications { get; }
        IGenericRepository<NotificationSettings> NotificationSettings { get; }

        Task SaveAsync();
    }
}