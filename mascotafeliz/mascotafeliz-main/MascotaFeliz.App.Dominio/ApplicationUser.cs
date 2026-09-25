using Microsoft.AspNetCore.Identity;

namespace MascotaFeliz.App.Dominio
{
    /// <summary>
    /// Usuario de la aplicación (personal de la clínica).
    /// Extiende IdentityUser con datos del empleado.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        [System.ComponentModel.DataAnnotations.Display(Name = "Nombres")]
        [System.ComponentModel.DataAnnotations.StringLength(60)]
        public string Nombres { get; set; }

        [System.ComponentModel.DataAnnotations.Display(Name = "Apellidos")]
        [System.ComponentModel.DataAnnotations.StringLength(60)]
        public string Apellidos { get; set; }
    }
}
