using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace MascotaFeliz.App.Frontend.Servicios
{
    /// <summary>
    /// Implementación local de IServicioFotos para desarrollo.
    /// Guarda las fotos en wwwroot/uploads/mascotas/ y las sirve como
    /// archivos estáticos. En producción se usa ServicioFotosBlob.
    /// </summary>
    public class ServicioFotosLocal : IServicioFotos
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ServicioFotosLocal> _logger;

        public ServicioFotosLocal(IWebHostEnvironment env, ILogger<ServicioFotosLocal> logger)
        {
            _env = env;
            _logger = logger;
        }

        public async Task<string> GuardarFotoAsync(IFormFile archivo, int mascotaId)
        {
            if (archivo == null || archivo.Length == 0)
            {
                return null;
            }

            // Validar extensión.
            var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();
            if (!IServicioFotos.ExtensionesPermitidas.Contains(extension))
            {
                throw new ArgumentException(
                    $"Extensión no permitida. Use: {string.Join(", ", IServicioFotos.ExtensionesPermitidas)}.");
            }

            // Validar tamaño.
            if (archivo.Length > IServicioFotos.TamanoMaximo)
            {
                throw new ArgumentException($"El archivo supera el tamaño máximo de {IServicioFotos.TamanoMaximo / 1024 / 1024} MB.");
            }

            // Crear la carpeta si no existe.
            var carpeta = Path.Combine(_env.WebRootPath, "uploads", "mascotas");
            Directory.CreateDirectory(carpeta);

            // Nombre único: mascota-{id}-{guid}.{ext}
            var nombreArchivo = $"mascota-{mascotaId}-{Guid.NewGuid():N}{extension}";
            var rutaCompleta = Path.Combine(carpeta, nombreArchivo);

            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }

            _logger.LogInformation("Foto guardada: {Ruta}", rutaCompleta);

            // URL relativa para servir vía StaticFiles.
            return $"/uploads/mascotas/{nombreArchivo}";
        }

        public Task EliminarFotoAsync(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return Task.CompletedTask;
            }

            // Solo eliminar si es una URL local (no de Blob Storage).
            if (url.StartsWith("/uploads/"))
            {
                var rutaRelativa = url.TrimStart('/');
                var rutaCompleta = Path.Combine(_env.WebRootPath, rutaRelativa);
                if (File.Exists(rutaCompleta))
                {
                    File.Delete(rutaCompleta);
                    _logger.LogInformation("Foto eliminada: {Ruta}", rutaCompleta);
                }
            }

            return Task.CompletedTask;
        }
    }
}
