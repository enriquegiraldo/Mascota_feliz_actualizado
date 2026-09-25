using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MascotaFeliz.App.Dominio;

namespace MascotaFeliz.App.Persistencia
{
    /// <summary>
    /// DbContext de la aplicación. Hereda de IdentityDbContext para incluir
    /// las tablas de ASP.NET Core Identity en la misma base de datos.
    /// </summary>
    public class AppContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Dueno> Duenos { get; set; }
        public DbSet<Veterinario> Veterinarios { get; set; }
        public DbSet<Mascota> Mascotas { get; set; }
        public DbSet<Historia> Historias { get; set; }
        public DbSet<VisitaPyP> VisitasPyP { get; set; }
        public DbSet<Cita> Citas { get; set; }

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
                // Fallback para dev/consola si no se configura vía DI.
                // LocalDB (Windows) — ver appsettings para el connection string real.
                optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=MascotaFelizDev;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True");
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

            modelBuilder.Entity<Cita>()
                .HasKey(c => c.Id);

            // Una cita pertenece a una mascota y la atiende un veterinario.
            // Restrict: no se puede borrar una mascota/veterinario con citas asociadas.
            modelBuilder.Entity<Cita>()
                .HasOne(c => c.Mascota)
                .WithMany()
                .HasForeignKey(c => c.MascotaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cita>()
                .HasOne(c => c.Veterinario)
                .WithMany()
                .HasForeignKey(c => c.VeterinarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // El estado se almacena como entero en la base de datos.
            modelBuilder.Entity<Cita>()
                .Property(c => c.Estado)
                .HasConversion<int>();
        }
    }
}
