using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MascotaFeliz.App.Dominio;
using MascotaFeliz.App.Persistencia;
using MascotaFeliz.App.Frontend.Servicios;

namespace MascotaFeliz.App.Frontend.Pages
{
    /// <summary>
    /// Página de inicio: si el usuario no está autenticado, redirige al sitio público.
    /// Si está autenticado, muestra el panel interno.
    /// </summary>
    public class IndexModel : PageModel
    {
        private readonly IRepositorioMascota _repoMascota;
        private readonly IRepositorioDueno _repoDueno;
        private readonly IRepositorioVeterinario _repoVeterinario;
        private readonly IRepositorioVisitaPyP _repoVisitaPyP;
        private readonly IServicioRecordatorios _servicioRecordatorios;

        public int TotalMascotas { get; set; }
        public int TotalDuenos { get; set; }
        public int TotalVeterinarios { get; set; }
        public int TotalVisitas { get; set; }

        /// <summary>Distribución de mascotas por especie (para el donut).</summary>
        public Dictionary<string, int> DistribucionEspecies { get; set; } = new Dictionary<string, int>();

        /// <summary>HTML del donut SVG generado en servidor (evita problemas de Razor con tags SVG).</summary>
        public string DonutSvg { get; set; }

        /// <summary>HTML de la leyenda del donut (generado en servidor para evitar problemas de Razor anidado).</summary>
        public string LeyendaDonutHtml { get; set; }

        /// <summary>HTML de la sección de recordatorios de vacunas (generado en servidor).</summary>
        public string RecordatoriosHtml { get; set; }

        /// <summary>Vacunas próximas a vencer o vencidas (recordatorios visuales).</summary>
        public IEnumerable<RecordatorioVacuna> Recordatorios { get; set; }
        public int VacunasProximas { get; set; }
        public int VacunasVencidas { get; set; }

        public IndexModel(
            IRepositorioMascota repoMascota,
            IRepositorioDueno repoDueno,
            IRepositorioVeterinario repoVeterinario,
            IRepositorioVisitaPyP repoVisitaPyP,
            IServicioRecordatorios servicioRecordatorios)
        {
            _repoMascota = repoMascota;
            _repoDueno = repoDueno;
            _repoVeterinario = repoVeterinario;
            _repoVisitaPyP = repoVisitaPyP;
            _servicioRecordatorios = servicioRecordatorios;
        }

        public IActionResult OnGet()
        {
            // Si no está autenticado, redirigir al sitio público.
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Publico/Portada");
            }

            var todas = _repoMascota.GetAllMascotas().ToList();
            TotalMascotas = todas.Count;
            TotalDuenos = _repoDueno.GetAllDuenos().Count();
            TotalVeterinarios = _repoVeterinario.GetAllVeterinarios().Count();
            TotalVisitas = _repoVisitaPyP.GetAllVisitasPyP().Count();

            DistribucionEspecies = todas
                .GroupBy(m => m.Especie ?? "Otra")
                .OrderByDescending(g => g.Count())
                .ToDictionary(g => g.Key, g => g.Count());

            DonutSvg = GenerarDonutSvg(DistribucionEspecies);
            LeyendaDonutHtml = GenerarLeyendaDonutHtml(DistribucionEspecies);

            // Recordatorios de vacunas (ventana de 30 días hacia adelante + vencidas).
            Recordatorios = _servicioRecordatorios.ObtenerRecordatorios(30).ToList();
            VacunasProximas = _servicioRecordatorios.ContarProximas(30);
            VacunasVencidas = _servicioRecordatorios.ContarVencidas();
            RecordatoriosHtml = GenerarRecordatoriosHtml(Recordatorios, VacunasProximas, VacunasVencidas);
            return Page();
        }

        /// <summary>
        /// Genera el SVG del donut como string HTML para evitar problemas de Razor
        /// con tags SVG que colisionan con tags especiales de Razor.
        /// </summary>
        private static string GenerarDonutSvg(Dictionary<string, int> distribucion)
        {
            if (distribucion == null || !distribucion.Any()) return "";

            var colores = new[] { "#0d7a5f", "#f59e0b", "#6b7280", "#f08c00", "#2f9e44" };
            var especies = distribucion.ToList();
            var totalDist = especies.Sum(e => e.Value);
            double cx = 60, cy = 60, r = 44, rInt = 28;
            double angulo = -90;
            var sb = new StringBuilder();

            sb.Append("<svg width=\"120\" height=\"120\" viewBox=\"0 0 120 120\" role=\"img\" aria-label=\"Donut de distribución de mascotas por especie\" style=\"display:block;\">");

            for (int i = 0; i < especies.Count; i++)
            {
                var esp = especies[i].Key;
                var cuenta = especies[i].Value;
                var porcion = totalDist > 0 ? cuenta / (double)totalDist * 360 : 0;
                var a0 = angulo * System.Math.PI / 180;
                var a1 = (angulo + porcion) * System.Math.PI / 180;
                var x0o = cx + r * System.Math.Cos(a0); var y0o = cy + r * System.Math.Sin(a0);
                var x1o = cx + r * System.Math.Cos(a1); var y1o = cy + r * System.Math.Sin(a1);
                var x0i = cx + rInt * System.Math.Cos(a1); var y0i = cy + rInt * System.Math.Sin(a1);
                var x1i = cx + rInt * System.Math.Cos(a0); var y1i = cy + rInt * System.Math.Sin(a0);
                var largeArc = porcion > 180 ? 1 : 0;
                var color = colores[i % colores.Length];
                var pct = (porcion / 360 * 100).ToString("0");
                sb.Append($"<path d=\"M {x0o:F1},{y0o:F1} A {r},{r} 0 {largeArc} 1 {x1o:F1},{y1o:F1} L {x0i:F1},{y0i:F1} A {rInt},{rInt} 0 {largeArc} 0 {x1i:F1},{y1i:F1} Z\" fill=\"{color}\" stroke=\"#ffffff\" stroke-width=\"1.5\"><title>{E(esp)}: {cuenta} ({pct}%)</title></path>");
                angulo += porcion;
            }

            sb.Append($"<text x=\"60\" y=\"56\" text-anchor=\"middle\" font-size=\"20\" font-weight=\"800\" fill=\"#355c43\">{totalDist}</text>");
            sb.Append("<text x=\"60\" y=\"72\" text-anchor=\"middle\" font-size=\"9\" fill=\"#6b7280\">mascotas</text>");
            sb.Append("</svg>");

            return sb.ToString();
        }

        /// <summary>
        /// Genera el HTML de la leyenda del donut como string para evitar
        /// problemas de Razor con bloques anidados.
        /// </summary>
        private static string GenerarLeyendaDonutHtml(Dictionary<string, int> distribucion)
        {
            if (distribucion == null || !distribucion.Any()) return "";

            var colores = new[] { "#0d7a5f", "#f59e0b", "#6b7280", "#f08c00", "#2f9e44" };
            var total = distribucion.Sum(x => x.Value);
            var sb = new StringBuilder();
            int i = 0;

            foreach (var kv in distribucion)
            {
                var pct = (kv.Value * 100.0 / total).ToString("0");
                var color = colores[i % colores.Length];
                sb.Append($"<li class=\"mf-leyenda-item\">");
                sb.Append($"<span class=\"mf-leyenda-punto\" style=\"background:{color};\" aria-hidden=\"true\"></span>");
                sb.Append($"<span>{E(kv.Key)}</span>");
                sb.Append($"<span class=\"mf-leyenda-valor\">{kv.Value} · {pct}%</span>");
                sb.Append($"</li>");
                i++;
            }

            return sb.ToString();
        }

        /// <summary>
        /// HTML-encode de un valor para prevenir XSS cuando se inserta en
        /// HTML generado manualmente con StringBuilder + @Html.Raw.
        /// </summary>
        private static string E(object valor)
        {
            return valor == null ? "" : WebUtility.HtmlEncode(valor.ToString());
        }

        /// <summary>
        /// Genera el HTML de la sección de recordatorios de vacunas.
        /// Todos los valores de usuario se HTML-encoden con E().
        /// </summary>
        private static string GenerarRecordatoriosHtml(IEnumerable<RecordatorioVacuna> recordatorios, int proximas, int vencidas)
        {
            if (recordatorios == null || !recordatorios.Any()) return "";

            var cultura = new System.Globalization.CultureInfo("es-CO");
            var sb = new StringBuilder();
            sb.Append("<section aria-labelledby=\"vacunas-titulo\" class=\"mf-ficha\" style=\"margin-top:1.5rem;\">");
            sb.Append("<div class=\"mf-ficha-titulo\">");
            sb.Append("<svg class=\"mf-ficha-ico\" xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\"><path d=\"M11 2v6h6M3 9l9-7 9 7M5 10v8a2 2 0 0 0 2 2h10a2 2 0 0 0 2-2v-8\"/></svg>");
            sb.Append("<h2 id=\"vacunas-titulo\" style=\"margin:0;\">Vacunas próximas a vencer</h2>");
            if (vencidas > 0) sb.Append($"<span class=\"mf-badge mf-badge--vencida\">{vencidas} vencida(s)</span>");
            if (proximas > 0) sb.Append($"<span class=\"mf-badge mf-badge--ambar\">{proximas} próxima(s)</span>");
            sb.Append("</div>");
            sb.Append("<div class=\"mf-lista-recordatorios\">");

            foreach (var rec in recordatorios)
            {
                var claseSev = rec.Severidad == "vencida" ? "mf-badge mf-badge--vencida"
                             : rec.Severidad == "proxima" ? "mf-badge mf-badge--ambar"
                             : "mf-badge mf-badge--exito";
                sb.Append($"<a class=\"mf-card mf-card--clickable mf-tarjeta-recordatorio\" href=\"/Mascotas/DetallesMascotas?mascotaId={rec.MascotaId}\">");
                sb.Append($"<span class=\"mf-tarjeta-recordatorio-ico mf-tarjeta-recordatorio-ico--{rec.Severidad}\" aria-hidden=\"true\">");
                sb.Append("<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"22\" height=\"22\" viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\"><path d=\"M9 11l3 3 8-8\"/><path d=\"M21 12v7a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h11\"/></svg>");
                sb.Append("</span>");
                sb.Append("<span style=\"flex:1;min-width:0;\">");
                sb.Append($"<strong>{E(rec.MascotaNombre)}</strong>");
                var fechaStr = rec.ProximaVacuna.ToString("d 'de' MMMM", cultura);
                sb.Append($"<div class=\"mf-muted\" style=\"font-size:.82rem;\">{E(rec.TipoProximaVacuna)} · {E(fechaStr)}</div>");
                sb.Append("</span>");
                sb.Append($"<span class=\"{claseSev}\">{E(rec.Etiqueta)}</span>");
                sb.Append("</a>");
            }

            sb.Append("</div>");
            sb.Append("</section>");
            return sb.ToString();
        }
    }
}
