using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Kasboek.WebApp.Data;
using Kasboek.WebApp.Models;
using Kasboek.WebApp.Services;

namespace Kasboek.WebApp.Controllers
{
    public class InstellingenController : Controller
    {
        private readonly KasboekDbContext _context;

        public InstellingenController(KasboekDbContext context)
        {
            _context = context;
        }

        // GET: Instellingen
        public async Task<IActionResult> Index()
        {
            var instellingen = await _context.Instellingen
                .Include(i => i.StandaardVanRekening)
                .SingleAsync();

            return View(instellingen);
        }

        // GET: Instellingen/Edit
        public async Task<IActionResult> Edit()
        {
            var instellingen = await _context.Instellingen.SingleAsync();
            ViewData["StandaardVanRekeningId"] = SelectListService.GetRekeningen(_context, instellingen.StandaardVanRekeningId);
            return View(instellingen);
        }

        // POST: Instellingen/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("InstellingenId,StandaardVanRekeningId")] Instellingen instellingen)
        {
            var instellingenId = await _context.Instellingen.Select(i => i.InstellingenId).SingleAsync();

            if (instellingenId != instellingen.InstellingenId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(instellingen);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            ViewData["StandaardVanRekeningId"] = SelectListService.GetRekeningen(_context, instellingen.StandaardVanRekeningId);
            return View(instellingen);
        }
    }
}
