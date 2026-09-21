using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MascotaFeliz.App.Dominio;
using MascotaFeliz.App.Persistencia;

namespace MascotaFeliz.App.Frontend.Pages
{
    public class DetallesDuenosModel : PageModel
    {
        private readonly IRepositorioDueno _repoDueno;
        public Dueno dueno { get; set; }

        public DetallesDuenosModel(IRepositorioDueno repoDueno)
        {
            _repoDueno = repoDueno;
        }

        public IActionResult OnGet(int duenoId)
        {
            dueno = _repoDueno.GetDueno(duenoId);
            if (dueno == null)
            {
                return RedirectToPage("/NotFound");
            }
            return Page();
        }
    }
}
