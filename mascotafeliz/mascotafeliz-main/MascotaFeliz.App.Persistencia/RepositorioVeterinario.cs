using MascotaFeliz.App.Dominio;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace MascotaFeliz.App.Persistencia
{
    public class RepositorioVeterinario : IRepositorioVeterinario
    {
        private readonly AppContext _appContext;

        public RepositorioVeterinario(AppContext appContext)
        {
            _appContext = appContext;
        }

        public Veterinario AddVeterinario(Veterinario veterinario)
        {
            var veterinarioAdicionado = _appContext.Veterinarios.Add(veterinario);
            _appContext.SaveChanges();
            return veterinarioAdicionado.Entity;
        }

        public Veterinario GetVeterinario(int idVeterinario)
        {
            return _appContext.Veterinarios.Find(idVeterinario);
        }

        public Veterinario UpdateVeterinario(Veterinario veterinario)
        {
            var veterinarioEncontrado = _appContext.Veterinarios.Find(veterinario.Id);
            if (veterinarioEncontrado != null)
            {
                veterinarioEncontrado.Nombres = veterinario.Nombres;
                veterinarioEncontrado.Apellidos = veterinario.Apellidos;
                veterinarioEncontrado.Direccion = veterinario.Direccion;
                veterinarioEncontrado.Telefono = veterinario.Telefono;
                veterinarioEncontrado.TarjetaProfesional = veterinario.TarjetaProfesional;
                _appContext.SaveChanges();
            }
            return veterinarioEncontrado;
        }

        public IEnumerable<Veterinario> GetAllVeterinarios()
        {
            return _appContext.Veterinarios;
        }

        public IEnumerable<Veterinario> GetVeterinariosPorFiltro(string filtro)
        {
            return _appContext.Veterinarios
                .Where(v => v.Nombres.Contains(filtro) || v.Apellidos.Contains(filtro));
        }

        public void DeleteVeterinario(int idVeterinario)
        {
            var veterinarioEncontrado = _appContext.Veterinarios.Find(idVeterinario);
            if (veterinarioEncontrado == null)
                return;
            _appContext.Veterinarios.Remove(veterinarioEncontrado);
            _appContext.SaveChanges();
        }
    }
}
