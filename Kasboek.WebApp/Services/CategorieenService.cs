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

        private IQueryable<Categorie> GetListQuery()
        {
            return _context.Categorieen
                .OrderBy(c => c.Omschrijving);
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
            return await GetListQuery()
                .Select(r => new KeyValuePair<int, string>(r.CategorieId, r.Omschrijving))
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

        public async Task<bool> IsOmschrijvingInUseAsync(Categorie categorie)
        {
            if (string.IsNullOrWhiteSpace(categorie.Omschrijving)) return false;

            return await _context.Categorieen
                .AnyAsync(c =>
                    c.CategorieId != categorie.CategorieId
                    && c.Omschrijving == categorie.Omschrijving);
        }
    }
}
