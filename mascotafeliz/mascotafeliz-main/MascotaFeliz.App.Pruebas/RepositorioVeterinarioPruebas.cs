using Microsoft.Data.Sqlite;
using AppContext = MascotaFeliz.App.Persistencia.AppContext;
using System.Linq;
using Xunit;
using MascotaFeliz.App.Dominio;
using MascotaFeliz.App.Persistencia;

namespace MascotaFeliz.App.Pruebas
{
    public class RepositorioVeterinarioPruebas : IDisposable
    {
        private readonly AppContext _contexto;
        private readonly SqliteConnection _conexion;
        private readonly RepositorioVeterinario _repo;

        public RepositorioVeterinarioPruebas()
        {
            (_contexto, _conexion) = ContextoPruebas.CrearContexto();
            _repo = new RepositorioVeterinario(_contexto);
            _repo.AddVeterinario(new Veterinario { Nombres = "Dr. Andrés", Apellidos = "López", Telefono = "3205556677", TarjetaProfesional = "TP0001" });
            _repo.AddVeterinario(new Veterinario { Nombres = "Dra. Laura", Apellidos = "Martínez", Telefono = "3208889900", TarjetaProfesional = "TP0002" });
        }

        public void Dispose()
        {
            _contexto.Dispose();
            _conexion.Dispose();
        }

        [Fact]
        public void AddVeterinario_AgregaYAsignaId()
        {
            var nuevo = new Veterinario { Nombres = "Dr. Jaime", Apellidos = "Duque", Telefono = "3208889900", TarjetaProfesional = "TP0003" };
            var resultado = _repo.AddVeterinario(nuevo);
            Assert.True(resultado.Id > 0);
            Assert.Equal("Dr. Jaime", _repo.GetVeterinario(resultado.Id).Nombres);
        }

        [Fact]
        public void UpdateVeterinario_ActualizaTarjeta()
        {
            var vet = _repo.GetVeterinario(1);
            vet.TarjetaProfesional = "TP9999";
            var actualizado = _repo.UpdateVeterinario(vet);
            Assert.Equal("TP9999", actualizado.TarjetaProfesional);
        }

        [Fact]
        public void GetVeterinariosPorFiltro_CaseInsensitive()
        {
            var minuscula = _repo.GetVeterinariosPorFiltro("lópez").ToList();
            Assert.Single(minuscula);
            Assert.Equal("Dr. Andrés", minuscula[0].Nombres);
        }

        [Fact]
        public void DeleteVeterinario_Elimina()
        {
            _repo.DeleteVeterinario(2);
            Assert.Null(_repo.GetVeterinario(2));
        }
    }
}
