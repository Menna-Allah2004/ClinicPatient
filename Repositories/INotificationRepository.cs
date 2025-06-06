﻿using System.Collections.Generic;
using System.Threading.Tasks;
using ClinicPatient.Models;

namespace ClinicPatient.Repositories
{
    public interface INotificationRepository
    {
        Task<IEnumerable<Notification>> GetAllAsync();
        Task<Notification> GetByIdAsync(int id);
        Task AddAsync(Notification notification);
        Task UpdateAsync(Notification notification);
        Task DeleteAsync(int id);
    }
}