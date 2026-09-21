using MascotaFeliz.App.Dominio;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace MascotaFeliz.App.Persistencia
{
    public class RepositorioHistoria : IRepositorioHistoria
    {
        private readonly AppContext _appContext;

        public RepositorioHistoria(AppContext appContext)
        {
            _appContext = appContext;
        }

        public Historia AddHistoria(Historia historia)
        {
            var historiaAdicionada = _appContext.Historias.Add(historia);
            _appContext.SaveChanges();
            return historiaAdicionada.Entity;
        }

        public Historia GetHistoria(int idHistoria)
        {
            return _appContext.Historias
                .Include(h => h.VisitasPyP)
                .FirstOrDefault(h => h.Id == idHistoria);
        }

        public Historia UpdateHistoria(Historia historia)
        {
            var historiaEncontrada = _appContext.Historias.Find(historia.Id);
            if (historiaEncontrada != null)
            {
                historiaEncontrada.FechaInicial = historia.FechaInicial;
                _appContext.SaveChanges();
            }
            return historiaEncontrada;
        }

        public IEnumerable<Historia> GetAllHistorias()
        {
            return _appContext.Historias.Include(h => h.VisitasPyP);
        }

        public void DeleteHistoria(int idHistoria)
        {
            var historiaEncontrada = _appContext.Historias.Find(idHistoria);
            if (historiaEncontrada == null)
                return;
            _appContext.Historias.Remove(historiaEncontrada);
            _appContext.SaveChanges();
        }
    }
}
