using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MascotaFeliz.App.Dominio;
using MascotaFeliz.App.Persistencia;

namespace MascotaFeliz.App.Frontend.Pages
{
    public class ListaMascotasModel : PageModel
    {
        private readonly IRepositorioMascota _repoMascota;

        public IEnumerable<Mascota> listaMascotas { get; set; }
        public IEnumerable<string> Especies { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Q { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Especie { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Orden { get; set; }
        [BindProperty(SupportsGet = true)]
        public int Pagina { get; set; } = 1;
        public int TotalPaginas { get; set; }
        public int Total { get; set; }

        private const int TamanoPagina = 8;

        public ListaMascotasModel(IRepositorioMascota repoMascota)
        {
            _repoMascota = repoMascota;
        }

        public void OnGet()
        {
            Especies = _repoMascota.GetEspecies();

            var todo = string.IsNullOrWhiteSpace(Q)
                ? _repoMascota.GetAllMascotas()
                : _repoMascota.GetMascotasPorFiltro(Q);

            if (!string.IsNullOrWhiteSpace(Especie))
            {
                todo = todo.Where(m => m.Especie == Especie);
            }

            // Ordenamiento por columna.
            todo = Orden switch
            {
                "especie" => todo.OrderBy(m => m.Especie).ThenBy(m => m.Nombre),
                "especie-desc" => todo.OrderByDescending(m => m.Especie).ThenBy(m => m.Nombre),
                "raza" => todo.OrderBy(m => m.Raza).ThenBy(m => m.Nombre),
                "raza-desc" => todo.OrderByDescending(m => m.Raza).ThenBy(m => m.Nombre),
                "nombre-desc" => todo.OrderByDescending(m => m.Nombre),
                _ => todo.OrderBy(m => m.Nombre),
            };

            Total = todo.Count();
            TotalPaginas = Math.Max(1, (int)Math.Ceiling(Total / (double)TamanoPagina));
            if (Pagina < 1) { Pagina = 1; }
            if (Pagina > TotalPaginas) { Pagina = TotalPaginas; }

            listaMascotas = todo
                .Skip((Pagina - 1) * TamanoPagina)
                .Take(TamanoPagina);
        }

        public IActionResult OnPostEliminar(int mascotaId)
        {
            try
            {
                _repoMascota.DeleteMascota(mascotaId);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException)
            {
                TempData["Error"] = "No se pudo eliminar la mascota. Intenta de nuevo.";
            }
            return RedirectToPage("./ListaMascotas", new { Q, Especie, Orden, Pagina });
        }

        /// <summary>
        /// Exporta las mascotas (con los filtros actuales) a un archivo CSV.
        /// </summary>
        public IActionResult OnGetExportarCsv(string q, string especie)
        {
            var todo = string.IsNullOrWhiteSpace(q)
                ? _repoMascota.GetAllMascotas()
                : _repoMascota.GetMascotasPorFiltro(q);

            if (!string.IsNullOrWhiteSpace(especie))
            {
                todo = todo.Where(m => m.Especie == especie);
            }

            var sb = new StringBuilder();
            sb.AppendLine("Id,Nombre,Especie,Raza,Color,Dueño,Veterinario");
            foreach (var m in todo.OrderBy(m => m.Nombre))
            {
                var dueno = m.Dueno != null ? $"{m.Dueno.Nombres} {m.Dueno.Apellidos}" : "";
                var vet = m.Veterinario != null ? $"{m.Veterinario.Nombres} {m.Veterinario.Apellidos}" : "";
                sb.AppendLine($"{m.Id},{EscaparCsv(m.Nombre)},{EscaparCsv(m.Especie)},{EscaparCsv(m.Raza)},{EscaparCsv(m.Color)},{EscaparCsv(dueno)},{EscaparCsv(vet)}");
            }

            var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
            var nombreArchivo = $"mascotas_{DateTime.Now:yyyy-MM-dd}.csv";
            return File(bytes, "text/csv; charset=utf-8", nombreArchivo);
        }

        private static string EscaparCsv(string valor)
        {
            if (string.IsNullOrEmpty(valor)) return "";
            if (valor.Contains(",") || valor.Contains("\""))
            {
                return "\"" + valor.Replace("\"", "\"\"") + "\"";
            }
            return valor;
        }
    }
}
