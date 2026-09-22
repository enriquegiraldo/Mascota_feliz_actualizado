using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MascotaFeliz.App.Dominio;
using MascotaFeliz.App.Persistencia;

namespace MascotaFeliz.App.Frontend.Pages
{
    /// <summary>
    /// Detalle de una visita PyP concreta.
    /// </summary>
    public class DetallesVisitaPyPModel : PageModel
    {
        private readonly IRepositorioVisitaPyP _repoVisitaPyP;
        public VisitaPyP visitaPyP { get; set; }

        public DetallesVisitaPyPModel(IRepositorioVisitaPyP repoVisitaPyP)
        {
            _repoVisitaPyP = repoVisitaPyP;
        }

        public IActionResult OnGet(int visitaPyPId)
        {
            visitaPyP = _repoVisitaPyP.GetVisitaPyP(visitaPyPId);
            if (visitaPyP == null)
            {
                return RedirectToPage("/NotFound");
            }
            return Page();
        }
    }
}
