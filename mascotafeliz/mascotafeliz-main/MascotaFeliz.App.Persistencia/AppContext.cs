using Microsoft.EntityFrameworkCore;
using MascotaFeliz.App.Dominio;

namespace MascotaFeliz.App.Persistencia
{
    public class AppContext : DbContext
    {
        public DbSet<Dueno> Duenos { get; set; }
        public DbSet<Veterinario> Veterinarios { get; set; }
        public DbSet<Mascota> Mascotas { get; set; }
        public DbSet<Historia> Historias { get; set; }
        public DbSet<VisitaPyP> VisitasPyP { get; set; }

        public AppContext()
        {
        }

        public AppContext(DbContextOptions<AppContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=MascotaFeliz.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Dueno>()
                .HasKey(d => d.Id);

            modelBuilder.Entity<Veterinario>()
                .HasKey(v => v.Id);

            modelBuilder.Entity<Mascota>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<Historia>()
                .HasKey(h => h.Id);

            modelBuilder.Entity<VisitaPyP>()
                .HasKey(v => v.Id);

            modelBuilder.Entity<Mascota>()
                .HasOne(m => m.Dueno)
                .WithMany()
                .HasForeignKey(m => m.DuenoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Mascota>()
                .HasOne(m => m.Veterinario)
                .WithMany()
                .HasForeignKey(m => m.VeterinarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Mascota>()
                .HasOne(m => m.Historia)
                .WithMany()
                .HasForeignKey(m => m.HistoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<VisitaPyP>()
                .HasOne(v => v.Historia)
                .WithMany(h => h.VisitasPyP)
                .HasForeignKey(v => v.HistoriaId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
