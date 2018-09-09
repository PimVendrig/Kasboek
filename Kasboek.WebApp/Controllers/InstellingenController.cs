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
            ViewData["StandaardVanRekeningId"] = SelectListUtil.GetSelectList(await _rekeningenService.GetSelectListAsync(), instellingen.StandaardVanRekeningId);
            return View(instellingen);
        }

        // POST: Instellingen/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("InstellingenId,StandaardVanRekeningId")] Instellingen instellingen)
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
            ViewData["StandaardVanRekeningId"] = SelectListUtil.GetSelectList(await _rekeningenService.GetSelectListAsync(), instellingen.StandaardVanRekeningId);
            return View(instellingen);
        }
    }
}
