using MascotaFeliz.App.Dominio;
using System.Collections.Generic;

namespace MascotaFeliz.App.Persistencia
{
    public interface IRepositorioVeterinario
    {
        Veterinario AddVeterinario(Veterinario veterinario);
        Veterinario GetVeterinario(int idVeterinario);
        Veterinario UpdateVeterinario(Veterinario veterinario);
        IEnumerable<Veterinario> GetAllVeterinarios();
        IEnumerable<Veterinario> GetVeterinariosPorFiltro(string filtro);
        void DeleteVeterinario(int idVeterinario);
    }
}
