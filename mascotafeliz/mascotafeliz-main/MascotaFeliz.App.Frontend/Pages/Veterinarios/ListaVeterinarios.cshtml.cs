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
    public class ListaVeterinariosModel : PageModel
    {
        private readonly IRepositorioVeterinario _repoVeterinario;

        public IEnumerable<Veterinario> listaVeterinarios { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Q { get; set; }
        [BindProperty(SupportsGet = true)]
        public int Pagina { get; set; } = 1;
        public int TotalPaginas { get; set; }
        public int Total { get; set; }

        private const int TamanoPagina = 8;

        public ListaVeterinariosModel(IRepositorioVeterinario repoVeterinario)
        {
            _repoVeterinario = repoVeterinario;
        }

        public void OnGet()
        {
            var todo = string.IsNullOrWhiteSpace(Q)
                ? _repoVeterinario.GetAllVeterinarios()
                : _repoVeterinario.GetVeterinariosPorFiltro(Q);

            Total = todo.Count();
            TotalPaginas = Math.Max(1, (int)Math.Ceiling(Total / (double)TamanoPagina));
            if (Pagina < 1) { Pagina = 1; }
            if (Pagina > TotalPaginas) { Pagina = TotalPaginas; }

            listaVeterinarios = todo
                .OrderBy(v => v.Apellidos)
                .ThenBy(v => v.Nombres)
                .Skip((Pagina - 1) * TamanoPagina)
                .Take(TamanoPagina);
        }

        public IActionResult OnPostEliminar(int veterinarioId)
        {
            try
            {
                _repoVeterinario.DeleteVeterinario(veterinarioId);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException)
            {
                TempData["Error"] = "No se puede eliminar el veterinario porque tiene mascotas asociadas.";
            }
            return RedirectToPage("./ListaVeterinarios", new { Q, Pagina });
        }
    }
}
