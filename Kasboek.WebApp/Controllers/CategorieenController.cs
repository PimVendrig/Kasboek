using Kasboek.WebApp.Models;
using Kasboek.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Kasboek.WebApp.Controllers
{
    public class CategorieenController : Controller
    {
        private readonly ICategorieenService _categorieenService;
        private readonly IRekeningenService _rekeningenService;
        private readonly ITransactiesService _transactiesService;

        public CategorieenController(ICategorieenService categorieenService, IRekeningenService rekeningenService, ITransactiesService transactiesService)
        {
            _categorieenService = categorieenService;
            _rekeningenService = rekeningenService;
            _transactiesService = transactiesService;
        }

        // GET: Categorieen
        public async Task<IActionResult> Index()
        {
            return View(await _categorieenService.GetListAsync());
        }

        // GET: Categorieen/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categorie = await _categorieenService.GetSingleOrDefaultAsync(id.Value);
            if (categorie == null)
            {
                return NotFound();
            }
            await Task.WhenAll(
                SetSaldoAsync(categorie),
                SetRekeningenMetStandaardCategorieAsync(categorie),
                SetTransactiesAsync(categorie));

            return View(categorie);
        }

        // GET: Categorieen/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categorieen/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategorieId,Omschrijving")] Categorie categorie)
        {
            await PerformExtraValidationsAsync(categorie);
            if (ModelState.IsValid)
            {
                _categorieenService.Add(categorie);
                await _categorieenService.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = categorie.CategorieId });
            }
            return View(categorie);
        }

        // GET: Categorieen/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categorie = await _categorieenService.GetRawSingleOrDefaultAsync(id.Value);
            if (categorie == null)
            {
                return NotFound();
            }
            await SetSaldoAsync(categorie);
            return View(categorie);
        }

        // POST: Categorieen/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategorieId,Omschrijving")] Categorie categorie)
        {
            if (id != categorie.CategorieId)
            {
                return NotFound();
            }

            await PerformExtraValidationsAsync(categorie);
            if (ModelState.IsValid)
            {
                try
                {
                    _categorieenService.Update(categorie);
                    await _categorieenService.SaveChangesAsync();
                    return RedirectToAction(nameof(Details), new { id = categorie.CategorieId });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _categorieenService.ExistsAsync(categorie.CategorieId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            await SetSaldoAsync(categorie);
            return View(categorie);
        }

        // GET: Categorieen/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categorie = await _categorieenService.GetSingleOrDefaultAsync(id.Value);
            if (categorie == null)
            {
                return NotFound();
            }
            await SetSaldoAsync(categorie);

            return View(categorie);
        }

        // POST: Categorieen/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var categorie = await _categorieenService.GetRawSingleOrDefaultAsync(id);
            if (categorie == null)
            {
                return NotFound();
            }
            _categorieenService.Remove(categorie);
            await _categorieenService.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task SetSaldoAsync(Categorie categorie)
        {
            ViewData["Saldo"] = await _categorieenService.GetSaldoAsync(categorie);
        }

        private async Task SetRekeningenMetStandaardCategorieAsync(Categorie categorie)
        {
            ViewBag.RekeningenMetStandaardCategorie = await _rekeningenService.GetListByStandaardCategorieAsync(categorie);
        }

        private async Task SetTransactiesAsync(Categorie categorie)
        {
            ViewBag.Transacties = await _transactiesService.GetListByCategorieAsync(categorie);
        }

        private async Task PerformExtraValidationsAsync(Categorie categorie)
        {
            if (await _categorieenService.IsOmschrijvingInUseAsync(categorie))
            {
                ModelState.AddModelError(nameof(Categorie.Omschrijving), "Deze omschrijving is al in gebruik.");
            }
        }
    }
}
