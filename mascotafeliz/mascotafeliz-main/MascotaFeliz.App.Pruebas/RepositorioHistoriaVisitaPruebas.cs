using Microsoft.Data.Sqlite;
using AppContext = MascotaFeliz.App.Persistencia.AppContext;
using System;
using System.Linq;
using Xunit;
using MascotaFeliz.App.Dominio;
using MascotaFeliz.App.Persistencia;

namespace MascotaFeliz.App.Pruebas
{
    public class RepositorioHistoriaVisitaPruebas : IDisposable
    {
        private readonly AppContext _contexto;
        private readonly SqliteConnection _conexion;
        private readonly RepositorioHistoria _repoHistoria;
        private readonly RepositorioVisitaPyP _repoVisita;

        public RepositorioHistoriaVisitaPruebas()
        {
            (_contexto, _conexion) = ContextoPruebas.CrearContexto();
            _repoHistoria = new RepositorioHistoria(_contexto);
            _repoVisita = new RepositorioVisitaPyP(_contexto);
            Sembrar();
        }

        private void Sembrar()
        {
            _repoHistoria.AddHistoria(new Historia { FechaInicial = new DateTime(2023, 1, 15) });
            _repoHistoria.AddHistoria(new Historia { FechaInicial = new DateTime(2023, 3, 20) });
            _repoVisita.AddVisitaPyP(new VisitaPyP
            {
                HistoriaId = 1,
                FechaVisita = new DateTime(2023, 1, 15),
                Temperatura = 38.5f,
                Peso = 28.0f,
                FrecuenciaRespiratoria = 22,
                FrecuenciaCardiaca = 110,
                EstadoAnimo = "Activo",
                Recomendaciones = "Vacunas"
            });
            _repoVisita.AddVisitaPyP(new VisitaPyP
            {
                HistoriaId = 1,
                FechaVisita = new DateTime(2023, 6, 10),
                Temperatura = 38.2f,
                Peso = 29.5f,
                FrecuenciaRespiratoria = 20,
                FrecuenciaCardiaca = 105
            });
        }

        public void Dispose()
        {
            _contexto.Dispose();
            _conexion.Dispose();
        }

        [Fact]
        public void GetHistoria_IncluyeVisitasPyP()
        {
            var historia = _repoHistoria.GetHistoria(1);
            Assert.NotNull(historia);
            Assert.Equal(2, historia.VisitasPyP.Count);
        }

        [Fact]
        public void AddHistoria_AgregaYAsignaId()
        {
            var nueva = new Historia { FechaInicial = new DateTime(2024, 1, 1) };
            var resultado = _repoHistoria.AddHistoria(nueva);
            Assert.True(resultado.Id > 0);
        }

        [Fact]
        public void UpdateVisitaPyP_ActualizaHistoriaId()
        {
            var visita = _repoVisita.GetVisitaPyP(1);
            Assert.Equal(1, visita.HistoriaId);
            visita.HistoriaId = 2;
            var actualizada = _repoVisita.UpdateVisitaPyP(visita);
            Assert.Equal(2, actualizada.HistoriaId);
        }

        [Fact]
        public void UpdateVisitaPyP_ActualizaSignosVitales()
        {
            var visita = _repoVisita.GetVisitaPyP(1);
            visita.Peso = 30.5f;
            visita.EstadoAnimo = "Tranquilo";
            var actualizada = _repoVisita.UpdateVisitaPyP(visita);
            Assert.Equal(30.5f, actualizada.Peso);
            Assert.Equal("Tranquilo", actualizada.EstadoAnimo);
        }

        [Fact]
        public void DeleteVisitaPyP_Elimina()
        {
            _repoVisita.DeleteVisitaPyP(1);
            Assert.Null(_repoVisita.GetVisitaPyP(1));
            Assert.Single(_repoHistoria.GetHistoria(1).VisitasPyP);
        }

        [Fact]
        public void GetAllVisitasPyP_IncluyeHistoria()
        {
            var visitas = _repoVisita.GetAllVisitasPyP().ToList();
            Assert.Equal(2, visitas.Count);
            Assert.All(visitas, v => Assert.NotNull(v.Historia));
        }
    }
}
