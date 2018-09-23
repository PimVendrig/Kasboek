using Kasboek.WebApp.Data;
using Kasboek.WebApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kasboek.WebApp.Services
{
    public class CategorieenService : CrudService<Categorie>, ICategorieenService
    {
        public CategorieenService(KasboekDbContext context) : base(context)
        {
        }

        private IQueryable<Categorie> GetRawListQuery()
        {
            return _context.Categorieen
                .OrderBy(c => c.Omschrijving);
        }

        private IQueryable<Categorie> GetListQuery()
        {
            return GetRawListQuery();
        }

        public async override Task<IList<Categorie>> GetListAsync()
        {
            return await GetListQuery()
                .ToListAsync();
        }

        public async override Task<Categorie> GetRawSingleOrDefaultAsync(int id)
        {
            return await _context.Categorieen
                .SingleOrDefaultAsync(c => c.CategorieId == id);
        }

        public async override Task<Categorie> GetSingleOrDefaultAsync(int id)
        {
            return await GetRawSingleOrDefaultAsync(id);
        }

        public async Task<IList<KeyValuePair<int, string>>> GetSelectListAsync()
        {
            return await GetRawListQuery()
                .Select(c => new KeyValuePair<int, string>(c.CategorieId, c.Omschrijving))
                .ToListAsync();
        }

        public async Task<IList<KeyValuePair<int, string>>> GetSelectListForIdsAsync(IList<int> ids)
        {
            return await GetListQuery()
                .Where(c => ids.Contains(c.CategorieId))
                .Select(c => new KeyValuePair<int, string>(c.CategorieId, c.Omschrijving))
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Categorieen
                .AnyAsync(c => c.CategorieId == id);
        }

        public async Task<decimal> GetSaldoAsync(Categorie categorie)
        {
            return await _context.Transacties
                .Where(t => t.Categorie == categorie)
                .SumAsync(t => 
                    t.NaarRekening.IsEigenRekening && !t.VanRekening.IsEigenRekening ? t.Bedrag
                    : t.VanRekening.IsEigenRekening && !t.NaarRekening.IsEigenRekening ? (-1M * t.Bedrag)
                    : 0);
        }

        public async Task<bool> IsOmschrijvingInUseAsync(string omschrijving, IList<int> excludeIds)
        {
            if (string.IsNullOrWhiteSpace(omschrijving)) return false;

            return await _context.Categorieen
                .AnyAsync(c =>
                    !excludeIds.Contains(c.CategorieId)
                    && c.Omschrijving == omschrijving);
        }

        public async Task<IList<Categorie>> GetRawListByIdsAsync(IList<int> ids)
        {
            return await GetRawListQuery()
                .Where(c => ids.Contains(c.CategorieId))
                .ToListAsync();
        }
    }
}
