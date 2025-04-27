using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ClinicPatient.Models;

namespace ClinicPatient.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetByIdAsync(object id);
        Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        void Update(T entity);
        void Remove(T entity);
    }
}