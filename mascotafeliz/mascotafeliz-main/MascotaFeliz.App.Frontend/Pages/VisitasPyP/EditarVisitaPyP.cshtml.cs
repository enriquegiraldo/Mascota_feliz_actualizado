using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MascotaFeliz.App.Dominio;
using MascotaFeliz.App.Persistencia;

namespace MascotaFeliz.App.Frontend.Pages
{
    /// <summary>
    /// Crear o editar una visita PyP. Permite preseleccionar la historia
    /// vía query string (desde el perfil de una mascota).
    /// </summary>
    public class EditarVisitaPyPModel : PageModel
    {
        private readonly IRepositorioVisitaPyP _repoVisitaPyP;
        private readonly IRepositorioHistoria _repoHistoria;

        [BindProperty]
        public VisitaPyP visitaPyP { get; set; }

        public List<SelectListItem> HistoriasOpciones { get; set; } = new List<SelectListItem>();

        public EditarVisitaPyPModel(IRepositorioVisitaPyP repoVisitaPyP, IRepositorioHistoria repoHistoria)
        {
            _repoVisitaPyP = repoVisitaPyP;
            _repoHistoria = repoHistoria;
        }

        public IActionResult OnGet(int? visitaPyPId, int? historiaId)
        {
            CargarSelectores();

            if (visitaPyPId.HasValue)
            {
                visitaPyP = _repoVisitaPyP.GetVisitaPyP(visitaPyPId.Value);
            }
            else
            {
                visitaPyP = new VisitaPyP
                {
                    FechaVisita = System.DateTime.Today
                };
                if (historiaId.HasValue)
                {
                    visitaPyP.HistoriaId = historiaId.Value;
                }
            }

            if (visitaPyP == null)
            {
                return RedirectToPage("/NotFound");
            }
            return Page();
        }

        public IActionResult OnPost()
        {
            CargarSelectores();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (visitaPyP.Id > 0)
            {
                _repoVisitaPyP.UpdateVisitaPyP(visitaPyP);
                TempData["Mensaje"] = "Visita actualizada.";
            }
            else
            {
                _repoVisitaPyP.AddVisitaPyP(visitaPyP);
                TempData["Mensaje"] = "Visita registrada.";
            }

            return RedirectToPage("/Historias/DetallesHistoria", new { historiaId = visitaPyP.HistoriaId });
        }

        public IActionResult OnPostEliminar(int visitaPyPId)
        {
            var visita = _repoVisitaPyP.GetVisitaPyP(visitaPyPId);
            int? historiaId = visita?.HistoriaId;
            _repoVisitaPyP.DeleteVisitaPyP(visitaPyPId);
            if (historiaId.HasValue)
            {
                return RedirectToPage("/Historias/DetallesHistoria", new { historiaId });
            }
            return RedirectToPage("./ListaVisitasPyP");
        }

        private void CargarSelectores()
        {
            HistoriasOpciones = _repoHistoria.GetAllHistorias()
                .OrderBy(h => h.FechaInicial)
                .Select(h => new SelectListItem
                {
                    Value = h.Id.ToString(),
                    Text = $"Historia #{h.Id} · {h.FechaInicial:dd/MM/yyyy}"
                }).ToList();
        }
    }
}
