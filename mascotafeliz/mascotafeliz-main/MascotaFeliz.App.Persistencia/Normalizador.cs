using System.Globalization;
using System.Text;

namespace MascotaFeliz.App.Persistencia
{
    /// <summary>
    /// Utilidades de normalización de texto para búsqueda.
    /// Permite que las búsquedas sean insensibles a mayúsculas y acentos
    /// (p. ej. "LOPEZ" encuentra "López"), lo cual es esencial para es-CO.
    /// </summary>
    public static class Normalizador
    {
        /// <summary>
        /// Normaliza un texto: minúsculas, sin acentos y sin espacios al borde.
        /// Quitar acentos usando descomposición Unicode (FormD) y descartando
        /// las marcas de combinación (UnicodeCategory.NonSpacingMark).
        /// </summary>
        public static string Normalizar(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                return "";
            }

            var normalizado = texto.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder(normalizado.Length);
            foreach (var c in normalizado)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
