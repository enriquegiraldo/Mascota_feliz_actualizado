using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MascotaFeliz.App.Dominio;
using MascotaFeliz.App.Persistencia;

namespace MascotaFeliz.App.Frontend.Pages
{
    /// <summary>
    /// Detalle de una historia clínica: muestra la mascota asociada y la
    /// línea de tiempo de sus visitas PyP.
    /// </summary>
    [Authorize]
    public class DetallesHistoriaModel : PageModel
    {
        private readonly IRepositorioHistoria _repoHistoria;
        private readonly IRepositorioMascota _repoMascota;

        public Historia historia { get; set; }
        public Mascota mascota { get; set; }

        public DetallesHistoriaModel(IRepositorioHistoria repoHistoria, IRepositorioMascota repoMascota)
        {
            _repoHistoria = repoHistoria;
            _repoMascota = repoMascota;
        }

        public IActionResult OnGet(int historiaId)
        {
            historia = _repoHistoria.GetHistoria(historiaId);
            if (historia == null)
            {
                return RedirectToPage("/NotFound");
            }
            mascota = _repoMascota.GetMascotaPorHistoriaId(historiaId);
            return Page();
        }
    }
}
