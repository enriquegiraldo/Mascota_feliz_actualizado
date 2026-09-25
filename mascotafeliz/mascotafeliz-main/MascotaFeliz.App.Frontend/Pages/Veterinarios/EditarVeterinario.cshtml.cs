using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MascotaFeliz.App.Dominio;
using MascotaFeliz.App.Persistencia;

namespace MascotaFeliz.App.Frontend.Pages
{
    [Authorize]
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
                TempData["Mensaje"] = "Veterinario actualizado.";
            }
            else
            {
                _repoVeterinario.AddVeterinario(veterinario);
                TempData["Mensaje"] = "Veterinario creado.";
            }

            return RedirectToPage("./ListaVeterinarios");
        }
    }
}
