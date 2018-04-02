using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Kasboek.WebApp.Data;
using Kasboek.WebApp.Models;

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
            var kasboekDbContext = _context.Transacties.Include(t => t.NaarRekening).Include(t => t.VanRekening);
            return View(await kasboekDbContext.ToListAsync());
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
                .SingleOrDefaultAsync(m => m.TransactieId == id);
            if (transactie == null)
            {
                return NotFound();
            }

            return View(transactie);
        }

        // GET: Transacties/Create
        public IActionResult Create()
        {
            ViewData["NaarRekeningId"] = new SelectList(_context.Rekeningen, "RekeningId", "Naam");
            ViewData["VanRekeningId"] = new SelectList(_context.Rekeningen, "RekeningId", "Naam");
            return View();
        }

        // POST: Transacties/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TransactieId,Datum,Bedrag,Omschrijving,VanRekeningId,NaarRekeningId")] Transactie transactie)
        {
            if (ModelState.IsValid)
            {
                _context.Add(transactie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["NaarRekeningId"] = new SelectList(_context.Rekeningen, "RekeningId", "Naam", transactie.NaarRekeningId);
            ViewData["VanRekeningId"] = new SelectList(_context.Rekeningen, "RekeningId", "Naam", transactie.VanRekeningId);
            return View(transactie);
        }

        // GET: Transacties/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transactie = await _context.Transacties.SingleOrDefaultAsync(m => m.TransactieId == id);
            if (transactie == null)
            {
                return NotFound();
            }
            ViewData["NaarRekeningId"] = new SelectList(_context.Rekeningen, "RekeningId", "Naam", transactie.NaarRekeningId);
            ViewData["VanRekeningId"] = new SelectList(_context.Rekeningen, "RekeningId", "Naam", transactie.VanRekeningId);
            return View(transactie);
        }

        // POST: Transacties/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TransactieId,Datum,Bedrag,Omschrijving,VanRekeningId,NaarRekeningId")] Transactie transactie)
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
            ViewData["NaarRekeningId"] = new SelectList(_context.Rekeningen, "RekeningId", "Naam", transactie.NaarRekeningId);
            ViewData["VanRekeningId"] = new SelectList(_context.Rekeningen, "RekeningId", "Naam", transactie.VanRekeningId);
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
                .SingleOrDefaultAsync(m => m.TransactieId == id);
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
            var transactie = await _context.Transacties.SingleOrDefaultAsync(m => m.TransactieId == id);
            _context.Transacties.Remove(transactie);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransactieExists(int id)
        {
            return _context.Transacties.Any(e => e.TransactieId == id);
        }
    }
}
