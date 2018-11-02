using Kasboek.WebApp.Models;
using Kasboek.WebApp.Services;
using Kasboek.WebApp.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Kasboek.WebApp.Controllers
{
    public class TransactiesController : Controller
    {
        private readonly ITransactiesService _transactiesService;
        private readonly ICategorieenService _categorieenService;
        private readonly IRekeningenService _rekeningenService;
        private readonly IInstellingenService _instellingenService;

        public TransactiesController(ITransactiesService transactiesService, ICategorieenService categorieenService, IRekeningenService rekeningenService, IInstellingenService instellingenService)
        {
            _transactiesService = transactiesService;
            _categorieenService = categorieenService;
            _rekeningenService = rekeningenService;
            _instellingenService = instellingenService;
        }

        // GET: Transacties
        public async Task<IActionResult> Index(int? afterId, bool? hasCategorie, DateTime? startDatum, DateTime? eindDatum, DateTime? nearDatum, decimal? vanafBedrag)
        {
            var instellingen = await _instellingenService.GetSingleAsync();
            ViewBag.TransactiesAnchorAction = instellingen.TransactieMeteenBewerken ? "Edit" : "Details";
            ViewBag.NearDatum = nearDatum;
            return View(await _transactiesService.GetListWithFilterAsync(afterId, hasCategorie, startDatum, eindDatum, nearDatum, vanafBedrag));
        }

        // GET: Transacties/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transactie = await _transactiesService.GetSingleOrDefaultAsync(id.Value);
            if (transactie == null)
            {
                return NotFound();
            }

            return View(transactie);
        }

        // GET: Transacties/Create
        public async Task<IActionResult> Create()
        {
            //Bij aanmaken transactie deze voorvullen
            var instellingen = await _instellingenService.GetSingleAsync();
            var transactie = new Transactie
            {
                Datum = DateTime.Today,
                Bedrag = 0,
                VanRekeningId = instellingen.StandaardVanRekeningId ?? 0
            };

            await SetSelectListsAsync(transactie);
            return View(transactie);
        }

        // POST: Transacties/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TransactieId,Datum,Bedrag,Omschrijving,VanRekeningId,NaarRekeningId,CategorieId")] Transactie transactie)
        {
            if (ModelState.IsValid)
            {
                await _transactiesService.DetermineCategorieAsync(transactie);
                _transactiesService.Add(transactie);
                await _transactiesService.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = transactie.TransactieId });
            }
            await SetSelectListsAsync(transactie);
            return View(transactie);
        }

        // GET: Transacties/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transactie = await _transactiesService.GetRawSingleOrDefaultAsync(id.Value);
            if (transactie == null)
            {
                return NotFound();
            }
            await SetSelectListsAsync(transactie);
            return View(transactie);
        }

        // POST: Transacties/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TransactieId,Datum,Bedrag,Omschrijving,VanRekeningId,NaarRekeningId,CategorieId")] Transactie transactie)
        {
            if (id != transactie.TransactieId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _transactiesService.Update(transactie);
                    await _transactiesService.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _transactiesService.ExistsAsync(transactie.TransactieId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Details), new { id = transactie.TransactieId });
            }
            await SetSelectListsAsync(transactie);
            return View(transactie);
        }

        // GET: Transacties/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transactie = await _transactiesService.GetSingleOrDefaultAsync(id.Value);
            if (transactie == null)
            {
                return NotFound();
            }

            return View(transactie);
        }

        // POST: Transacties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transactie = await _transactiesService.GetRawSingleOrDefaultAsync(id);
            if (transactie == null)
            {
                return NotFound();
            }
            _transactiesService.Remove(transactie);
            await _transactiesService.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task SetSelectListsAsync(Transactie transactie)
        {
            var rekeningenTask = _rekeningenService.GetSelectListAsync();
            var categorieenTask = _categorieenService.GetSelectListAsync();
            await Task.WhenAll(rekeningenTask, categorieenTask);
            ViewData["NaarRekeningId"] = SelectListUtil.GetSelectList(rekeningenTask.Result, transactie.NaarRekeningId);
            ViewData["VanRekeningId"] = SelectListUtil.GetSelectList(rekeningenTask.Result, transactie.VanRekeningId);
            ViewData["CategorieId"] = SelectListUtil.GetSelectList(categorieenTask.Result, transactie.CategorieId);
        }
    }
}
