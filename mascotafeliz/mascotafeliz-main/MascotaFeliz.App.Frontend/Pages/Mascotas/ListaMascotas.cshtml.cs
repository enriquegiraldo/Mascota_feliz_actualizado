using System;
using System.Collections.Generic;
using System.Linq;
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

            Total = todo.Count();
            TotalPaginas = Math.Max(1, (int)Math.Ceiling(Total / (double)TamanoPagina));
            if (Pagina < 1) { Pagina = 1; }
            if (Pagina > TotalPaginas) { Pagina = TotalPaginas; }

            listaMascotas = todo
                .OrderBy(m => m.Nombre)
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
            return RedirectToPage("./ListaMascotas", new { Q, Especie, Pagina });
        }
    }
}
