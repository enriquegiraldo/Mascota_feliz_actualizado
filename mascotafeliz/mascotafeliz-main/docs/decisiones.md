# Decisiones de diseño y producto — MascotaFeliz

> Registro de decisiones tomadas (lo pequeño se decide aquí; lo que cambia mucho el resultado se pregunta al usuario en la Fase 2).

## Decisiones ya tomadas (Fases 0–1)

- **Idioma y estilo:** todo en español (es-CO), PascalCase para tipos/props, camelCase para locales, según AGENTE.md.
- **Arquitectura:** mantener 4 capas; la lógica de negocio vive en Persistencia (repositorios) o una futura capa de Servicios, nunca en el Frontend.
- **Diagnóstico de bugs confirmados (a corregir en Fase 3):**
  - `UpdateMascota` sobrescribe `DuenoId/VeterinarioId/HistoriaId` con null porque el formulario no los envía.
  - `DetallesMascotas` rotula mal los campos y omite dueño/veterinario/historia.
  - Las 9 páginas instancian `new Repositorio(new AppContext())` en vez de usar DI.
  - `RedirectToPage("./NotFound")` desde subcarpetas y `UseExceptionHandler("/Error")` apuntan a rutas inexistentes (500/404).
  - Restos de plantilla: título "Privacy", pie "MascotaFeliz.App.Frontend".
- **Versión de runtime (preliminar, sujeto a respuesta del usuario):** .NET 9 es STS con fin de soporte 10-nov-2026; .NET 10 es LTS hasta nov-2028. Se recomienda migrar a .NET 10 LTS — pendiente confirmación.
- **Estilo visual base (preliminar):** tema propio con variables CSS sobre Bootstrap 5.3 (color modes) + mejora progresiva con htmx — pendiente confirmación.

## Pendientes de decisión del usuario (Fase 2)

- Alcance (sitio público + panel / solo panel / solo público).
- Base de UI (modernizar Razor + tema propio vs. reescribir SPA/Blazor).
- Versión de .NET (9 STS vs. 10 LTS).
- Funciones nuevas (Citas, foto, recordatorios, roles, PWA).
- Identidad (logo/colores/nombre; elección de dirección visual).
- Despliegue (¿se publica? afecta SQLite, HTTPS, almacenamiento de fotos).
