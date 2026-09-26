using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MascotaFeliz.App.Dominio;
using MascotaFeliz.App.Persistencia;

namespace MascotaFeliz.App.Frontend.Pages.Publico
{
    [AllowAnonymous]
    public class PortadaModel : PageModel
    {
        private readonly IRepositorioVeterinario _repoVet;
        private readonly IRepositorioMascota _repoMascota;

        public int TotalMascotas { get; set; }
        public int TotalVeterinarios { get; set; }
        public string HtmlEquipo { get; set; }

        public PortadaModel(IRepositorioVeterinario repoVet, IRepositorioMascota repoMascota)
        {
            _repoVet = repoVet;
            _repoMascota = repoMascota;
        }

        public void OnGet()
        {
            TotalMascotas = _repoMascota.GetAllMascotas().Count();
            TotalVeterinarios = _repoVet.GetAllVeterinarios().Count();

            var vets = _repoVet.GetAllVeterinarios().Take(4).ToList();
            HtmlEquipo = GenerarHtmlEquipo(vets);
        }

        private static string GenerarHtmlEquipo(List<Veterinario> vets)
        {
            if (!vets.Any()) return "";

            var sb = new System.Text.StringBuilder();
            foreach (var v in vets)
            {
                var nombreCompleto = System.Net.WebUtility.HtmlEncode(v.Nombres + " " + v.Apellidos);
                var tarjeta = System.Net.WebUtility.HtmlEncode(v.TarjetaProfesional ?? "");
                sb.Append("<div class=\"mf-equipo-card\">");
                sb.Append("<div class=\"mf-equipo-avatar\" aria-hidden=\"true\">");
                sb.Append("<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"36\" height=\"36\" viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\"><path d=\"M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2\"/><circle cx=\"9\" cy=\"7\" r=\"4\"/><path d=\"M23 21v-2a4 4 0 0 0-3-3.87\"/><path d=\"M16 3.13a4 4 0 0 1 0 7.75\"/></svg>");
                sb.Append("</div>");
                sb.Append($"<h3>{nombreCompleto}</h3>");
                sb.Append($"<p class=\"mf-muted\">Tarjeta profesional: {tarjeta}</p>");
                sb.Append("</div>");
            }
            return sb.ToString();
        }
    }
}
