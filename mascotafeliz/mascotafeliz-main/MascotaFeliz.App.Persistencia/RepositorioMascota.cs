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
                // Los tres vínculos (dueño, veterinario e historia) se actualizan
                // desde el formulario mediante selectores. Si el usuario elige
                // "sin asignar", el valor llega null y se desvincula correctamente.
                mascotaEncontrada.VeterinarioId = mascota.VeterinarioId;
                mascotaEncontrada.DuenoId = mascota.DuenoId;
                mascotaEncontrada.HistoriaId = mascota.HistoriaId;
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
            var f = Normalizador.Normalizar(filtro);
            // AsEnumerable materializa antes de aplicar el Where con el método C#
            // Normalizador (que EF Core no puede traducir a SQL).
            return _appContext.Mascotas
                .Include(m => m.Veterinario)
                .Include(m => m.Dueno)
                .AsEnumerable()
                .Where(m => Normalizador.Normalizar(m.Nombre).Contains(f)
                         || Normalizador.Normalizar(m.Raza).Contains(f)
                         || Normalizador.Normalizar(m.Especie).Contains(f));
        }

        public Mascota GetMascotaPorHistoriaId(int idHistoria)
        {
            return _appContext.Mascotas
                .Include(m => m.Dueno)
                .Include(m => m.Veterinario)
                .FirstOrDefault(m => m.HistoriaId == idHistoria);
        }

        public IEnumerable<Mascota> GetMascotasPorDueno(int idDueno)
        {
            return _appContext.Mascotas
                .Include(m => m.Veterinario)
                .Where(m => m.DuenoId == idDueno)
                .OrderBy(m => m.Nombre);
        }

        public IEnumerable<Mascota> GetMascotasPorVeterinario(int idVeterinario)
        {
            return _appContext.Mascotas
                .Include(m => m.Dueno)
                .Where(m => m.VeterinarioId == idVeterinario)
                .OrderBy(m => m.Nombre);
        }

        /// <summary>
        /// Devuelve la lista de especies distintas registradas (para los filtros).
        /// </summary>
        public IEnumerable<string> GetEspecies()
        {
            return _appContext.Mascotas
                .Where(m => m.Especie != null && m.Especie != "")
                .Select(m => m.Especie)
                .Distinct()
                .OrderBy(e => e)
                .ToList();
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
