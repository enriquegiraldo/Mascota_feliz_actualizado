using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MascotaFeliz.App.Dominio;

namespace MascotaFeliz.App.Frontend.ViewComponents
{
    /// <summary>
    /// Renderiza una lista de mascotas como tarjetas clicables.
    /// Reutilizable en DetallesDuenos y DetallesVeterinarios (DRY).
    /// </summary>
    public class TarjetasMascotasViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(IEnumerable<Mascota> mascotas, string titulo = "Mascotas", bool mostrarDueno = false)
        {
            ViewBag.Titulo = titulo;
            ViewBag.MostrarDueno = mostrarDueno;
            return View(mascotas);
        }
    }
}
