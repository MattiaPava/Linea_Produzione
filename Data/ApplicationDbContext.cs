using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // <--- FONDAMENTALE PER IDENTITY
using Microsoft.EntityFrameworkCore;
using Linea_Produzione.Models;

namespace Linea_Produzione.Data
{
    // Ereditando da IdentityDbContext, genererà automaticamente anche tutte le tabelle degli utenti (AspNetUsers, ecc.)
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // La tua tabella delle distinte
        public DbSet<DistintaBase> DistinteBase { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Mantiene intatte le configurazioni delle tabelle di Identity (Ruoli, Utenti, ecc.)
            base.OnModelCreating(modelBuilder);

            // Configurazione della tua tabella DistinteBase
            modelBuilder.Entity<DistintaBase>(entity =>
            {
                entity.ToTable("DistinteBase");

                entity.HasKey(e => e.Id);

                entity.HasIndex(e => new { e.Codice, e.Versione })
                      .IsUnique()
                      .HasDatabaseName("UQ_DistinteBase_Codice_Versione");
            });
        }
    }
}