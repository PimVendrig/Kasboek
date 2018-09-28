﻿// <auto-generated />
using System;
using Kasboek.WebApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Kasboek.WebApp.Data.Migrations
{
    [DbContext(typeof(KasboekDbContext))]
    partial class KasboekDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.3-rtm-32065")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Kasboek.WebApp.Models.Categorie", b =>
                {
                    b.Property<int>("CategorieId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Omschrijving")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.HasKey("CategorieId");

                    b.HasIndex("Omschrijving")
                        .IsUnique();

                    b.ToTable("Categorieen");
                });

            modelBuilder.Entity("Kasboek.WebApp.Models.Instellingen", b =>
                {
                    b.Property<int>("InstellingenId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("StandaardVanRekeningId");

                    b.Property<bool>("TransactieMeteenBewerken");

                    b.HasKey("InstellingenId");

                    b.HasIndex("StandaardVanRekeningId");

                    b.ToTable("Instellingen");
                });

            modelBuilder.Entity("Kasboek.WebApp.Models.Rekening", b =>
                {
                    b.Property<int>("RekeningId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsEigenRekening");

                    b.Property<string>("Naam")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("Rekeningnummer")
                        .HasMaxLength(100);

                    b.Property<int?>("StandaardCategorieId");

                    b.HasKey("RekeningId");

                    b.HasIndex("Naam")
                        .IsUnique();

                    b.HasIndex("Rekeningnummer")
                        .IsUnique()
                        .HasFilter("[Rekeningnummer] IS NOT NULL");

                    b.HasIndex("StandaardCategorieId");

                    b.ToTable("Rekeningen");
                });

            modelBuilder.Entity("Kasboek.WebApp.Models.Transactie", b =>
                {
                    b.Property<int>("TransactieId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("Bedrag")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int?>("CategorieId");

                    b.Property<DateTime>("Datum")
                        .HasColumnType("date");

                    b.Property<int>("NaarRekeningId");

                    b.Property<string>("Omschrijving")
                        .HasMaxLength(500);

                    b.Property<int>("VanRekeningId");

                    b.HasKey("TransactieId");

                    b.HasIndex("CategorieId");

                    b.HasIndex("NaarRekeningId");

                    b.HasIndex("Omschrijving");

                    b.HasIndex("VanRekeningId");

                    b.ToTable("Transacties");
                });

            modelBuilder.Entity("Kasboek.WebApp.Models.Instellingen", b =>
                {
                    b.HasOne("Kasboek.WebApp.Models.Rekening", "StandaardVanRekening")
                        .WithMany()
                        .HasForeignKey("StandaardVanRekeningId")
                        .OnDelete(DeleteBehavior.SetNull);
                });

            modelBuilder.Entity("Kasboek.WebApp.Models.Rekening", b =>
                {
                    b.HasOne("Kasboek.WebApp.Models.Categorie", "StandaardCategorie")
                        .WithMany()
                        .HasForeignKey("StandaardCategorieId")
                        .OnDelete(DeleteBehavior.SetNull);
                });

            modelBuilder.Entity("Kasboek.WebApp.Models.Transactie", b =>
                {
                    b.HasOne("Kasboek.WebApp.Models.Categorie", "Categorie")
                        .WithMany("Transacties")
                        .HasForeignKey("CategorieId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("Kasboek.WebApp.Models.Rekening", "NaarRekening")
                        .WithMany("NaarTransacties")
                        .HasForeignKey("NaarRekeningId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Kasboek.WebApp.Models.Rekening", "VanRekening")
                        .WithMany("VanTransacties")
                        .HasForeignKey("VanRekeningId")
                        .OnDelete(DeleteBehavior.Restrict);
                });
#pragma warning restore 612, 618
        }
    }
}
