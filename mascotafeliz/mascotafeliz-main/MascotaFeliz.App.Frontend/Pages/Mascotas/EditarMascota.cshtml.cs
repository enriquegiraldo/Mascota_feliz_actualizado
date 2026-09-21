using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MascotaFeliz.App.Dominio;
using MascotaFeliz.App.Persistencia;

namespace MascotaFeliz.App.Frontend.Pages
{
    public class EditarMascotaModel : PageModel
    {
        private readonly IRepositorioMascota _repoMascota;
        private readonly IRepositorioDueno _repoDueno;
        private readonly IRepositorioVeterinario _repoVeterinario;

        [BindProperty]
        public Mascota mascota { get; set; }

        // Opciones para los selectores de dueño y veterinario (texto = nombre completo).
        public List<SelectListItem> DuenosOpciones { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> VeterinariosOpciones { get; set; } = new List<SelectListItem>();

        public EditarMascotaModel(
            IRepositorioMascota repoMascota,
            IRepositorioDueno repoDueno,
            IRepositorioVeterinario repoVeterinario)
        {
            _repoMascota = repoMascota;
            _repoDueno = repoDueno;
            _repoVeterinario = repoVeterinario;
        }

        public IActionResult OnGet(int? mascotaId)
        {
            CargarSelectores();

            if (mascotaId.HasValue)
            {
                mascota = _repoMascota.GetMascota(mascotaId.Value);
            }
            else
            {
                mascota = new Mascota();
            }

            if (mascota == null)
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

            if (mascota.Id > 0)
            {
                _repoMascota.UpdateMascota(mascota);
            }
            else
            {
                _repoMascota.AddMascota(mascota);
            }

            return RedirectToPage("./ListaMascotas");
        }

        private void CargarSelectores()
        {
            DuenosOpciones = _repoDueno.GetAllDuenos()
                .OrderBy(d => d.Apellidos).ThenBy(d => d.Nombres)
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = $"{d.Nombres} {d.Apellidos}"
                }).ToList();

            VeterinariosOpciones = _repoVeterinario.GetAllVeterinarios()
                .OrderBy(v => v.Apellidos).ThenBy(v => v.Nombres)
                .Select(v => new SelectListItem
                {
                    Value = v.Id.ToString(),
                    Text = $"{v.Nombres} {v.Apellidos}"
                }).ToList();
        }
    }
}
