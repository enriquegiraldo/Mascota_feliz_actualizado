# 🐾 Mascota Feliz - Grupo 5

Sistema de gestión para la clínica veterinaria **Mascota Feliz**, desarrollado con **ASP.NET Core Razor Pages** y **Entity Framework Core** sobre **SQLite**, siguiendo una arquitectura en capas (Dominio, Persistencia, Frontend, Consola).

---

## 📋 Tabla de Contenidos

1. [Tecnologías](#tecnologías)
2. [Estructura del Proyecto](#estructura-del-proyecto)
3. [Requisitos Previos](#requisitos-previos)
4. [Primeros Pasos](#primeros-pasos)
5. [Ejecución en Modo Desarrollo](#ejecución-en-modo-desarrollo)
6. [Compilar y Publicar](#compilar-y-publicar)
7. [Base de Datos](#base-de-datos)
8. [URLs y Navegación](#urls-y-navegación)
9. [Contribuir](#contribuir)
10. [Licencia](#licencia)

---

## 🛠️ Tecnologías

- **.NET 10.0 LTS** (SDK `10.0.401`) — migrado desde .NET 9 STS.
- **ASP.NET Core Razor Pages** (Frontend)
- **Entity Framework Core 10.0.12** (ORM) sobre SQLite (en proceso de migración a Azure SQL).
- **SQLite** (Base de datos embebida para dev; producción será Azure SQL Database).
- **Bootstrap 5.3** (UI base por CDN) + **tema propio** en `wwwroot/css/site.css` (en proceso de reemplazo desde Bootstrap 4.6).
- **jQuery Validate** (validación en cliente) y **JS ligero** propio en `wwwroot/js/site.js`, `wwwroot/js/command-palette.js`.

> **Fase 3 en curso:** migración a .NET 10 completada. Pendiente: cambio a Bootstrap 5 + nueva identidad visual, migración SQLite→Azure SQL, Blob Storage para fotos, ASP.NET Core Identity con roles, entidad Cita. Ver `docs/decisiones.md` y `docs/propuesta-visual.md`.

---

## 📁 Estructura del Proyecto

```
MascotaFelizGrupo5/
├── MascotaFeliz.sln                          ← Archivo de solución
│
├── MascotaFeliz.App.Dominio/                 ← 🎯 Capa de Dominio (entidades)
│   ├── Dueno.cs
│   ├── Veterinario.cs
│   ├── Mascota.cs
│   ├── Historia.cs
│   ├── VisitaPyP.cs
│   └── MascotaFeliz.App.Dominio.csproj
│
├── MascotaFeliz.App.Persistencia/            ← 🗄️ Capa de Persistencia
│   ├── AppContext.cs                         ← DbContext de EF Core
│   ├── IRepositorioDueno.cs                  ← Interfaces
│   ├── IRepositorioVeterinario.cs
│   ├── IRepositorioMascota.cs
│   ├── IRepositorioHistoria.cs
│   ├── IRepositorioVisitaPyP.cs
│   ├── RepositorioDueno.cs                   ← Implementaciones
│   ├── RepositorioVeterinario.cs
│   ├── RepositorioMascota.cs
│   ├── RepositorioHistoria.cs
│   ├── RepositorioVisitaPyP.cs
│   └── MascotaFeliz.App.Persistencia.csproj
│
├── MascotaFeliz.App.Frontend/                ← 🌐 Capa de Presentación (Web)
│   ├── Program.cs                            ← Punto de entrada, DI, seed data
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   ├── Pages/
│   │   ├── Index.cshtml / Index.cshtml.cs    ← Página de inicio
│   │   ├── Privacy.cshtml / Privacy.cshtml.cs
│   │   ├── NotFound.cshtml / NotFound.cshtml.cs
│   │   ├── _ViewStart.cshtml
│   │   ├── _ViewImports.cshtml
│   │   ├── Shared/
│   │   │   ├── _Layout.cshtml                ← Layout principal (menú, navbar, footer pegado)
│   │   │   └── _ValidationScriptsPartial.cshtml
│   │   ├── Duenos/                           ← CRUD Dueños (buscar, eliminar, paginar, validar)
│   │   ├── Veterinarios/                     ← CRUD Veterinarios (buscar, eliminar, paginar, validar)
│   │   ├── Mascotas/                         ← CRUD Mascotas (selectores dueño/vet, perfil con línea de tiempo)
│   │   ├── Historias/                        ← Lista, detalle y editar historias clínicas
│   │   ├── VisitasPyP/                       ← CRUD Visitas de PyP (signos vitales + recomendaciones)
│   │   ├── Error.cshtml                      ← Página de errores (UseExceptionHandler)
│   │   ├── NotFound.cshtml                   ← 404 con estilo
│   │   └── Privacy.cshtml                    ← Política de privacidad (es-CO)
│   ├── wwwroot/                              ← 🎨 Recursos estáticos propios (NUEVO)
│   │   ├── css/site.css                      ← Tokens + tema "Cálida clínica"
│   │   ├── js/site.js                        ← Confirmar borrado, toasts, nav activa
│   │   └── img/logo.svg, favicon.svg         ← Identidad propia (SVG, licencia libre)
│   └── MascotaFeliz.App.Frontend.csproj
│
├── MascotaFeliz.App.Consola/                 ← 💻 Proyecto de Consola (pruebas)
│   ├── Program.cs
│   └── MascotaFeliz.App.Consola.csproj
│
├── MascotaFeliz.App.Pruebas/                 ← 🧪 Proyecto de Pruebas (xUnit)
│   ├── ContextoPruebas.cs                   ← Helper SQLite en memoria
│   ├── RepositorioMascotaPruebas.cs
│   ├── RepositorioDuenoPruebas.cs
│   ├── RepositorioVeterinarioPruebas.cs
│   ├── RepositorioHistoriaVisitaPruebas.cs
│   └── MascotaFeliz.App.Pruebas.csproj
│
└── README.md
```

---

## ✅ Requisitos Previos

- **.NET 10.0 SDK** (LTS) o superior
  - Verificar instalación:
    ```bash
    dotnet --version
    ```
  - Descargar: <https://dotnet.microsoft.com/es-es/download>

---

## 🚀 Primeros Pasos

### 1. Clonar / Acceder al repositorio

```bash
cd "C:\Users\Santiago Sofia\Downloads\MascotaFelizGrupo5"
```

### 2. Restaurar paquetes NuGet

```bash
dotnet restore
```

### 3. Compilar la solución

```bash
dotnet build
```

---

## 👨‍💻 Ejecución en Modo Desarrollo

### Opción A: Con Hot Reload (recomendada)

Cada cambio en `.cs` o `.cshtml` se aplica **sin reiniciar** el servidor.

```powershell
# PowerShell
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet watch run --project MascotaFeliz.App.Frontend
```

```bat
:: CMD (símbolo del sistema)
set ASPNETCORE_ENVIRONMENT=Development
dotnet watch run --project MascotaFeliz.App.Frontend
```

### Opción B: Sin Hot Reload

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet run --project MascotaFeliz.App.Frontend
```

> ⚠️ **Importante:** `ASPNETCORE_ENVIRONMENT=Development` activa:
>
> - Páginas de error detalladas
> - Recarga automática de archivos estáticos
> - Mensajes de logging más verbosos

---

## 📦 Compilar y Publicar

### Compilar en modo Release

```bash
dotnet build -c Release
```

### Publicar para producción

```bash
dotnet publish MascotaFeliz.App.Frontend -c Release -o ./publish
```

El contenido listo para deploy queda en la carpeta `publish/`.

---

## 🗄️ Base de Datos

El proyecto usa **SQLite**, por lo que no requiere instalar ningún motor de base de datos. Todo se guarda en un archivo local.

### Ubicación

`MascotaFeliz.App.Frontend/MascotaFeliz.db` (se crea automáticamente al iniciar).

### Cadena de conexión

Está configurada en `Program.cs`:

```csharp
optionsBuilder.UseSqlite("Data Source=MascotaFeliz.db");
```

### Datos de ejemplo (Seed Data)

Al iniciar la aplicación por primera vez, se insertan automáticamente registros de prueba:

- 3 **Dueños** (Juan Pérez, María Gómez, Carlos Rodríguez)
- 3 **Veterinarios** (Dr. Andrés, Dra. Laura, Dr. Jaime)
- 3 **Historias Clínicas**
- 4 **Mascotas** (Firulais, Susi, Michi, Rocky)
- 3 **Visitas PyP** (2 para Firulais, 1 para Michi) con signos vitales y recomendaciones

> Para reiniciar la base de datos, **borra** el archivo `MascotaFeliz.db` y vuelve a ejecutar la app.

### Migraciones (opcional)

Si necesitas generar migraciones formalmente:

```bash
# Crear migración inicial
dotnet ef migrations add InitialCreate --project MascotaFeliz.App.Persistencia --startup-project MascotaFeliz.App.Frontend

# Aplicar migraciones
dotnet ef database update --project MascotaFeliz.App.Persistencia --startup-project MascotaFeliz.App.Frontend
```

---

## 🌐 URLs y Navegación

| Ruta                                                 | Descripción                |
| ---------------------------------------------------- | -------------------------- |
| `http://localhost:5000/`                             | Página de inicio           |
| `/Duenos/ListaDuenos`                                | Listado de dueños          |
| `/Duenos/DetallesDuenos?duenoId=1`                   | Detalles de un dueño       |
| `/Duenos/EditarDueno`                                | Crear/Editar dueño         |
| `/Veterinarios/ListaVeterinarios`                    | Listado de veterinarios    |
| `/Veterinarios/DetallesVeterinarios?veterinarioId=1` | Detalles de un veterinario |
| `/Veterinarios/EditarVeterinario`                    | Crear/Editar veterinario   |
| `/Mascotas/ListaMascotas`                            | Listado de mascotas        |
| `/Mascotas/DetallesMascotas?mascotaId=1`             | Detalles de una mascota    |
| `/Mascotas/EditarMascota`                            | Crear/Editar mascota       |
| `/Privacy`                                           | Política de privacidad     |
| `/NotFound`                                          | Página 404 con estilo      |
| `/Error`                                             | Página de errores (handler)|
| `/Historias/ListaHistorias`                          | Listado de historias clínicas |
| `/Historias/DetallesHistoria?historiaId=1`           | Detalle de historia + línea de tiempo |
| `/Historias/EditarHistoria`                          | Crear historia clínica      |
| `/Historias/EditarHistoria?historiaId=1`             | Editar / eliminar historia  |
| `/VisitasPyP/ListaVisitasPyP`                        | Listado de visitas PyP     |
| `/VisitasPyP/DetallesVisitaPyP?visitaPyPId=1`        | Detalle de una visita      |
| `/VisitasPyP/EditarVisitaPyP?historiaId=1`           | Crear visita para una historia |

> Por defecto el servidor escucha en **HTTP (5000)**. Para HTTPS (5001):
>
> ```bash
> dotnet dev-certs https --trust
> ```

---

## 👥 Contribuir

1. Haz los cambios en una rama nueva.
2. Asegúrate de que `dotnet build` compile sin errores.
3. Ejecuta `dotnet test` para verificar que las pruebas xUnit pasen.
4. Verifica que las páginas CRUD funcionen correctamente.
5. No elimines datos de ejemplo a menos que se indique.

### Pruebas (xUnit)

El proyecto `MascotaFeliz.App.Pruebas` contiene pruebas unitarias de la capa de
Persistencia usando SQLite en memoria (`DataSource=:memory:`). Cubren CRUD,
relaciones, búsqueda case-insensitive y la política de borrado.

```bash
dotnet test
```

---

## 📝 Licencia

Proyecto académico - Grupo 5 / Clínica Veterinaria Mascota Feliz.
