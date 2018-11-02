using Kasboek.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kasboek.WebApp.Services
{
    public interface ITransactiesService : ICrudService<Transactie>
    {
        Task<bool> ExistsAsync(int id);
        Task DetermineCategorieAsync(Transactie transactie);
        Task<IList<Transactie>> GetListByRekeningAsync(Rekening rekening, DateTime? datum);
        Task<IList<Transactie>> GetListByCategorieAsync(Categorie categorie, DateTime? startDatum, DateTime? eindDatum);
        Task<DateTime?> GetFirstTransactieDatumAsync();
        Task<DateTime?> GetLastTransactieDatumAsync();
        Task<int?> GetLastIdAsync();
        Task<IList<Transactie>> GetListWithFilterAsync(int? afterId, bool? hasCategorie, DateTime? startDatum, DateTime? eindDatum, DateTime? nearDatum, decimal? vanafBedrag);
        Task<IList<Transactie>> GetRawListWithNoCategorieByRekeningAsync(Rekening rekening);
    }
}
