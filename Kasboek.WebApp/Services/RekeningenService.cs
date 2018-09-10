using Kasboek.WebApp.Data;
using Kasboek.WebApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kasboek.WebApp.Services
{
    public class RekeningenService : CrudService<Rekening>, IRekeningenService
    {
        public RekeningenService(KasboekDbContext context) : base(context)
        {
        }

        private IQueryable<Rekening> GetListQuery()
        {
            return _context.Rekeningen
                .Include(r => r.StandaardCategorie)
                .OrderByDescending(r => r.IsEigenRekening)
                .ThenBy(r => r.Naam);
        }

        public async override Task<IList<Rekening>> GetListAsync()
        {
            return await GetListQuery()
                .ToListAsync();
        }

        public async override Task<Rekening> GetRawSingleOrDefaultAsync(int id)
        {
            return await _context.Rekeningen
                .SingleOrDefaultAsync(r => r.RekeningId == id);
        }

        public async override Task<Rekening> GetSingleOrDefaultAsync(int id)
        {
            return await _context.Rekeningen
                .Include(r => r.StandaardCategorie)
                .SingleOrDefaultAsync(r => r.RekeningId == id);
        }

        public async Task<IList<KeyValuePair<int, string>>> GetSelectListAsync()
        {
            return await GetListQuery()
                .Select(r => new KeyValuePair<int, string>(r.RekeningId, r.Naam))
                .ToListAsync();
        }
        
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Rekeningen
                .AnyAsync(r => r.RekeningId == id);
        }

        public async Task<decimal> GetSaldoAsync(Rekening rekening)
        {
            return await _context.Transacties
                .Where(t => t.VanRekening == rekening || t.NaarRekening == rekening)
                .SumAsync(t => t.NaarRekening == rekening ? t.Bedrag : (-1M * t.Bedrag));
        }

        public async Task<bool> HasTransactiesAsync(Rekening rekening)
        {
            return await _context.Transacties
                .Where(t => t.VanRekening == rekening || t.NaarRekening == rekening)
                .AnyAsync();
        }

        public async Task<bool> IsNaamInUseAsync(Rekening rekening)
        {
            if (string.IsNullOrWhiteSpace(rekening.Naam)) return false;

            return await _context.Rekeningen
                .AnyAsync(r =>
                    r.RekeningId != rekening.RekeningId
                    && r.Naam == rekening.Naam);
        }

        public async Task<bool> IsRekeningnummerInUseAsync(Rekening rekening)
        {
            if (string.IsNullOrWhiteSpace(rekening.Rekeningnummer)) return false;

            return await _context.Rekeningen
                .AnyAsync(r =>
                    r.RekeningId != rekening.RekeningId
                    && r.Rekeningnummer == rekening.Rekeningnummer);
        }

        public async Task<IList<Rekening>> GetListByStandaardCategorieAsync(Categorie categorie)
        {
            return await GetListQuery()
                .Where(r => r.StandaardCategorie == categorie)
                .ToListAsync();
        }
    }
}
