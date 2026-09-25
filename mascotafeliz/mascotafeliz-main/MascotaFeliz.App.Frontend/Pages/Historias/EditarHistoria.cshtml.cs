using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MascotaFeliz.App.Dominio;
using MascotaFeliz.App.Persistencia;

namespace MascotaFeliz.App.Frontend.Pages
{
    /// <summary>
    /// Crear o editar una historia clínica. La historia es el contenedor
    /// de las visitas PyP de una mascota.
    /// </summary>
    [Authorize]
    public class EditarHistoriaModel : PageModel
    {
        private readonly IRepositorioHistoria _repoHistoria;

        [BindProperty]
        public Historia historia { get; set; }

        public EditarHistoriaModel(IRepositorioHistoria repoHistoria)
        {
            _repoHistoria = repoHistoria;
        }

        public IActionResult OnGet(int? historiaId)
        {
            if (historiaId.HasValue)
            {
                historia = _repoHistoria.GetHistoria(historiaId.Value);
            }
            else
            {
                historia = new Historia { FechaInicial = System.DateTime.Today };
            }

            if (historia == null)
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

            if (historia.Id > 0)
            {
                _repoHistoria.UpdateHistoria(historia);
                TempData["Mensaje"] = "Historia clínica actualizada.";
            }
            else
            {
                _repoHistoria.AddHistoria(historia);
                TempData["Mensaje"] = "Historia clínica creada.";
            }

            return RedirectToPage("./ListaHistorias");
        }

        public IActionResult OnPostEliminar(int historiaId)
        {
            try
            {
                _repoHistoria.DeleteHistoria(historiaId);
                TempData["Mensaje"] = "Historia clínica eliminada.";
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException)
            {
                TempData["Error"] = "No se puede eliminar la historia porque tiene una mascota asociada. Desvincula la mascota primero.";
            }
            return RedirectToPage("./ListaHistorias");
        }
    }
}
