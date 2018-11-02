using AutoMapper;
using Kasboek.WebApp.Models;
using Kasboek.WebApp.Models.CategorieenViewModels;
using Kasboek.WebApp.Models.RekeningenViewModels;
using Kasboek.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MergeViewModel = Kasboek.WebApp.Models.CategorieenViewModels.MergeViewModel;

namespace Kasboek.WebApp.Controllers
{
    public class CategorieenController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ICategorieenService _categorieenService;
        private readonly IRekeningenService _rekeningenService;
        private readonly ITransactiesService _transactiesService;
        private readonly IInstellingenService _instellingenService;

        public CategorieenController(IMapper mapper, ICategorieenService categorieenService, IRekeningenService rekeningenService, ITransactiesService transactiesService, IInstellingenService instellingenService)
        {
            _mapper = mapper;
            _categorieenService = categorieenService;
            _rekeningenService = rekeningenService;
            _transactiesService = transactiesService;
            _instellingenService = instellingenService;
        }

        // GET: Categorieen
        public async Task<IActionResult> Index()
        {
            return View(_mapper.Map<IList<CategorieViewModel>>(await _categorieenService.GetListAsync()));
        }

        // GET: Categorieen/Details/5
        public async Task<IActionResult> Details(int? id, DateTime? startDatum, DateTime? eindDatum)
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
                SetSaldoAsync(categorie, startDatum, eindDatum),
                SetRekeningenMetStandaardCategorieAsync(categorie),
                SetTransactiesAsync(categorie, startDatum, eindDatum),
                SetTransactiesAnchorAction());

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
            await SetSaldoAsync(categorie, null, null);
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
            await SetSaldoAsync(categorie, null, null);
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
            await SetSaldoAsync(categorie, null, null);

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

        // POST: Categorieen/Merge
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Merge(IList<CategorieViewModel> categorieViewModels)
        {
            var categorieIds = categorieViewModels.Where(c => c.Selected).Select(c => c.CategorieId).ToList();
            if (categorieIds.Count < 2)
            {
                //Kan niet mergen met minder dat 2 categorieën
                return RedirectToAction(nameof(Index));
            }
            var categorieen = await _categorieenService.GetRawListByIdsAsync(categorieIds);
            if (categorieen.Count != categorieIds.Count)
            {
                //Niet alle aangegeven categorieën konden gevonden worden
                return NotFound();
            }

            var mergeViewModel = new MergeViewModel
            {
                CategorieIds = categorieIds,
                Omschrijving = string.Join(", ", categorieen.Select(c => c.Omschrijving))
            };

            await SetSaldoAsync(mergeViewModel);

            return View(mergeViewModel);
        }

        // POST: Categorieen/CompleteMerge
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteMerge(MergeViewModel mergeViewModel)
        {
            await PerformExtraValidationsAsync(mergeViewModel);
            if (ModelState.IsValid)
            {
                var categorieen = await _categorieenService.GetRawListByIdsAsync(mergeViewModel.CategorieIds);
                if (categorieen.Count != mergeViewModel.CategorieIds.Count)
                {
                    //Niet alle aangegeven categorieën konden gevonden worden
                    return NotFound();
                }

                //Alles naar de eerste toe zetten
                var uiteindelijkeCategorie = categorieen.First();
                var overigeCategorieen = categorieen.Skip(1).ToList();
                _mapper.Map(mergeViewModel, uiteindelijkeCategorie);
                List<Task> moveTasks = new List<Task>();
                foreach (var overigeCategorie in overigeCategorieen)
                {
                    moveTasks.Add(MoveTransactiesAsync(uiteindelijkeCategorie, overigeCategorie));
                    moveTasks.Add(MoveRekeningenAsync(uiteindelijkeCategorie, overigeCategorie));
                }
                await Task.WhenAll(moveTasks);
                await _categorieenService.SaveChangesAsync();

                //Splits verwijderen en verplaatsen in twee SaveChanges
                //Transacties nullificeren de Categorie ipv verplaatsen indien voor de Remove niet eerst een SaveChanges wordt gedaan.
                //Rekeningen hebben dit issue niet, alleen Transacties. Nog uitzoeken wat hier mee aan de hand is.
                //Dit heeft nu als bijeffect dat de uiteindelijke omschijving niet een van de te verwijderen mag hebben (unique constraint exception)
                foreach (var overigeCategorie in overigeCategorieen)
                {
                    _categorieenService.Remove(overigeCategorie);
                }
                await _categorieenService.SaveChangesAsync();

                return RedirectToAction(nameof(Details), new { id = uiteindelijkeCategorie.CategorieId });
            }
            return View(nameof(Merge), mergeViewModel);
        }

        private async Task MoveTransactiesAsync(Categorie uiteindelijkeCategorie, Categorie overigeCategorie)
        {
            var transacties = await _transactiesService.GetListByCategorieAsync(overigeCategorie, null, null);
            foreach (var transactie in transacties)
            {
                transactie.Categorie = uiteindelijkeCategorie;
            }
        }

        private async Task MoveRekeningenAsync(Categorie uiteindelijkeCategorie, Categorie overigeCategorie)
        {
            var rekeningen = await _rekeningenService.GetListByStandaardCategorieAsync(overigeCategorie);
            foreach (var rekening in rekeningen)
            {
                rekening.StandaardCategorie = uiteindelijkeCategorie;
            }
        }

        private async Task SetSaldoAsync(MergeViewModel mergeViewModel)
        {
            List<Task<decimal>> saldoTasks = new List<Task<decimal>>();
            foreach (var categorieId in mergeViewModel.CategorieIds)
            {
                var saldoTask = _categorieenService.GetSaldoAsync(new Categorie { CategorieId = categorieId });
                saldoTasks.Add(saldoTask);
            }
            var saldos = await Task.WhenAll(saldoTasks);
            mergeViewModel.Saldo = saldos.Sum();
        }

        private async Task SetSaldoAsync(Categorie categorie, DateTime? startDatum, DateTime? eindDatum)
        {
            ViewData["Saldo"] = await _categorieenService.GetSaldoForPeriodeAsync(categorie, startDatum, eindDatum);
        }

        private async Task SetRekeningenMetStandaardCategorieAsync(Categorie categorie)
        {
            ViewBag.RekeningenMetStandaardCategorie = _mapper.Map<IList<RekeningViewModel>>(await _rekeningenService.GetListByStandaardCategorieAsync(categorie));
        }

        private async Task SetTransactiesAsync(Categorie categorie, DateTime? startDatum, DateTime? eindDatum)
        {
            ViewBag.Transacties = await _transactiesService.GetListByCategorieAsync(categorie, startDatum, eindDatum);
        }

        private async Task SetTransactiesAnchorAction()
        {
            var instellingen = await _instellingenService.GetSingleAsync();
            ViewBag.TransactiesAnchorAction = instellingen.TransactieMeteenBewerken ? "Edit" : "Details";
        }

        private async Task PerformExtraValidationsAsync(MergeViewModel mergeViewModel)
        {
            await ValidateOmschrijvingInUseAsync(mergeViewModel.Omschrijving, mergeViewModel.CategorieIds);
        }

        private async Task PerformExtraValidationsAsync(Categorie categorie)
        {
            await ValidateOmschrijvingInUseAsync(categorie.Omschrijving, new List<int> { categorie.CategorieId });
        }

        private async Task ValidateOmschrijvingInUseAsync(string omschrijving, IList<int> ids)
        {
            if (await _categorieenService.IsOmschrijvingInUseAsync(omschrijving, ids))
                ModelState.AddModelError(nameof(Categorie.Omschrijving), "Deze omschrijving is al in gebruik.");
        }

    }
}
