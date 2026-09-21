# Investigación de UI/UX — MascotaFeliz

**Fecha de consulta de las fuentes:** 2026-09-21
**Autor:** Z.ai Code (agente)
**Propósito:** Recoger patrones reales de software veterinario, webs de mascotas y referencias de producto/UX para diseñar la nueva MascotaFeliz. *Inspiración en patrones; no se copia código, textos ni imágenes de terceros.*

---

## 1. Tabla de sitios analizados

| # | Sitio | URL | Qué hace bien | Patrón a adoptar | Qué evitar |
|---|------|-----|---------------|------------------|------------|
| 1 | Digitail | digitail.com / reviews en capterra.com, g2.com | App para dueños (Pet Parent App), dictado SOAP con IA, recordatorios automáticos, captura de cobros en tiempo real | Portal del cliente + recordatorios automáticos + flujo de visita guiado (SOAP) | Sobrecargar el formulario de visita con campos que el usuario rara vez llena; agrupar por sección |
| 2 | Shepherd | shepherd.vet | Navegación intuitiva, SOAP como flujo, automatizaciones, multi-sede | Flujo de visita en pasos (más allá de un CRUD) y panel multi-sede/lista | Tablas planas sin contexto del paciente |
| 3 | ezyVet (IDEXX) | ezyvet.com | SMS bidireccional para confirmar citas, portal del cliente con agenda en línea y pago | Agenda en línea + confirmación por SMS/WhatsApp | Pedir datos clínicos sensibles en formularios largos sin progreso |
| 4 | Provet / IDEXX Neo | provet.com, idexx.com | Agenda automatizada, recordatorios, panel "360°" del hospital, resúmenes con IA | Dashboard de resumen con KPIs (mascotas, próximas visitas) | Métricas sin contexto accionable |
| 5 | Instinct Science | instinct.vet | Notas SOAP con IA (ScribbleVet), automatización de seguimientos, foco en alta complejidad | Autocompletar campos repetitivos y plantillas de recomendaciones | Comportamiento que parece "mágico" sin explicar al usuario qué se guardó |
| 6 | Vetspire | vetspire.ai | Pizarra electrónica (NOVA) de hojas de tratamiento, recordatorios automáticos, todo en uno | "Línea de tiempo" de la mascota (visitas con signos vitales) | Lista de visitas sin orden ni datos clínicos visibles de un vistazo |
| 7 | Rover | rover.com | Conexión dueño⇄cuidador, pago seguro, prueba social masiva (200k+ sitters) | Prueba social y confianza (reseñas, equipo visible) | Oscurecer el precio o el siguiente paso |
| 8 | Chewy | chewy.com | Farmacia de confianza, categorización clara, descubrimiento alto, política 100% satisfacción | Categorías claras (perro/gato/etc.) y garantía visible | Menús saturados sin jerarquía |
| 9 | The Farmer's Dog | thefarmersdog.com | Hero orientado a conversión, narrativa de confianza (alimento veterinario), CTA único y claro | Hero con propuesta de valor + CTA único ("Agendar cita") | Varios CTAs compitiendo en el hero |
| 10 | Petco | petco.com | Servicios (grooming, vacunas), "7-Point Pet Care Check" como diferenciador | Lista de servicios con iconos y precio/beneficio | Mezclar tienda y servicios sin separar la intención del usuario |
| 11 | Linear | linear.app | Modo oscuro first, *command palette* (Cmd+K), teclado, alta densidad sin ruido visual | Búsqueda global (Cmd+K) en el panel interno | Densidad que sacrifique objetivos táctiles ≥44px en móvil |
| 12 | Stripe | stripe.com, docs.stripe.com | Formularios accesibles, consistencia de UI, "high accessibility bar" por diseño | Validación de formularios en cliente + servidor, estados claros | Mensajes de error genéricos ("algo salió mal") |
| 13 | GOV.UK Design System | design-system.service.gov.uk | Patrones de formularios accesibles, alineación con WCAG 2.2 | Etiquetas asociadas, mensajes de error específicos en español, foco visible | Placeholders como única etiqueta |
| 14 | Material Design 3 | m3.material.io | Color dinámico, roles de color, esquemas accesibles | Sistema de tokens (color, tipografía, forma) con modo claro/oscuro | Paletas planas sin jerarquía semántica |
| 15 | Clínica Veterinaria Vetas (Bogotá) | clinicavetas.agendapro.com | Agenda en línea integrada, dirección y horarios visibles | Agenda en línea + datos de contacto y horarios en el sitio público | Forzar llamada/WhatsApp como único canal de cita |
| 16 | AnimalesBog (Distrito, Bogotá) | animalesbog.gov.co | Telesalud veterinaria, sección "Te Ayudamos" con paso a paso | Sección de ayuda paso a paso y urgencias clara | Lenguaje burocrático |
| 17 | Marketing Pet (LatAm) | marketingpet.cl | Diseño 100% personalizado, CTA "comprar/contactar por WhatsApp" | Botón flotante de WhatsApp como canal primario regional | WhatsApp sin mensaje prellenado |

> **Vigencia verificada:** los 6 productos veterinarios aparecen en resultados con fechas de 2025–2026 (reseñas, lanzamientos de IA), por lo que están operativos. Rover/Chewy/Petco/Farmer's Dog son marcas activas. Vetas y AnimalesBog activos en Bogotá.

---

## 2. Diez patrones a adoptar

1. **Hero con propuesta de valor + un solo CTA** (estilo Farmer's Dog): la portada pública debe responder en 3 segundos "qué es, para quién y qué hacer ahora". CTA principal: *Agendar cita*.
2. **Portal del cliente (Pet Parent App / ezyVet):** el dueño debe poder ver a su mascota, su historial y pedir cita sin hablar con nadie. Aunque en MascotaFeliz sea un panel simple, reservar una vista "perfil de mascota" accesible y un flujo "pedir cita".
3. **Línea de tiempo de la mascota (Vetspire NOVA):** cada visita PyP se ve como una tarjeta con fecha, signos vitales y recomendaciones; gráfico de peso/tendencia. Es el "corazón clínico".
4. **Búsqueda global (Linear Cmd+K):** en el panel interno, un buscador que encuentre mascotas, dueños y veterinarios por nombre/documento, accesible por teclado.
5. **Lista con búsqueda en vivo, filtros y paginación** (Stripe/Linear): sin recarga completa, con estados vacíos claros ("Aún no hay mascotas — registrar la primera").
6. **Formularios accesibles y validados (GOV.UK + WCAG 2.2):** etiqueta siempre asociada al campo, error específico en español, foco visible ≥2px, objetivo táctil ≥44px, validación en cliente + servidor, antiforgery.
7. **Recordatorios y confirmaciones por WhatsApp/SMS** (ezyVet, contexto Colombia): canal primario regional; mensaje prellenado y confirmación de cita.
8. **Dashboard de resumen (Provet 360°):** tarjetas de KPIs (mascotas activas, próximas visitas, vacunas pendientes) con accesos rápidos.
9. **Sistema de tokens con modo claro/oscuro (Material 3 + Bootstrap 5.3 color modes):** variables CSS para color, tipografía fluida, espaciado, radios y sombras; identidad propia, no "Bootstrap por defecto".
10. **Mejora progresiva con htmx + View Transitions:** interacción sin recarga (búsqueda en vivo, toasts) y transición suave entre páginas `@view-transition { navigation: auto; }`, con fallback si no hay JS — todo sigue funcionando.

---

## 3. Cinco anti-patrones a evitar

1. **CRUD de plantilla:** tablas sin contexto, sin dueño/veterinario visible, sin estado vacío ni paginación (como el `ListaMascotas` actual).
2. **Etiquetas rotuladas mal / campos sin relación:** `DetallesMascotas` muestra "Apellido/Dirección/Teléfono" para Color/Especie/Raza y omite dueño/vet/historia. *Cero copy pegada de otra entidad.*
3. **`RedirectToPage("./NotFound")` relativo desde subcarpetas** y `UseExceptionHandler("/Error")` sin página: rutas rotas que devuelven 500/404 en producción.
4. **Instanciar `new Repositorio(new AppContext())` en cada página:** ignora la DI, crea un DbContext por request y desacopla mal la arquitectura; viola "nada de lógica de negocio en el Frontend".
5. **Iconos, fuentes o imágenes sin licencia documentada:** usar assets libres (o SVG propios) y dejarlo registrado; nada de CDNs de jQuery por defecto si no se necesita.

---

## 4. Estado del arte y versiones (consultado 2026-09-21)

- **.NET:** .NET 9 es STS, **fin de soporte 10 nov 2026** (≈1,5 meses). **.NET 10 es LTS** (lanzado 11 nov 2025), soporte hasta 14 nov 2028. Recomendación: migrar a .NET 10 LTS. Fuente: dotnet.microsoft.com (política oficial), endoflife.date, learn.microsoft.com.
- **EF Core:** el proyecto usa 8.0.0 sobre net9.0 (compila, pero conviene alinear la versión con el runtime elegido).
- **Bootstrap:** 5.3 soporta *color modes* (modo oscuro) vía variables CSS y mixin `color-mode()`. Mantenerlo como base y poner un tema propio encima es viable. Fuente: getbootstrap.com, blog.getbootstrap.com.
- **htmx + Razor Pages:** patrón documentado y vigente para mejora progresiva sin SPA (woodruff.dev 2025, htmx-workshop.com, dev.to 2026).
- **View Transitions API:** cross-document `@view-transition { navigation: auto; }` soportado en Chromium 111+; degrada con elegancia en otros navegadores. Fuente: developer.mozilla.org, developer.chrome.com, css-tricks.com (2026).
- **WCAG 2.2:** vigente desde oct-2023, añade 9 criterios nuevos (p. ej. *Focus Not Obscured*, *Focus Appearance*, *Target Size mínimo 24×24px*, *Dragging Movements*). Objetivo AA. Fuente: w3.org, access-board.gov, nngroup.com.

---

## 5. Fuentes

| Fuente | URL | Fecha consulta |
|--------|-----|----------------|
| Capterra – Digitail | https://www.capterra.com | 2026-09-21 |
| G2 – Digitail | https://www.g2.com | 2026-09-21 |
| Shepherd | https://www.shepherd.vet | 2026-09-21 |
| ezyVet | https://www.ezyvet.com | 2026-09-21 |
| IDEXX / Provet | https://www.idexx.com , https://www.provet.com | 2026-09-21 |
| Instinct Science | https://instinct.vet | 2026-09-21 |
| Vetspire | https://www.vetspire.ai | 2026-09-21 |
| Rover | https://www.rover.com | 2026-09-21 |
| Chewy | https://www.chewy.com | 2026-09-21 |
| The Farmer's Dog | https://www.thefarmersdog.com | 2026-09-21 |
| Petco | https://www.petco.com | 2026-09-21 |
| Linear | https://linear.app | 2026-09-21 |
| Stripe (docs) | https://stripe.com , https://docs.stripe.com | 2026-09-21 |
| GOV.UK Design System | https://design-system.service.gov.uk | 2026-09-21 |
| Material Design 3 | https://m3.material.io | 2026-09-21 |
| Clínica Veterinaria Vetas (Bogotá) | https://clinicavetas.agendapro.com | 2026-09-21 |
| AnimalesBog (Bogotá) | https://www.animalesbog.gov.co | 2026-09-21 |
| Marketing Pet (LatAm) | https://marketingpet.cl | 2026-09-21 |
| W3C WCAG 2.2 | https://www.w3.org | 2026-09-21 |
| MDN – View Transitions API | https://developer.mozilla.org | 2026-09-21 |
| Bootstrap 5.3 color modes | https://getbootstrap.com | 2026-09-21 |
| .NET support policy | https://dotnet.microsoft.com , https://endoflife.date | 2026-09-21 |
| NN/g – Trustworthiness | https://www.nngroup.com | 2026-09-21 |
| htmx + Razor Pages (Woodruff) | https://www.woodruff.dev | 2026-09-21 |
| htmx workshop | https://htmx-workshop.com | 2026-09-21 |
