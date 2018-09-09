using Kasboek.WebApp.Data;
using Kasboek.WebApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kasboek.WebApp.Services
{
    public class RekeningenService : CrudService<Rekening>, IRekeningenService
    {
        public RekeningenService(KasboekDbContext context) : base(context)
        {
        }

        public override Task<IList<Rekening>> GetListAsync()
        {
            throw new NotImplementedException();
        }

        public override Task<Rekening> GetRawSingleOrDefaultAsync(int id)
        {
            throw new NotImplementedException();
        }

        public override Task<Rekening> GetSingleOrDefaultAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<KeyValuePair<int, string>>> GetSelectListAsync()
        {
            return await _context.Rekeningen
                .OrderByDescending(r => r.IsEigenRekening)
                .ThenBy(r => r.Naam)
                .Select(r => new KeyValuePair<int, string>(r.RekeningId, r.Naam))
                .ToListAsync();
        }
    }
}
