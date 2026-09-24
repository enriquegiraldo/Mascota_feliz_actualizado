using Microsoft.AspNetCore.Http;

namespace MascotaFeliz.App.Frontend.Servicios
{
    /// <summary>
    /// Abstracción del servicio de almacenamiento de fotos.
    /// Permite cambiar entre almacenamiento local (dev) y Blob Storage (prod)
    /// sin tocar el resto de la aplicación.
    /// </summary>
    public interface IServicioFotos
    {
        /// <summary>
        /// Guarda la foto subida y devuelve la URL relativa (o absoluta en prod)
        /// que se almacena en Mascota.FotoUrl.
        /// </summary>
        /// <param name="archivo">Archivo subido (IFormFile).</param>
        /// <param name="mascotaId">Id de la mascota (para nombrar el archivo).</param>
        /// <returns>URL de la foto guardada, o null si el archivo es inválido.</returns>
        Task<string> GuardarFotoAsync(IFormFile archivo, int mascotaId);

        /// <summary>
        /// Elimina la foto anterior si existe (para reemplazarla).
        /// </summary>
        Task EliminarFotoAsync(string url);

        /// <summary>
        /// Extensiones permitidas (sin punto).
        /// </summary>
        static readonly string[] ExtensionesPermitidas = { ".jpg", ".jpeg", ".png", ".webp" };

        /// <summary>
        /// Tamaño máximo en bytes (2 MB).
        /// </summary>
        const long TamanoMaximo = 2 * 1024 * 1024;
    }
}
