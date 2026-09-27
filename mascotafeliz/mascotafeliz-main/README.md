# 🐾 Mascota Feliz

Sistema de gestión para la clínica veterinaria **Mascota Feliz**, desarrollado con **.NET 10 + ASP.NET Core Razor Pages + EF Core + SQL Server**, con **ASP.NET Core Identity** (roles Admin/Veterinario/Recepción), sitio público para dueños, panel interno para la clínica, y despliegue en **Azure App Service**.

---

## 📋 Tabla de Contenidos

1. [Tecnologías](#tecnologías)
2. [Estructura del Proyecto](#estructura-del-proyecto)
3. [Requisitos Previos](#requisitos-previos)
4. [Primeros Pasos](#primeros-pasos)
5. [Ejecución en Modo Desarrollo](#ejecución-en-modo-desarrollo)
6. [Compilar y Publicar](#compilar-y-publicar)
7. [Base de Datos](#base-de-datos)
8. [Autenticación y Roles](#autenticación-y-roles)
9. [URLs y Navegación](#urls-y-navegación)
10. [Pruebas](#pruebas)
11. [CI/CD](#cicd)
12. [Contribuir](#contribuir)
13. [Licencia](#licencia)

---

## 🛠️ Tecnologías

- **.NET 10.0 LTS** (SDK `10.0.401`)
- **ASP.NET Core Razor Pages** (Frontend)
- **ASP.NET Core Identity** con roles (`Admin`, `Veterinario`, `Recepción`)
- **Entity Framework Core 10.0.12** (ORM) sobre **SQL Server**
- **SQL Server LocalDB** (dev, Windows) / **Azure SQL Database** (prod)
- **Azure Blob Storage** para fotos de mascota (dev: carpeta `wwwroot/uploads`)
- **Bootstrap 5.3** + **tema propio "Verde Cálido"** (esmeralda `#0d7a5f` + ámbar `#f59e0b`)
- **Tipografías:** Inter (UI) + Fraunces (titulares)
- **Modo claro/oscuro** automático (`prefers-color-scheme`)
- **Búsqueda global** (Command Palette con `Ctrl/Cmd+K`)
- **GitHub Actions** (CI/CD → Azure App Service)

---

## 📁 Estructura del Proyecto

```
MascotaFeliz/
├── MascotaFeliz.sln                              ← Archivo de solución
│
├── MascotaFeliz.App.Dominio/                     ← 🎯 Entidades y enumeraciones
│   ├── Dueno.cs
│   ├── Veterinario.cs
│   ├── Mascota.cs
│   ├── Historia.cs
│   ├── VisitaPyP.cs
│   ├── Cita.cs
│   ├── EstadoCita.cs                            ← Enum (Pendiente/Confirmada/Cancelada/Atendida)
│   ├── ApplicationUser.cs                        ← Usuario de Identity (con Nombres/Apellidos)
│   └── MascotaFeliz.App.Dominio.csproj
│
├── MascotaFeliz.App.Persistencia/                ← 🗄️ DbContext, repositorios, migraciones
│   ├── AppContext.cs                             ← IdentityDbContext<ApplicationUser>
│   ├── Normalizador.cs                           ← Búsqueda insensible a acentos (es-CO)
│   ├── Migraciones/                              ← Migraciones EF Core
│   ├── IRepositorio*.cs + Repositorio*.cs        ← 6 repositorios
│   └── MascotaFeliz.App.Persistencia.csproj
│
├── MascotaFeliz.App.Frontend/                    ← 🌐 Panel interno + Sitio público
│   ├── Program.cs                                ← DI, Identity, seed data, migraciones
│   ├── appsettings.json / .Development / .Production
│   ├── Pages/
│   │   ├── Index.cshtml                          ← Doble propósito: público (redirect) / panel
│   │   ├── Buscar.cshtml                         ← Endpoint JSON de búsqueda global (Cmd+K)
│   │   ├── Cuenta/                               ← Login, Logout, AccesoDenegado
│   │   ├── Mascotas/                             ← CRUD + perfil + gráfico de peso + foto
│   │   ├── Duenos/                               ← CRUD + mascotas asociadas
│   │   ├── Veterinarios/                         ← CRUD + pacientes atendidos
│   │   ├── Historias/                             ← Lista, detalle, editar
│   │   ├── VisitasPyP/                           ← CRUD + recordatorios de vacunas
│   │   ├── Citas/                                ← CRUD + estados + filtros
│   │   ├── Publico/                              ← Portada + Agendar (sin login)
│   │   ├── Error.cshtml / NotFound.cshtml / Privacy.cshtml
│   │   ├── Shared/
│   │   │   ├── _Layout.cshtml                    ← Layout panel interno
│   │   │   ├── _LayoutPublico.cshtml             ← Layout sitio público
│   │   │   ├── _CommandPalette.cshtml            ← Dialog de búsqueda global
│   │   │   ├── _ValidationScriptsPartial.cshtml
│   │   │   └── Components/TarjetasMascotas/      ← ViewComponent reutilizable
│   ├── ViewComponents/                           ← TarjetasMascotasViewComponent
│   ├── Servicios/                                ← IServicioFotos, IServicioRecordatorios
│   ├── wwwroot/
│   │   ├── css/site.css                          ← Tema "Verde Cálido" (tokens + componentes)
│   │   ├── js/site.js                            ← Toasts, confirmar borrado, nav activa
│   │   ├── js/command-palette.js                 ← Búsqueda global (Ctrl+K)
│   │   ├── img/logo.svg, favicon.svg             ← Estetoscopio + corazón ámbar (SVG)
│   │   └── uploads/mascotas/                     ← Fotos subidas en dev (gitignored)
│   ├── .github/workflows/ci-cd.yml               ← Pipeline GitHub Actions
│   └── MascotaFeliz.App.Frontend.csproj
│
├── MascotaFeliz.App.Consola/                     ← 💻 Proyecto de Consola (pruebas rápidas)
│
├── MascotaFeliz.App.Pruebas/                     ← 🧪 Pruebas xUnit (85 tests)
│   ├── ContextoPruebas.cs                       ← Helper SQLite en memoria
│   ├── RepositorioMascotaPruebas.cs              ← 15 tests
│   ├── RepositorioDuenoPruebas.cs                ← 8 tests
│   ├── RepositorioVeterinarioPruebas.cs          ← 5 tests
│   ├── RepositorioHistoriaVisitaPruebas.cs       ← 7 tests
│   ├── RepositorioCitaPruebas.cs                 ← 11 tests
│   ├── NormalizadorPruebas.cs                    ← 13 tests (accent-insensitive)
│   └── AutorizacionPruebas.cs                    ← 25 tests (matriz de roles)
│
├── docs/
│   ├── decisiones.md                             ← Decisiones de diseño y producto
│   ├── investigacion-ui.md                       ← Investigación de UI/UX (Fase 1)
│   ├── propuesta-visual.md                       ← 3 direcciones visuales propuestas
│   └── despliegue.md                             ← Guía de despliegue en Azure
│
└── README.md
```

---

## ✅ Requisitos Previos

- **.NET 10.0 SDK** (LTS)
  - Verificar: `dotnet --version`
  - Descargar: <https://dotnet.microsoft.com/download>
- **SQL Server LocalDB** (viene con Visual Studio o SQL Server Express)
  - Para producción: Azure SQL Database
- **EF Core Tools** (opcional, para migraciones):
  ```bash
  dotnet tool install --global dotnet-ef --version 10.0.12
  ```

---

## 🚀 Primeros Pasos

```bash
# 1. Clonar el repositorio
git clone <url-del-repo>
cd MascotaFeliz

# 2. Restaurar dependencias
dotnet restore

# 3. Compilar (debe dar 0 errores, 0 warnings)
dotnet build

# 4. Ejecutar pruebas (85 tests)
dotnet test

# 5. Ejecutar la app
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet run --project MascotaFeliz.App.Frontend
```

Al primer arranque, la app aplica las migraciones EF automáticamente (`Database.Migrate()`) y siembra:
- Roles: `Admin`, `Veterinario`, `Recepción`
- Usuario Admin: `admin@mascotafeliz.com` / `Admin123!` (solo en Development)
- Datos de ejemplo: 3 dueños, 3 veterinarios, 4 mascotas, 3 historias, 3 visitas PyP, 4 citas

---

## 👨‍💻 Ejecución en Modo Desarrollo

```powershell
# PowerShell
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet watch run --project MascotaFeliz.App.Frontend
```

> La app escucha en `http://localhost:5000`. Sin login, `/` redirige al sitio público.

---

## 🗄️ Base de Datos

| Entorno | Motor | Connection string |
|---------|-------|-------------------|
| Desarrollo (Windows) | SQL Server LocalDB | `appsettings.Development.json` |
| Producción (Azure) | Azure SQL Database (Basic) | Configuración de App Service |

Las migraciones se aplican automáticamente al iniciar (`Database.Migrate()`). Para crear migraciones nuevas:

```bash
dotnet ef migrations add <Nombre> --project MascotaFeliz.App.Persistencia --startup-project MascotaFeliz.App.Frontend --output-dir Migraciones
```

**Migraciones existentes:** `Inicial`, `AddCita`, `AddFotoMascota`, `AddProximaVacuna`, `AddIdentity`.

---

## 🔐 Autenticación y Roles

### Usuario de prueba (Development)
- **Email:** `admin@mascotafeliz.com`
- **Contraseña:** `Admin123!`
- **Rol:** Admin

### Matriz de permisos

| Módulo | Admin | Veterinario | Recepción |
|--------|:-----:|:-----------:|:---------:|
| Dashboard, buscar, mascotas/dueños/citas (ver/editar) | ✅ | ✅ | ✅ |
| Eliminar mascotas, dueños, citas | ✅ | ❌ | ❌ |
| Veterinarios (crear/editar/eliminar) | ✅ | ❌ | ❌ |
| Historias/VisitasPyP (editar) | ✅ | ✅ | ❌ (solo lectura) |
| Sitio público (/Publico/*) | Sin login | Sin login | Sin login |

### Seguridad
- Honeypot + rate limiting (3/min por IP) en `/Publico/Agendar`.
- HTML-encoding (`WebUtility.HtmlEncode`) en todo HTML generado manualmente con `@Html.Raw`.
- En producción, el Admin inicial usa contraseña aleatoria (mostrada una sola vez en consola).

---

## 🌐 URLs y Navegación

### Sitio público (sin login)

| Ruta | Descripción |
|------|-------------|
| `/Publico/Portada` | Portada (hero, servicios, equipo, CTA) |
| `/Publico/Agendar` | Solicitar cita online (crea Dueño + Mascota + Cita Pendiente) |
| `/Privacy` | Política de privacidad |

### Panel interno (requiere login)

| Ruta | Descripción |
|------|-------------|
| `/` | Dashboard (KPIs, donut, recordatorios de vacunas) |
| `/Cuenta/Login` | Iniciar sesión |
| `/Cuenta/Logout` | Cerrar sesión |
| `/Mascotas/ListaMascotas` | Mascotas (buscar, filtrar por especie, ordenar, paginar, CSV) |
| `/Mascotas/DetallesMascotas?mascotaId=1` | Perfil (foto, datos, dueño, vet, gráfico de peso, línea de tiempo) |
| `/Mascotas/EditarMascota` | Crear/editar (con subida de foto) |
| `/Duenos/ListaDuenos` | Dueños |
| `/Duenos/DetallesDuenos?duenoId=1` | Detalle + mascotas asociadas |
| `/Veterinarios/ListaVeterinarios` | Veterinarios |
| `/Veterinarios/DetallesVeterinarios?veterinarioId=1` | Detalle + pacientes |
| `/Historias/ListaHistorias` | Historias clínicas |
| `/Historias/DetallesHistoria?historiaId=1` | Detalle + línea de tiempo de visitas |
| `/VisitasPyP/ListaVisitasPyP` | Visitas PyP |
| `/VisitasPyP/EditarVisitaPyP` | Crear/editar visita (signos vitales + próxima vacuna) |
| `/Citas/ListaCitas` | Citas (buscar, filtrar por estado, paginar) |
| `/Citas/EditarCita` | Crear/editar cita |
| `/Buscar?q=fir` | Búsqueda global JSON (mascotas, dueños, vets, citas) |
| `/NotFound` | 404 |
| `/Error` | Error handler |

> `Ctrl/Cmd+K` abre el Command Palette de búsqueda global.

---

## 🧪 Pruebas

```bash
dotnet test    # 85 pruebas
```

| Archivo | Tests | Cubre |
|---------|-------|-------|
| `RepositorioMascotaPruebas.cs` | 15 | CRUD, relaciones, búsqueda, especies, foto |
| `RepositorioDuenoPruebas.cs` | 8 | CRUD, búsqueda accent-insensitive |
| `RepositorioVeterinarioPruebas.cs` | 5 | CRUD, búsqueda |
| `RepositorioHistoriaVisitaPruebas.cs` | 7 | Historia + visitas, UpdateVisitaPyP |
| `RepositorioCitaPruebas.cs` | 11 | CRUD, filtros, estados, accent-insensitive |
| `NormalizadorPruebas.cs` | 13 | Normalización de acentos (López/LOPEZ/gomez) |
| `AutorizacionPruebas.cs` | 25 | Matriz de roles (atributos + handlers) |

---

## 🔄 CI/CD

El pipeline `.github/workflows/ci-cd.yml` se ejecuta en cada push a `main`:

1. **Build**: `dotnet build --configuration Release`
2. **Test**: `dotnet test` (85 pruebas)
3. **Publish**: `dotnet publish`
4. **Deploy**: Azure App Service (`mascotafeliz-prod`)

Los pull requests ejecutan solo build + test.

### Secrets necesarios (GitHub → Settings → Secrets)
- `AZURE_PUBLISH_PROFILE`: publish profile del App Service.

### Configuración de App Service (Azure Portal → Configuration)
- `ConnectionStrings__MascotaFeliz`: connection string de Azure SQL.
- `Storage__ConnectionString`: connection string de Blob Storage.

Ver `docs/despliegue.md` para la guía completa.

---

## 👥 Contribuir

1. Haz los cambios en una rama nueva.
2. `dotnet build` → 0 errores, 0 warnings.
3. `dotnet test` → 0 pruebas fallidas.
4. Verifica que las páginas funcionen.
5. Si añades lógica en Persistencia, incluye pruebas xUnit.
6. No pongas credenciales en el código ni en el workflow.

---

## 📝 Licencia

Proyecto académico — Clínica Veterinaria Mascota Feliz.
