using Kasboek.WebApp.Models;
using Kasboek.WebApp.Services;
using Kasboek.WebApp.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Kasboek.WebApp.Controllers
{
    public class InstellingenController : Controller
    {
        private readonly IInstellingenService _instellingenService;
        private readonly IRekeningenService _rekeningenService;

        public InstellingenController(IInstellingenService instellingenService, IRekeningenService rekeningenService)
        {
            _instellingenService = instellingenService;
            _rekeningenService = rekeningenService;
        }

        // GET: Instellingen
        public async Task<IActionResult> Index()
        {
            return View(await _instellingenService.GetSingleAsync());
        }

        // GET: Instellingen/Edit
        public async Task<IActionResult> Edit()
        {
            var instellingen = await _instellingenService.GetRawSingleAsync();
            await SetSelectListsAsync(instellingen);
            return View(instellingen);
        }

        // POST: Instellingen/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("InstellingenId,StandaardVanRekeningId,TransactieMeteenBewerken")] Instellingen instellingen)
        {
            var instellingenId = await _instellingenService.GetIdAsync();

            if (instellingenId != instellingen.InstellingenId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _instellingenService.Update(instellingen);
                await _instellingenService.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            await SetSelectListsAsync(instellingen);
            return View(instellingen);
        }

        private async Task SetSelectListsAsync(Instellingen instellingen)
        {
            ViewData["StandaardVanRekeningId"] = SelectListUtil.GetSelectList(await _rekeningenService.GetSelectListAsync(), instellingen.StandaardVanRekeningId);
        }
    }
}
