using MascotaFeliz.App.Dominio;
using System.Collections.Generic;

namespace MascotaFeliz.App.Persistencia
{
    public interface IRepositorioHistoria
    {
        Historia AddHistoria(Historia historia);
        Historia GetHistoria(int idHistoria);
        Historia UpdateHistoria(Historia historia);
        IEnumerable<Historia> GetAllHistorias();
        void DeleteHistoria(int idHistoria);
    }
}
