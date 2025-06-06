﻿using ClinicPatient.Models;
using System.Threading.Tasks;

namespace ClinicPatient.Repositories
{
    public interface IPatientRepository : IRepository<Patient>
    {
        Task<Patient> GetPatientByUserIdAsync(string userId);
        Task UpdateMedicalHistoryAsync(int patientId, string medicalHistory);
    }
}