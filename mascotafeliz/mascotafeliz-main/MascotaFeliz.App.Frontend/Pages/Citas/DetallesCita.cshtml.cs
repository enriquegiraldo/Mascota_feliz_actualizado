using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MascotaFeliz.App.Dominio;
using MascotaFeliz.App.Persistencia;

namespace MascotaFeliz.App.Frontend.Pages
{
    /// <summary>
    /// Detalle de una cita concreta.
    /// </summary>
    [Authorize]
    public class DetallesCitaModel : PageModel
    {
        private readonly IRepositorioCita _repoCita;
        public Cita cita { get; set; }

        public DetallesCitaModel(IRepositorioCita repoCita)
        {
            _repoCita = repoCita;
        }

        public IActionResult OnGet(int citaId)
        {
            cita = _repoCita.GetCita(citaId);
            if (cita == null)
            {
                return RedirectToPage("/NotFound");
            }
            return Page();
        }
    }
}
