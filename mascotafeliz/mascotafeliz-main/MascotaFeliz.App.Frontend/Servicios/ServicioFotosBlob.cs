using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace MascotaFeliz.App.Frontend.Servicios
{
    /// <summary>
    /// Implementación de IServicioFotos para producción con Azure Blob Storage.
    /// Se activa cuando la configuración "Storage:ConnectionString" está presente.
    /// El contenedor es "mascotas-fotos" (se crea si no existe).
    /// </summary>
    public class ServicioFotosBlob : IServicioFotos
    {
        private readonly BlobServiceClient _cliente;
        private readonly string _contenedor = "mascotas-fotos";
        private readonly ILogger<ServicioFotosBlob> _logger;

        public ServicioFotosBlob(IConfiguration config, ILogger<ServicioFotosBlob> logger)
        {
            var connectionString = config["Storage:ConnectionString"]
                ?? throw new InvalidOperationException("Falta Storage:ConnectionString en la configuración.");
            _cliente = new BlobServiceClient(connectionString);
            _logger = logger;
        }

        public async Task<string> GuardarFotoAsync(IFormFile archivo, int mascotaId)
        {
            if (archivo == null || archivo.Length == 0)
            {
                return null;
            }

            var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();
            if (!IServicioFotos.ExtensionesPermitidas.Contains(extension))
            {
                throw new ArgumentException(
                    $"Extensión no permitida. Use: {string.Join(", ", IServicioFotos.ExtensionesPermitidas)}.");
            }

            if (archivo.Length > IServicioFotos.TamanoMaximo)
            {
                throw new ArgumentException($"El archivo supera el tamaño máximo de {IServicioFotos.TamanoMaximo / 1024 / 1024} MB.");
            }

            var contenedor = _cliente.GetBlobContainerClient(_contenedor);
            await contenedor.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var nombreBlob = $"mascota-{mascotaId}-{Guid.NewGuid():N}{extension}";
            var blob = contenedor.GetBlobClient(nombreBlob);

            using (var stream = archivo.OpenReadStream())
            {
                await blob.UploadAsync(stream, new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders { ContentType = archivo.ContentType }
                });
            }

            _logger.LogInformation("Foto subida a Blob Storage: {Blob}", nombreBlob);
            return blob.Uri.ToString();
        }

        public async Task EliminarFotoAsync(string url)
        {
            if (string.IsNullOrWhiteSpace(url) || !Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                return;
            }

            try
            {
                var uri = new Uri(url);
                var nombreBlob = Path.GetFileName(uri.LocalPath);
                var contenedor = _cliente.GetBlobContainerClient(_contenedor);
                var blob = contenedor.GetBlobClient(nombreBlob);
                await blob.DeleteIfExistsAsync();
                _logger.LogInformation("Foto eliminada de Blob Storage: {Blob}", nombreBlob);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "No se pudo eliminar la foto: {Url}", url);
            }
        }
    }
}
