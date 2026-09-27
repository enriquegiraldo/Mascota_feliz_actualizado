using System;
using System.Linq;
using Xunit;
using Microsoft.Data.Sqlite;
using AppContext = MascotaFeliz.App.Persistencia.AppContext;
using MascotaFeliz.App.Dominio;
using MascotaFeliz.App.Persistencia;

namespace MascotaFeliz.App.Pruebas
{
    public class RepositorioCitaPruebas : IDisposable
    {
        private readonly AppContext _contexto;
        private readonly SqliteConnection _conexion;
        private readonly RepositorioCita _repo;
        private readonly RepositorioMascota _repoMascota;
        private readonly RepositorioVeterinario _repoVet;

        public RepositorioCitaPruebas()
        {
            (_contexto, _conexion) = ContextoPruebas.CrearContexto();
            _repo = new RepositorioCita(_contexto);
            _repoMascota = new RepositorioMascota(_contexto);
            _repoVet = new RepositorioVeterinario(_contexto);
            Sembrar();
        }

        private void Sembrar()
        {
            // Necesitamos un dueño y un veterinario antes de la mascota y la cita.
            var repoDueno = new RepositorioDueno(_contexto);
            repoDueno.AddDueno(new Dueno { Nombres = "Juan", Apellidos = "Pérez", Telefono = "3001234567" });
            _repoVet.AddVeterinario(new Veterinario { Nombres = "Dr. Andrés", Apellidos = "López", Telefono = "3205556677", TarjetaProfesional = "TP0001" });
            _repoMascota.AddMascota(new Mascota { Nombre = "Firulais", Especie = "Perro", Raza = "Labrador", DuenoId = 1, VeterinarioId = 1 });
            _repo.AddCita(new Cita
            {
                MascotaId = 1,
                VeterinarioId = 1,
                FechaHora = new DateTime(2026, 9, 28, 10, 0, 0),
                Motivo = "Control de vacunación",
                Estado = EstadoCita.Pendiente
            });
            _repo.AddCita(new Cita
            {
                MascotaId = 1,
                VeterinarioId = 1,
                FechaHora = new DateTime(2026, 8, 15, 9, 0, 0),
                Motivo = "Consulta general",
                Estado = EstadoCita.Atendida
            });
        }

        public void Dispose()
        {
            _contexto.Dispose();
            _conexion.Dispose();
        }

        [Fact]
        public void AddCita_AgregaYAsignaId()
        {
            var nueva = new Cita
            {
                MascotaId = 1,
                VeterinarioId = 1,
                FechaHora = new DateTime(2026, 10, 1, 14, 0, 0),
                Motivo = "Desparasitación",
                Estado = EstadoCita.Confirmada
            };
            var resultado = _repo.AddCita(nueva);
            Assert.True(resultado.Id > 0);
            Assert.Equal("Desparasitación", _repo.GetCita(resultado.Id).Motivo);
        }

        [Fact]
        public void GetCita_IncluyeRelaciones()
        {
            var cita = _repo.GetCita(1);
            Assert.NotNull(cita);
            Assert.NotNull(cita.Mascota);
            Assert.NotNull(cita.Veterinario);
            Assert.Equal("Firulais", cita.Mascota.Nombre);
            Assert.Equal("Dr. Andrés", cita.Veterinario.Nombres);
        }

        [Fact]
        public void UpdateCita_ActualizaEstadoYFecha()
        {
            var cita = _repo.GetCita(1);
            cita.Estado = EstadoCita.Confirmada;
            cita.FechaHora = new DateTime(2026, 9, 30, 11, 0, 0);
            var actualizada = _repo.UpdateCita(cita);
            Assert.Equal(EstadoCita.Confirmada, actualizada.Estado);
            Assert.Equal(new DateTime(2026, 9, 30, 11, 0, 0), actualizada.FechaHora);
        }

        [Fact]
        public void GetCitasPorMascota_DevuelveSoloLasDeEsaMascota()
        {
            var citas = _repo.GetCitasPorMascota(1).ToList();
            Assert.Equal(2, citas.Count);
            Assert.All(citas, c => Assert.Equal(1, c.MascotaId));
        }

        [Fact]
        public void GetCitasPorVeterinario_DevuelveSoloLasDeEseVet()
        {
            var citas = _repo.GetCitasPorVeterinario(1).ToList();
            Assert.Equal(2, citas.Count);
            Assert.All(citas, c => Assert.Equal(1, c.VeterinarioId));
        }

        [Fact]
        public void GetCitasPorEstado_DevuelveSoloLasDeEseEstado()
        {
            var pendientes = _repo.GetCitasPorEstado(EstadoCita.Pendiente).ToList();
            var atendidas = _repo.GetCitasPorEstado(EstadoCita.Atendida).ToList();
            Assert.Single(pendientes);
            Assert.Single(atendidas);
            Assert.All(pendientes, c => Assert.Equal(EstadoCita.Pendiente, c.Estado));
            Assert.All(atendidas, c => Assert.Equal(EstadoCita.Atendida, c.Estado));
        }

        [Fact]
        public void GetCitasPorFiltro_BuscaPorMotivo()
        {
            var porMotivo = _repo.GetCitasPorFiltro("vacunación").ToList();
            Assert.Single(porMotivo);
            Assert.Contains("vacunación", porMotivo[0].Motivo);
        }

        [Fact]
        public void GetCitasPorFiltro_BuscaPorFecha()
        {
            var porFecha = _repo.GetCitasPorFiltro("28/09/2026").ToList();
            Assert.Single(porFecha);
            Assert.Equal(1, porFecha[0].Id);
        }

        [Fact]
        public void GetCitasPorFiltro_AccentInsensitive()
        {
            var sinAcento = _repo.GetCitasPorFiltro("vacunacion").ToList();
            Assert.NotEmpty(sinAcento);
            Assert.All(sinAcento, c => Assert.Contains("vacunación", c.Motivo));
        }

        [Fact]
        public void DeleteCita_Elimina()
        {
            _repo.DeleteCita(1);
            Assert.Null(_repo.GetCita(1));
        }

        [Fact]
        public void GetAllCitas_IncluyeRelaciones()
        {
            var todas = _repo.GetAllCitas().ToList();
            Assert.Equal(2, todas.Count);
            Assert.All(todas, c => Assert.NotNull(c.Mascota));
            Assert.All(todas, c => Assert.NotNull(c.Veterinario));
        }
    }
}
