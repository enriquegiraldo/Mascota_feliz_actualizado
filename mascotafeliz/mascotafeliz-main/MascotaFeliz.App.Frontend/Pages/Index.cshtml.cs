using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MascotaFeliz.App.Dominio;
using MascotaFeliz.App.Persistencia;

namespace MascotaFeliz.App.Frontend.Pages
{
    /// <summary>
    /// Página de inicio del panel interno: muestra un resumen con conteos,
    /// distribución por especie (donut) y accesos rápidos.
    /// </summary>
    public class IndexModel : PageModel
    {
        private readonly IRepositorioMascota _repoMascota;
        private readonly IRepositorioDueno _repoDueno;
        private readonly IRepositorioVeterinario _repoVeterinario;
        private readonly IRepositorioVisitaPyP _repoVisitaPyP;

        public int TotalMascotas { get; set; }
        public int TotalDuenos { get; set; }
        public int TotalVeterinarios { get; set; }
        public int TotalVisitas { get; set; }

        /// <summary>Distribución de mascotas por especie (para el donut).</summary>
        public Dictionary<string, int> DistribucionEspecies { get; set; } = new Dictionary<string, int>();

        public IndexModel(
            IRepositorioMascota repoMascota,
            IRepositorioDueno repoDueno,
            IRepositorioVeterinario repoVeterinario,
            IRepositorioVisitaPyP repoVisitaPyP)
        {
            _repoMascota = repoMascota;
            _repoDueno = repoDueno;
            _repoVeterinario = repoVeterinario;
            _repoVisitaPyP = repoVisitaPyP;
        }

        public void OnGet()
        {
            var todas = _repoMascota.GetAllMascotas().ToList();
            TotalMascotas = todas.Count;
            TotalDuenos = _repoDueno.GetAllDuenos().Count();
            TotalVeterinarios = _repoVeterinario.GetAllVeterinarios().Count();
            TotalVisitas = _repoVisitaPyP.GetAllVisitasPyP().Count();

            DistribucionEspecies = todas
                .GroupBy(m => m.Especie ?? "Otra")
                .OrderByDescending(g => g.Count())
                .ToDictionary(g => g.Key, g => g.Count());
        }
    }
}
