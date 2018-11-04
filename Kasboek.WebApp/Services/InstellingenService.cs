using Kasboek.WebApp.Data;
using Kasboek.WebApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kasboek.WebApp.Services
{
    public class InstellingenService : CrudService<Instellingen>, IInstellingenService
    {
        public InstellingenService(KasboekDbContext context) : base(context)
        {
        }

        public override Task<IList<Instellingen>> GetListAsync()
        {
            throw new NotSupportedException();
        }

        public override Task<Instellingen> GetRawSingleOrDefaultAsync(int id)
        {
            throw new NotSupportedException();
        }

        public async override Task<Instellingen> GetSingleOrDefaultAsync(int id)
        {
            return await GetRawSingleOrDefaultAsync(id);
        }

        public async Task<Instellingen> GetRawSingleAsync()
        {
            return await _context.Instellingen
                .SingleAsync();
        }

        public async Task<Instellingen> GetSingleAsync()
        {
            return await _context.Instellingen
                .Include(i => i.StandaardVanRekening)
                .Include(i => i.PortemonneeRekening)
                .SingleAsync();
        }

        public async Task<int> GetIdAsync()
        {
            return await _context.Instellingen
                .Select(i => i.InstellingenId)
                .SingleAsync();
        }
    }
}
