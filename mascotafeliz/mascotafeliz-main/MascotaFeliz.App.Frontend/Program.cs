using AppContext = MascotaFeliz.App.Persistencia.AppContext;
using MascotaFeliz.App.Dominio;
using MascotaFeliz.App.Persistencia;
using MascotaFeliz.App.Frontend.Servicios;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

// DbContext de la aplicación (SQL Server: LocalDB en dev, Azure SQL en prod).
builder.Services.AddDbContext<AppContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MascotaFeliz")
        ?? "Server=(localdb)\\MSSQLLocalDB;Database=MascotaFelizDev;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"));

// ASP.NET Core Identity con roles.
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        // Configuración de contraseñas (relajada para dev, ajustar en prod).
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = true;
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedEmail = false; // Sin confirmación de email en dev.
    })
    .AddEntityFrameworkStores<AppContext>()
    .AddDefaultTokenProviders();

// Configuración de cookies de Identity.
builder.Services.ConfigureApplicationCookie(options =>
    {
        options.LoginPath = "/Cuenta/Login";
        options.LogoutPath = "/Cuenta/Logout";
        options.AccessDeniedPath = "/Cuenta/AccesoDenegado";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

// Repositorios (Scoped).
builder.Services.AddScoped<IRepositorioDueno, RepositorioDueno>();
builder.Services.AddScoped<IRepositorioVeterinario, RepositorioVeterinario>();
builder.Services.AddScoped<IRepositorioMascota, RepositorioMascota>();
builder.Services.AddScoped<IRepositorioHistoria, RepositorioHistoria>();
builder.Services.AddScoped<IRepositorioVisitaPyP, RepositorioVisitaPyP>();
builder.Services.AddScoped<IRepositorioCita, RepositorioCita>();

// Servicio de fotos: Blob Storage en producción (si hay connection string),
// almacenamiento local en wwwroot/uploads en desarrollo.
if (!string.IsNullOrWhiteSpace(builder.Configuration["Storage:ConnectionString"]))
{
    builder.Services.AddScoped<IServicioFotos, ServicioFotosBlob>();
}
else
{
    builder.Services.AddScoped<IServicioFotos, ServicioFotosLocal>();
}

// Servicio de recordatorios de vacunas (visual, sin email).
builder.Services.AddScoped<IServicioRecordatorios, ServicioRecordatorios>();

var app = builder.Build();

// Datos de ejemplo idempotentes + semilla de Identity (roles + Admin inicial).
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppContext>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    // Aplica migraciones pendientes (crea la BD si no existe).
    context.Database.Migrate();

    // --- Semilla de Identity: roles + usuario Admin inicial ---
    await SemillaIdentity.SembrarAsync(roleManager, userManager);

    // --- Datos de ejemplo de la clínica ---
    if (!context.Duenos.Any())
    {
        context.Duenos.AddRange(
            new Dueno { Nombres = "Juan", Apellidos = "Pérez", Direccion = "Calle 1 # 1-1", Telefono = "3001234567", Correo = "juan@correo.com" },
            new Dueno { Nombres = "María", Apellidos = "Gómez", Direccion = "Calle 2 # 2-2", Telefono = "3007654321", Correo = "maria@correo.com" },
            new Dueno { Nombres = "Carlos", Apellidos = "Rodríguez", Direccion = "Calle 3 # 3-3", Telefono = "3101112233", Correo = "carlos@correo.com" }
        );
    }

    if (!context.Veterinarios.Any())
    {
        context.Veterinarios.AddRange(
            new Veterinario { Nombres = "Dr. Andrés", Apellidos = "López", Direccion = "Transversal 10 # 5-50", Telefono = "3205556677", TarjetaProfesional = "TP0001" },
            new Veterinario { Nombres = "Dra. Laura", Apellidos = "Martínez", Direccion = "Carrera 15 # 20-30", Telefono = "3208889900", TarjetaProfesional = "TP0002" },
            new Veterinario { Nombres = "Dr. Jaime", Apellidos = "Duque", Direccion = "Transversal 5 # 17A-100", Telefono = "1234577", TarjetaProfesional = "TP0003" }
        );
    }

    if (!context.Historias.Any())
    {
        context.Historias.AddRange(
            new Historia { FechaInicial = new DateTime(2023, 1, 15) },
            new Historia { FechaInicial = new DateTime(2023, 3, 20) },
            new Historia { FechaInicial = new DateTime(2023, 5, 10) }
        );
        context.SaveChanges();
    }

    if (!context.Mascotas.Any())
    {
        context.Mascotas.AddRange(
            new Mascota { Nombre = "Firulais", Color = "Café", Especie = "Perro", Raza = "Labrador", DuenoId = 1, VeterinarioId = 1, HistoriaId = 1 },
            new Mascota { Nombre = "Susi", Color = "Blanca", Especie = "Perro", Raza = "Chihuahua", DuenoId = 2, VeterinarioId = 2, HistoriaId = 2 },
            new Mascota { Nombre = "Michi", Color = "Negro", Especie = "Gato", Raza = "Siamés", DuenoId = 3, VeterinarioId = 3, HistoriaId = 3 },
            new Mascota { Nombre = "Rocky", Color = "Gris", Especie = "Perro", Raza = "Husky", DuenoId = 1, VeterinarioId = 2 }
        );
    }

    if (!context.VisitasPyP.Any())
    {
        var hoy = DateTime.Today;
        context.VisitasPyP.AddRange(
            new VisitaPyP
            {
                HistoriaId = 1, FechaVisita = hoy.AddDays(-60),
                Temperatura = 38.5f, Peso = 28.0f, FrecuenciaRespiratoria = 22, FrecuenciaCardiaca = 110,
                EstadoAnimo = "Activo y alerta", Recomendaciones = "Dieta balanceada para perro adulto.",
                ProximaVacuna = hoy.AddDays(5), TipoProximaVacuna = "Refuerzo de rabia"
            },
            new VisitaPyP
            {
                HistoriaId = 1, FechaVisita = hoy.AddDays(-120),
                Temperatura = 38.2f, Peso = 29.5f, FrecuenciaRespiratoria = 20, FrecuenciaCardiaca = 105,
                EstadoAnimo = "Tranquilo", Recomendaciones = "Control de peso adecuado. Desparasitación trimestral.",
                ProximaVacuna = hoy.AddDays(-10), TipoProximaVacuna = "Refuerzo de moquillo"
            },
            new VisitaPyP
            {
                HistoriaId = 3, FechaVisita = hoy.AddDays(-90),
                Temperatura = 38.8f, Peso = 4.2f, FrecuenciaRespiratoria = 28, FrecuenciaCardiaca = 160,
                EstadoAnimo = "Receloso", Recomendaciones = "Esterilización recomendada. Revisión dental en 6 meses.",
                ProximaVacuna = hoy.AddDays(20), TipoProximaVacuna = "Triple felina"
            }
        );
    }

    if (!context.Citas.Any())
    {
        context.Citas.AddRange(
            new Cita { MascotaId = 1, VeterinarioId = 1, FechaHora = new DateTime(2026, 9, 28, 10, 0, 0), Motivo = "Control de vacunación anual", Estado = EstadoCita.Pendiente, NotasInternas = "Verificar refuerzo de rabia." },
            new Cita { MascotaId = 3, VeterinarioId = 2, FechaHora = new DateTime(2026, 9, 25, 15, 30, 0), Motivo = "Consulta general y desparasitación", Estado = EstadoCita.Confirmada, NotasInternas = "Dueño confirma asistencia por WhatsApp." },
            new Cita { MascotaId = 2, VeterinarioId = 2, FechaHora = new DateTime(2026, 9, 20, 9, 0, 0), Motivo = "Limpieza dental", Estado = EstadoCita.Atendida, NotasInternas = "Sin complicaciones. Control en 30 días." },
            new Cita { MascotaId = 4, VeterinarioId = 1, FechaHora = new DateTime(2026, 9, 18, 14, 0, 0), Motivo = "Cojera en pata delantera", Estado = EstadoCita.Cancelada, NotasInternas = "Dueño canceló por inconveniente de horario." }
        );
    }

    context.SaveChanges();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.Run();

/// <summary>
/// Semilla de Identity: crea los roles Admin, Veterinario y Recepción,
/// y un usuario Admin inicial si no existe.
/// </summary>
public static class SemillaIdentity
{
    public static async Task SembrarAsync(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
    {
        // Roles.
        string[] roles = { "Admin", "Veterinario", "Recepcion" };
        foreach (var rol in roles)
        {
            if (!await roleManager.RoleExistsAsync(rol))
            {
                await roleManager.CreateAsync(new IdentityRole(rol));
            }
        }

        // Usuario Admin inicial.
        var adminEmail = "admin@mascotafeliz.com";
        var admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin == null)
        {
            admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                Nombres = "Administrador",
                Apellidos = "Sistema",
                EmailConfirmed = true
            };
            var resultado = await userManager.CreateAsync(admin, "Admin123!");
            if (resultado.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}
