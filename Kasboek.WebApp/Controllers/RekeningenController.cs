using AutoMapper;
using Kasboek.WebApp.Models;
using Kasboek.WebApp.Models.RekeningenViewModels;
using Kasboek.WebApp.Services;
using Kasboek.WebApp.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kasboek.WebApp.Controllers
{
    public class RekeningenController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IRekeningenService _rekeningenService;
        private readonly ICategorieenService _categorieenService;
        private readonly ITransactiesService _transactiesService;
        private readonly IInstellingenService _instellingenService;

        public RekeningenController(IMapper mapper, IRekeningenService rekeningenService, ICategorieenService categorieenService, ITransactiesService transactiesService, IInstellingenService instellingenService)
        {
            _mapper = mapper;
            _rekeningenService = rekeningenService;
            _categorieenService = categorieenService;
            _transactiesService = transactiesService;
            _instellingenService = instellingenService;
        }

        // GET: Rekeningen
        public async Task<IActionResult> Index(int? afterId)
        {
            return View(_mapper.Map<IList<RekeningViewModel>>(await _rekeningenService.GetListAfterIdAsync(afterId)));
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
            await Task.WhenAll(
                SetSaldoAsync(rekening),
                SetTransactiesAsync(rekening),
                SetTransactiesAnchorAction());

            return View(rekening);
        }

        // GET: Rekeningen/Create
        public async Task<IActionResult> Create()
        {
            await SetSelectListsAsync((Rekening)null);
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
            await SetSelectListsAsync(rekening);
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
            await Task.WhenAll(
                SetSelectListsAsync(rekening),
                SetSaldoAsync(rekening));
            
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
            await Task.WhenAll(
                SetSelectListsAsync(rekening),
                SetSaldoAsync(rekening));

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
            var instellingen = await _instellingenService.GetSingleAsync();
            if (rekening == instellingen.PortemonneeRekening)
            {
                ModelState.AddModelError(string.Empty, "De rekening is ingesteld als de portemonnee rekening en kan daardoor niet verwijderd worden.");
                ViewBag.DisableForm = true;
            }
            await SetSaldoAsync(rekening);

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

        // POST: Rekeningen/Merge
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Merge(IList<RekeningViewModel> rekeningViewModels)
        {
            var rekeningIds = rekeningViewModels.Where(r => r.Selected).Select(r => r.RekeningId).ToList();
            if (rekeningIds.Count < 2)
            {
                //Kan niet mergen met minder dat 2 rekeningen
                return RedirectToAction(nameof(Index));
            }
            var rekeningen = await _rekeningenService.GetRawListByIdsAsync(rekeningIds);
            if (rekeningen.Count != rekeningIds.Count)
            {
                //Niet alle aangegeven rekeningen konden gevonden worden
                return NotFound();
            }

            var categorieIds = rekeningen.Where(r => r.StandaardCategorieId.HasValue).Select(r => r.StandaardCategorieId.Value).Distinct().ToList();

            var mergeViewModel = new MergeViewModel
            {
                RekeningIds = rekeningIds,
                Naam = string.Join(", ", rekeningen.Select(r => r.Naam)),
                Rekeningnummer = string.Join(", ", rekeningen.Select(r => r.Rekeningnummer).Where(s => !string.IsNullOrEmpty(s))),
                IsEigenRekening = rekeningen.Any(r => r.IsEigenRekening),//Als één van de rekeningen eigen is, is de gemergede standaard eigen
                StandaardCategorieId = categorieIds.Count == 1 ? categorieIds.First() : (int?)null, //Als er één categorie is, deze voorvullen
                CategorieIds = categorieIds
            };

            await Task.WhenAll(
                SetSelectListsAsync(mergeViewModel),
                SetSaldoAsync(mergeViewModel));

            return View(mergeViewModel);
        }

        // POST: Rekeningen/CompleteMerge
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteMerge(MergeViewModel mergeViewModel)
        {
            await PerformExtraValidationsAsync(mergeViewModel);
            if (ModelState.IsValid)
            {
                var rekeningen = await _rekeningenService.GetRawListByIdsAsync(mergeViewModel.RekeningIds);
                if (rekeningen.Count != mergeViewModel.RekeningIds.Count)
                {
                    //Niet alle aangegeven rekeningen konden gevonden worden
                    return NotFound();
                }

                //Alles naar de eerste toe zetten
                var uiteindelijkeRekening = rekeningen.First();
                var overigeRekeningen = rekeningen.Skip(1).ToList();
                _mapper.Map(mergeViewModel, uiteindelijkeRekening);
                foreach(var overigeRekening in overigeRekeningen)
                {
                    //Transactie verplaatsen meteen awaiten. 
                    //Anders zou een transactie met een boeking naar zichzelf mogelijk kunnen worden
                    //Vb: RekA, RekB, RekC allen naar RekA toe. Transactie tussen RekB en RekC zou asynchroon de boeking-naar-zichzelf-check kunnen missen
                    await MoveTranssactiesAsync(uiteindelijkeRekening, overigeRekening);
                    _rekeningenService.Remove(overigeRekening);
                }
                await _rekeningenService.SaveChangesAsync();

                return RedirectToAction(nameof(Details), new { id = uiteindelijkeRekening.RekeningId });
            }
            await SetSelectListsAsync(mergeViewModel);

            return View(nameof(Merge), mergeViewModel);
        }

        // POST: Rekeningen/SetStandaardCategorieOnTransactiesWithoutCategorie
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetStandaardCategorieOnTransactiesWithoutCategorie(int rekeningId)
        {
            var rekening = await _rekeningenService.GetRawSingleOrDefaultAsync(rekeningId);
            if (rekening == null || !rekening.StandaardCategorieId.HasValue)
            {
                return NotFound();
            }

            var transacties = await _transactiesService.GetRawListWithNoCategorieByRekeningAsync(rekening);
            foreach (var transactie in transacties)
            {
                transactie.CategorieId = rekening.StandaardCategorieId;
            }

            await _rekeningenService.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = rekeningId });
        }

        private async Task MoveTranssactiesAsync(Rekening uiteindelijkeRekening, Rekening overigeRekening)
        {
            var transacties = await _transactiesService.GetListByRekeningAsync(overigeRekening);
            foreach (var transactie in transacties)
            {
                if (transactie.VanRekening == uiteindelijkeRekening || transactie.NaarRekening == uiteindelijkeRekening)
                {
                    //Dit wordt een boeking naar zichzelf.
                    _transactiesService.Remove(transactie);
                }
                else if (transactie.VanRekening == overigeRekening)
                {
                    transactie.VanRekening = uiteindelijkeRekening;
                }
                else if (transactie.NaarRekening == overigeRekening)
                {
                    transactie.NaarRekening = uiteindelijkeRekening;
                }
            }
        }

        private async Task SetSelectListsAsync(MergeViewModel mergeViewModel)
        {
            ViewData["StandaardCategorieId"] = SelectListUtil.GetSelectList(await _categorieenService.GetSelectListForIdsAsync(mergeViewModel.CategorieIds), mergeViewModel.StandaardCategorieId);
        }

        private async Task SetSaldoAsync(MergeViewModel mergeViewModel)
        {
            List<Task<decimal>> saldoTasks = new List<Task<decimal>>();
            foreach(var rekeningId in mergeViewModel.RekeningIds)
            {
                var saldoTask = _rekeningenService.GetSaldoAsync(new Rekening { RekeningId = rekeningId });
                saldoTasks.Add(saldoTask);
            }
            var saldos = await Task.WhenAll(saldoTasks);
            mergeViewModel.Saldo = saldos.Sum();
        }

        private async Task SetSelectListsAsync(Rekening rekening)
        {
            ViewData["StandaardCategorieId"] = SelectListUtil.GetSelectList(await _categorieenService.GetSelectListAsync(), rekening?.StandaardCategorieId);
        }

        private async Task SetSaldoAsync(Rekening rekening)
        {
            ViewData["Saldo"] = await _rekeningenService.GetSaldoAsync(rekening);
        }

        private async Task SetTransactiesAsync(Rekening rekening)
        {
            ViewBag.Transacties = await _transactiesService.GetListByRekeningAsync(rekening);
        }

        private async Task SetTransactiesAnchorAction()
        {
            var instellingen = await _instellingenService.GetSingleAsync();
            ViewBag.TransactiesAnchorAction = instellingen.TransactieMeteenBewerken ? "Edit" : "Details";
        }

        private async Task PerformExtraValidationsAsync(Rekening rekening)
        {
            await Task.WhenAll(
                ValidateNaamInUseAsync(rekening.Naam, new List<int> { rekening.RekeningId }),
                ValidateRekeningnummerInUseAsync(rekening.Rekeningnummer, new List<int> { rekening.RekeningId }));
        }

        private async Task PerformExtraValidationsAsync(MergeViewModel mergeViewModel)
        {
            await Task.WhenAll(
                ValidateNaamInUseAsync(mergeViewModel.Naam, mergeViewModel.RekeningIds),
                ValidateRekeningnummerInUseAsync(mergeViewModel.Rekeningnummer, mergeViewModel.RekeningIds));
        }

        private async Task ValidateNaamInUseAsync(string naam, IList<int> ids)
        {
            if (await _rekeningenService.IsNaamInUseAsync(naam, ids))
                ModelState.AddModelError(nameof(Rekening.Naam), "Deze naam is al in gebruik.");
        }

        private async Task ValidateRekeningnummerInUseAsync(string rekeningnummer, IList<int> ids)
        {
            if (await _rekeningenService.IsRekeningnummerInUseAsync(rekeningnummer, ids))
                ModelState.AddModelError(nameof(Rekening.Rekeningnummer), "Dit rekeningnummer is al in gebruik.");
        }
    }
}
