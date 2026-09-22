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
    /// Lista de historias clínicas. Cada historia se muestra con su mascota
    /// asociada, la fecha inicial y el número de visitas registradas.
    /// </summary>
    public class ListaHistoriasModel : PageModel
    {
        private readonly IRepositorioHistoria _repoHistoria;
        private readonly IRepositorioMascota _repoMascota;

        public IEnumerable<Historia> listaHistorias { get; set; }
        public Dictionary<int, Mascota> MascotaPorHistoria { get; set; } = new Dictionary<int, Mascota>();

        [BindProperty(SupportsGet = true)]
        public int Pagina { get; set; } = 1;
        public int TotalPaginas { get; set; }
        public int Total { get; set; }

        private const int TamanoPagina = 8;

        public ListaHistoriasModel(IRepositorioHistoria repoHistoria, IRepositorioMascota repoMascota)
        {
            _repoHistoria = repoHistoria;
            _repoMascota = repoMascota;
        }

        public void OnGet()
        {
            var todo = _repoHistoria.GetAllHistorias().OrderBy(h => h.FechaInicial).ToList();
            Total = todo.Count;
            TotalPaginas = Math.Max(1, (int)Math.Ceiling(Total / (double)TamanoPagina));
            if (Pagina < 1) { Pagina = 1; }
            if (Pagina > TotalPaginas) { Pagina = TotalPaginas; }

            listaHistorias = todo
                .Skip((Pagina - 1) * TamanoPagina)
                .Take(TamanoPagina);

            // Mapa de historia -> mascota (lookup en persistencia).
            foreach (var h in listaHistorias)
            {
                MascotaPorHistoria[h.Id] = _repoMascota.GetMascotaPorHistoriaId(h.Id);
            }
        }
    }
}
