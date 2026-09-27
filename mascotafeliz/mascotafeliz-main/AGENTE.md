# 🤖 Instrucciones para Agentes / IA (AGENTE.md)

Archivo de guía para que cualquier agente de IA pueda contribuir al repositorio **MascotaFeliz** de forma correcta y consistente.

---

## 🧭 Reglas Generales OBLIGATORIAS

1. **Idioma de la base de código:** Español. Nombres de clases, métodos, archivos, páginas y comentarios en español.
2. **Arquitectura en 4 capas (SÍGUELA ESTRICTAMENTE):**
   - **Dominio →** solo entidades y enumeraciones. Referencias mínimas externas (Identity stores para ApplicationUser).
   - **Persistencia →** DbContext (`IdentityDbContext<ApplicationUser>`), interfaces, repositorios y migraciones. Depende de Dominio.
   - **Frontend →** Razor Pages + DI + Identity + Servicios. Depende de Dominio y Persistencia.
   - **Consola →** pruebas rápidas. Depende de Dominio y Persistencia.
   - **Pruebas →** xUnit con SQLite en memoria. Depende de Dominio, Persistencia y Frontend (para pruebas de autorización).
3. **NUNCA pongas lógica de negocios en el Frontend.** Eso pertenece a Persistencia (repositorios), Servicios (`IServicioFotos`, `IServicioRecordatorios`) o una futura capa de Servicios.
4. **Inyección de dependencias:** los repositorios y servicios se registran como `Scoped` en `Program.cs` y se inyectan por constructor en las páginas. **NO** uses `new Repositorio(new AppContext())`.
5. **Política de borrado (`AppContext.OnModelCreating`):** todas las FK de Mascota (Dueno, Veterinario, Historia) y de Cita (Mascota, Veterinario) son `Restrict`. La FK Historia→VisitaPyP es `Cascade`. Cambiar esto requiere generar una nueva migración EF.
6. **Base de datos:** SQL Server (LocalDB en dev, Azure SQL en prod). Se usan **migraciones EF** (`Database.Migrate()` en `Program.cs`), NO `EnsureCreated()`.
7. **Nombres PascalCase** para clases, namespaces y propiedades.
8. **Nombres camelCase** para variables locales y campos privados.
9. **No realices commits** a menos que el usuario lo pida explícitamente.
10. **No elimines páginas o entidades existentes** sin confirmar con el usuario.
11. **Pruebas xUnit:** si añades lógica nueva en Persistencia, incluye pruebas en `MascotaFeliz.App.Pruebas` (SQLite en memoria). Ejecuta `dotnet test` antes de entregar.
12. **Seguridad — Html.Raw:** si generas HTML manualmente con `StringBuilder` y lo renderizas con `@Html.Raw()`, **siempre** HTML-encodea los valores de usuario con el helper `E()` (`WebUtility.HtmlEncode`). Nunca insertes datos de usuario directamente en el HTML.
13. **Seguridad — Semilla de Admin:** `SemillaIdentity.SembrarAsync` recibe `isDevelopment`. En dev usa `Admin123!`; en prod genera contraseña aleatoria (mostrada una sola vez en consola). Los datos de ejemplo solo se insertan en `IsDevelopment`.
14. **Autorización granular:** las páginas del panel usan `[Authorize(Roles=...)]` según la matriz de permisos. Los handlers de eliminar verifican `User.IsInRole("Admin")`. Los botones/enlaces se ocultan en la UI con `@if (User.IsInRole("Admin"))`. Ver matriz completa abajo.
15. **Fuera de alcance:** el memorial digital / recordatorio de mascota fallecida es un producto aparte. **No construir.** Ignorar si aparece.

---

## 🔐 Matriz de autorización

| Módulo | Admin | Veterinario | Recepción |
|--------|:-----:|:-----------:|:---------:|
| Inicio (dashboard) | ✅ | ✅ | ✅ |
| Mascotas (ver/crear/editar) | ✅ | ✅ | ✅ |
| Mascotas (eliminar) | ✅ | ❌ | ❌ |
| Dueños (ver/crear/editar) | ✅ | ✅ | ✅ |
| Dueños (eliminar) | ✅ | ❌ | ❌ |
| Historias / VisitasPyP (ver) | ✅ | ✅ | ✅ |
| Historias / VisitasPyP (editar) | ✅ | ✅ | ❌ solo lectura |
| Veterinarios (ver) | ✅ | ✅ | ✅ |
| Veterinarios (crear/editar/eliminar) | ✅ | ❌ | ❌ |
| Citas (ver/crear/confirmar/cancelar) | ✅ | ✅ | ✅ |
| Citas (eliminar) | ✅ | ❌ | ❌ |
| Buscar (command palette) | ✅ | ✅ | ✅ |
| Sitio público (/Publico/*) | Sin login | Sin login | Sin login |

**IndexModel es la excepción:** usa `[AllowAnonymous]` (no `[Authorize]`) porque hace redirección manual en `OnGet`. NO "corregir" añadiendo `[Authorize]`.

---

## 🛠️ Stack tecnológico

- **.NET 10 LTS** (SDK 10.0.401)
- **EF Core 10.0.12** con SQL Server (LocalDB dev / Azure SQL prod)
- **ASP.NET Core Identity** con roles Admin/Veterinario/Recepción
- **Bootstrap 5.3** + tema propio "Verde Cálido" (esmeralda #0d7a5f + ámbar #f59e0b)
- **Tipografías:** Inter (UI) + Fraunces (titulares) vía Google Fonts
- **Azure.Storage.Blobs** para fotos de mascota (Blob Storage prod / wwwroot/uploads dev)
- **Modo claro/oscuro** automático con `data-bs-theme` + `prefers-color-scheme`

---

## 🏗️ Cómo agregar una nueva entidad (ej. `Empleado`)

### Paso 1: Capa Dominio
Crea `MascotaFeliz.App.Dominio/Empleado.cs` con `DataAnnotations` de validación.

### Paso 2: Capa Persistencia
- Agregar `DbSet<Empleado>` en `AppContext.cs`.
- Configurar relaciones en `OnModelCreating` (si hay FKs).
- Crear interfaz `IRepositorioEmpleado.cs` y implementación `RepositorioEmpleado.cs`.
- Generar migración: `dotnet ef migrations add AddEmpleado --project MascotaFeliz.App.Persistencia --startup-project MascotaFeliz.App.Frontend --output-dir Migraciones`

### Paso 3: Capa Frontend
- Crear `Pages/Empleados/` con Lista, Detalles y Editar.
- Añadir `[Authorize]` o `[Authorize(Roles=...)]` según la matriz.
- Añadir el link al menú en `_Layout.cshtml`.

### Paso 4: Program.cs
- Registrar el repositorio en DI (`builder.Services.AddScoped<IRepositorioEmpleado, RepositorioEmpleado>()`).
- Si hay seed data, ponerla dentro de `if (app.Environment.IsDevelopment())`.

### Paso 5: Pruebas
- Crear `MascotaFeliz.App.Pruebas/RepositorioEmpleadoPruebas.cs` con SQLite en memoria.
- Ejecutar `dotnet test`.

### Paso 6: Verificar
```bash
dotnet build    # 0 errores, 0 warnings
dotnet test     # Todas las pruebas pasan
```

---

## ⚙️ Patrón Repositorio

```csharp
public interface IRepositorio<T>
{
    T        Add(T entidad);
    T        Get(int id);
    T        Update(T entidad);
    IEnumerable<T> GetAll();
    IEnumerable<T> GetPorFiltro(string filtro);   // opcional, insensible a mayúsculas y acentos
    void     Delete(int id);
}
```

Las implementaciones reciben `AppContext` vía constructor. La búsqueda usa `Normalizador.Normalizar()` (quita acentos + ToLower) con `AsEnumerable()` antes del `Where` (EF Core no traduce métodos C# a SQL).

---

## 🔍 Cómo localizar código rápidamente

| Qué buscas | Dónde está |
|---|---|
| Entidad `Dueno` | `MascotaFeliz.App.Dominio/Dueno.cs` |
| Entidad `Cita` + `EstadoCita` | `MascotaFeliz.App.Dominio/Cita.cs`, `EstadoCita.cs` |
| Usuario Identity | `MascotaFeliz.App.Dominio/ApplicationUser.cs` |
| DbContext | `MascotaFeliz.App.Persistencia/AppContext.cs` |
| Migraciones EF | `MascotaFeliz.App.Persistencia/Migraciones/` |
| Normalizador (búsqueda) | `MascotaFeliz.App.Persistencia/Normalizador.cs` |
| CRUD Dueños | `MascotaFeliz.App.Frontend/Pages/Duenos/` |
| CRUD Mascotas | `MascotaFeliz.App.Frontend/Pages/Mascotas/` |
| CRUD Veterinarios | `MascotaFeliz.App.Frontend/Pages/Veterinarios/` |
| CRUD Historias | `MascotaFeliz.App.Frontend/Pages/Historias/` |
| CRUD Visitas PyP | `MascotaFeliz.App.Frontend/Pages/VisitasPyP/` |
| CRUD Citas | `MascotaFeliz.App.Frontend/Pages/Citas/` |
| Sitio público | `MascotaFeliz.App.Frontend/Pages/Publico/Portada.cshtml`, `Agendar.cshtml` |
| Login/Logout | `MascotaFeliz.App.Frontend/Pages/Cuenta/` |
| Servicio de fotos | `MascotaFeliz.App.Frontend/Servicios/IServicioFotos.cs`, `ServicioFotosLocal.cs`, `ServicioFotosBlob.cs` |
| Servicio de recordatorios | `MascotaFeliz.App.Frontend/Servicios/ServicioRecordatorios.cs` |
| Búsqueda global (Cmd+K) | `MascotaFeliz.App.Frontend/Pages/Buscar.cshtml.cs`, `wwwroot/js/command-palette.js` |
| Sistema de diseño CSS | `MascotaFeliz.App.Frontend/wwwroot/css/site.css` |
| Layout panel interno | `MascotaFeliz.App.Frontend/Pages/Shared/_Layout.cshtml` |
| Layout sitio público | `MascotaFeliz.App.Frontend/Pages/Shared/_LayoutPublico.cshtml` |
| ViewComponent tarjetas mascota | `MascotaFeliz.App.Frontend/ViewComponents/TarjetasMascotasViewComponent.cs` |
| Punto de entrada / Seed / Identity | `MascotaFeliz.App.Frontend/Program.cs` |
| CI/CD | `.github/workflows/ci-cd.yml` |
| Pruebas | `MascotaFeliz.App.Pruebas/` (85 tests: repositorios + autorización) |

---

## 🛠️ Comandos frecuentes

```bash
# Restaurar dependencias
dotnet restore

# Compilar (0 errores, 0 warnings)
dotnet build

# Ejecutar Frontend en modo desarrollo
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet run --project MascotaFeliz.App.Frontend

# Ejecutar pruebas (85 tests)
dotnet test

# Generar migración EF
dotnet ef migrations add <Nombre> --project MascotaFeliz.App.Persistencia --startup-project MascotaFeliz.App.Frontend --output-dir Migraciones

# Aplicar migraciones a la BD
dotnet ef database update --project MascotaFeliz.App.Persistencia --startup-project MascotaFeliz.App.Frontend

# Limpiar
dotnet clean
```

---

## ⚠️ Problemas comunes y soluciones

| Error | Causa | Solución |
|---|---|---|
| `AppContext` ambiguo con `System.AppContext` | Conflicto de nombres | `using AppContext = MascotaFeliz.App.Persistencia.AppContext;` |
| `LocalDB is not supported on this platform` | Linux no tiene SQL Server | Usar Windows con LocalDB o cambiar a SQL Server en contenedor |
| Razor: `asp-items` parseado como resta (`asp - items`) | Bug del compilador Razor .NET 10 con `@if` dentro de `<form>` | Generar options con `@foreach` manual o HTML en code-behind + `@Html.Raw()` |
| Razor: tags `<text>` SVG colisionan con `<text>` de Razor | Conflicto de nombres | Generar SVG en code-behind con `StringBuilder` + `@Html.Raw()` |
| `Framework Microsoft.AspNetCore.App 8.0 not found` | csproj apunta a versión vieja | Cambiar `TargetFramework` a `net10.0` |
| "Failed to determine the https port" | No hay certificado dev HTTPS | `dotnet dev-certs https --trust` o usar HTTP (`http://localhost:5000`) |

---

## ✅ Checklist antes de entregar cambios

- [ ] `dotnet build` pasa con **0 errores y 0 warnings**.
- [ ] `dotnet test` pasa con **0 pruebas fallidas** (85+ tests).
- [ ] Todas las entidades nuevas tienen `Id` (PK) y propiedades en PascalCase.
- [ ] Las relaciones están configuradas en `AppContext.OnModelCreating`.
- [ ] Se generó migración EF (`dotnet ef migrations add ...`).
- [ ] Las páginas Razor usan `[Authorize(Roles=...)]` según la matriz de permisos.
- [ ] Los botones de eliminar están ocultos con `@if (User.IsInRole("Admin"))` si corresponde.
- [ ] HTML generado manualmente usa `E()` (WebUtility.HtmlEncode) para prevenir XSS.
- [ ] Si añadiste lógica en Persistencia, incluye pruebas en `MascotaFeliz.App.Pruebas`.
- [ ] No hay credenciales en el código ni en el workflow de CI/CD.

---

## 🧪 Cómo verificar funcionalidad (sanity check)

1. Abre `http://localhost:5000/` sin login → redirige al sitio público (`/Publico/Portada`).
2. Inicia sesión en `/Cuenta/Login` con `admin@mascotafeliz.com` / `Admin123!` (dev).
3. GET `/` → muestra el panel interno (KPIs, donut, recordatorios).
4. `/Mascotas/ListaMascotas` → lista con búsqueda, filtros, paginación, ordenamiento.
5. Como Recepción, el botón "Eliminar" NO aparece; como Admin, sí.
6. Como Veterinario, `/Veterinarios/EditarVeterinario` redirige a AccesoDenegado.
7. `/Publico/Agendar` → formulario público que crea Dueño + Mascota + Cita Pendiente.
8. `Ctrl+K` abre el command palette de búsqueda global.

---

## 📌 Notas finales

- La base de datos usa **SQL Server** (LocalDB en dev, Azure SQL en prod) con migraciones EF.
- Las migraciones se aplican automáticamente al iniciar (`Database.Migrate()` en `Program.cs`).
- Los datos de ejemplo (dueños, mascotas, citas, etc.) solo se insertan en `IsDevelopment`.
- El usuario Admin inicial: en dev `admin@mascotafeliz.com / Admin123!`; en prod, contraseña aleatoria mostrada una sola vez en consola.
- La UI usa **Bootstrap 5.3 por CDN** + tema propio "Verde Cálido" en `wwwroot/css/site.css`.
- Las fotos se guardan en `wwwroot/uploads/mascotas/` (dev) o Azure Blob Storage (prod).
- El pipeline de CI/CD (`.github/workflows/ci-cd.yml`) hace build + test + publish + deploy a Azure App Service.
