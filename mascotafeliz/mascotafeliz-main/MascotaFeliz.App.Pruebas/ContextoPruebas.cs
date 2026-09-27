using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MascotaFeliz.App.Persistencia;
using AppContext = MascotaFeliz.App.Persistencia.AppContext;

namespace MascotaFeliz.App.Pruebas
{
    /// <summary>
    /// Fábrica de AppContext para pruebas.
    ///
    /// Usa SQLite en memoria (DataSource=:memory:) porque:
    /// - Es rápido (no requiere servidor).
    /// - Funciona en cualquier SO (Linux CI, Windows dev, macOS).
    /// - Preserva el comportamiento relacional (FK, NOT NULL, case-insensitivity).
    /// - No requiere LocalDB (solo Windows) ni Docker.
    ///
    /// En el entorno Windows de Giraldo se puede cambiar a SQL Server LocalDB
    /// para probar el proveedor real de producción, pero las pruebas unitarias
    /// de lógica de repositorios funcionan igual con SQLite.
    /// </summary>
    public static class ContextoPruebas
    {
        public static (AppContext contexto, SqliteConnection conexion) CrearContexto()
        {
            var conexion = new SqliteConnection("DataSource=:memory:");
            conexion.Open();

            var opciones = new DbContextOptionsBuilder<AppContext>()
                .UseSqlite(conexion)
                .Options;

            var contexto = new AppContext(opciones);
            contexto.Database.EnsureCreated();
            return (contexto, conexion);
        }
    }
}
