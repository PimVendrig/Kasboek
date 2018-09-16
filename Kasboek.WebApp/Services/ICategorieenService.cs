using Kasboek.WebApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kasboek.WebApp.Services
{
    public interface ICategorieenService : ICrudService<Categorie>
    {
        Task<IList<KeyValuePair<int, string>>> GetSelectListAsync();
        Task<IList<KeyValuePair<int, string>>> GetSelectListForIdsAsync(IList<int> ids);
        Task<bool> ExistsAsync(int id);
        Task<decimal> GetSaldoAsync(Categorie categorie);
        Task<bool> IsOmschrijvingInUseAsync(Categorie categorie);
    }
}
