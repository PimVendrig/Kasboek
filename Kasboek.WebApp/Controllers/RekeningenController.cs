﻿using Kasboek.WebApp.Models;
using Kasboek.WebApp.Services;
using Kasboek.WebApp.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Kasboek.WebApp.Controllers
{
    public class RekeningenController : Controller
    {
        private readonly IRekeningenService _rekeningenService;
        private readonly ICategorieenService _categorieenService;

        public RekeningenController(IRekeningenService rekeningenService, ICategorieenService categorieenService)
        {
            _rekeningenService = rekeningenService;
            _categorieenService = categorieenService;
        }

        // GET: Rekeningen
        public async Task<IActionResult> Index()
        {
            return View(await _rekeningenService.GetListAsync());
        }

        // GET: Rekeningen/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rekening = await _rekeningenService.GetSingleOrDefaultAsync(id.Value);
            if (rekening == null)
            {
                return NotFound();
            }

            ViewData["Saldo"] = await _rekeningenService.GetSaldoAsync(rekening);
            return View(rekening);
        }

        // GET: Rekeningen/Create
        public async Task<IActionResult> Create()
        {
            ViewData["StandaardCategorieId"] = SelectListUtil.GetSelectList(await _categorieenService.GetSelectListAsync());
            return View();
        }

        // POST: Rekeningen/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RekeningId,Naam,Rekeningnummer,IsEigenRekening,StandaardCategorieId")] Rekening rekening)
        {
            await PerformExtraValidationsAsync(rekening);
            if (ModelState.IsValid)
            {
                _rekeningenService.Add(rekening);
                await _rekeningenService.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = rekening.RekeningId });
            }
            ViewData["StandaardCategorieId"] = SelectListUtil.GetSelectList(await _categorieenService.GetSelectListAsync(), rekening.StandaardCategorieId);
            return View(rekening);
        }

        // GET: Rekeningen/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rekening = await _rekeningenService.GetRawSingleOrDefaultAsync(id.Value);
            if (rekening == null)
            {
                return NotFound();
            }
            ViewData["StandaardCategorieId"] = SelectListUtil.GetSelectList(await _categorieenService.GetSelectListAsync(), rekening.StandaardCategorieId);
            ViewData["Saldo"] = await _rekeningenService.GetSaldoAsync(rekening);
            return View(rekening);
        }

        // POST: Rekeningen/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RekeningId,Naam,Rekeningnummer,IsEigenRekening,StandaardCategorieId")] Rekening rekening)
        {
            if (id != rekening.RekeningId)
            {
                return NotFound();
            }

            await PerformExtraValidationsAsync(rekening);
            if (ModelState.IsValid)
            {
                try
                {
                    _rekeningenService.Update(rekening);
                    await _rekeningenService.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _rekeningenService.ExistsAsync(rekening.RekeningId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Details), new { id = rekening.RekeningId });
            }
            ViewData["StandaardCategorieId"] = SelectListUtil.GetSelectList(await _categorieenService.GetSelectListAsync(), rekening.StandaardCategorieId);
            ViewData["Saldo"] = await _rekeningenService.GetSaldoAsync(rekening);
            return View(rekening);
        }

        // GET: Rekeningen/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rekening = await _rekeningenService.GetSingleOrDefaultAsync(id.Value);
            if (rekening == null)
            {
                return NotFound();
            }

            ViewBag.DisableForm = false;
            if (await _rekeningenService.HasTransactiesAsync(rekening))
            {
                ModelState.AddModelError(string.Empty, "De rekening heeft nog transacties en kan daardoor niet verwijderd worden.");
                ViewBag.DisableForm = true;
            }
            ViewData["Saldo"] = await _rekeningenService.GetSaldoAsync(rekening);
            return View(rekening);
        }

        // POST: Rekeningen/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rekening = await _rekeningenService.GetRawSingleOrDefaultAsync(id);
            if (rekening == null)
            {
                return NotFound();
            }
            _rekeningenService.Remove(rekening);
            await _rekeningenService.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task PerformExtraValidationsAsync(Rekening rekening)
        {
            await Task.WhenAll(
                ValidateNaamInUseAsync(rekening),
                ValidateRekeningnummerInUseAsync(rekening));
        }

        private async Task ValidateNaamInUseAsync(Rekening rekening)
        {
            if (await _rekeningenService.IsNaamInUseAsync(rekening))
                ModelState.AddModelError(nameof(Rekening.Naam), "Deze naam is al in gebruik.");
        }

        private async Task ValidateRekeningnummerInUseAsync(Rekening rekening)
        {
            if (await _rekeningenService.IsRekeningnummerInUseAsync(rekening))
                ModelState.AddModelError(nameof(Rekening.Rekeningnummer), "Dit rekeningnummer is al in gebruik.");
        }
    }
}
