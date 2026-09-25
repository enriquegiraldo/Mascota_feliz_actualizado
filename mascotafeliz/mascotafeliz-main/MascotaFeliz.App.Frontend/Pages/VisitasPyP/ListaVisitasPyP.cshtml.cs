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
    /// Listado de todas las visitas PyP con búsqueda por fecha/estado de ánimo
    /// y paginación.
    /// </summary>
    [Authorize]
    public class ListaVisitasPyPModel : PageModel
    {
        private readonly IRepositorioVisitaPyP _repoVisitaPyP;

        public IEnumerable<VisitaPyP> listaVisitasPyP { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Q { get; set; }
        [BindProperty(SupportsGet = true)]
        public int Pagina { get; set; } = 1;
        public int TotalPaginas { get; set; }
        public int Total { get; set; }

        private const int TamanoPagina = 8;

        public ListaVisitasPyPModel(IRepositorioVisitaPyP repoVisitaPyP)
        {
            _repoVisitaPyP = repoVisitaPyP;
        }

        public void OnGet()
        {
            var todo = _repoVisitaPyP.GetAllVisitasPyP().OrderByDescending(v => v.FechaVisita).ToList();

            if (!string.IsNullOrWhiteSpace(Q))
            {
                todo = todo.Where(v =>
                    (v.EstadoAnimo != null && v.EstadoAnimo.Contains(Q, StringComparison.OrdinalIgnoreCase)) ||
                    (v.Recomendaciones != null && v.Recomendaciones.Contains(Q, StringComparison.OrdinalIgnoreCase)) ||
                    v.FechaVisita.ToString("dd/MM/yyyy").Contains(Q)).ToList();
            }

            Total = todo.Count;
            TotalPaginas = Math.Max(1, (int)Math.Ceiling(Total / (double)TamanoPagina));
            if (Pagina < 1) { Pagina = 1; }
            if (Pagina > TotalPaginas) { Pagina = TotalPaginas; }

            listaVisitasPyP = todo
                .Skip((Pagina - 1) * TamanoPagina)
                .Take(TamanoPagina);
        }
    }
}
