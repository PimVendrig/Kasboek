using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Kasboek.WebApp.Data;
using Kasboek.WebApp.Models;
using Kasboek.WebApp.Services;

namespace Kasboek.WebApp.Controllers
{
    public class RekeningenController : Controller
    {
        private readonly KasboekDbContext _context;

        public RekeningenController(KasboekDbContext context)
        {
            _context = context;
        }

        // GET: Rekeningen
        public async Task<IActionResult> Index()
        {
            var rekeningen = _context.Rekeningen
                .Include(r => r.StandaardCategorie)
                .OrderByDescending(r => r.IsEigenRekening)
                .ThenBy(r => r.Naam);
            return View(await rekeningen.ToListAsync());
        }

        // GET: Rekeningen/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rekening = await _context.Rekeningen
                .Include(r => r.StandaardCategorie)
                .SingleOrDefaultAsync(r => r.RekeningId == id);
            if (rekening == null)
            {
                return NotFound();
            }

            ViewData["Saldo"] = GetSaldo(rekening);
            return View(rekening);
        }

        public decimal GetSaldo(Rekening rekening)
        {
            var vanTotaal = _context.Transacties
                .Where(t => t.VanRekening == rekening)
                .Sum(t => t.Bedrag);
            var naarTotaal = _context.Transacties
                .Where(t => t.NaarRekening == rekening)
                .Sum(t => t.Bedrag);
            return naarTotaal - vanTotaal;
        }

        // GET: Rekeningen/Create
        public IActionResult Create()
        {
            ViewData["StandaardCategorieId"] = SelectListService.GetCategorieen(_context);
            return View();
        }

        // POST: Rekeningen/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RekeningId,Naam,Rekeningnummer,IsEigenRekening,StandaardCategorieId")] Rekening rekening)
        {
            if (ModelState.IsValid)
            {
                _context.Add(rekening);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["StandaardCategorieId"] = SelectListService.GetCategorieen(_context, rekening.StandaardCategorieId);
            return View(rekening);
        }

        // GET: Rekeningen/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rekening = await _context.Rekeningen.SingleOrDefaultAsync(r => r.RekeningId == id);
            if (rekening == null)
            {
                return NotFound();
            }
            ViewData["StandaardCategorieId"] = SelectListService.GetCategorieen(_context, rekening.StandaardCategorieId);
            ViewData["Saldo"] = GetSaldo(rekening);
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

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rekening);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RekeningExists(rekening.RekeningId))
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
            ViewData["StandaardCategorieId"] = SelectListService.GetCategorieen(_context, rekening.StandaardCategorieId);
            ViewData["Saldo"] = GetSaldo(rekening);
            return View(rekening);
        }

        // GET: Rekeningen/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rekening = await _context.Rekeningen
                .Include(r => r.StandaardCategorie)
                .SingleOrDefaultAsync(r => r.RekeningId == id);
            if (rekening == null)
            {
                return NotFound();
            }

            ViewData["Saldo"] = GetSaldo(rekening);
            return View(rekening);
        }

        // POST: Rekeningen/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rekening = await _context.Rekeningen.SingleOrDefaultAsync(r => r.RekeningId == id);
            if (rekening == null)
            {
                return NotFound();
            }
            _context.Rekeningen.Remove(rekening);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RekeningExists(int id)
        {
            return _context.Rekeningen.Any(r => r.RekeningId == id);
        }
    }
}
