﻿using Kasboek.WebApp.Models;
using System;
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
        Task<decimal> GetSaldoForPeriodeAsync(Categorie categorie, DateTime? startDatum, DateTime? eindDatum);
        Task<bool> IsOmschrijvingInUseAsync(string omschrijving, IList<int> excludeIds);
        Task<IList<Categorie>> GetRawListByIdsAsync(IList<int> ids);
        Task<IList<Categorie>> GetRawListForResultatenrekeningAsync();
    }
}
