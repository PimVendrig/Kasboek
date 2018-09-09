using Kasboek.WebApp.Data;
using Kasboek.WebApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kasboek.WebApp.Services
{
    public class TransactiesService : CrudService<Transactie>, ITransactiesService
    {
        public TransactiesService(KasboekDbContext context) : base(context)
        {
        }

        public async override Task<IList<Transactie>> GetListAsync()
        {
            return await _context.Transacties
                .Include(t => t.NaarRekening)
                .Include(t => t.VanRekening)
                .Include(t => t.Categorie)
                .OrderByDescending(t => t.Datum)
                .ThenByDescending(t => t.TransactieId)
                .ToListAsync();
        }

        public async override Task<Transactie> GetRawSingleOrDefaultAsync(int id)
        {
            return await _context.Transacties
                .SingleOrDefaultAsync(t => t.TransactieId == id);
        }

        public async override Task<Transactie> GetSingleOrDefaultAsync(int id)
        {
            return await _context.Transacties
                .Include(t => t.NaarRekening)
                .Include(t => t.VanRekening)
                .Include(t => t.Categorie)
                .SingleOrDefaultAsync(t => t.TransactieId == id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Transacties
                .AnyAsync(t => t.TransactieId == id);
        }
    }
}
