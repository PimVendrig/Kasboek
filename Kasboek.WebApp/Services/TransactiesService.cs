using Kasboek.WebApp.Data;
using Kasboek.WebApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kasboek.WebApp.Services
{
    public class TransactiesService : CrudService<Transactie>, ITransactiesService
    {
        private readonly IRekeningenService _rekeningenService;

        public TransactiesService(KasboekDbContext context, IRekeningenService rekeningenService) : base(context)
        {
            _rekeningenService = rekeningenService;
        }

        private IQueryable<Transactie> GetRawListQuery()
        {
            return _context.Transacties
                .OrderByDescending(t => t.Datum)
                .ThenByDescending(t => t.TransactieId);
        }

        private IQueryable<Transactie> GetListQuery()
        {
            return GetRawListQuery()
                .Include(t => t.NaarRekening)
                .Include(t => t.VanRekening)
                .Include(t => t.Categorie);
        }

        public async override Task<IList<Transactie>> GetListAsync()
        {
            return await GetListQuery()
                .ToListAsync();
        }

        public async override Task<Transactie> GetRawSingleOrDefaultAsync(int id)
        {
            return await _context.Transacties
                .SingleOrDefaultAsync(t => t.TransactieId == id);
        }

        public async override Task<Transactie> GetSingleOrDefaultAsync(int id)
        {
            return await _context.Transacties
                .Include(t => t.NaarRekening)
                .Include(t => t.VanRekening)
                .Include(t => t.Categorie)
                .SingleOrDefaultAsync(t => t.TransactieId == id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Transacties
                .AnyAsync(t => t.TransactieId == id);
        }

        public async Task DetermineCategorieAsync(Transactie transactie)
        {
            if (!transactie.CategorieId.HasValue)
            {
                //Als er geen categorie is ingesteld, achterhaal via de standaard categorieën
                await SetStandaardCategorieAsync(transactie);
            }
        }

        private async Task SetStandaardCategorieAsync(Transactie transactie)
        {
            var vanRekening = transactie.VanRekening;
            if (vanRekening == null)
            {
                //Alleen VanRekeningId is weggeschreven. Haal op uit de database
                vanRekening = await _rekeningenService.GetRawSingleOrDefaultAsync(transactie.VanRekeningId);
            }
            if (vanRekening.StandaardCategorieId.HasValue)
            {
                transactie.CategorieId = vanRekening.StandaardCategorieId;
                return;
            }
            var naarRekening = transactie.NaarRekening;
            if (naarRekening == null)
            {
                //Alleen NaarRekeningId is weggeschreven. Haal op uit de database
                naarRekening = await _rekeningenService.GetRawSingleOrDefaultAsync(transactie.NaarRekeningId);
            }
            if (naarRekening.StandaardCategorieId.HasValue)
            {
                transactie.CategorieId = naarRekening.StandaardCategorieId;
            }
        }

        public async Task<IList<Transactie>> GetListByRekeningAsync(Rekening rekening, DateTime? datum)
        {
            return await GetListQuery()
                .Where(t => t.VanRekening == rekening || t.NaarRekening == rekening)
                .Where(t => !datum.HasValue || t.Datum <= datum.Value)
                .ToListAsync();
        }

        public async Task<IList<Transactie>> GetListByCategorieAsync(Categorie categorie, DateTime? startDatum, DateTime? eindDatum)
        {
            return await GetListQuery()
                .Where(t => t.Categorie == categorie)
                .Where(t => !startDatum.HasValue || t.Datum >= startDatum.Value)
                .Where(t => !eindDatum.HasValue || t.Datum <= eindDatum.Value)
                .ToListAsync();
        }

        public async Task<DateTime?> GetFirstTransactieDatumAsync()
        {
            return await _context.Transacties
                .MinAsync(t => (DateTime?)t.Datum);
        }

        public async Task<DateTime?> GetLastTransactieDatumAsync()
        {
            return await _context.Transacties
                .MaxAsync(t => (DateTime?)t.Datum);
        }

        public async Task<int?> GetLastIdAsync()
        {
            return await _context.Transacties
                .MaxAsync(t => (int?)t.TransactieId);
        }

        public async Task<IList<Transactie>> GetListWithFilterAsync(int? afterId, bool? hasCategorie, DateTime? startDatum, DateTime? eindDatum, DateTime? nearDatum, decimal? vanafBedrag)
        {
            return await GetListQuery()
                .Where(t => !afterId.HasValue || t.TransactieId > afterId.Value)
                .Where(t => !hasCategorie.HasValue || (hasCategorie.Value && t.Categorie != null) || (!hasCategorie.Value && t.Categorie == null))
                .Where(t => !startDatum.HasValue || t.Datum >= startDatum.Value)
                .Where(t => !eindDatum.HasValue || t.Datum <= eindDatum.Value)
                .Where(t => !nearDatum.HasValue || (t.Datum >= nearDatum.Value.AddDays(-3) && t.Datum <= nearDatum.Value.AddDays(3)))
                .Where(t => !vanafBedrag.HasValue || t.Bedrag >= vanafBedrag.Value)
                .ToListAsync();
        }

        public async Task<IList<Transactie>> GetRawListWithNoCategorieByRekeningAsync(Rekening rekening)
        {
            return await GetRawListQuery()
                .Where(t => t.VanRekening == rekening || t.NaarRekening == rekening)
                .Where(t => t.Categorie == null)
                .ToListAsync();
        }
    }
}
