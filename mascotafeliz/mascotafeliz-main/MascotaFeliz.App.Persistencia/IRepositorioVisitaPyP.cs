using MascotaFeliz.App.Dominio;
using System.Collections.Generic;

namespace MascotaFeliz.App.Persistencia
{
    public interface IRepositorioVisitaPyP
    {
        VisitaPyP AddVisitaPyP(VisitaPyP visitaPyP);
        VisitaPyP GetVisitaPyP(int idVisitaPyP);
        VisitaPyP UpdateVisitaPyP(VisitaPyP visitaPyP);
        IEnumerable<VisitaPyP> GetAllVisitasPyP();
        IEnumerable<VisitaPyP> GetVisitasPyPPorFiltro(string filtro);
        void DeleteVisitaPyP(int idVisitaPyP);
    }
}
