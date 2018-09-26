using Kasboek.WebApp.Models.VerslagViewModels;
using Kasboek.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kasboek.WebApp.Controllers
{
    public class VerslagController : Controller
    {
        private readonly ITransactiesService _transactiesService;
        private readonly IRekeningenService _rekeningenService;

        public VerslagController(ITransactiesService transactiesService, IRekeningenService rekeningenService)
        {
            _transactiesService = transactiesService;
            _rekeningenService = rekeningenService;
        }

        // GET: Verslag
        public async Task<IActionResult> Index()
        {
            var verslag = new VerslagViewModel
            {
                Balans = await GetBalansAsync()
            };
            return View(verslag);
        }

        private async Task<BalansViewModel> GetBalansAsync()
        {
            var balans = new BalansViewModel
            {
                Datums = await GetBalansDatumsAsync()
            };
            balans.RekeningRegels = await GetBalansRekeningRegelsAsync(balans.Datums);
            balans.TotaalRegel = GetBalansTotaalRegel(balans.Datums, balans.RekeningRegels);
            return balans;
        }

        private async Task<List<DateTime>> GetBalansDatumsAsync()
        {
            //Balans toont situatie voor ieder jaar op 31 december.
            //Te beginnen bij de datum van de eerste transactie, en te eindigen bij de datum van de laatste transactie
            var firstDatum = await _transactiesService.GetFirstTransactieDatumAsync();

            List<DateTime> datums = new List<DateTime>();

            if (!firstDatum.HasValue)
            {
                //Als de eerste datum geen waarde heeft, is er niets
                return datums;
            }

            datums.Add(firstDatum.Value);
            var lastDatum = await _transactiesService.GetLastTransactieDatumAsync();

            for (var lastDayOfYear = new DateTime(firstDatum.Value.Year, 12, 31); lastDayOfYear < lastDatum.Value; lastDayOfYear = lastDayOfYear.AddYears(1))
            {
                //Eerste datum kan evt 31 december zijn, niet dubbel toevoegen
                if (!datums.Contains(lastDayOfYear))
                {
                    datums.Add(lastDayOfYear);
                }
            }

            //Als er een eerste datum is, is er altijd een laatste datum (evt dezelfde datum, dus niet dubbel toevoegen)
            if (!datums.Contains(lastDatum.Value))
            {
                datums.Add(lastDatum.Value);
            }

            return datums;
        }

        private async Task<List<BalansRegel>> GetBalansRekeningRegelsAsync(List<DateTime> datums)
        {
            var rekeningRegels = new List<BalansRegel>();
            var rekeningen = await _rekeningenService.GetRawEigenRekeningListAsync();
            
            foreach(var rekening in rekeningen)
            {
                var rekeningRegel = new BalansRegel
                {
                    RekeningId = rekening.RekeningId,
                    RekeningNaam = rekening.Naam,
                    Saldos = new List<decimal>()
                };

                rekeningRegels.Add(rekeningRegel);

                foreach(var datum in datums)
                {
                    rekeningRegel.Saldos.Add(await _rekeningenService.GetSaldoOnDatumAsync(rekening, datum));
                }
            }

            return rekeningRegels;
        }

        private List<decimal> GetBalansTotaalRegel(List<DateTime> datums, List<BalansRegel> rekeningRegels)
        {
            var totaalRegel = new List<decimal>();

            for (var i = 0; i < datums.Count; i++)
            {
                totaalRegel.Add(rekeningRegels.Select(r => r.Saldos[i]).Sum());
            }

            return totaalRegel;
        }
    }
}
