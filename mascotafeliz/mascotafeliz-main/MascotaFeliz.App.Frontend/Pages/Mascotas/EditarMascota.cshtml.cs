using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MascotaFeliz.App.Dominio;
using MascotaFeliz.App.Persistencia;
using MascotaFeliz.App.Frontend.Servicios;

namespace MascotaFeliz.App.Frontend.Pages
{
    public class EditarMascotaModel : PageModel
    {
        private readonly IRepositorioMascota _repoMascota;
        private readonly IRepositorioDueno _repoDueno;
        private readonly IRepositorioVeterinario _repoVeterinario;
        private readonly IRepositorioHistoria _repoHistoria;
        private readonly IServicioFotos _servicioFotos;

        [BindProperty]
        public Mascota mascota { get; set; }

        // Foto subida (opcional). No se bindea al modelo, se procesa aparte.
        [BindProperty]
        public IFormFile Foto { get; set; }

        public List<SelectListItem> DuenosOpciones { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> VeterinariosOpciones { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> HistoriasOpciones { get; set; } = new List<SelectListItem>();

        public EditarMascotaModel(
            IRepositorioMascota repoMascota,
            IRepositorioDueno repoDueno,
            IRepositorioVeterinario repoVeterinario,
            IRepositorioHistoria repoHistoria,
            IServicioFotos servicioFotos)
        {
            _repoMascota = repoMascota;
            _repoDueno = repoDueno;
            _repoVeterinario = repoVeterinario;
            _repoHistoria = repoHistoria;
            _servicioFotos = servicioFotos;
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

        public async Task<IActionResult> OnPostAsync()
        {
            CargarSelectores();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Procesar la foto si se subió una.
            if (Foto != null && Foto.Length > 0)
            {
                try
                {
                    // Si es edición y ya tenía foto, eliminar la anterior.
                    if (mascota.Id > 0 && !string.IsNullOrEmpty(mascota.FotoUrl))
                    {
                        await _servicioFotos.EliminarFotoAsync(mascota.FotoUrl);
                    }
                    // Guardar la nueva (usa Id temporal para nuevas mascotas).
                    var idTemp = mascota.Id > 0 ? mascota.Id : 0;
                    mascota.FotoUrl = await _servicioFotos.GuardarFotoAsync(Foto, idTemp);
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError("Foto", ex.Message);
                    return Page();
                }
            }

            if (mascota.Id > 0)
            {
                _repoMascota.UpdateMascota(mascota);
                TempData["Mensaje"] = "Mascota actualizada.";
            }
            else
            {
                _repoMascota.AddMascota(mascota);
                TempData["Mensaje"] = "Mascota creada.";
            }

            return RedirectToPage("./ListaMascotas");
        }

        public async Task<IActionResult> OnPostEliminarFotoAsync(int mascotaId)
        {
            var mascota = _repoMascota.GetMascota(mascotaId);
            if (mascota != null && !string.IsNullOrEmpty(mascota.FotoUrl))
            {
                await _servicioFotos.EliminarFotoAsync(mascota.FotoUrl);
                mascota.FotoUrl = null;
                _repoMascota.UpdateMascota(mascota);
                TempData["Mensaje"] = "Foto eliminada.";
            }
            return RedirectToPage("./EditarMascota", new { mascotaId });
        }

        private void CargarSelectores()
        {
            DuenosOpciones = _repoDueno.GetAllDuenos()
                .OrderBy(d => d.Apellidos).ThenBy(d => d.Nombres)
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Nombres + " " + d.Apellidos
                }).ToList();

            VeterinariosOpciones = _repoVeterinario.GetAllVeterinarios()
                .OrderBy(v => v.Apellidos).ThenBy(v => v.Nombres)
                .Select(v => new SelectListItem
                {
                    Value = v.Id.ToString(),
                    Text = v.Nombres + " " + v.Apellidos
                }).ToList();

            HistoriasOpciones = _repoHistoria.GetAllHistorias()
                .OrderBy(h => h.FechaInicial)
                .Select(h => new SelectListItem
                {
                    Value = h.Id.ToString(),
                    Text = "Historia #" + h.Id + " · " + h.FechaInicial.ToString("dd/MM/yyyy")
                }).ToList();
        }
    }
}
