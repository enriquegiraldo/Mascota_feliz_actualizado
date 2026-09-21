using System;
using System.ComponentModel.DataAnnotations;

namespace MascotaFeliz.App.Dominio
{
    /// <summary>
    /// Mascota registrada en la clínica. Se relaciona con su dueño,
    /// su veterinario responsable y su historia clínica.
    /// </summary>
    public class Mascota
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la mascota es obligatorio.")]
        [StringLength(60, ErrorMessage = "El nombre no puede superar 60 caracteres.")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [StringLength(40, ErrorMessage = "El color no puede superar 40 caracteres.")]
        [Display(Name = "Color")]
        public string Color { get; set; }

        [Required(ErrorMessage = "La especie es obligatoria.")]
        [StringLength(40, ErrorMessage = "La especie no puede superar 40 caracteres.")]
        [Display(Name = "Especie")]
        public string Especie { get; set; }

        [Required(ErrorMessage = "La raza es obligatoria.")]
        [StringLength(40, ErrorMessage = "La raza no puede superar 40 caracteres.")]
        [Display(Name = "Raza")]
        public string Raza { get; set; }

        [Display(Name = "Veterinario")]
        public int? VeterinarioId { get; set; }
        public Veterinario Veterinario { get; set; }

        [Display(Name = "Dueño")]
        public int? DuenoId { get; set; }
        public Dueno Dueno { get; set; }

        [Display(Name = "Historia clínica")]
        public int? HistoriaId { get; set; }
        public Historia Historia { get; set; }
    }
}
