using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kasboek.WebApp.Models;
using Kasboek.WebApp.Models.ImporterenViewModels;
using Kasboek.WebApp.Services;
using Kasboek.WebApp.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Kasboek.WebApp.Controllers
{
    public class ImporterenController : Controller
    {
        private readonly ICategorieenService _categorieenService;
        private readonly IRekeningenService _rekeningenService;
        private readonly ITransactiesService _transactiesService;
        private readonly IInstellingenService _instellingenService;

        public ImporterenController(ICategorieenService categorieenService, IRekeningenService rekeningenService, ITransactiesService transactiesService, IInstellingenService instellingenService)
        {
            _categorieenService = categorieenService;
            _rekeningenService = rekeningenService;
            _transactiesService = transactiesService;
            _instellingenService = instellingenService;
        }
        
        // GET: Importeren
        public IActionResult Index()
        {
            return View();
        }

        // GET: Importeren/RekeningenOudeApplicatie
        public IActionResult RekeningenOudeApplicatie()
        {
            return View(new UploadViewModel { Action = nameof(RekeningenOudeApplicatie) });
        }

        // POST: Importeren/RekeningenOudeApplicatie
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RekeningenOudeApplicatie(UploadViewModel uploadViewModel)
        {
            var importRows = await TryGetImportRowsAsync(uploadViewModel, ';', null, 4);
            if (importRows == null)
            {
                //Invalid uploadViewModel
                return View(uploadViewModel);
            }

            await FillNewDataLinks(uploadViewModel);

            var messages = await ImportRekeningenOudeApplicatieAsync(importRows);
            if (messages.Any())
            {
                foreach (var message in messages)
                {
                    ModelState.AddModelError(nameof(UploadViewModel.Bestand), message);
                }
                uploadViewModel.ResultMessage = $"Niet alle {importRows.Count} rekeningen zijn succesvol geïmporteerd, zie bovenstaande meldingen.";
            }
            else
            {
                uploadViewModel.ResultMessage = $"Alle {importRows.Count} rekeningen zijn succesvol geïmporteerd.";
            }
            return View(uploadViewModel);
        }

        // GET: Importeren/TransactiesOudeApplicatie
        public IActionResult TransactiesOudeApplicatie()
        {
            return View(new UploadViewModel { Action = nameof(TransactiesOudeApplicatie) });
        }

        // POST: Importeren/TransactiesOudeApplicatie
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TransactiesOudeApplicatie(UploadViewModel uploadViewModel)
        {
            var importRows = await TryGetImportRowsAsync(uploadViewModel, '\t', null, 6);
            if (importRows == null)
            {
                //Invalid uploadViewModel
                return View(uploadViewModel);
            }

            await FillNewDataLinks(uploadViewModel);

            var messages = await ImportTransactiesOudeApplicatieAsync(importRows);
            if (messages.Any())
            {
                foreach (var message in messages)
                {
                    ModelState.AddModelError(nameof(UploadViewModel.Bestand), message);
                }
                uploadViewModel.ResultMessage = $"Niet alle {importRows.Count} transacties zijn succesvol geïmporteerd, zie bovenstaande meldingen.";
            }
            else
            {
                uploadViewModel.ResultMessage = $"Alle {importRows.Count} transacties zijn succesvol geïmporteerd.";
            }
            return View(uploadViewModel);
        }

        // GET: Importeren/TransactiesRabobankCsv2013
        public IActionResult TransactiesRabobankCsv2013()
        {
            return View(new UploadViewModel { Action = nameof(TransactiesRabobankCsv2013) });
        }

        // POST: Importeren/TransactiesRabobankCsv2013
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TransactiesRabobankCsv2013(UploadViewModel uploadViewModel)
        {
            var importRows = await TryGetImportRowsAsync(uploadViewModel, ',', '"', 19);
            if (importRows == null)
            {
                //Invalid uploadViewModel
                return View(uploadViewModel);
            }

            await FillNewDataLinks(uploadViewModel);

            var messages = await ImportTransactiesRabobankCsv2013Async(importRows);
            if (messages.Any())
            {
                foreach (var message in messages)
                {
                    ModelState.AddModelError(nameof(UploadViewModel.Bestand), message);
                }
                uploadViewModel.ResultMessage = $"Niet alle {importRows.Count} transacties zijn succesvol geïmporteerd, zie bovenstaande meldingen.";
            }
            else
            {
                uploadViewModel.ResultMessage = $"Alle {importRows.Count} transacties zijn succesvol geïmporteerd.";
            }
            return View(uploadViewModel);
        }

        private async Task FillNewDataLinks(UploadViewModel uploadViewModel)
        {
            //Haal de huidige max ids op om de nieuwe items te kunnen weergeven
            var lastTransactieId = await _transactiesService.GetLastIdAsync() ?? 0;
            uploadViewModel.NewTransactiesLinkParameters = new Dictionary<string, string> { { "afterId", lastTransactieId.ToString() } };

            var lastRekeningId = await _rekeningenService.GetLastIdAsync() ?? 0;
            uploadViewModel.NewRekeningenLinkParameters = new Dictionary<string, string> { { "afterId", lastRekeningId.ToString() } };
        }

        private async Task<List<List<string>>> TryGetImportRowsAsync(UploadViewModel uploadViewModel, char separator, char? quote, int amountOfValues)
        {
            if (!ModelState.IsValid)
            {
                return null;
            }
            var fileContent = await FileHelpers.ProcessFormFile(uploadViewModel.Bestand, ModelState);

            if (!ModelState.IsValid)
            {
                return null;
            }

            var csvReader = new CsvReader(fileContent, separator, quote, amountOfValues);
            var errors = csvReader.Validate();
            if (errors.Any())
            {
                foreach (var error in errors)
                {
                    ModelState.AddModelError(nameof(UploadViewModel.Bestand), error);
                }
                return null;
            }
            return csvReader.ReadAll();
        }

        private async Task<List<string>> ImportRekeningenOudeApplicatieAsync(List<List<string>> importRows)
        {
            var messages = new List<string>();

            //Massa import, laad alle categorieën en rekeningen van te voren in.
            var rekeningenTask = _rekeningenService.GetListAsync();
            var categorieenTask = _categorieenService.GetListAsync();
            await Task.WhenAll(rekeningenTask, categorieenTask);
            var rekeningen = rekeningenTask.Result;
            var categorieen = categorieenTask.Result;

            foreach (var importRow in importRows)
            {
                var errorMessages = new List<string>();
                var infoMessages = new List<string>();

                var rekening = new Rekening
                {
                    Naam = importRow[1].Trim(),
                    Rekeningnummer = string.IsNullOrWhiteSpace(importRow[0]) ? null : importRow[0].Trim()
                };

                var isEigenRekeningImportValue = importRow[2].Trim();
                if (isEigenRekeningImportValue.Equals("Ja", StringComparison.InvariantCultureIgnoreCase))
                {
                    rekening.IsEigenRekening = true;
                }
                else if (!isEigenRekeningImportValue.Equals("Nee", StringComparison.InvariantCultureIgnoreCase))
                {
                    infoMessages.Add($"Rekening '{rekening.Naam}' bevat onbekende waarde '{isEigenRekeningImportValue}' voor is eigen rekening. Is eigen rekening is op uit gezet.");
                }

                var validationResults = new List<ValidationResult>();
                Validator.TryValidateObject(rekening, new ValidationContext(rekening, null, null), validationResults, true);
                foreach (var validationResult in validationResults)
                {
                    errorMessages.Add($"Rekening '{rekening.Naam}' is overgeslagen, {validationResult.ErrorMessage}");
                }

                if (!string.IsNullOrWhiteSpace(rekening.Rekeningnummer) && 
                    rekeningen.Any(r => r.Rekeningnummer != null && 
                        r.Rekeningnummer.Equals(rekening.Rekeningnummer, StringComparison.InvariantCultureIgnoreCase)))
                {
                    errorMessages.Add($"Rekening '{rekening.Naam}' is overgeslagen, er bestaat al een rekening met rekeningnummer '{rekening.Rekeningnummer}'.");
                }
                if (!string.IsNullOrWhiteSpace(rekening.Naam) && 
                    rekeningen.Any(r => r.Naam.Equals(rekening.Naam, StringComparison.InvariantCultureIgnoreCase)))
                {
                    errorMessages.Add($"Rekening '{rekening.Naam}' is overgeslagen, er bestaat al een rekening met deze naam.");
                }

                //Doe categorie, alleen indien er nog geen fouten zijn
                if (!errorMessages.Any())
                {
                    var standaardCategorieImportValue = importRow[3].Trim();
                    (var categorie, var categorieErrorMessages) = FindOrImportCategorie(categorieen, standaardCategorieImportValue);
                    rekening.StandaardCategorie = categorie;
                    foreach (var categorieErrorMessage in categorieErrorMessages)
                    {
                        infoMessages.Add($"Rekening '{rekening.Naam}' bevat een ongeldige standaard categorie, {categorieErrorMessage} standaard categorie is op leeg gezet.");
                    }
                }

                if (errorMessages.Any())
                {
                    messages.AddRange(errorMessages);
                }
                else
                {
                    messages.AddRange(infoMessages);
                    _rekeningenService.Add(rekening);
                    rekeningen.Add(rekening);
                }
            }
            await _rekeningenService.SaveChangesAsync();

            return messages;
        }

        private async Task<List<string>> ImportTransactiesOudeApplicatieAsync(List<List<string>> importRows)
        {
            var messages = new List<string>();

            //Massa import, laad alle categorieën en rekeningen van te voren in.
            var rekeningenTask = _rekeningenService.GetListAsync();
            var categorieenTask = _categorieenService.GetListAsync();
            await Task.WhenAll(rekeningenTask, categorieenTask);
            var rekeningen = rekeningenTask.Result;
            var categorieen = categorieenTask.Result;

            foreach (var importRow in importRows)
            {
                var errorMessages = new List<string>();
                var infoMessages = new List<string>();

                var transactie = new Transactie
                {
                    Omschrijving = string.IsNullOrWhiteSpace(importRow[4]) ? null : importRow[4].Trim().Replace("-ENTER-", Environment.NewLine)
                };

                var datumImportValue = importRow[0].Trim();
                if (DateTime.TryParseExact(datumImportValue, "d-M-yyyy", CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime datum))
                {
                    transactie.Datum = datum;
                }
                else
                {
                    errorMessages.Add($"Transactie '{transactie.Omschrijving}' is overgeslagen, '{datumImportValue}' is geen geldige datum.");
                }

                var bedragImportValue = importRow[1].Trim();
                if (decimal.TryParse(bedragImportValue, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal bedrag))
                {
                    transactie.Bedrag = bedrag;
                }
                else
                {
                    errorMessages.Add($"Transactie '{transactie.Omschrijving}' is overgeslagen, '{bedragImportValue}' is geen geldig bedrag.");
                }

                var vanRekeningImportValue = importRow[2].Trim();
                transactie.VanRekening = FindRekeningByNaam(rekeningen, vanRekeningImportValue);
                if (transactie.VanRekening == null)
                {
                    //Validatie werkt op Id, die niet null kan zijn (maar 0).
                    errorMessages.Add($"Transactie '{transactie.Omschrijving}' is overgeslagen, '{vanRekeningImportValue}' is geen geldige rekening.");
                }
                else
                {
                    //Validatie werkt op Id, zet voor nu expliciet (voor o.a. de Unlike validator)
                    transactie.VanRekeningId = transactie.VanRekening.RekeningId;
                }

                var naarRekeningImportValue = importRow[3].Trim();
                transactie.NaarRekening = FindRekeningByNaam(rekeningen, naarRekeningImportValue);
                if (transactie.NaarRekening == null)
                {
                    //Validatie werkt op Id, die niet null kan zijn (maar 0).
                    errorMessages.Add($"Transactie '{transactie.Omschrijving}' is overgeslagen, '{naarRekeningImportValue}' is geen geldige rekening.");
                }
                else
                {
                    //Validatie werkt op Id, zet voor nu expliciet (voor o.a. de Unlike validator)
                    transactie.NaarRekeningId = transactie.NaarRekening.RekeningId;
                }

                var validationResults = new List<ValidationResult>();
                Validator.TryValidateObject(transactie, new ValidationContext(transactie, null, null), validationResults, true);
                foreach (var validationResult in validationResults)
                {
                    errorMessages.Add($"Transactie '{transactie.Omschrijving}' is overgeslagen, {validationResult.ErrorMessage}");
                }

                //Doe categorie, alleen indien er nog geen fouten zijn
                if (!errorMessages.Any())
                {
                    var categorieImportValue = importRow[5].Trim();
                    (var categorie, var categorieErrorMessages) = FindOrImportCategorie(categorieen, categorieImportValue);
                    transactie.Categorie = categorie;
                    foreach (var categorieErrorMessage in categorieErrorMessages)
                    {
                        infoMessages.Add($"Transactie '{transactie.Omschrijving}' bevat een ongeldige categorie, {categorieErrorMessage} categorie is op leeg gezet.");
                    }
                }

                if (errorMessages.Any())
                {
                    messages.AddRange(errorMessages);
                }
                else
                {
                    messages.AddRange(infoMessages);
                    //DetermineCategorieAsync dient niet aangeroepen te worden. De oude applicatie heeft dit bepaald
                    _transactiesService.Add(transactie);
                }
            }
            await _transactiesService.SaveChangesAsync();

            return messages;
        }

        private async Task<List<string>> ImportTransactiesRabobankCsv2013Async(List<List<string>> importRows)
        {
            var messages = new List<string>();

            //Massa import, laad alle rekeningen en instellingen van te voren in.
            var rekeningenTask = _rekeningenService.GetListAsync();
            var instellingenTask = _instellingenService.GetSingleAsync();
            await Task.WhenAll(rekeningenTask, instellingenTask);
            var rekeningen = rekeningenTask.Result;
            var instellingen = instellingenTask.Result;

            var importedTransacties = new List<Transactie>();

            for (var rowIndex = 0; rowIndex < importRows.Count; rowIndex++)
            {
                var importRow = importRows[rowIndex];

                var errorMessages = new List<string>();
                var infoMessages = new List<string>();

                var eigenRekeningnummer = importRow[0].Trim();

                var datumImportValue = importRow[2].Trim();
                if (!DateTime.TryParseExact(datumImportValue, "yyyyMMdd", CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime datum))
                {
                    errorMessages.Add($"Transactie op regel {rowIndex + 1} is overgeslagen, '{datumImportValue}' is geen geldige datum.");
                }

                var isBijschrijving = false;
                var isBijschrijvingImportValue = importRow[3].Trim();
                if (isBijschrijvingImportValue.Equals("C", StringComparison.InvariantCultureIgnoreCase))
                {
                    isBijschrijving = true;
                }
                else if (!isBijschrijvingImportValue.Equals("D", StringComparison.InvariantCultureIgnoreCase))
                {
                    errorMessages.Add($"Transactie op regel {rowIndex + 1} is overgeslagen, '{isBijschrijvingImportValue}' is geen geldige waarde voor is bijschrijving.");
                }

                //Bedragen met een punt, gebruik InvariantCulture
                var bedragImportValue = importRow[4].Trim();
                if (!decimal.TryParse(bedragImportValue, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal bedrag))
                {
                    errorMessages.Add($"Transactie op regel {rowIndex + 1} is overgeslagen, '{bedragImportValue}' is geen geldig bedrag.");
                }

                var tegenRekeningnummer = importRow[5].Trim();
                var tegenRekeningNaam = importRow[6].Trim();
                var boekcode = importRow[8].Trim();

                var skipOmschrijving1 = false;
                if (boekcode == "ba" || boekcode == "bc")
                {
                    //Betaalautomaat, Betalen contactloos
                    //In deze gevallen is er geen tegenrekening, en staat de naam in Omschrijving1
                    if (tegenRekeningnummer != string.Empty || tegenRekeningNaam != string.Empty)
                    {
                        infoMessages.Add($"Transactie op regel {rowIndex + 1} heeft boekcode '{boekcode}' en zou daarom geen tegen rekeningnummer '{tegenRekeningnummer}' of tegen rekening naam '{tegenRekeningNaam}' moeten hebben. Deze zijn overschreven met omschrijving1.");
                        tegenRekeningnummer = string.Empty;
                    }

                    tegenRekeningNaam = importRow[10].Trim();
                    skipOmschrijving1 = true;
                }

                var omschrijvingSb = new StringBuilder();
                if (!skipOmschrijving1)
                {
                    omschrijvingSb.AppendLine(importRow[10].Trim());
                }
                omschrijvingSb.AppendLine(importRow[11].Trim());
                omschrijvingSb.AppendLine(importRow[12].Trim());
                omschrijvingSb.AppendLine(importRow[13].Trim());
                omschrijvingSb.AppendLine(importRow[14].Trim());
                omschrijvingSb.AppendLine(importRow[15].Trim());
                var omschrijving = omschrijvingSb.ToString().Trim();

                var eigenRekening = FindRekeningByRekeningnummer(rekeningen, eigenRekeningnummer);
                if (eigenRekening == null)
                {
                    errorMessages.Add($"Transactie op regel {rowIndex + 1} is overgeslagen, eigen rekening '{eigenRekeningnummer}' is niet gevonden.");
                }

                Rekening tegenRekening = null;
                if (boekcode == "kh" || boekcode == "ga" || boekcode == "gb")
                {
                    //Kasafhandeling, Geldautomaat Euro, Geldautomaat VV
                    tegenRekening = instellingen.PortemonneeRekening;
                    if (tegenRekening == null)
                    {
                        errorMessages.Add($"Transactie op regel {rowIndex + 1} is overgeslagen, er is geen portemonnee ingesteld.");
                    }
                }
                else
                {
                    (var rekening, var rekeningErrorMessages) = FindOrImportRekening(rekeningen, tegenRekeningnummer, tegenRekeningNaam);
                    tegenRekening = rekening;
                    foreach (var rekeningErrorMessage in rekeningErrorMessages)
                    {
                        errorMessages.Add($"Transactie op regel {rowIndex + 1} is overgeslagen, {rekeningErrorMessage}");
                    }
                }

                Transactie transactie = null;
                if (!errorMessages.Any())
                {
                    //De waarden zijn succesvol geparsed. Maak de transactie
                    transactie = new Transactie
                    {
                        Datum = datum,
                        Bedrag = bedrag,
                        VanRekening = isBijschrijving ? tegenRekening : eigenRekening,
                        NaarRekening = isBijschrijving ? eigenRekening : tegenRekening,
                        Omschrijving = string.IsNullOrWhiteSpace(omschrijving) ? null : omschrijving
                    };

                    //Validatie werkt op Id, zet voor nu expliciet (voor o.a. de Unlike validator)
                    transactie.VanRekeningId = transactie.VanRekening.RekeningId;
                    transactie.NaarRekeningId = transactie.NaarRekening.RekeningId;

                    await _transactiesService.DetermineCategorieAsync(transactie);

                    var validationResults = new List<ValidationResult>();
                    Validator.TryValidateObject(transactie, new ValidationContext(transactie, null, null), validationResults, true);
                    foreach (var validationResult in validationResults)
                    {
                        errorMessages.Add($"Transactie op regel {rowIndex + 1} is overgeslagen, {validationResult.ErrorMessage}");
                    }

                    if (importedTransacties.Any(t =>
                        t.Datum == transactie.Datum &&
                        t.Bedrag == transactie.Bedrag &&
                        t.VanRekening == transactie.VanRekening &&
                        t.NaarRekening == transactie.NaarRekening &&
                        t.Omschrijving == transactie.Omschrijving))
                    {
                        errorMessages.Add($"Transactie op regel {rowIndex + 1} is overgeslagen, er is in dit bestand al een andere transactie op {transactie.Datum:ddd d/M/yyyy} met {transactie.Bedrag:C} van '{transactie.VanRekening.Naam}' naar '{transactie.NaarRekening.Naam}' met omschrijving '{transactie.Omschrijving}'.");
                    }
                }

                if (errorMessages.Any())
                {
                    messages.AddRange(errorMessages);
                }
                else
                {
                    messages.AddRange(infoMessages);
                    _transactiesService.Add(transactie);
                    importedTransacties.Add(transactie);
                }
            }

            await _transactiesService.SaveChangesAsync();

            return messages;
        }

        private (Categorie categorie, List<string> errorMessages) FindOrImportCategorie(IList<Categorie> categorieen, string omschrijving)
        {
            var errorMessages = new List<string>();

            if (string.IsNullOrWhiteSpace(omschrijving))
            {
                //Geen categorie, vinden of importeren niet nodig
                return (null, errorMessages);
            }

            var categorie = categorieen.FirstOrDefault(r => r.Omschrijving.Equals(omschrijving, StringComparison.InvariantCultureIgnoreCase));
            if (categorie != null)
            {
                return (categorie, errorMessages);
            }

            //Niet gevonden, importeren
            categorie = new Categorie
            {
                Omschrijving = omschrijving
            };

            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(categorie, new ValidationContext(categorie, null, null), validationResults, true);
            foreach (var validationResult in validationResults)
            {
                errorMessages.Add($"Categorie '{categorie.Omschrijving}' is ongeldig, {validationResult.ErrorMessage}");
            }

            if (errorMessages.Any())
            {
                return (null, errorMessages);
            }
            else
            {
                _categorieenService.Add(categorie);
                categorieen.Add(categorie);
                return (categorie, errorMessages);
            }
        }

        private Rekening FindRekeningByNaam(IList<Rekening> rekeningen, string naam)
        {
            if (string.IsNullOrWhiteSpace(naam))
            {
                //Geen rekening, vinden onmogelijk
                return null;
            }

            var rekening = rekeningen.FirstOrDefault(r => r.Naam.Equals(naam, StringComparison.InvariantCultureIgnoreCase));
            return rekening;
        }

        private Rekening FindRekeningByRekeningnummer(IList<Rekening> rekeningen, string rekeningnummer)
        {
            if (string.IsNullOrWhiteSpace(rekeningnummer))
            {
                //Geen rekening, vinden onmogelijk
                return null;
            }

            var rekening = rekeningen.FirstOrDefault(r => 
                r.Rekeningnummer != null
                && r.Rekeningnummer.Equals(rekeningnummer, StringComparison.InvariantCultureIgnoreCase));
            return rekening;
        }

        private (Rekening rekening, List<string> errorMessages) FindOrImportRekening(IList<Rekening> rekeningen, string rekeningnummer, string naam)
        {
            var errorMessages = new List<string>();

            var rekening = FindRekeningByRekeningnummer(rekeningen, rekeningnummer);
            if (rekening != null)
            {
                return (rekening, errorMessages);
            }
            rekening = FindRekeningByNaam(rekeningen, naam);
            if (rekening != null)
            {
                return (rekening, errorMessages);
            }

            //Niet gevonden, importeren
            rekening = new Rekening
            {
                Naam = naam,
                Rekeningnummer = string.IsNullOrWhiteSpace(rekeningnummer) ? null : rekeningnummer,
                IsEigenRekening = false
            };

            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(rekening, new ValidationContext(rekening, null, null), validationResults, true);
            foreach (var validationResult in validationResults)
            {
                errorMessages.Add($"Rekening met rekeningnummer '{rekening.Rekeningnummer}' en naam '{rekening.Naam}' is ongeldig, {validationResult.ErrorMessage}");
            }

            if (errorMessages.Any())
            {
                return (null, errorMessages);
            }
            else
            {
                _rekeningenService.Add(rekening);
                rekeningen.Add(rekening);
                return (rekening, errorMessages);
            }
        }
    }
}