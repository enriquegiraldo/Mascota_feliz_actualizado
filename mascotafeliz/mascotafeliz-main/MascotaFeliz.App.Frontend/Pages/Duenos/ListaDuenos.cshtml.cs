using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MascotaFeliz.App.Dominio;
using MascotaFeliz.App.Persistencia;

namespace MascotaFeliz.App.Frontend.Pages
{
    [Authorize]
    public class ListaDuenosModel : PageModel
    {
        private readonly IRepositorioDueno _repoDueno;

        public IEnumerable<Dueno> listaDuenos { get; set; }

        // Búsqueda y paginación
        [BindProperty(SupportsGet = true)]
        public string Q { get; set; }
        [BindProperty(SupportsGet = true)]
        public int Pagina { get; set; } = 1;
        public int TotalPaginas { get; set; }
        public int Total { get; set; }

        private const int TamanoPagina = 8;

        public ListaDuenosModel(IRepositorioDueno repoDueno)
        {
            _repoDueno = repoDueno;
        }

        public void OnGet()
        {
            var todo = string.IsNullOrWhiteSpace(Q)
                ? _repoDueno.GetAllDuenos()
                : _repoDueno.GetDuenosPorFiltro(Q);

            Total = todo.Count();
            TotalPaginas = Math.Max(1, (int)Math.Ceiling(Total / (double)TamanoPagina));
            if (Pagina < 1) { Pagina = 1; }
            if (Pagina > TotalPaginas) { Pagina = TotalPaginas; }

            listaDuenos = todo
                .OrderBy(d => d.Apellidos)
                .ThenBy(d => d.Nombres)
                .Skip((Pagina - 1) * TamanoPagina)
                .Take(TamanoPagina);
        }

        public IActionResult OnPostEliminar(int duenoId)
        {
            try
            {
                _repoDueno.DeleteDueno(duenoId);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException)
            {
                TempData["Error"] = "No se puede eliminar el dueño porque tiene mascotas asociadas.";
            }
            return RedirectToPage("./ListaDuenos", new { Q, Pagina });
        }
    }
}
