using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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

        public ImporterenController(ICategorieenService categorieenService, IRekeningenService rekeningenService)
        {
            _categorieenService = categorieenService;
            _rekeningenService = rekeningenService;
        }
        
        // GET: Importeren
        public IActionResult Index()
        {
            return View();
        }

        // GET: Importeren/Rekeningen
        public IActionResult Rekeningen()
        {
            return View();
        }

        // POST: Importeren/Rekeningen
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rekeningen(RekeningenViewModel rekeningenViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(rekeningenViewModel);
            }
            var fileContent = await FileHelpers.ProcessFormFile(rekeningenViewModel.UploadRekeningen, ModelState);

            if (!ModelState.IsValid)
            {
                return View(rekeningenViewModel);
            }

            var csvReader = new CsvReader(fileContent, ';', 4);
            var errors = csvReader.Validate();
            if (errors.Any())
            {
                foreach (var error in errors)
                {
                    ModelState.AddModelError(nameof(RekeningenViewModel.UploadRekeningen), error);
                }
                return View(rekeningenViewModel);
            }

            var importRows = csvReader.ReadAll();
            var messages = await ImportRekeningenAsync(importRows);
            if (messages.Any())
            {
                foreach (var message in messages)
                {
                    ModelState.AddModelError(nameof(RekeningenViewModel.UploadRekeningen), message);
                }
                rekeningenViewModel.ResultMessage = $"Niet alle {importRows.Count} rekeningen zijn succesvol geïmporteerd, zie bovenstaande meldingen.";
            }
            else
            {
                rekeningenViewModel.ResultMessage = $"Alle {importRows.Count} rekeningen zijn succesvol geïmporteerd.";
            }
            return View(rekeningenViewModel);
        }

        private async Task<List<string>> ImportRekeningenAsync(List<List<string>> importRows)
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
                    (var categorie, var categorieErrorMessages) = FindOrImportCategorie(categorieen, importRow[3].Trim());
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
                errorMessages.Add($"Categorie '{categorie.Omschrijving}' is overgeslagen, {validationResult.ErrorMessage}");
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

    }
}