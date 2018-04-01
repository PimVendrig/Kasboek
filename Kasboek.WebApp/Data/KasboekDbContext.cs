using Kasboek.WebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace Kasboek.WebApp.Data
{
    public class KasboekDbContext : DbContext
    {

        public KasboekDbContext(DbContextOptions<KasboekDbContext> options) : base(options)
        {
            
        }

        public DbSet<Rekening> Rekeningen { get; set; }

        public DbSet<Transactie> Transacties { get; set; }

    }
}
