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

        public DbSet<Label> Labels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Transactie>()
                .HasOne(r => r.VanRekening)
                .WithMany("VanTransacties")
                .HasForeignKey("VanRekeningId")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transactie>()
                .HasOne(r => r.NaarRekening)
                .WithMany("NaarTransacties")
                .HasForeignKey("NaarRekeningId")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TransactieLabel>()
                .ToTable("TransactieLabels")
                .HasKey(transactieLabel => new { transactieLabel.TransactieId, transactieLabel.LabelId });

            modelBuilder.Entity<RekeningLabel>()
                .ToTable("RekeningLabels")
                .HasKey(rekeningLabel => new { rekeningLabel.RekeningId, rekeningLabel.LabelId });
        }

    }
}
