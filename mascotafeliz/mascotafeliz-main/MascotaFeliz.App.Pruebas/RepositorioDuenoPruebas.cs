using Microsoft.Data.Sqlite;
using AppContext = MascotaFeliz.App.Persistencia.AppContext;
using System.Linq;
using Xunit;
using MascotaFeliz.App.Dominio;
using MascotaFeliz.App.Persistencia;

namespace MascotaFeliz.App.Pruebas
{
    public class RepositorioDuenoPruebas : IDisposable
    {
        private readonly AppContext _contexto;
        private readonly SqliteConnection _conexion;
        private readonly RepositorioDueno _repo;

        public RepositorioDuenoPruebas()
        {
            (_contexto, _conexion) = ContextoPruebas.CrearContexto();
            _repo = new RepositorioDueno(_contexto);
            _repo.AddDueno(new Dueno { Nombres = "Juan", Apellidos = "Pérez", Telefono = "3001234567" });
            _repo.AddDueno(new Dueno { Nombres = "María", Apellidos = "Gómez", Telefono = "3007654321" });
            _repo.AddDueno(new Dueno { Nombres = "Carlos", Apellidos = "Rodríguez", Telefono = "3101112233" });
        }

        public void Dispose()
        {
            _contexto.Dispose();
            _conexion.Dispose();
        }

        [Fact]
        public void AddDueno_AgregaYAsignaId()
        {
            var nuevo = new Dueno { Nombres = "Ana", Apellidos = "Torres", Telefono = "3205556677" };
            var resultado = _repo.AddDueno(nuevo);
            Assert.True(resultado.Id > 0);
            Assert.Equal("Ana", _repo.GetDueno(resultado.Id).Nombres);
        }

        [Fact]
        public void UpdateDueno_ActualizaCampos()
        {
            var dueno = _repo.GetDueno(1);
            dueno.Telefono = "3009998888";
            var actualizado = _repo.UpdateDueno(dueno);
            Assert.Equal("3009998888", actualizado.Telefono);
        }

        [Fact]
        public void GetDuenosPorFiltro_CaseInsensitive()
        {
            var minuscula = _repo.GetDuenosPorFiltro("juan").ToList();
            var mayuscula = _repo.GetDuenosPorFiltro("JUAN").ToList();
            Assert.Single(minuscula);
            Assert.Equal("Juan", minuscula[0].Nombres);
            Assert.Single(mayuscula);
            Assert.Equal("Juan", mayuscula[0].Nombres);
        }

        [Fact]
        public void GetDuenosPorFiltro_BuscaPorApellido()
        {
            var porApellido = _repo.GetDuenosPorFiltro("gómez").ToList();
            Assert.Single(porApellido);
            Assert.Equal("María", porApellido[0].Nombres);
        }

        [Fact]
        public void DeleteDueno_Elimina()
        {
            _repo.DeleteDueno(3);
            Assert.Null(_repo.GetDueno(3));
        }

        [Fact]
        public void GetAllDuenos_DevuelveTodos()
        {
            var todos = _repo.GetAllDuenos().ToList();
            Assert.Equal(3, todos.Count);
        }
    }
}
