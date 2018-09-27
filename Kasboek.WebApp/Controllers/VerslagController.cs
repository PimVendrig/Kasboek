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
        private readonly ICategorieenService _categorieenService;

        public VerslagController(ITransactiesService transactiesService, IRekeningenService rekeningenService, ICategorieenService categorieenService)
        {
            _transactiesService = transactiesService;
            _rekeningenService = rekeningenService;
            _categorieenService = categorieenService;
        }

        // GET: Verslag
        public async Task<IActionResult> Index()
        {
            var verslag = new VerslagViewModel
            {
                Balans = await GetBalansAsync(),
                Resultatenrekening = await GetResultatenrekeningAsync()
            };
            return View(verslag);
        }

        private async Task<BalansViewModel> GetBalansAsync()
        {
            var balans = new BalansViewModel
            {
                Datums = await GetBalansDatumsAsync()
            };
            balans.VerslagRegels = await GetBalansRegelsAsync(balans.Datums);
            balans.TotaalRegel = GetTotaalRegel(balans.Datums.Count, balans.VerslagRegels);
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

        private async Task<List<VerslagRegelViewModel>> GetBalansRegelsAsync(List<DateTime> datums)
        {
            var regels = new List<VerslagRegelViewModel>();
            var rekeningen = await _rekeningenService.GetRawEigenRekeningListAsync();
            
            foreach(var rekening in rekeningen)
            {
                var regel = new VerslagRegelViewModel
                {
                    Id = rekening.RekeningId,
                    Tekst = rekening.Naam,
                    Bedragen = new List<decimal>()
                };

                regels.Add(regel);

                foreach(var datum in datums)
                {
                    regel.Bedragen.Add(await _rekeningenService.GetSaldoOnDatumAsync(rekening, datum));
                }
            }

            return regels;
        }

        private List<decimal> GetTotaalRegel(int numberOfBedragen, List<VerslagRegelViewModel> verslagRegels)
        {
            var totaalRegel = new List<decimal>();

            for (var i = 0; i < numberOfBedragen; i++)
            {
                totaalRegel.Add(verslagRegels.Select(r => r.Bedragen[i]).Sum());
            }

            return totaalRegel;
        }

        private async Task<ResultatenrekeningViewModel> GetResultatenrekeningAsync()
        {
            var resultatenrekening = new ResultatenrekeningViewModel
            {
                Periodes = await GetResultatenrekeningPeriodesAsync()
            };
            resultatenrekening.VerslagRegels = await GetResultatenrekeningRegelsAsync(resultatenrekening.Periodes);
            resultatenrekening.TotaalRegel = GetTotaalRegel(resultatenrekening.Periodes.Count, resultatenrekening.VerslagRegels);
            return resultatenrekening;
        }

        private async Task<List<Tuple<DateTime, DateTime>>> GetResultatenrekeningPeriodesAsync()
        {
            //Resultatenrekening toont situatie voor ieder jaar van 1 januari tot en met 31 december.
            //Te beginnen bij de datum van de eerste transactie, en te eindigen bij de datum van de laatste transactie
            var datums = await GetBalansDatumsAsync();

            var periodes = new List<Tuple<DateTime, DateTime>>();
            if (datums.Count < 2)
            {
                //Geen reeks te maken met minder dan 2 datums
                return periodes;
            }

            //De startdatum is een dag na de vorige einddatum, om een periode van 1 januari t/m 31 december te krijgen
            var startDatum = datums.First().AddDays(1);

            //Begin met het overall totaal
            periodes.Add(new Tuple<DateTime, DateTime>(startDatum, datums.Last()));

            foreach (var eindDatum in datums.Skip(1))
            {
                periodes.Add(new Tuple<DateTime, DateTime>(startDatum, eindDatum));
                startDatum = eindDatum.AddDays(1);
            }
            return periodes;
        }

        private async Task<List<VerslagRegelViewModel>> GetResultatenrekeningRegelsAsync(List<Tuple<DateTime, DateTime>> periodes)
        {
            var regels = new List<VerslagRegelViewModel>();
            var categorieen = await _categorieenService.GetRawListForResultatenrekeningAsync();

            foreach (var categorie in categorieen)
            {
                var regel = new VerslagRegelViewModel
                {
                    Id = categorie.CategorieId,
                    Tekst = categorie.Omschrijving,
                    Bedragen = new List<decimal>()
                };

                regels.Add(regel);

                foreach (var periode in periodes)
                {
                    regel.Bedragen.Add(await _categorieenService.GetSaldoForPeriodeAsync(categorie, periode.Item1, periode.Item2));
                }
            }

            return regels;
        }
    }
}
