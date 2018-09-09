using Kasboek.WebApp.Data;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kasboek.WebApp.Services
{
    public abstract class CrudService<TEntity> : ICrudService<TEntity> where TEntity : class
    {
        protected readonly KasboekDbContext _context;

        public CrudService(KasboekDbContext context)
        {
            _context = context;
        }

        public EntityEntry<TEntity> Add(TEntity entity)
        {
            return _context.Add(entity);
        }

        public EntityEntry<TEntity> Update(TEntity entity)
        {
            return _context.Update(entity);
        }

        public EntityEntry<TEntity> Remove(TEntity entity)
        {
            return _context.Remove(entity);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        
        public abstract Task<IList<TEntity>> GetListAsync();
        public abstract Task<TEntity> GetRawSingleOrDefaultAsync(int id);
        public abstract Task<TEntity> GetSingleOrDefaultAsync(int id);
    }
}
