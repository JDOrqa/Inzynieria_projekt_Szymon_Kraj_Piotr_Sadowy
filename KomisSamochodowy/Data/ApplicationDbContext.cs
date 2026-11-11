using KomisSamochodowy.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KomisSamochodowy.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Samochod> Samochody { get; set; }
        public DbSet<Transakcja> Transakcje { get; set; }
        public DbSet<Rezerwacja> Rezerwacje { get; set; }
        public DbSet<Marka> Marki { get; set; }
        public DbSet<Model> Modele { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Samochod>()
                .HasOne(s => s.Model)
                .WithMany()
                .HasForeignKey(s => s.ModelId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Samochod>()
                .HasOne(s => s.Marka)
                .WithMany()
                .HasForeignKey(s => s.MarkaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}