using System;

namespace MascotaFeliz.App.Dominio
{
    public class Veterinario
    {
        public int Id { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string TarjetaProfesional { get; set; }
    }
}
