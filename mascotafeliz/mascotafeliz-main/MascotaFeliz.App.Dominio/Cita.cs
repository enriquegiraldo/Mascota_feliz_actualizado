using System;
using System.ComponentModel.DataAnnotations;

namespace MascotaFeliz.App.Dominio
{
    /// <summary>
    /// Cita veterinaria solicitada por un dueño y gestionada por la clínica.
    /// </summary>
    public class Cita
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La fecha y hora de la cita es obligatoria.")]
        [Display(Name = "Fecha y hora")]
        [DataType(DataType.DateTime)]
        public DateTime FechaHora { get; set; }

        [Required(ErrorMessage = "La mascota es obligatoria.")]
        [Display(Name = "Mascota")]
        public int MascotaId { get; set; }
        public Mascota Mascota { get; set; }

        [Required(ErrorMessage = "El veterinario es obligatorio.")]
        [Display(Name = "Veterinario")]
        public int VeterinarioId { get; set; }
        public Veterinario Veterinario { get; set; }

        [StringLength(120, ErrorMessage = "El motivo no puede superar 120 caracteres.")]
        [Display(Name = "Motivo de la consulta")]
        public string Motivo { get; set; }

        [Display(Name = "Estado")]
        public EstadoCita Estado { get; set; } = EstadoCita.Pendiente;

        [StringLength(500, ErrorMessage = "Las notas internas no pueden superar 500 caracteres.")]
        [Display(Name = "Notas internas")]
        public string NotasInternas { get; set; }
    }
}
