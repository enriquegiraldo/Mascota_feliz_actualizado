using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MascotaFeliz.App.Dominio;
using MascotaFeliz.App.Persistencia;

namespace MascotaFeliz.App.Frontend.Pages
{
    public class DetallesDuenosModel : PageModel
    {
        private readonly IRepositorioDueno _repoDueno;
        private readonly IRepositorioMascota _repoMascota;

        public Dueno dueno { get; set; }
        public IEnumerable<Mascota> mascotas { get; set; }

        public DetallesDuenosModel(IRepositorioDueno repoDueno, IRepositorioMascota repoMascota)
        {
            _repoDueno = repoDueno;
            _repoMascota = repoMascota;
        }

        public IActionResult OnGet(int duenoId)
        {
            dueno = _repoDueno.GetDueno(duenoId);
            if (dueno == null)
            {
                return RedirectToPage("/NotFound");
            }
            mascotas = _repoMascota.GetMascotasPorDueno(duenoId).ToList();
            return Page();
        }
    }
}
