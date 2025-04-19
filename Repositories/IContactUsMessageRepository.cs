using ClinicPatient.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClinicPatient.Repositories
{
    public interface IContactUsMessageRepository : IRepository<ContactUsMessage>
    {
        Task<IEnumerable<ContactUsMessage>> GetRecentMessagesAsync(int count);
    }
}