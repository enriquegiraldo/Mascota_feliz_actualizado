using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MascotaFeliz.App.Persistencia;
using AppContext = MascotaFeliz.App.Persistencia.AppContext;

namespace MascotaFeliz.App.Pruebas
{
    /// <summary>
    /// Fábrica de AppContext con SQLite en memoria para pruebas.
    /// Mantiene la conexión abierta mientras dura el test.
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
