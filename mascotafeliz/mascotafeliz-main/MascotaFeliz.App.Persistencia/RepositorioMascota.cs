using MascotaFeliz.App.Dominio;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace MascotaFeliz.App.Persistencia
{
    public class RepositorioMascota : IRepositorioMascota
    {
        private readonly AppContext _appContext;

        public RepositorioMascota(AppContext appContext)
        {
            _appContext = appContext;
        }

        public Mascota AddMascota(Mascota mascota)
        {
            var mascotaAdicionada = _appContext.Mascotas.Add(mascota);
            _appContext.SaveChanges();
            return mascotaAdicionada.Entity;
        }

        public Mascota GetMascota(int idMascota)
        {
            return _appContext.Mascotas
                .Include(m => m.Veterinario)
                .Include(m => m.Dueno)
                .Include(m => m.Historia)
                    .ThenInclude(h => h.VisitasPyP)
                .FirstOrDefault(m => m.Id == idMascota);
        }

        public Mascota UpdateMascota(Mascota mascota)
        {
            var mascotaEncontrada = _appContext.Mascotas.Find(mascota.Id);
            if (mascotaEncontrada != null)
            {
                mascotaEncontrada.Nombre = mascota.Nombre;
                mascotaEncontrada.Color = mascota.Color;
                mascotaEncontrada.Especie = mascota.Especie;
                mascotaEncontrada.Raza = mascota.Raza;
                // Los vínculos con dueño y veterinario se actualizan desde el formulario
                // (selectores). La historia clínica se preserva: nunca se sobrescribe a
                // null desde la edición de la mascota (se gestiona desde su propia sección).
                mascotaEncontrada.VeterinarioId = mascota.VeterinarioId;
                mascotaEncontrada.DuenoId = mascota.DuenoId;
                if (mascota.HistoriaId.HasValue)
                {
                    mascotaEncontrada.HistoriaId = mascota.HistoriaId;
                }
                _appContext.SaveChanges();
            }
            return mascotaEncontrada;
        }

        public IEnumerable<Mascota> GetAllMascotas()
        {
            return _appContext.Mascotas
                .Include(m => m.Veterinario)
                .Include(m => m.Dueno);
        }

        public IEnumerable<Mascota> GetMascotasPorFiltro(string filtro)
        {
            return _appContext.Mascotas
                .Include(m => m.Veterinario)
                .Include(m => m.Dueno)
                .Where(m => m.Nombre.Contains(filtro) || m.Raza.Contains(filtro) || m.Especie.Contains(filtro));
        }

        public void DeleteMascota(int idMascota)
        {
            var mascotaEncontrada = _appContext.Mascotas.Find(idMascota);
            if (mascotaEncontrada == null)
                return;
            _appContext.Mascotas.Remove(mascotaEncontrada);
            _appContext.SaveChanges();
        }
    }
}
