using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Kasboek.WebApp.Data;
using Kasboek.WebApp.Models;
using Kasboek.WebApp.Services;
using System;

namespace Kasboek.WebApp.Controllers
{
    public class TransactiesController : Controller
    {
        private readonly KasboekDbContext _context;

        public TransactiesController(KasboekDbContext context)
        {
            _context = context;
        }

        // GET: Transacties
        public async Task<IActionResult> Index()
        {
            var transacties = _context.Transacties
                .Include(t => t.NaarRekening)
                .Include(t => t.VanRekening)
                .Include(t => t.Categorie)
                .OrderByDescending(t => t.Datum)
                .ThenByDescending(t => t.TransactieId);
            return View(await transacties.ToListAsync());
        }

        // GET: Transacties/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transactie = await _context.Transacties
                .Include(t => t.NaarRekening)
                .Include(t => t.VanRekening)
                .Include(t => t.Categorie)
                .SingleOrDefaultAsync(t => t.TransactieId == id);
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
            var instellingen = await _context.Instellingen.SingleAsync();
            var transactie = new Transactie
            {
                Datum = DateTime.Today,
                Bedrag = 0,
                VanRekeningId = instellingen.StandaardVanRekeningId ?? 0
            };

            ViewData["NaarRekeningId"] = SelectListService.GetRekeningen(_context);
            ViewData["VanRekeningId"] = SelectListService.GetRekeningen(_context, transactie.VanRekeningId);
            ViewData["CategorieId"] = SelectListService.GetCategorieen(_context);
            return View(transactie);
        }

        // POST: Transacties/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TransactieId,Datum,Bedrag,Omschrijving,VanRekeningId,NaarRekeningId,CategorieId")] Transactie transactie)
        {
            if (ModelState.IsValid)
            {
                _context.Add(transactie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["NaarRekeningId"] = SelectListService.GetRekeningen(_context, transactie.NaarRekeningId);
            ViewData["VanRekeningId"] = SelectListService.GetRekeningen(_context, transactie.VanRekeningId);
            ViewData["CategorieId"] = SelectListService.GetCategorieen(_context, transactie.CategorieId);
            return View(transactie);
        }

        // GET: Transacties/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transactie = await _context.Transacties.SingleOrDefaultAsync(t => t.TransactieId == id);
            if (transactie == null)
            {
                return NotFound();
            }
            ViewData["NaarRekeningId"] = SelectListService.GetRekeningen(_context, transactie.NaarRekeningId);
            ViewData["VanRekeningId"] = SelectListService.GetRekeningen(_context, transactie.VanRekeningId);
            ViewData["CategorieId"] = SelectListService.GetCategorieen(_context, transactie.CategorieId);
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
                    _context.Update(transactie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactieExists(transactie.TransactieId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["NaarRekeningId"] = SelectListService.GetRekeningen(_context, transactie.NaarRekeningId);
            ViewData["VanRekeningId"] = SelectListService.GetRekeningen(_context, transactie.VanRekeningId);
            ViewData["CategorieId"] = SelectListService.GetCategorieen(_context, transactie.CategorieId);
            return View(transactie);
        }

        // GET: Transacties/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transactie = await _context.Transacties
                .Include(t => t.NaarRekening)
                .Include(t => t.VanRekening)
                .Include(t => t.Categorie)
                .SingleOrDefaultAsync(t => t.TransactieId == id);
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
            var transactie = await _context.Transacties.SingleOrDefaultAsync(t => t.TransactieId == id);
            if (transactie == null)
            {
                return NotFound();
            }
            _context.Transacties.Remove(transactie);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransactieExists(int id)
        {
            return _context.Transacties.Any(t => t.TransactieId == id);
        }
    }
}
