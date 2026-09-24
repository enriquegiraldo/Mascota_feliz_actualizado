using System;
using System.Collections.Generic;
using System.Linq;
using MascotaFeliz.App.Dominio;
using Microsoft.EntityFrameworkCore;

namespace MascotaFeliz.App.Persistencia
{
    /// <summary>
    /// Repositorio de citas veterinarias.
    /// </summary>
    public class RepositorioCita : IRepositorioCita
    {
        private readonly AppContext _appContext;

        public RepositorioCita(AppContext appContext)
        {
            _appContext = appContext;
        }

        public Cita AddCita(Cita cita)
        {
            var citaAdicionada = _appContext.Citas.Add(cita);
            _appContext.SaveChanges();
            return citaAdicionada.Entity;
        }

        public Cita GetCita(int idCita)
        {
            return _appContext.Citas
                .Include(c => c.Mascota)
                .Include(c => c.Veterinario)
                .FirstOrDefault(c => c.Id == idCita);
        }

        public Cita UpdateCita(Cita cita)
        {
            var citaEncontrada = _appContext.Citas.Find(cita.Id);
            if (citaEncontrada != null)
            {
                citaEncontrada.FechaHora = cita.FechaHora;
                citaEncontrada.MascotaId = cita.MascotaId;
                citaEncontrada.VeterinarioId = cita.VeterinarioId;
                citaEncontrada.Motivo = cita.Motivo;
                citaEncontrada.Estado = cita.Estado;
                citaEncontrada.NotasInternas = cita.NotasInternas;
                _appContext.SaveChanges();
            }
            return citaEncontrada;
        }

        public IEnumerable<Cita> GetAllCitas()
        {
            return _appContext.Citas
                .Include(c => c.Mascota)
                .Include(c => c.Veterinario);
        }

        /// <summary>
        /// Filtra citas por nombre de mascota, veterinario o motivo.
        /// Búsqueda insensible a mayúsculas y acentos (es-CO).
        /// </summary>
        public IEnumerable<Cita> GetCitasPorFiltro(string filtro)
        {
            var f = Normalizador.Normalizar(filtro);
            return _appContext.Citas
                .Include(c => c.Mascota)
                .Include(c => c.Veterinario)
                .AsEnumerable()
                .Where(c => Normalizador.Normalizar(c.Mascota?.Nombre).Contains(f)
                         || Normalizador.Normalizar(c.Veterinario?.Nombres).Contains(f)
                         || Normalizador.Normalizar(c.Veterinario?.Apellidos).Contains(f)
                         || Normalizador.Normalizar(c.Motivo).Contains(f)
                         || c.FechaHora.ToString("dd/MM/yyyy").Contains(f));
        }

        public IEnumerable<Cita> GetCitasPorMascota(int mascotaId)
        {
            return _appContext.Citas
                .Include(c => c.Veterinario)
                .Where(c => c.MascotaId == mascotaId)
                .OrderByDescending(c => c.FechaHora);
        }

        public IEnumerable<Cita> GetCitasPorVeterinario(int veterinarioId)
        {
            return _appContext.Citas
                .Include(c => c.Mascota)
                .Where(c => c.VeterinarioId == veterinarioId)
                .OrderBy(c => c.FechaHora);
        }

        public IEnumerable<Cita> GetCitasPorEstado(EstadoCita estado)
        {
            return _appContext.Citas
                .Include(c => c.Mascota)
                .Include(c => c.Veterinario)
                .Where(c => c.Estado == estado)
                .OrderBy(c => c.FechaHora);
        }

        public void DeleteCita(int idCita)
        {
            var citaEncontrada = _appContext.Citas.Find(idCita);
            if (citaEncontrada == null)
                return;
            _appContext.Citas.Remove(citaEncontrada);
            _appContext.SaveChanges();
        }
    }
}
