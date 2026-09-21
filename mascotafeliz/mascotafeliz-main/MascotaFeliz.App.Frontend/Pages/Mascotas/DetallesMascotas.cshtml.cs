using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MascotaFeliz.App.Dominio;
using MascotaFeliz.App.Persistencia;

namespace MascotaFeliz.App.Frontend.Pages
{
    public class DetallesMascotasModel : PageModel
    {
        private readonly IRepositorioMascota _repoMascota;
        public Mascota mascota { get; set; }

        public DetallesMascotasModel(IRepositorioMascota repoMascota)
        {
            _repoMascota = repoMascota;
        }

        public IActionResult OnGet(int mascotaId)
        {
            mascota = _repoMascota.GetMascota(mascotaId);
            if (mascota == null)
            {
                return RedirectToPage("/NotFound");
            }
            return Page();
        }
    }
}
