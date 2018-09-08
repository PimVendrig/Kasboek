using Kasboek.WebApp.Data;
using Kasboek.WebApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kasboek.WebApp.Services
{
    public class CategorieenService : CrudService<Categorie>, ICategorieenService
    {

        public CategorieenService(KasboekDbContext context) : base(context)
        {
        }

        public async override Task<IList<Categorie>> GetListAsync()
        {
            return await _context.Categorieen
                .OrderBy(c => c.Omschrijving)
                .ToListAsync();
        }

        public async override Task<Categorie> GetSingleOrDefaultAsync(int id)
        {
            return await _context.Categorieen
                .SingleOrDefaultAsync(c => c.CategorieId == id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Categorieen
                .AnyAsync(c => c.CategorieId == id);
        }

        public async Task<bool> IsOmschrijvingInUseAsync(Categorie categorie)
        {
            if (string.IsNullOrWhiteSpace(categorie.Omschrijving)) return false;

            return await _context.Categorieen
                .AnyAsync(c =>
                    c.CategorieId != categorie.CategorieId
                    && c.Omschrijving == categorie.Omschrijving);
        }

    }
}
