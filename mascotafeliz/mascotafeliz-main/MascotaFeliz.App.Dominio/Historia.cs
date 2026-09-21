using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MascotaFeliz.App.Dominio
{
    /// <summary>
    /// Historia clínica de una mascota. Agrupa todas sus visitas PyP.
    /// </summary>
    public class Historia
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La fecha inicial es obligatoria.")]
        [Display(Name = "Fecha inicial")]
        [DataType(DataType.Date)]
        public DateTime FechaInicial { get; set; }

        [Display(Name = "Visitas PyP")]
        public ICollection<VisitaPyP> VisitasPyP { get; set; }
    }
}
