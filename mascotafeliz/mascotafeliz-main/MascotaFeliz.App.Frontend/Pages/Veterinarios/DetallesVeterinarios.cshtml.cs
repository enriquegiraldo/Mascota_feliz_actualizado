using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MascotaFeliz.App.Dominio;
using MascotaFeliz.App.Persistencia;

namespace MascotaFeliz.App.Frontend.Pages
{
    [Authorize]
    public class DetallesVeterinariosModel : PageModel
    {
        private readonly IRepositorioVeterinario _repoVeterinario;
        private readonly IRepositorioMascota _repoMascota;

        public Veterinario veterinario { get; set; }
        public IEnumerable<Mascota> mascotas { get; set; }

        public DetallesVeterinariosModel(IRepositorioVeterinario repoVeterinario, IRepositorioMascota repoMascota)
        {
            _repoVeterinario = repoVeterinario;
            _repoMascota = repoMascota;
        }

        public IActionResult OnGet(int veterinarioId)
        {
            veterinario = _repoVeterinario.GetVeterinario(veterinarioId);
            if (veterinario == null)
            {
                return RedirectToPage("/NotFound");
            }
            mascotas = _repoMascota.GetMascotasPorVeterinario(veterinarioId).ToList();
            return Page();
        }
    }
}
