using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MascotaFeliz.App.Dominio;
using MascotaFeliz.App.Persistencia;

namespace MascotaFeliz.App.Frontend.Pages
{
    public class DetallesVeterinariosModel : PageModel
    {
        private readonly IRepositorioVeterinario _repoVeterinario;
        public Veterinario veterinario { get; set; }

        public DetallesVeterinariosModel(IRepositorioVeterinario repoVeterinario)
        {
            _repoVeterinario = repoVeterinario;
        }

        public IActionResult OnGet(int veterinarioId)
        {
            veterinario = _repoVeterinario.GetVeterinario(veterinarioId);
            if (veterinario == null)
            {
                return RedirectToPage("/NotFound");
            }
            return Page();
        }
    }
}
