using Xunit;
using Microsoft.Data.Sqlite;
using AppContext = MascotaFeliz.App.Persistencia.AppContext;
using MascotaFeliz.App.Persistencia;

namespace MascotaFeliz.App.Pruebas
{
    /// <summary>
    /// Pruebas del Normalizador (insensibilidad a mayúsculas y acentos).
    /// </summary>
    public class NormalizadorPruebas
    {
        [Theory]
        [InlineData("López", "lopez")]
        [InlineData("LOPEZ", "lopez")]
        [InlineData("LÓPEZ", "lopez")]
        [InlineData("lópez", "lopez")]
        [InlineData("Ñúñez", "nunez")]
        [InlineData("MÉNDEZ", "mendez")]
        [InlineData("Gómez", "gomez")]
        [InlineData("Café", "cafe")]
        [InlineData("  Pérez  ", "perez")]
        [InlineData("", "")]
        [InlineData(null, "")]
        public void Normalizar_QuitaAcentosYMayusculas(string entrada, string esperado)
        {
            Assert.Equal(esperado, Normalizador.Normalizar(entrada));
        }

        [Fact]
        public void Normalizar_EsIdempotente()
        {
            var texto = "López Martínez";
            Assert.Equal(Normalizador.Normalizar(texto), Normalizador.Normalizar(Normalizador.Normalizar(texto)));
        }
    }
}
