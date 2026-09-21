using MascotaFeliz.App.Dominio;
using System.Collections.Generic;

namespace MascotaFeliz.App.Persistencia
{
    public interface IRepositorioDueno
    {
        Dueno AddDueno(Dueno dueno);
        Dueno GetDueno(int idDueno);
        Dueno UpdateDueno(Dueno dueno);
        IEnumerable<Dueno> GetAllDuenos();
        IEnumerable<Dueno> GetDuenosPorFiltro(string filtro);
        void DeleteDueno(int idDueno);
    }
}
