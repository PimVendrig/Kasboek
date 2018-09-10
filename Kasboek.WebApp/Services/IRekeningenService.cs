using Kasboek.WebApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kasboek.WebApp.Services
{
    public interface IRekeningenService : ICrudService<Rekening>
    {
        Task<IList<KeyValuePair<int, string>>> GetSelectListAsync();
        Task<bool> ExistsAsync(int id);
        Task<decimal> GetSaldoAsync(Rekening rekening);
        Task<bool> HasTransactiesAsync(Rekening rekening);
        Task<bool> IsNaamInUseAsync(Rekening rekening);
        Task<bool> IsRekeningnummerInUseAsync(Rekening rekening);
        Task<IList<Rekening>> GetListByStandaardCategorieAsync(Categorie categorie);
    }
}
