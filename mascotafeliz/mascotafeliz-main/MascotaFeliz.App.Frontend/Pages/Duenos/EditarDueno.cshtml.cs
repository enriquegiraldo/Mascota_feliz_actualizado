using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MascotaFeliz.App.Dominio;
using MascotaFeliz.App.Persistencia;

namespace MascotaFeliz.App.Frontend.Pages
{
    public class EditarDuenoModel : PageModel
    {
        private readonly IRepositorioDueno _repoDueno;

        [BindProperty]
        public Dueno dueno { get; set; }

        public EditarDuenoModel(IRepositorioDueno repoDueno)
        {
            _repoDueno = repoDueno;
        }

        public IActionResult OnGet(int? duenoId)
        {
            if (duenoId.HasValue)
            {
                dueno = _repoDueno.GetDueno(duenoId.Value);
            }
            else
            {
                dueno = new Dueno();
            }

            if (dueno == null)
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

            if (dueno.Id > 0)
            {
                _repoDueno.UpdateDueno(dueno);
            }
            else
            {
                _repoDueno.AddDueno(dueno);
            }

            return RedirectToPage("./ListaDuenos");
        }
    }
}
