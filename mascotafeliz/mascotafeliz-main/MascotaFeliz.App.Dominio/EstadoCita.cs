namespace MascotaFeliz.App.Dominio
{
    /// <summary>
    /// Estado de una cita en el ciclo de vida de la atención veterinaria.
    /// </summary>
    public enum EstadoCita
    {
        /// <summary>El dueño la solicitó; la clínica aún no la confirma.</summary>
        Pendiente = 0,

        /// <summary>La clínica la confirmó con el dueño.</summary>
        Confirmada = 1,

        /// <summary>El dueño o la clínica la cancelaron.</summary>
        Cancelada = 2,

        /// <summary>La mascota fue atendida; la cita se cerró.</summary>
        Atendida = 3
    }
}
