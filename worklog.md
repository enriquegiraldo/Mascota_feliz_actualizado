# Worklog — Proyecto MascotaFeliz (clínica veterinaria, .NET 9 + Razor Pages)

> Bitácora compartida por todos los agentes. Cada sección empieza con `---`.
> El proyecto vive en `/home/z/my-project/mascotafeliz/mascotafeliz-main`.

---
Task ID: 0
Agent: Z.ai Code (orquestador)
Task: Fase 0 — Diagnóstico del repositorio MascotaFeliz (leer AGENTE.md/README, `dotnet build`, recorrer páginas, resumir estado real en ≤10 líneas).

Work Log:
- Extraído `mascotafeliz-main.zip` a `/home/z/my-project/mascotafeliz/mascotafeliz-main`.
- Instalado .NET SDK 9.0.318 en `$HOME/.dotnet` (no existía en el entorno).
- Leídos AGENTE.md y README.md: confirman 4 capas (Dominio, Persistencia, Frontend, Consola), Razor Pages, EF Core sobre SQLite, UI Bootstrap 4.6 por CDN, sin wwwroot.
- `dotnet build MascotaFeliz.sln` → **0 errores, 0 warnings** (EF Core 8.0.0 sobre net9.0 compila; pendiente revisar coherencia de versión).
- Ejecutada app en http://localhost:5000 (Development): arranca, seed data OK (Firulais/Susi/Michi/Rocky; Juan Pérez en DetallesDuenos). Log avisa `WebRootPath was not found` → no hay wwwroot.
- Verificado en runtime: `GET /Error` → 404 (UseExceptionHandler roto); `GET /Mascotas/EditarMascota?mascotaId=9999` → 500 (el `RedirectToPage("./NotFound")` desde subcarpeta resuelve a `/Mascotas/NotFound`, inexistente).

Stage Summary (diagnóstico Fase 0, ≤10 líneas):
1. Compila limpio (0 errores/0 warnings); el binario corre y la BD SQLite se siembra bien (3 dueños, 3 vets, 3 historias, 4 mascotas).
2. **NO existe `wwwroot`**: 0 CSS/JS/imágenes propios; todo vía CDN (Bootstrap 4.6.2 + jQuery 3.6.4). El layout es plantilla por defecto.
3. **Bug edit mascota confirmado**: `EditarMascota.cshtml` no envía `DuenoId/VeterinarioId/HistoriaId`; `UpdateMascota` los sobrescribe con null → se pierden los vínculos al guardar.
4. **Etiquetas rotuladas mal en `DetallesMascotas`**: Color→"Apellido", Especie→"Direccion", Raza→"Telefono"; no muestra dueño, veterinario ni historia.
5. **Sin DI**: las 9 páginas hacen `new RepositorioX(new Persistencia.AppContext())` (cada request crea su propio DbContext, ignora el `AddDbContext` de Program.cs). El Consola idem.
6. **`Historia` y `VisitaPyP` no tienen páginas**; **no existe entidad Cita**. Las interfaces/repos de Historia y VisitaPyP sí existen (Add/Get/Update/GetAll/Delete).
7. Listas sin buscador (a pesar de `GetMascotasPorFiltro`), sin paginación, sin estado vacío, sin botón eliminar; formularios sin validación ni selectores de dueño/vet.
8. **Rutas rotas**: `RedirectToPage("./NotFound")` desde subcarpetas → 404/500; `UseExceptionHandler("/Error")` → 404 (no hay página Error).
9. Restos de plantilla: título "Privacy", pie "MascotaFeliz.App.Frontend".
10. Borrado en cascada: Mascota→Historia=Cascade, Historia→VisitaPyP=Cascade (al borrar mascota se borra su historia y visitas); Dueno/Vet→Mascota=Restrict (no se puede borrar dueño/vet con mascotas). Revisar si esa política es la deseada.

Decisiones de la Fase 0: no se modificó código (fase de diagnóstico). Se documentan hallazgos para discutir en Fase 2.

---
Task ID: 1
Agent: Z.ai Code (orquestador)
Task: Fase 1 — Investigación web de 10+ sitios de referencia (software veterinario, webs de mascotas, UX/UI, locales de Colombia, estándares) y entrega de docs/investigacion-ui.md.

Work Log:
- Verificada disponibilidad de web-search (skill). Ejecutadas 22 búsquedas con `z-ai function -n web_search` (Digitail, Shepherd, ezyVet, Provet/IDEXX, Instinct, Vetspire, Rover, Chewy, Farmer's Dog, Petco, Linear, Stripe, GOV.UK, Material 3, clínicas/tiendas Colombia-LatAm, WCAG 2.2, lifecycle .NET, Bootstrap 5.3, htmx+Razor, View Transitions, NN/g).
- Resultados en /home/z/my-project/research-cache/*.json (caché de búsqueda).
- Redactado docs/investigacion-ui.md con: tabla de 17 sitios (sitio · URL · qué hace bien · patrón a adoptar · qué evitar), 10 patrones a adoptar, 5 anti-patrones, estado del arte de versiones y 25 fuentes con fecha de consulta (2026-09-21).
- Hallazgo clave para Fase 2: .NET 9 STS termina soporte 10-nov-2026; .NET 10 LTS (hasta nov-2028). Bootstrap 5.3 ya soporta dark mode nativo. htmx+Razor y View Transitions son viables como mejora progresiva.

Stage Summary:
- docs/investigacion-ui.md entregado en /home/z/my-project/mascotafeliz/mascotafeliz-main/docs/.
- docs/decisiones.md creado con decisiones Fase 0–1 y pendientes de Fase 2.
- Listo para presentar Fase 2 (propuesta + 6 preguntas) y detenerse.
