# 🤖 Instrucciones para Agentes / IA (AGENTE.md)

Archivo de guía para que cualquier agente de IA (incluyendo TRAE) pueda contribuir al repositorio **MascotaFelizGrupo5** de forma correcta y consistente.

---

## 🧭 Reglas Generales OBLIGATORIAS

1. **Idioma de la base de código:** Español. Nombres de clases, métodos, archivos, páginas y comentarios en español.
2. **Arquitectura en 4 capas (SÍGUELA ESTRICTAMENTE):**
   - **Dominio →** solo entidades y enumeraciones. SIN referencias externas.
   - **Persistencia →** DbContext, interfaces y repositorios. Depende solo de Dominio.
   - **Frontend →** Razor Pages + DI. Depende de Dominio y Persistencia.
   - **Consola →** pruebas rápidas. Depende de Dominio y Persistencia.
3. **NUNCA pongas lógica de negocios en el Frontend.** Eso pertenece a Persistencia (repositorios) o a una capa de Servicios si se agrega.
4. **Nombres PascalCase** para clases, namespaces y propiedades.
5. **Nombres camelCase** para variables locales y campos privados.
6. **No realices commits** a menos que el usuario lo pida explícitamente.
7. **No elimines páginas o entidades existentes** sin confirmar con el usuario.

---

## 🏗️ Cómo agregar una nueva entidad (ej. `Empleado`)

### Paso 1: Capa Dominio
Crea `MascotaFeliz.App.Dominio/Empleado.cs`
```csharp
namespace MascotaFeliz.App.Dominio
{
    public class Empleado
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        // ... otras propiedades
    }
}
```

### Paso 2: Capa Persistencia
**a)** Agregar `DbSet` en `AppContext.cs`:
```csharp
public DbSet<Empleado> Empleados { get; set; }
```

**b)** Crear interfaz `IRepositorioEmpleado.cs`
**c)** Crear implementación `RepositorioEmpleado.cs`
**d)** Configurar relaciones en `AppContext.OnModelCreating` (si hay FKs).

### Paso 3: Capa Frontend
Crea una carpeta `Pages/Empleados/` con:
- `ListaEmpleados.cshtml` + `ListaEmpleados.cshtml.cs` (lectura)
- `DetallesEmpleados.cshtml` + `DetallesEmpleados.cshtml.cs` (detalle)
- `EditarEmpleado.cshtml` + `EditarEmpleado.cshtml.cs` (crear/editar)

Agrega el link al menú en `Pages/Shared/_Layout.cshtml`:
```html
<li class="nav-item">
    <a class="nav-link text-dark" asp-area="" asp-page="/Empleados/ListaEmpleados">Empleados</a>
</li>
```

### Paso 4: Seed Data (opcional)
En `MascotaFeliz.App.Frontend/Program.cs`, dentro de `EnsureCreated`, agrega registros ejemplo:
```csharp
if (!context.Empleados.Any()) { /* ... insert ... */ }
```

### Paso 5: Verificar
```bash
dotnet build
# Si hay errores, arréglalos. Debe compilar con 0 errores.
```

---

## ⚙️ Patrón Repositorio usado en el proyecto

Siempre sigue esta interfaz mínima para un repositorio:

```csharp
public interface IRepositorio<T>
{
    T        Add(T entidad);
    T        Get(int id);
    T        Update(T entidad);
    IEnumerable<T> GetAll();
    IEnumerable<T> GetPorFiltro(string filtro);   // opcional
    void     Delete(int id);
}
```

Las implementaciones **deben recibir** `AppContext` vía constructor (sin instanciarlo internamente).

---

## 🏗️ Cómo agregar una relación entre entidades

Ejemplo: una `Mascota` tiene un `Dueno` (uno a muchos).

1. **Dominio → Mascota.cs**:
   ```csharp
   public int? DuenoId { get; set; }
   public Dueno Dueno { get; set; }
   ```

2. **Persistencia → AppContext.OnModelCreating**:
   ```csharp
   modelBuilder.Entity<Mascota>()
       .HasOne(m => m.Dueno)
       .WithMany()
       .HasForeignKey(m => m.DuenoId)
       .OnDelete(DeleteBehavior.Restrict);
   ```

3. **RepositorioMascota.GetMascota / GetAllMascotas**:
   ```csharp
   return _appContext.Mascotas.Include(m => m.Dueno);
   ```

4. **Seed Data:** Asegurar que la entidad relacionada exista antes de referenciarla.

---

## 🔍 Cómo localizar código rápidamente

| Qué buscas | Dónde está |
|---|---|
| Entidad `Dueno` | [Dueno.cs](file:///C:/Users/Santiago%20Sofia/Downloads/MascotaFelizGrupo5/MascotaFeliz.App.Dominio/Dueno.cs) |
| CRUD Dueños (web) | [Pages/Duenos/](file:///C:/Users/Santiago%20Sofia/Downloads/MascotaFelizGrupo5/MascotaFeliz.App.Frontend/Pages/Duenos) |
| Repositorio Mascota | [RepositorioMascota.cs](file:///C:/Users/Santiago%20Sofia/Downloads/MascotaFelizGrupo5/MascotaFeliz.App.Persistencia/RepositorioMascota.cs) |
| Configuración EF / Relaciones | [AppContext.cs](file:///C:/Users/Santiago%20Sofia/Downloads/MascotaFelizGrupo5/MascotaFeliz.App.Persistencia/AppContext.cs) |
| Layout / Navbar principal | [_Layout.cshtml](file:///C:/Users/Santiago%20Sofia/Downloads/MascotaFelizGrupo5/MascotaFeliz.App.Frontend/Pages/Shared/_Layout.cshtml) |
| Punto de entrada / Seed Data | [Program.cs](file:///C:/Users/Santiago%20Sofia/Downloads/MascotaFelizGrupo5/MascotaFeliz.App.Frontend/Program.cs) |
| _ViewImports (namespaces globales) | [_ViewImports.cshtml](file:///C:/Users/Santiago%20Sofia/Downloads/MascotaFelizGrupo5/MascotaFeliz.App.Frontend/Pages/_ViewImports.cshtml) |

---

## 🛠️ Comandos frecuentes

```bash
# Restaurar dependencias
dotnet restore

# Compilar (Obligatorio antes de asumir que algo funciona)
dotnet build

# Ejecutar Frontend en modo desarrollo (PowerShell)
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet run --project MascotaFeliz.App.Frontend

# Ejecutar Frontend con hot reload
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet watch run --project MascotaFeliz.App.Frontend

# Ejecutar proyecto consola
dotnet run --project MascotaFeliz.App.Consola

# Limpiar bin/obj
dotnet clean
```

---

## ⚠️ Problemas comunes y soluciones

| Error | Causa | Solución |
|---|---|---|
| `AppContext` ambiguo con `System.AppContext` | Conflicto de nombres | En `Program.cs` usar: `using AppContext = MascotaFeliz.App.Persistencia.AppContext;` |
| Error `AddDbContext was called with configuration but context only declares parameterless constructor` | Falta constructor con `DbContextOptions` | Añadir a `AppContext`: `public AppContext(DbContextOptions<AppContext> options) : base(options) {}` |
| Razor no encuentra `*Model` | Falta namespace en `_ViewImports` | Añadir `@namespace MascotaFeliz.App.Frontend.Pages` y `@using MascotaFeliz.App.Frontend.Pages` |
| `Framework Microsoft.AspNetCore.App 8.0 not found` | SDK instalado es 9.0, csproj apunta a 8.0 | Cambiar todos los `TargetFramework` a `net9.0` |
| Datos duplicados después de un reinicio | Seed data corre cada vez | Envolver cada insert en `if (!context.Tabla.Any())` (ya hecho en Program.cs) |
| "Failed to determine the https port" | No hay certificado dev HTTPS | Ejecutar `dotnet dev-certs https --trust` o usar HTTP (`http://localhost:5000`) |

---

## ✅ Checklist antes de entregar cambios

- [ ] `dotnet build` pasa con **0 errores y 0 warnings** nuevos.
- [ ] Todas las entidades nuevas tienen `Id` (PK) y propiedades en PascalCase.
- [ ] Las relaciones están configuradas en `AppContext.OnModelCreating`.
- [ ] Las páginas Razor usan el namespace correcto `MascotaFeliz.App.Frontend.Pages.*` (**Pages** plural).
- [ ] El link a la nueva sección fue agregado en `Pages/Shared/_Layout.cshtml`.
- [ ] No hay archivos `.db` viejos en el repo si no se desea conservar datos.

---

## 🧪 Cómo verificar funcionalidad (sanity check)

Después de cualquier cambio, ejecuta la app y comprueba:

1. Abre `http://localhost:5000/` → Página de inicio muestra 3 tarjetas.
2. Abre `http://localhost:5000/Duenos/ListaDuenos` → Muestra 3 registros.
3. Entra en "Editar Dueño" (cualquiera), cambia un campo, guardar → Lista refleja el cambio.
4. Lo mismo para `Veterinarios` y `Mascotas`.
5. Clickear "Detalle" → Muestra los campos correctamente.
6. Crear un registro nuevo → Aparece en la lista.

---

## 📌 Notas finales

- La base de datos es **local y efímera** (archivo SQLite). Para reiniciarla, borra `MascotaFeliz.App.Frontend/MascotaFeliz.db`.
- El proyecto no usa migraciones formales por defecto, usa `Database.EnsureCreated()`; si luego se requieren, hay que crearlas con `dotnet ef migrations add`.
- La UI usa **Bootstrap por CDN** (no requiere `lib` local ni `npm`).
