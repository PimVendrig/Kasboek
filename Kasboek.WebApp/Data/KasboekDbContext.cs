using Kasboek.WebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace Kasboek.WebApp.Data
{
    public class KasboekDbContext : DbContext
    {

        public KasboekDbContext(DbContextOptions<KasboekDbContext> options) : base(options)
        {
            
        }
        
        public DbSet<Instellingen> Instellingen { get; set; }

        public DbSet<Rekening> Rekeningen { get; set; }

        public DbSet<Transactie> Transacties { get; set; }

        public DbSet<Categorie> Categorieen { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Transactie>()
                .HasOne(t => t.VanRekening)
                .WithMany(nameof(Rekening.VanTransacties))
                .HasForeignKey(nameof(Transactie.VanRekeningId))
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transactie>()
                .HasOne(t => t.NaarRekening)
                .WithMany(nameof(Rekening.NaarTransacties))
                .HasForeignKey(nameof(Transactie.NaarRekeningId))
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transactie>()
                .HasOne(t => t.Categorie)
                .WithMany(nameof(Categorie.Transacties))
                .HasForeignKey(nameof(Transactie.CategorieId))
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Rekening>()
                .HasOne(r => r.StandaardCategorie)
                .WithMany(nameof(Categorie.Rekeningen))
                .HasForeignKey(nameof(Rekening.StandaardCategorieId))
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Instellingen>()
                .HasOne(r => r.StandaardVanRekening)
                .WithMany()
                .HasForeignKey(nameof(Models.Instellingen.StandaardVanRekeningId))
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Categorie>()
                .HasIndex(c => c.Omschrijving)
                .IsUnique();

            modelBuilder.Entity<Transactie>()
                .HasIndex(t => t.Omschrijving);

            modelBuilder.Entity<Rekening>()
                .HasIndex(c => c.Naam)
                .IsUnique();

            modelBuilder.Entity<Rekening>()
                .HasIndex(c => c.Rekeningnummer)
                .IsUnique();
        }

    }
}
