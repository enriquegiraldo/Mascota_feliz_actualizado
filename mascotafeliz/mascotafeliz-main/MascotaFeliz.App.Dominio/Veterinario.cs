using System;
using System.ComponentModel.DataAnnotations;

namespace MascotaFeliz.App.Dominio
{
    /// <summary>
    /// Profesional veterinario que atiende a las mascotas.
    /// </summary>
    public class Veterinario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(60, ErrorMessage = "El nombre no puede superar 60 caracteres.")]
        [Display(Name = "Nombres")]
        public string Nombres { get; set; }

        [Required(ErrorMessage = "Los apellidos son obligatorios.")]
        [StringLength(60, ErrorMessage = "Los apellidos no pueden superar 60 caracteres.")]
        [Display(Name = "Apellidos")]
        public string Apellidos { get; set; }

        [StringLength(120, ErrorMessage = "La dirección no puede superar 120 caracteres.")]
        [Display(Name = "Dirección")]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [Phone(ErrorMessage = "El teléfono no es válido.")]
        [StringLength(20, ErrorMessage = "El teléfono no puede superar 20 caracteres.")]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "La tarjeta profesional es obligatoria.")]
        [StringLength(20, ErrorMessage = "La tarjeta profesional no puede superar 20 caracteres.")]
        [Display(Name = "Tarjeta profesional")]
        public string TarjetaProfesional { get; set; }
    }
}
