using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MascotaFeliz.App.Dominio;
using MascotaFeliz.App.Persistencia;

namespace MascotaFeliz.App.Frontend.Pages
{
    public class EditarVeterinarioModel : PageModel
    {
        private readonly IRepositorioVeterinario _repoVeterinario;

        [BindProperty]
        public Veterinario veterinario { get; set; }

        public EditarVeterinarioModel(IRepositorioVeterinario repoVeterinario)
        {
            _repoVeterinario = repoVeterinario;
        }

        public IActionResult OnGet(int? veterinarioId)
        {
            if (veterinarioId.HasValue)
            {
                veterinario = _repoVeterinario.GetVeterinario(veterinarioId.Value);
            }
            else
            {
                veterinario = new Veterinario();
            }

            if (veterinario == null)
            {
                return RedirectToPage("/NotFound");
            }
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (veterinario.Id > 0)
            {
                _repoVeterinario.UpdateVeterinario(veterinario);
            }
            else
            {
                _repoVeterinario.AddVeterinario(veterinario);
            }

            return RedirectToPage("./ListaVeterinarios");
        }
    }
}
