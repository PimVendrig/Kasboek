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
        public async Task<IActionResult> Index(DateTime? peilDatum)
        {
            var verslag = new VerslagViewModel
            {
                Balans = await GetBalansAsync(peilDatum),
                Resultatenrekening = await GetResultatenrekeningAsync(peilDatum)
            };
            return View(verslag);
        }

        private async Task<BalansViewModel> GetBalansAsync(DateTime? peilDatum)
        {
            var balans = new BalansViewModel
            {
                Datums = await GetBalansDatumsAsync(peilDatum)
            };
            balans.VerslagRegels = await GetBalansRegelsAsync(balans.Datums);
            balans.TotaalRegel = GetTotaalRegel(balans.Datums.Count, balans.VerslagRegels);
            return balans;
        }

        private async Task<List<DateTime>> GetBalansDatumsAsync(DateTime? peilDatum)
        {
            List<DateTime> datums = new List<DateTime>();

            //Balans toont situatie voor ieder jaar op 31 december.
            //Te beginnen bij de datum van de eerste transactie, en te eindigen bij de datum van de laatste transactie
            //Bij het opgeven van een peildatum tonen we de situatie op het eind van iedere maand in dat jaar.
            var firstDatum = await _transactiesService.GetFirstTransactieDatumAsync();

            if (!firstDatum.HasValue)
            {
                //Als de eerste datum geen waarde heeft, is er niets
                return datums;
            }
            if (peilDatum.HasValue && firstDatum.Value > peilDatum.Value)
            {
                //Als de eerste datum na de peildatum ligt, is er niets
                return datums;
            }

            var lastDatum = peilDatum.HasValue ? peilDatum : await _transactiesService.GetLastTransactieDatumAsync();

            if (!peilDatum.HasValue)
            {
                //Datums voor ieder jaar
                datums.Add(firstDatum.Value);
                for (var lastDayOfYear = new DateTime(firstDatum.Value.Year, 12, 31); lastDayOfYear < lastDatum.Value; lastDayOfYear = lastDayOfYear.AddYears(1))
                {
                    //Eerste datum kan evt 31 december zijn, niet dubbel toevoegen
                    if (!datums.Contains(lastDayOfYear))
                    {
                        datums.Add(lastDayOfYear);
                    }
                }
            }
            else
            {
                //Datums voor iedere maand in het peiljaar. Niet nodig om te kijken naar firstDatum
                for (var lastDayOfMonth = new DateTime(lastDatum.Value.Year - 1, 12, 31); lastDayOfMonth < lastDatum.Value; lastDayOfMonth = GetNextMonthsLastDay(lastDayOfMonth))
                {
                    datums.Add(lastDayOfMonth);
                }
            }

            //Laatste datum kan evt dezelfde datum als eerste datum zijn, dus niet dubbel toevoegen
            if (!datums.Contains(lastDatum.Value))
            {
                datums.Add(lastDatum.Value);
            }

            return datums;
        }

        private DateTime GetNextMonthsLastDay(DateTime currentMonthsLastDay)
        {
            var nextMonthsFirstDay = currentMonthsLastDay.AddDays(1);
            var daysInNextMonth = DateTime.DaysInMonth(nextMonthsFirstDay.Year, nextMonthsFirstDay.Month);
            return new DateTime(nextMonthsFirstDay.Year, nextMonthsFirstDay.Month, daysInNextMonth);
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

        private async Task<ResultatenrekeningViewModel> GetResultatenrekeningAsync(DateTime? peilDatum)
        {
            var resultatenrekening = new ResultatenrekeningViewModel
            {
                Periodes = await GetResultatenrekeningPeriodesAsync(peilDatum)
            };
            resultatenrekening.VerslagRegels = await GetResultatenrekeningRegelsAsync(resultatenrekening.Periodes);
            resultatenrekening.TotaalRegel = GetTotaalRegel(resultatenrekening.Periodes.Count, resultatenrekening.VerslagRegels);
            resultatenrekening.TotaalPerMaandRegel = GetTotaalPerMaandRegel(resultatenrekening.Periodes, resultatenrekening.TotaalRegel);
            return resultatenrekening;
        }

        private async Task<List<Tuple<DateTime, DateTime>>> GetResultatenrekeningPeriodesAsync(DateTime? peilDatum)
        {
            var periodes = new List<Tuple<DateTime, DateTime>>();

            //Resultatenrekening toont situatie voor ieder jaar van 1 januari tot en met 31 december.
            //Te beginnen bij de datum van de eerste transactie, en te eindigen bij de datum van de laatste transactie
            //Bij het opgeven van een peildatum tonen we de situatie van iedere maand in dat jaar.
            var datums = await GetBalansDatumsAsync(peilDatum);

            if (datums.Count < 2)
            {
                //Geen reeks te maken met minder dan 2 datums
                return periodes;
            }

            //De startdatum is een dag na de vorige einddatum, om een periode van 1 januari t/m 31 december te krijgen
            //Of een periode van 1 januari t/m 31 januari etc
            var startDatum = datums.First().AddDays(1);
            var lastDatum = datums.Last();

            if (peilDatum.HasValue && !(lastDatum.Month == 12 && lastDatum.Day == 31))
            {
                //Indien we een jaar tonen, en het is geen volledig kalenderjaar (bijv. weergave t/m augustus)
                //Tonen we voor het overall totaal (year-to-date) ook de situatie van een volledig jaar
                periodes.Add(new Tuple<DateTime, DateTime>(lastDatum.AddDays(1).AddYears(-1), lastDatum));
            }

            //Begin met het overall totaal
            periodes.Add(new Tuple<DateTime, DateTime>(startDatum, lastDatum));

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
                    Bedragen = new List<decimal>(),
                    BedragenPerMaand = new List<decimal?>()
                };

                foreach (var periode in periodes)
                {
                    var bedrag = await _categorieenService.GetSaldoForPeriodeAsync(categorie, periode.Item1, periode.Item2);
                    regel.Bedragen.Add(bedrag);

                    int aantalMaanden = GetAantalMaanden(periode);
                    regel.BedragenPerMaand.Add(aantalMaanden == 1 ? (decimal?)null : bedrag / aantalMaanden);
                }

                if (regel.Bedragen.Any(b => b != 0M))
                {
                    //Laat regels waarin nergens een bedrag staat er uit
                    regels.Add(regel);
                }

            }

            return regels;
        }

        private int GetAantalMaanden(Tuple<DateTime, DateTime> periode)
        {
            //Periodes lopen van 1-1 t/m 31-12. Dit geeft een verschil van 11 maanden, dus één optellen.
            //Ook in het geval van 1-1 t/m 15-1, willen we dit als 1 maand zien.
            return 1 + ((periode.Item2.Year - periode.Item1.Year) * 12) + periode.Item2.Month - periode.Item1.Month;
        }

        private List<decimal?> GetTotaalPerMaandRegel(List<Tuple<DateTime, DateTime>> periodes, List<decimal> totaalRegel)
        {
            var totaalPerMaandRegel = new List<decimal?>();

            for (var i = 0; i < periodes.Count; i++)
            {
                var totaalBedrag = totaalRegel[i];
                var periode = periodes[i];
                int aantalMaanden = GetAantalMaanden(periode);
                totaalPerMaandRegel.Add(aantalMaanden == 1 ? (decimal?) null : totaalBedrag / aantalMaanden);
            }

            return totaalPerMaandRegel;
        }
    }
}
