using System;
using System.ComponentModel.DataAnnotations;

namespace MascotaFeliz.App.Dominio
{
    /// <summary>
    /// Persona responsable (propietaria) de una o varias mascotas.
    /// </summary>
    public class Dueno
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

        [EmailAddress(ErrorMessage = "El correo no es válido.")]
        [StringLength(80, ErrorMessage = "El correo no puede superar 80 caracteres.")]
        [Display(Name = "Correo")]
        public string Correo { get; set; }
    }
}
