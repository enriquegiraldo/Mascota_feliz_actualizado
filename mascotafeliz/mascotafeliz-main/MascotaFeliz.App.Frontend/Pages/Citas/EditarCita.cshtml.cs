using Microsoft.AspNetCore.Authorization;
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
    /// Crear o editar una cita. Permite preseleccionar la mascota
    /// vía query string (desde el perfil de una mascota).
    /// </summary>
    [Authorize]
    public class EditarCitaModel : PageModel
    {
        private readonly IRepositorioCita _repoCita;
        private readonly IRepositorioMascota _repoMascota;
        private readonly IRepositorioVeterinario _repoVeterinario;

        [BindProperty]
        public Cita cita { get; set; }

        public List<SelectListItem> MascotasOpciones { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> VeterinariosOpciones { get; set; } = new List<SelectListItem>();

        public EditarCitaModel(
            IRepositorioCita repoCita,
            IRepositorioMascota repoMascota,
            IRepositorioVeterinario repoVeterinario)
        {
            _repoCita = repoCita;
            _repoMascota = repoMascota;
            _repoVeterinario = repoVeterinario;
        }

        public IActionResult OnGet(int? citaId, int? mascotaId)
        {
            CargarSelectores();

            if (citaId.HasValue)
            {
                cita = _repoCita.GetCita(citaId.Value);
            }
            else
            {
                cita = new Cita
                {
                    FechaHora = System.DateTime.Today.AddDays(1).AddHours(10),
                    Estado = EstadoCita.Pendiente
                };
                if (mascotaId.HasValue)
                {
                    cita.MascotaId = mascotaId.Value;
                }
            }

            if (cita == null)
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

            if (cita.Id > 0)
            {
                _repoCita.UpdateCita(cita);
                TempData["Mensaje"] = "Cita actualizada.";
            }
            else
            {
                _repoCita.AddCita(cita);
                TempData["Mensaje"] = "Cita agendada.";
            }

            return RedirectToPage("./ListaCitas");
        }

        private void CargarSelectores()
        {
            MascotasOpciones = _repoMascota.GetAllMascotas()
                .OrderBy(m => m.Nombre)
                .Select(m => new SelectListItem
                {
                    Value = m.Id.ToString(),
                    Text = m.Nombre + (m.Especie != null ? " · " + m.Especie : "")
                }).ToList();

            VeterinariosOpciones = _repoVeterinario.GetAllVeterinarios()
                .OrderBy(v => v.Apellidos).ThenBy(v => v.Nombres)
                .Select(v => new SelectListItem
                {
                    Value = v.Id.ToString(),
                    Text = v.Nombres + " " + v.Apellidos
                }).ToList();
        }
    }
}
