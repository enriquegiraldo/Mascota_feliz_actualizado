using Microsoft.Data.Sqlite;
using AppContext = MascotaFeliz.App.Persistencia.AppContext;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xunit;
using MascotaFeliz.App.Dominio;
using MascotaFeliz.App.Persistencia;

namespace MascotaFeliz.App.Pruebas
{
    /// <summary>
    /// Pruebas del repositorio de mascotas.
    /// </summary>
    public class RepositorioMascotaPruebas : IDisposable
    {
        private readonly AppContext _contexto;
        private readonly SqliteConnection _conexion;
        private readonly RepositorioMascota _repo;
        private readonly RepositorioDueno _repoDueno;
        private readonly RepositorioVeterinario _repoVet;
        private readonly RepositorioHistoria _repoHistoria;

        public RepositorioMascotaPruebas()
        {
            (_contexto, _conexion) = ContextoPruebas.CrearContexto();
            _repo = new RepositorioMascota(_contexto);
            _repoDueno = new RepositorioDueno(_contexto);
            _repoVet = new RepositorioVeterinario(_contexto);
            _repoHistoria = new RepositorioHistoria(_contexto);
            Sembrar();
        }

        private void Sembrar()
        {
            _repoDueno.AddDueno(new Dueno { Nombres = "Juan", Apellidos = "Pérez", Telefono = "3001234567" });
            _repoDueno.AddDueno(new Dueno { Nombres = "María", Apellidos = "Gómez", Telefono = "3007654321" });
            _repoVet.AddVeterinario(new Veterinario { Nombres = "Dr. Andrés", Apellidos = "López", Telefono = "3205556677", TarjetaProfesional = "TP0001" });
            _repoHistoria.AddHistoria(new Historia { FechaInicial = new DateTime(2023, 1, 15) });
            _repo.AddMascota(new Mascota { Nombre = "Firulais", Especie = "Perro", Raza = "Labrador", Color = "Café", DuenoId = 1, VeterinarioId = 1, HistoriaId = 1 });
            _repo.AddMascota(new Mascota { Nombre = "Michi", Especie = "Gato", Raza = "Siamés", Color = "Negro", DuenoId = 1 });
            _repo.AddMascota(new Mascota { Nombre = "Susi", Especie = "Perro", Raza = "Chihuahua", Color = "Blanca", DuenoId = 2, VeterinarioId = 1 });
        }

        public void Dispose()
        {
            _contexto.Dispose();
            _conexion.Dispose();
        }

        [Fact]
        public void AddMascota_AgregaYAsignaId()
        {
            var nueva = new Mascota { Nombre = "Rocky", Especie = "Perro", Raza = "Husky" };
            var resultado = _repo.AddMascota(nueva);
            Assert.True(resultado.Id > 0);
            Assert.Equal("Rocky", _repo.GetMascota(resultado.Id).Nombre);
        }

        [Fact]
        public void GetMascota_IncluyeRelaciones()
        {
            var mascota = _repo.GetMascota(1);
            Assert.NotNull(mascota);
            Assert.NotNull(mascota.Dueno);
            Assert.NotNull(mascota.Veterinario);
            Assert.NotNull(mascota.Historia);
            Assert.Equal("Juan", mascota.Dueno.Nombres);
            Assert.Equal("Dr. Andrés", mascota.Veterinario.Nombres);
        }

        [Fact]
        public void UpdateMascota_PreservaRelacionesYActualizaCampos()
        {
            var mascota = _repo.GetMascota(1);
            mascota.Nombre = "Firulais Editado";
            mascota.Color = "Gris";
            var actualizada = _repo.UpdateMascota(mascota);
            Assert.Equal("Firulais Editado", actualizada.Nombre);
            Assert.Equal("Gris", actualizada.Color);
            // Los vínculos se preservan.
            Assert.Equal(1, actualizada.DuenoId);
            Assert.Equal(1, actualizada.VeterinarioId);
            Assert.Equal(1, actualizada.HistoriaId);
        }

        [Fact]
        public void UpdateMascota_PermiteDesvincularHistoriaConNull()
        {
            // Decisión de la ronda 4: el selector permite "sin historia".
            // UpdateMascota copia HistoriaId tal cual (incluso null → desvincula).
            var mascota = _repo.GetMascota(1);
            mascota.HistoriaId = null;
            var actualizada = _repo.UpdateMascota(mascota);
            Assert.Null(actualizada.HistoriaId);
        }

        [Fact]
        public void UpdateMascota_PermiteDesvincularDuenoYVeterinario()
        {
            var mascota = _repo.GetMascota(1);
            mascota.DuenoId = null;
            mascota.VeterinarioId = null;
            var actualizada = _repo.UpdateMascota(mascota);
            Assert.Null(actualizada.DuenoId);
            Assert.Null(actualizada.VeterinarioId);
        }

        [Fact]
        public void GetMascotasPorFiltro_CaseInsensitive()
        {
            var minuscula = _repo.GetMascotasPorFiltro("fir").ToList();
            var mayuscula = _repo.GetMascotasPorFiltro("FIR").ToList();
            Assert.Single(minuscula);
            Assert.Equal("Firulais", minuscula[0].Nombre);
            Assert.Single(mayuscula);
            Assert.Equal("Firulais", mayuscula[0].Nombre);
        }

        [Fact]
        public void GetMascotasPorFiltro_BuscaEnEspecieYRaza()
        {
            var porEspecie = _repo.GetMascotasPorFiltro("gato").ToList();
            var porRaza = _repo.GetMascotasPorFiltro("labrador").ToList();
            Assert.Single(porEspecie);
            Assert.Equal("Michi", porEspecie[0].Nombre);
            Assert.Single(porRaza);
            Assert.Equal("Firulais", porRaza[0].Nombre);
        }

        [Fact]
        public void GetMascotasPorDueno_DevuelveSoloLasDeEseDueno()
        {
            var deJuan = _repo.GetMascotasPorDueno(1).ToList();
            var deMaria = _repo.GetMascotasPorDueno(2).ToList();
            Assert.Equal(2, deJuan.Count);
            Assert.Single(deMaria);
            Assert.Equal("Susi", deMaria[0].Nombre);
            Assert.All(deJuan, m => Assert.Equal(1, m.DuenoId));
        }

        [Fact]
        public void GetMascotasPorVeterinario_DevuelveSoloLasDeEseVet()
        {
            var delVet1 = _repo.GetMascotasPorVeterinario(1).ToList();
            Assert.Equal(2, delVet1.Count);
            Assert.All(delVet1, m => Assert.Equal(1, m.VeterinarioId));
        }

        [Fact]
        public void GetEspecies_DevuelveDistintasYOrdenadas()
        {
            var especies = _repo.GetEspecies().ToList();
            Assert.Equal(2, especies.Count);
            Assert.Equal("Gato", especies[0]);
            Assert.Equal("Perro", especies[1]);
        }

        [Fact]
        public void GetMascotaPorHistoriaId_DevuelveMascotaCorrecta()
        {
            var mascota = _repo.GetMascotaPorHistoriaId(1);
            Assert.NotNull(mascota);
            Assert.Equal("Firulais", mascota.Nombre);
        }

        [Fact]
        public void DeleteMascota_Elimina()
        {
            _repo.DeleteMascota(2);
            Assert.Null(_repo.GetMascota(2));
        }

        [Fact]
        public void GetAllMascotas_IncluyeDuenoYVeterinario()
        {
            var todas = _repo.GetAllMascotas().ToList();
            Assert.Equal(3, todas.Count);
            Assert.All(todas, m => Assert.NotNull(m.Dueno));
        }
    }
}
