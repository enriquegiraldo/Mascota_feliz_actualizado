using MascotaFeliz.App.Dominio;
using System.Collections.Generic;

namespace MascotaFeliz.App.Persistencia
{
    public interface IRepositorioMascota
    {
        Mascota AddMascota(Mascota mascota);
        Mascota GetMascota(int idMascota);
        Mascota UpdateMascota(Mascota mascota);
        IEnumerable<Mascota> GetAllMascotas();
        IEnumerable<Mascota> GetMascotasPorFiltro(string filtro);
        Mascota GetMascotaPorHistoriaId(int idHistoria);
        void DeleteMascota(int idMascota);
    }
}
