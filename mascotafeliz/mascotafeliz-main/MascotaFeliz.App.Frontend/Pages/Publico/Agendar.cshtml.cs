using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MascotaFeliz.App.Dominio;
using MascotaFeliz.App.Persistencia;

namespace MascotaFeliz.App.Frontend.Pages.Publico
{
    [AllowAnonymous]
    public class AgendarModel : PageModel
    {
        private readonly IRepositorioDueno _repoDueno;
        private readonly IRepositorioMascota _repoMascota;
        private readonly IRepositorioVeterinario _repoVet;
        private readonly IRepositorioCita _repoCita;

        [BindProperty]
        public SolicitudCita Solicitud { get; set; }

        public List<SelectListItem> VeterinariosOpciones { get; set; } = new List<SelectListItem>();

        public AgendarModel(
            IRepositorioDueno repoDueno,
            IRepositorioMascota repoMascota,
            IRepositorioVeterinario repoVet,
            IRepositorioCita repoCita)
        {
            _repoDueno = repoDueno;
            _repoMascota = repoMascota;
            _repoVet = repoVet;
            _repoCita = repoCita;
        }

        public void OnGet()
        {
            CargarSelectores();
            Solicitud = new SolicitudCita
            {
                FechaHora = System.DateTime.Today.AddDays(1).AddHours(10)
            };
        }

        public IActionResult OnPost()
        {
            CargarSelectores();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Buscar o crear el dueño por correo.
            var duenoExistente = _repoDueno.GetAllDuenos()
                .FirstOrDefault(d => d.Correo == Solicitud.CorreoDueno);

            int duenoId;
            if (duenoExistente != null)
            {
                duenoId = duenoExistente.Id;
            }
            else
            {
                var nuevoDueno = new Dueno
                {
                    Nombres = Solicitud.NombreDueno,
                    Apellidos = Solicitud.ApellidosDueno,
                    Telefono = Solicitud.TelefonoDueno,
                    Correo = Solicitud.CorreoDueno,
                    Direccion = Solicitud.DireccionDueno
                };
                duenoId = _repoDueno.AddDueno(nuevoDueno).Id;
            }

            // Crear la mascota (vinculada al dueño).
            var nuevaMascota = new Mascota
            {
                Nombre = Solicitud.NombreMascota,
                Especie = Solicitud.EspecieMascota,
                Raza = Solicitud.RazaMascota ?? "",
                DuenoId = duenoId,
                VeterinarioId = Solicitud.VeterinarioId
            };
            var mascotaCreada = _repoMascota.AddMascota(nuevaMascota);

            // Crear la cita con estado Pendiente.
            var nuevaCita = new Cita
            {
                MascotaId = mascotaCreada.Id,
                VeterinarioId = Solicitud.VeterinarioId,
                FechaHora = Solicitud.FechaHora,
                Motivo = Solicitud.Motivo ?? "Solicitud online",
                Estado = EstadoCita.Pendiente,
                NotasInternas = "Solicitud creada desde el sitio público por " + Solicitud.NombreDueno + " " + Solicitud.ApellidosDueno + ". Tel: " + Solicitud.TelefonoDueno
            };
            _repoCita.AddCita(nuevaCita);

            TempData["Exito"] = true;
            return RedirectToPage("./Agendar");
        }

        private void CargarSelectores()
        {
            VeterinariosOpciones = _repoVet.GetAllVeterinarios()
                .OrderBy(v => v.Apellidos).ThenBy(v => v.Nombres)
                .Select(v => new SelectListItem
                {
                    Value = v.Id.ToString(),
                    Text = v.Nombres + " " + v.Apellidos
                }).ToList();
        }
    }

    /// <summary>
    /// DTO para el formulario público de solicitud de cita.
    /// </summary>
    public class SolicitudCita
    {
        // Datos del dueño.
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Tu nombre es obligatorio.")]
        [System.ComponentModel.DataAnnotations.StringLength(60)]
        [System.ComponentModel.DataAnnotations.Display(Name = "Tu nombre")]
        public string NombreDueno { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Tu apellido es obligatorio.")]
        [System.ComponentModel.DataAnnotations.StringLength(60)]
        [System.ComponentModel.DataAnnotations.Display(Name = "Tu apellido")]
        public string ApellidosDueno { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "El teléfono es obligatorio.")]
        [System.ComponentModel.DataAnnotations.Phone(ErrorMessage = "El teléfono no es válido.")]
        [System.ComponentModel.DataAnnotations.StringLength(20)]
        [System.ComponentModel.DataAnnotations.Display(Name = "Teléfono")]
        public string TelefonoDueno { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "El correo es obligatorio.")]
        [System.ComponentModel.DataAnnotations.EmailAddress(ErrorMessage = "El correo no es válido.")]
        [System.ComponentModel.DataAnnotations.StringLength(80)]
        [System.ComponentModel.DataAnnotations.Display(Name = "Correo")]
        public string CorreoDueno { get; set; }

        [System.ComponentModel.DataAnnotations.StringLength(120)]
        [System.ComponentModel.DataAnnotations.Display(Name = "Dirección")]
        public string DireccionDueno { get; set; }

        // Datos de la mascota.
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "El nombre de la mascota es obligatorio.")]
        [System.ComponentModel.DataAnnotations.StringLength(60)]
        [System.ComponentModel.DataAnnotations.Display(Name = "Nombre de la mascota")]
        public string NombreMascota { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "La especie es obligatoria.")]
        [System.ComponentModel.DataAnnotations.StringLength(40)]
        [System.ComponentModel.DataAnnotations.Display(Name = "Especie")]
        public string EspecieMascota { get; set; }

        [System.ComponentModel.DataAnnotations.StringLength(40)]
        [System.ComponentModel.DataAnnotations.Display(Name = "Raza")]
        public string RazaMascota { get; set; }

        // Datos de la cita.
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "El veterinario es obligatorio.")]
        [System.ComponentModel.DataAnnotations.Display(Name = "Veterinario")]
        public int VeterinarioId { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "La fecha y hora son obligatorias.")]
        [System.ComponentModel.DataAnnotations.Display(Name = "Fecha y hora preferida")]
        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.DateTime)]
        public System.DateTime FechaHora { get; set; }

        [System.ComponentModel.DataAnnotations.StringLength(120)]
        [System.ComponentModel.DataAnnotations.Display(Name = "Motivo de la consulta")]
        public string Motivo { get; set; }
    }
}
