using ClinicPatient.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClinicPatient.Repositories
{
    public interface IDoctorRepository : IRepository<Doctor>
    {
        Task<IEnumerable<Doctor>> GetDoctorsBySpecializationAsync(string specialization);
        Task<IEnumerable<Doctor>> SearchDoctorsAsync(string searchTerm);
        Task UpdateRatingAsync(int doctorId, decimal newRating);
    }
}