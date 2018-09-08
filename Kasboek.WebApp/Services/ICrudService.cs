using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kasboek.WebApp.Services
{
    public interface ICrudService<TEntity> where TEntity : class
    {

        EntityEntry<TEntity> Add(TEntity entity);
        EntityEntry<TEntity> Update(TEntity entity);
        EntityEntry<TEntity> Remove(TEntity entity);
        Task<int> SaveChangesAsync();


        Task<IList<TEntity>> GetListAsync();
        Task<TEntity> GetSingleOrDefaultAsync(int id);

    }
}
