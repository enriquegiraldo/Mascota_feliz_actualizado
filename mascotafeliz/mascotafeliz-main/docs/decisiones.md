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

## Remediación de Fase 0 (ronda de QA automática, 2026-09-22)

El usuario sigue en Fase 2 esperando respuestas, así que **no se inició la Fase 3 (rediseño)**. Se ejecutó la remediación de los bugs confirmados en Fase 0 sobre la UI existente, más detalles de estilo y features mínimas. Decisiones tomadas (lo pequeño):

- **DI**: los 5 repositorios se registran como `Scoped` en `Program.cs`; las 9 páginas usan inyección por constructor (fin de `new Repositorio(new AppContext())` en el Frontend). El Consola se deja como estaba (no hay DI en consola).
- **Bug UpdateMascota**: el formulario ahora envía `DuenoId` (selector) y `VeterinarioId` (selector); `HistoriaId` va en hidden y `UpdateMascota` **nunca lo sobrescribe con null** (solo lo actualiza si viene un valor). Ya no se pierden vínculos al editar.
- **DetallesMascotas**: etiquetas correctas (Nombre/Especie/Raza/Color) y nuevas secciones Dueño, Veterinario e Historia clínica con línea de tiempo de visitas PyP. `GetMascota` ahora incluye `Historia.VisitasPyP`.
- **Rutas**: `RedirectToPage("./NotFound")` → `RedirectToPage("/NotFound")` (absoluto) en las 6 páginas. Creada `Pages/Error.cshtml` para `UseExceptionHandler("/Error")`.
- **Restos de plantilla**: título "Privacy" → "Política de Privacidad"; pie "MascotaFeliz.App.Frontend" → "Mascota Feliz · Clínica veterinaria".
- **Identidad y estilo (preliminar, sujeto a Q5)**: creada `wwwroot` con `css/site.css` (tokens: verde salvia #4a7c59 + terracota #d97757 + crema #fbf7f0; tipografía fluida; radios, sombras), `js/site.js` (confirmar borrado, toasts, nav activa) y `img/logo.svg`/`favicon.svg` (huella, SVG propio, licencia libre). Footer pegado al fondo (flex). Bootstrap 4.6 se mantiene como base (no se cambia de versión mayor sin confirmar Q2).
- **Features mínimas (remedian huecos de Fase 0)**: buscador en vivo en las 3 listas (usa `GetXPorFiltro`), botón Eliminar con confirmación + toast, estados vacíos, paginación (8 por página), validación en cliente+servidor con `DataAnnotations` en las entidades, selectores de Dueño/Veterinario en EditarMascota, patrón PRG (redirección tras guardar).
- **Borrado en cascada**: se mantiene la configuración existente (Mascota→Historia=Cascade, Historia→VisitaPyP=Cascade; Dueno/Vet→Mascota=Restrict). El Eliminar de dueño/vet avisa si hay mascotas asociadas (captura `DbUpdateException`). Queda revisar con el usuario si esa política es la deseada.
- **Versiones**: NO se cambió .NET (sigue 9) ni EF Core (sigue 8.0.0) — pendiente Q3. Compila con 0 errores/0 warnings.
- **QA**: verificado en runtime — `/Error` 200 (era 404), `EditarMascota?id=9999` 302 (era 500), búsqueda 200, contenido correcto (etiquetas, vínculos, footer, privacy). Capturas en `/home/z/my-project/download/qa-despues/` a 390px y 1440px; VLM confirma look profesional, paleta cálida, sin scroll horizontal.
