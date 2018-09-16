using System.Linq;
using System.Threading.Tasks;
using Kasboek.WebApp.Models.ImporterenViewModels;
using Kasboek.WebApp.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Kasboek.WebApp.Controllers
{
    public class ImporterenController : Controller
    {
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

            var csvReader = new CsvReader(fileContent, ';');
            var errors = csvReader.Validate();
            if (errors.Any())
            {
                foreach (var error in errors)
                {
                    ModelState.AddModelError(nameof(RekeningenViewModel.UploadRekeningen), error);
                }
                return View(rekeningenViewModel);
            }

            var rekeningRows = csvReader.ReadAll();

            //TODO: Importeren
            rekeningenViewModel.ResultMessage = $"Er zijn {rekeningRows.Count} rekeningen succesvol gelezen.";
            return View(rekeningenViewModel);
        }

    }
}