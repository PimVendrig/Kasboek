using Kasboek.WebApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kasboek.WebApp.Services
{
    public interface ITransactiesService : ICrudService<Transactie>
    {
        Task<bool> ExistsAsync(int id);
        Task DetermineCategorieAsync(Transactie transactie);
        Task<IList<Transactie>> GetListByRekeningAsync(Rekening rekening);
        Task<IList<Transactie>> GetListByCategorieAsync(Categorie categorie);
    }
}
