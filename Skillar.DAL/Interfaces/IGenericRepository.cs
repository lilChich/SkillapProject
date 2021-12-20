using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Skillap.DAL.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        //Task<IEnumerable<TEntity>> GetAllAsync();
        IEnumerable<TEntity> GetAll();
        Task<TEntity> GetByIdAsync(int id);
        Task<TEntity> GetByNameAsync(string name);
        //Task<TEntity> GetByEmailAsync(TEntity entity);
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, Boolean>> predicate);
        Task CreateAsync(TEntity entity);
        Task DeleteAsync(int id);
        Task DeleteByEmailAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task SaveAsync();
    }
}
