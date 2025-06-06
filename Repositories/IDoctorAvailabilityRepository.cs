﻿using ClinicPatient.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClinicPatient.Repositories
{
    public interface IDoctorAvailabilityRepository : IRepository<DoctorAvailability>
    {
        Task<IEnumerable<DoctorAvailability>> GetAvailabilityByDoctorIdAsync(int doctorId);
        Task<IEnumerable<DateTime>> GetAvailableTimeSlotsAsync(int doctorId, DateTime date);
    }
}