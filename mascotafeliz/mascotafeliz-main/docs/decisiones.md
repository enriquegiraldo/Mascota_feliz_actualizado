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

---

# FASE 2 — CERRADA (decisiones confirmadas por Giraldo, 2026-09-22)

Las decisiones anteriores quedan resueltas así:

1. **Alcance:** sitio público (dueños) + panel interno (clínica).
2. **Interfaz:** **Bootstrap 5** con tema propio (no SPA/Blazor). Se reemplaza el CDN de Bootstrap 4.6 actual.
3. **Versión de .NET:** migrar los 4 proyectos a **.NET 10 (LTS)** y alinear EF Core a la versión 10.x correspondiente (hoy 8.0.0 sobre net9.0).
4. **Funcionalidades a construir ya:**
   - **Citas online** (entidad `Cita`: Id, FechaHora, MascotaId, VeterinarioId, Motivo, Estado [Pendiente/Confirmada/Cancelada/Atendida]). Vista pública (dueño solicita) + vista panel (clínica gestiona).
   - **Fotos de mascota** (una foto principal por Mascota, `FotoUrl`; subida desde `EditarMascota` con redimensionado).
   - **Recordatorios de vacunas** (primero visual: tarjeta "vacunas próximas a vencer" en Inicio, calculado desde `VisitaPyP`/`Historia`). El envío por email queda como mejora posterior — preguntar antes.
   - **Login con roles:** ASP.NET Core Identity, roles `Admin`, `Veterinario`, `Recepción`. Sitio público sin login; panel interno protegido con `[Authorize(Roles=...)]`. Semilla de un usuario Admin inicial.
5. **Identidad visual:** descartar la paleta "Cálida clínica" y el logo de huella. Proponer **2–3 direcciones nuevas** en la Fase 3 (paleta, tipografía, tono) para que Giraldo elija.
6. **Despliegue:** sí, a producción pronto, en **Azure** (Azure for Students o GitHub Student Developer Pack).

## Arquitectura de despliegue (Azure)

- **Cómputo:** Azure App Service (Linux, .NET 10), un slot de producción.
- **Base de datos:** migrar SQLite → **Azure SQL Database (Basic)** (preferida por ser la ruta más directa con EF Core en Azure). Cambio de proveedor en `AppContext` (`UseSqlite` → `UseSqlServer`) + migración EF limpia (`dotnet ef migrations add`). Ya no `EnsureCreated` en producción.
- **Fotos de mascota:** Azure Blob Storage (contenedor `mascotas-fotos`), nunca el disco de App Service. En dev: Azurite o carpeta `wwwroot/uploads` tras `.gitignore`.
- **Secretos:** connection strings y claves de Storage en configuración de App Service (o Key Vault si hay tiempo), nunca en `appsettings.json` versionado.
- **CI/CD:** GitHub Actions → build, test, `dotnet publish`, deploy a App Service.

## Fuera de alcance (parqueado)

- **Memorial digital / sección de recordatorio cuando fallece la mascota:** se decidió que es un producto aparte. **No construir.** Si aparece en el código o conversación, ignorar salvo instrucción explícita.

## Nota operativa

- Antes de escribir código de infraestructura (migración .NET 10, SQL Server, Blob Storage, Identity), confirmar el plan con Giraldo.

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

## Ronda 3 — Historia/VisitaPyP + KPIs + accesibilidad (cron, 2026-09-22)

- Construidas páginas de Historia (Lista + Detalles) y VisitaPyP (Lista + Detalles + Editar con selector de historia). Sembradas 3 VisitasPyP. KPIs en el inicio. Skip-link + focus-visible.
- AGENTE.md actualizado con la política de borrado y la tabla de localización.

## Ronda 4 — Cascade fix + CRUD Historia completo + selector historia + toasts (cron, 2026-09-22)

- **Bug corregido (pedido explícito del usuario: "revisa el borrado en cascada Mascota→Historia")**: cambiado `Mascota→Historia` de `Cascade` a `Restrict`. Antes, borrar una Historia BORRARÍA la Mascota asociada (pérdida de datos). Ahora es consistente con Dueno/Vet→Mascota (Restrict). Historia→VisitaPyP sigue Cascade (borrar historia borra sus visitas). Requiere borrar `MascotaFeliz.db` y re-crear.
- **Bug corregido (mismo patrón que el de mascota)**: `UpdateVisitaPyP` no copiaba `HistoriaId`; al editar una visita y cambiar el selector de historia, el cambio se ignoraba. Ahora se actualiza.
- **UpdateMascota**: ya permite desvincular la historia (selector con opción "sin historia"); antes preservaba el valor viejo si llegaba null.
- **CRUD Historia completo**: nueva página `EditarHistoria` (crear/editar/eliminar con confirmación). Botones "+ Nueva historia" en ListaHistorias, "Editar" en la tabla y en DetallesHistoria. DeleteHistoria captura `DbUpdateException` (si una mascota la referencia) y avisa.
- **Selector de Historia en EditarMascota**: reemplazado el hidden por un selector (opciones "Historia #N · fecha"). Así se vincula/desvincula mascota↔historia desde el formulario.
- **Toasts de éxito (microinteracción)**: todas las páginas de edición setean `TempData["Mensaje"]` y el layout lo muestra como toast verde al cargar la página siguiente (PRG). Verificado en runtime (editar dueño → toast "Dueño actualizado." + teléfono actualizado en la lista).
- **Pulido de estilo**: animación `mf-fade-up` sutil de entrada de tarjetas (respeta `prefers-reduced-motion`), estilos `.mf-aviso` (inline) y variantes de toast (`--error`, `--aviso`), bloques de error unificados con icono en las 4 listas.
- `dotnet build` 0/0. QA runtime: rutas nuevas 200, contenido verificado (selector Historia #1/#2/#3, EditarHistoria con Eliminar), toast verificado con agent-browser, VLM confirma selector de historia + toast verde. Capturas en `/home/z/my-project/download/qa-ronda4/`.
- AGENTE.md y README.md actualizados.
