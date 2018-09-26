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

        private IQueryable<Rekening> GetRawListQuery()
        {
            return _context.Rekeningen
                .OrderByDescending(r => r.IsEigenRekening)
                .ThenBy(r => r.Naam);
        }

        private IQueryable<Rekening> GetListQuery()
        {
            return GetRawListQuery()
                .Include(r => r.StandaardCategorie);
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
            return await GetRawListQuery()
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
            return await GetSaldoOnDatumAsync(rekening, null);
        }

        public async Task<decimal> GetSaldoOnDatumAsync(Rekening rekening, DateTime? datum)
        {
            return await _context.Transacties
                .Where(t => t.VanRekening == rekening || t.NaarRekening == rekening)
                .Where(t => !datum.HasValue || t.Datum <= datum.Value)
                .SumAsync(t => t.NaarRekening == rekening ? t.Bedrag : (-1M * t.Bedrag));
        }

        public async Task<bool> HasTransactiesAsync(Rekening rekening)
        {
            return await _context.Transacties
                .Where(t => t.VanRekening == rekening || t.NaarRekening == rekening)
                .AnyAsync();
        }

        public async Task<bool> IsNaamInUseAsync(string naam, IList<int> excludeIds)
        {
            if (string.IsNullOrWhiteSpace(naam)) return false;

            return await _context.Rekeningen
                .AnyAsync(r =>
                    !excludeIds.Contains(r.RekeningId)
                    && r.Naam == naam);
        }

        public async Task<bool> IsRekeningnummerInUseAsync(string rekeningnummer, IList<int> excludeIds)
        {
            if (string.IsNullOrWhiteSpace(rekeningnummer)) return false;

            return await _context.Rekeningen
                .AnyAsync(r =>
                    !excludeIds.Contains(r.RekeningId)
                    && r.Rekeningnummer == rekeningnummer);
        }

        public async Task<IList<Rekening>> GetListByStandaardCategorieAsync(Categorie categorie)
        {
            return await GetListQuery()
                .Where(r => r.StandaardCategorie == categorie)
                .ToListAsync();
        }

        public async Task<IList<Rekening>> GetRawListByIdsAsync(IList<int> ids)
        {
            return await GetRawListQuery()
                .Where(r => ids.Contains(r.RekeningId))
                .ToListAsync();
        }

        public async Task<IList<Rekening>> GetRawEigenRekeningListAsync()
        {
            return await GetRawListQuery()
                .Where(r => r.IsEigenRekening)
                .ToListAsync();
        }
    }
}
