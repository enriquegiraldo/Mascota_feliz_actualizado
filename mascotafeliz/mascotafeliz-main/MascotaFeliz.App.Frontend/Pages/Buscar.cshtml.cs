using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MascotaFeliz.App.Dominio;
using MascotaFeliz.App.Persistencia;

namespace MascotaFeliz.App.Frontend.Pages
{
    /// <summary>
    /// Búsqueda global (Cmd/Ctrl+K). Devuelve JSON con resultados de
    /// mascotas, dueños, veterinarios, historias, visitas PyP y citas
    /// que coinciden con el término. La búsqueda es insensible a
    /// mayúsculas y acentos (es-CO).
    /// </summary>
    [Authorize]
    public class BuscarModel : PageModel
    {
        private readonly IRepositorioMascota _repoMascota;
        private readonly IRepositorioDueno _repoDueno;
        private readonly IRepositorioVeterinario _repoVeterinario;
        private readonly IRepositorioHistoria _repoHistoria;
        private readonly IRepositorioVisitaPyP _repoVisitaPyP;
        private readonly IRepositorioCita _repoCita;
        private readonly IRepositorioMascota _repoMascotaLookup;

        public BuscarModel(
            IRepositorioMascota repoMascota,
            IRepositorioDueno repoDueno,
            IRepositorioVeterinario repoVeterinario,
            IRepositorioHistoria repoHistoria,
            IRepositorioVisitaPyP repoVisitaPyP,
            IRepositorioCita repoCita)
        {
            _repoMascota = repoMascota;
            _repoDueno = repoDueno;
            _repoVeterinario = repoVeterinario;
            _repoHistoria = repoHistoria;
            _repoVisitaPyP = repoVisitaPyP;
            _repoCita = repoCita;
            _repoMascotaLookup = repoMascota;
        }

        public IActionResult OnGet(string q)
        {
            var termino = (q ?? "").Trim();
            var resultados = new List<ResultadoBusqueda>();
            var cultura = new CultureInfo("es-CO");

            if (string.IsNullOrWhiteSpace(termino))
            {
                return new JsonResult(new { resultados });
            }

            // Mascotas: nombre, especie o raza.
            foreach (var m in _repoMascota.GetMascotasPorFiltro(termino).Take(6))
            {
                resultados.Add(new ResultadoBusqueda
                {
                    Tipo = "Mascota",
                    Titulo = m.Nombre,
                    Subtitulo = $"{m.Especie} · {m.Raza}",
                    Url = $"/Mascotas/DetallesMascotas?mascotaId={m.Id}",
                    Icono = "mascota"
                });
            }

            // Dueños: nombres o apellidos.
            foreach (var d in _repoDueno.GetDuenosPorFiltro(termino).Take(5))
            {
                resultados.Add(new ResultadoBusqueda
                {
                    Tipo = "Dueño",
                    Titulo = $"{d.Nombres} {d.Apellidos}",
                    Subtitulo = d.Telefono,
                    Url = $"/Duenos/DetallesDuenos?duenoId={d.Id}",
                    Icono = "dueno"
                });
            }

            // Veterinarios: nombres o apellidos.
            foreach (var v in _repoVeterinario.GetVeterinariosPorFiltro(termino).Take(5))
            {
                resultados.Add(new ResultadoBusqueda
                {
                    Tipo = "Veterinario",
                    Titulo = $"{v.Nombres} {v.Apellidos}",
                    Subtitulo = v.TarjetaProfesional,
                    Url = $"/Veterinarios/DetallesVeterinarios?veterinarioId={v.Id}",
                    Icono = "veterinario"
                });
            }

            // Historias: por fecha inicial (dd/MM/yyyy) o por número.
            if (int.TryParse(termino, out int historiaId))
            {
                var h = _repoHistoria.GetHistoria(historiaId);
                if (h != null)
                {
                    resultados.Add(new ResultadoBusqueda
                    {
                        Tipo = "Historia",
                        Titulo = $"Historia #{h.Id}",
                        Subtitulo = h.FechaInicial.ToString("d 'de' MMMM 'de' yyyy", cultura),
                        Url = $"/Historias/DetallesHistoria?historiaId={h.Id}",
                        Icono = "historia"
                    });
                }
            }
            foreach (var h in _repoHistoria.GetAllHistorias()
                .Where(h => h.FechaInicial.ToString("dd/MM/yyyy").Contains(termino)).Take(3))
            {
                if (resultados.All(r => r.Url != $"/Historias/DetallesHistoria?historiaId={h.Id}"))
                {
                    resultados.Add(new ResultadoBusqueda
                    {
                        Tipo = "Historia",
                        Titulo = $"Historia #{h.Id}",
                        Subtitulo = h.FechaInicial.ToString("d 'de' MMMM 'de' yyyy", cultura),
                        Url = $"/Historias/DetallesHistoria?historiaId={h.Id}",
                        Icono = "historia"
                    });
                }
            }

            // Visitas PyP: estado de ánimo, recomendaciones o fecha.
            foreach (var v in _repoVisitaPyP.GetVisitasPyPPorFiltro(termino).Take(5))
            {
                var mascota = _repoMascotaLookup.GetMascotaPorHistoriaId(v.HistoriaId);
                resultados.Add(new ResultadoBusqueda
                {
                    Tipo = "Visita",
                    Titulo = $"Visita {v.FechaVisita.ToString("d 'de' MMM 'de' yyyy", cultura)}",
                    Subtitulo = mascota != null ? $"{mascota.Nombre} · {v.EstadoAnimo}" : v.EstadoAnimo,
                    Url = $"/VisitasPyP/DetallesVisitaPyP?visitaPyPId={v.Id}",
                    Icono = "visita"
                });
            }

            // Citas: por mascota, veterinario, motivo o fecha.
            foreach (var c in _repoCita.GetCitasPorFiltro(termino).Take(5))
            {
                resultados.Add(new ResultadoBusqueda
                {
                    Tipo = "Cita",
                    Titulo = $"Cita {c.FechaHora.ToString("d 'de' MMM, HH:mm", cultura)}",
                    Subtitulo = c.Mascota != null
                        ? $"{c.Mascota.Nombre} · {c.Estado}"
                        : c.Estado.ToString(),
                    Url = $"/Citas/DetallesCita?citaId={c.Id}",
                    Icono = "cita"
                });
            }

            return new JsonResult(new { resultados });
        }

        [Authorize]
    public class ResultadoBusqueda
        {
            public string Tipo { get; set; }
            public string Titulo { get; set; }
            public string Subtitulo { get; set; }
            public string Url { get; set; }
            public string Icono { get; set; }
        }
    }
}
