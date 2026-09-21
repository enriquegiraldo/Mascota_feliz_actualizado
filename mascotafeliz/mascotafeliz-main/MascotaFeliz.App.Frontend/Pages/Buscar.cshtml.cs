using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MascotaFeliz.App.Dominio;
using MascotaFeliz.App.Persistencia;

namespace MascotaFeliz.App.Frontend.Pages
{
    /// <summary>
    /// Búsqueda global (Cmd/Ctrl+K). Devuelve JSON con resultados de
    /// mascotas, dueños y veterinarios que coinciden con el término.
    /// </summary>
    public class BuscarModel : PageModel
    {
        private readonly IRepositorioMascota _repoMascota;
        private readonly IRepositorioDueno _repoDueno;
        private readonly IRepositorioVeterinario _repoVeterinario;

        public BuscarModel(
            IRepositorioMascota repoMascota,
            IRepositorioDueno repoDueno,
            IRepositorioVeterinario repoVeterinario)
        {
            _repoMascota = repoMascota;
            _repoDueno = repoDueno;
            _repoVeterinario = repoVeterinario;
        }

        public IActionResult OnGet(string q)
        {
            var termino = (q ?? "").Trim();
            var resultados = new List<ResultadoBusqueda>();

            if (string.IsNullOrWhiteSpace(termino))
            {
                return new JsonResult(new { resultados });
            }

            // Mascotas: coincide nombre, especie o raza.
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

            // Dueños: coincide nombres o apellidos.
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

            // Veterinarios: coincide nombres o apellidos.
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

            return new JsonResult(new { resultados });
        }

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
