using MascotaFeliz.App.Dominio;
using MascotaFeliz.App.Persistencia;

namespace MascotaFeliz.App.Frontend.Servicios
{
    /// <summary>
    /// DTO con los datos de un recordatorio de vacuna para mostrar en el panel.
    /// </summary>
    public class RecordatorioVacuna
    {
        public int VisitaId { get; set; }
        public int MascotaId { get; set; }
        public string MascotaNombre { get; set; }
        public string Especie { get; set; }
        public DateTime ProximaVacuna { get; set; }
        public string TipoProximaVacuna { get; set; }

        /// <summary>Días restantes hasta la próxima vacuna (negativo si ya venció).</summary>
        public int DiasRestantes => (int)(ProximaVacuna.Date - DateTime.Today).TotalDays;

        /// <summary>"Vence hoy", "Vence en 7 días", "Vencida hace 3 días", etc.</summary>
        public string Etiqueta
        {
            get
            {
                var d = DiasRestantes;
                if (d < 0) return "Vencida hace " + (-d) + " día(s)";
                if (d == 0) return "Vence hoy";
                if (d == 1) return "Vence mañana";
                return "Vence en " + d + " días";
            }
        }

        /// <summary>Estado para el badge: "vencida", "proxima", "futura".</summary>
        public string Severidad => DiasRestantes < 0 ? "vencida" : DiasRestantes <= 7 ? "proxima" : "futura";
    }

    /// <summary>
    /// Servicio que calcula recordatorios de vacunas próximas a vencer
    /// a partir del campo VisitaPyP.ProximaVacuna. Sin envío de email
    /// (eso queda como mejora posterior).
    /// </summary>
    public interface IServicioRecordatorios
    {
        /// <summary>Devuelve las vacunas próximas a vencer (ordenadas por fecha).</summary>
        /// <param name="diasVentana">Días hacia adelante a considerar (por defecto 30).</param>
        IEnumerable<RecordatorioVacuna> ObtenerRecordatorios(int diasVentana = 30);

        /// <summary>Cuenta cuántas vacunas vencen en los próximos N días.</summary>
        int ContarProximas(int diasVentana = 30);

        /// <summary>Cuenta cuántas vacunas ya están vencidas.</summary>
        int ContarVencidas();
    }

    public class ServicioRecordatorios : IServicioRecordatorios
    {
        private readonly IRepositorioVisitaPyP _repoVisita;
        private readonly IRepositorioMascota _repoMascota;

        public ServicioRecordatorios(IRepositorioVisitaPyP repoVisita, IRepositorioMascota repoMascota)
        {
            _repoVisita = repoVisita;
            _repoMascota = repoMascota;
        }

        public IEnumerable<RecordatorioVacuna> ObtenerRecordatorios(int diasVentana = 30)
        {
            var hoy = DateTime.Today;
            var limite = hoy.AddDays(diasVentana);

            // Tomar todas las visitas con ProximaVacuna dentro de la ventana
            // (incluye vencidas y futuras próximas).
            var visitas = _repoVisita.GetAllVisitasPyP()
                .Where(v => v.ProximaVacuna.HasValue && v.ProximaVacuna.Value <= limite)
                .OrderBy(v => v.ProximaVacuna)
                .ToList();

            var recordatorios = new List<RecordatorioVacuna>();
            foreach (var v in visitas)
            {
                var mascota = _repoMascota.GetMascotaPorHistoriaId(v.HistoriaId);
                if (mascota == null) continue;

                recordatorios.Add(new RecordatorioVacuna
                {
                    VisitaId = v.Id,
                    MascotaId = mascota.Id,
                    MascotaNombre = mascota.Nombre,
                    Especie = mascota.Especie,
                    ProximaVacuna = v.ProximaVacuna.Value,
                    TipoProximaVacuna = v.TipoProximaVacuna ?? "Vacuna"
                });
            }

            return recordatorios;
        }

        public int ContarProximas(int diasVentana = 30)
        {
            return ObtenerRecordatorios(diasVentana).Count(r => r.DiasRestantes >= 0);
        }

        public int ContarVencidas()
        {
            return ObtenerRecordatorios(365).Count(r => r.DiasRestantes < 0);
        }
    }
}
