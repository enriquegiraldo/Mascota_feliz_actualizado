using MascotaFeliz.App.Dominio;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace MascotaFeliz.App.Persistencia
{
    public class RepositorioVisitaPyP : IRepositorioVisitaPyP
    {
        private readonly AppContext _appContext;

        public RepositorioVisitaPyP(AppContext appContext)
        {
            _appContext = appContext;
        }

        public VisitaPyP AddVisitaPyP(VisitaPyP visitaPyP)
        {
            var visitaAdicionada = _appContext.VisitasPyP.Add(visitaPyP);
            _appContext.SaveChanges();
            return visitaAdicionada.Entity;
        }

        public VisitaPyP GetVisitaPyP(int idVisitaPyP)
        {
            return _appContext.VisitasPyP
                .Include(v => v.Historia)
                .FirstOrDefault(v => v.Id == idVisitaPyP);
        }

        public VisitaPyP UpdateVisitaPyP(VisitaPyP visitaPyP)
        {
            var visitaEncontrada = _appContext.VisitasPyP.Find(visitaPyP.Id);
            if (visitaEncontrada != null)
            {
                visitaEncontrada.FechaVisita = visitaPyP.FechaVisita;
                visitaEncontrada.Temperatura = visitaPyP.Temperatura;
                visitaEncontrada.Peso = visitaPyP.Peso;
                visitaEncontrada.FrecuenciaRespiratoria = visitaPyP.FrecuenciaRespiratoria;
                visitaEncontrada.FrecuenciaCardiaca = visitaPyP.FrecuenciaCardiaca;
                visitaEncontrada.EstadoAnimo = visitaPyP.EstadoAnimo;
                visitaEncontrada.Recomendaciones = visitaPyP.Recomendaciones;
                visitaEncontrada.ProximaVacuna = visitaPyP.ProximaVacuna;
                visitaEncontrada.TipoProximaVacuna = visitaPyP.TipoProximaVacuna;
                // La historia asociada se actualiza desde el formulario (selector).
                visitaEncontrada.HistoriaId = visitaPyP.HistoriaId;
                _appContext.SaveChanges();
            }
            return visitaEncontrada;
        }

        public IEnumerable<VisitaPyP> GetAllVisitasPyP()
        {
            return _appContext.VisitasPyP.Include(v => v.Historia);
        }

        /// <summary>
        /// Filtra visitas por estado de ánimo, recomendaciones o fecha (dd/MM/yyyy).
        /// Búsqueda insensible a mayúsculas y acentos.
        /// </summary>
        public IEnumerable<VisitaPyP> GetVisitasPyPPorFiltro(string filtro)
        {
            var f = Normalizador.Normalizar(filtro);
            return _appContext.VisitasPyP
                .Include(v => v.Historia)
                .AsEnumerable()
                .Where(v => Normalizador.Normalizar(v.EstadoAnimo).Contains(f)
                         || Normalizador.Normalizar(v.Recomendaciones).Contains(f)
                         || v.FechaVisita.ToString("dd/MM/yyyy").Contains(f));
        }

        public void DeleteVisitaPyP(int idVisitaPyP)
        {
            var visitaEncontrada = _appContext.VisitasPyP.Find(idVisitaPyP);
            if (visitaEncontrada == null)
                return;
            _appContext.VisitasPyP.Remove(visitaEncontrada);
            _appContext.SaveChanges();
        }
    }
}
