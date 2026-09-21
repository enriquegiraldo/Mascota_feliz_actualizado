using System;
using System.ComponentModel.DataAnnotations;

namespace MascotaFeliz.App.Dominio
{
    /// <summary>
    /// Visita de PyP (Promoción y Prevención). Registra los signos vitales
    /// y recomendaciones de una consulta dentro de una historia clínica.
    /// </summary>
    public class VisitaPyP
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La fecha de la visita es obligatoria.")]
        [Display(Name = "Fecha de la visita")]
        [DataType(DataType.Date)]
        public DateTime FechaVisita { get; set; }

        [Display(Name = "Temperatura (°C)")]
        [Range(30, 45, ErrorMessage = "La temperatura debe estar entre 30 y 45 °C.")]
        public float Temperatura { get; set; }

        [Display(Name = "Peso (kg)")]
        [Range(0.1, 200, ErrorMessage = "El peso debe estar entre 0,1 y 200 kg.")]
        public float Peso { get; set; }

        [Display(Name = "Frecuencia respiratoria (x min)")]
        [Range(0, 200, ErrorMessage = "La frecuencia respiratoria debe estar entre 0 y 200.")]
        public float FrecuenciaRespiratoria { get; set; }

        [Display(Name = "Frecuencia cardiaca (x min)")]
        [Range(0, 300, ErrorMessage = "La frecuencia cardiaca debe estar entre 0 y 300.")]
        public float FrecuenciaCardiaca { get; set; }

        [StringLength(40, ErrorMessage = "El estado de ánimo no puede superar 40 caracteres.")]
        [Display(Name = "Estado de ánimo")]
        public string EstadoAnimo { get; set; }

        [StringLength(500, ErrorMessage = "Las recomendaciones no pueden superar 500 caracteres.")]
        [Display(Name = "Recomendaciones")]
        public string Recomendaciones { get; set; }

        [Display(Name = "Historia clínica")]
        public int HistoriaId { get; set; }
        public Historia Historia { get; set; }
    }
}
