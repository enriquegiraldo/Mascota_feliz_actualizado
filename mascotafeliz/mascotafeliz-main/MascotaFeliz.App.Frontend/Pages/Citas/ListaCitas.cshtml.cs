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
    /// <summary>
    /// Lista de citas del panel interno, con búsqueda, filtro por estado
    /// y paginación.
    /// </summary>
    [Authorize]
    public class ListaCitasModel : PageModel
    {
        private readonly IRepositorioCita _repoCita;

        public IEnumerable<Cita> listaCitas { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Q { get; set; }
        [BindProperty(SupportsGet = true)]
        public EstadoCita? Estado { get; set; }
        [BindProperty(SupportsGet = true)]
        public int Pagina { get; set; } = 1;
        public int TotalPaginas { get; set; }
        public int Total { get; set; }

        private const int TamanoPagina = 10;

        public ListaCitasModel(IRepositorioCita repoCita)
        {
            _repoCita = repoCita;
        }

        public void OnGet()
        {
            var todo = string.IsNullOrWhiteSpace(Q)
                ? _repoCita.GetAllCitas()
                : _repoCita.GetCitasPorFiltro(Q);

            if (Estado.HasValue)
            {
                todo = todo.Where(c => c.Estado == Estado.Value);
            }

            // Ordenar: pendientes y confirmadas primero (por fecha asc),
            // luego atendidas y canceladas (por fecha desc).
            todo = todo
                .OrderBy(c => c.Estado == EstadoCita.Atendida || c.Estado == EstadoCita.Cancelada ? 1 : 0)
                .ThenBy(c => c.FechaHora);

            Total = todo.Count();
            TotalPaginas = Math.Max(1, (int)Math.Ceiling(Total / (double)TamanoPagina));
            if (Pagina < 1) { Pagina = 1; }
            if (Pagina > TotalPaginas) { Pagina = TotalPaginas; }

            listaCitas = todo
                .Skip((Pagina - 1) * TamanoPagina)
                .Take(TamanoPagina);
        }

        public IActionResult OnPostEliminar(int citaId)
        {
            if (!User.IsInRole("Admin"))
            {
                return RedirectToPage("/Cuenta/AccesoDenegado");
            }
            try
            {
                _repoCita.DeleteCita(citaId);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException)
            {
                TempData["Error"] = "No se pudo eliminar la cita. Intenta de nuevo.";
            }
            return RedirectToPage("./ListaCitas", new { Q, Estado, Pagina });
        }
    }
}
