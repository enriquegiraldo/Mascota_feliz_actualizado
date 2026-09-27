using MascotaFeliz.App.Dominio;
using System.Collections.Generic;

namespace MascotaFeliz.App.Persistencia
{
    public interface IRepositorioCita
    {
        Cita AddCita(Cita cita);
        Cita GetCita(int idCita);
        Cita UpdateCita(Cita cita);
        IEnumerable<Cita> GetAllCitas();
        IEnumerable<Cita> GetCitasPorFiltro(string filtro);
        IEnumerable<Cita> GetCitasPorMascota(int mascotaId);
        IEnumerable<Cita> GetCitasPorVeterinario(int veterinarioId);
        IEnumerable<Cita> GetCitasPorEstado(EstadoCita estado);
        void DeleteCita(int idCita);
    }
}
