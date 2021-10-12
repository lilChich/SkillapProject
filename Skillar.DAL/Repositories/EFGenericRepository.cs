using Microsoft.EntityFrameworkCore;
using Skillap.DAL.EF;
using Skillap.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Skillap.DAL.Repositories
{
    public class EFGenericRepository<TEntity> : IGenericRepository<TEntity>
        where TEntity : class
    {
        DataContext context;
        DbSet<TEntity> dbSet;

        public EFGenericRepository(DataContext context)
        {
            this.context = context;
            this.dbSet = context.Set<TEntity>();
        }

        public virtual async Task DeleteAsync(int id)
        {
            TEntity toDelete = dbSet.Find(id);
            if (toDelete != null)
            {
                dbSet.Remove(toDelete);
                await SaveAsync();
            }
        }

        public virtual async Task DeleteByEmailAsync(TEntity entity)
        {
            TEntity toDelete = dbSet.Find(entity);
            if (toDelete != null)
            {
                dbSet.Remove(toDelete);
                await SaveAsync();
            }
        }

        public virtual IEnumerable<TEntity> GetAll()
        {
            return dbSet.AsNoTracking().ToList();
        }

        public virtual async Task<TEntity> GetByIdAsync(int id)
        {
            return await dbSet.FindAsync(id);
        }

        public virtual async Task CreateAsync(TEntity entity)
        {
            if (entity != null)
            {
                await dbSet.AddAsync(entity);
                await SaveAsync();
            }
        }

        public virtual async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }

        public virtual async Task UpdateAsync(TEntity entity)
        {
            context.Entry(entity).State = EntityState.Modified;
            await SaveAsync();
        }

        public virtual async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await dbSet.AsNoTracking().Where(predicate).ToListAsync();
        }
    }
}
