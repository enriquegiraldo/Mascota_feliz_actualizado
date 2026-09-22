# Fase 3 — Propuestas de dirección visual (3.0)

**Contexto:** Giraldo pidió descartar la paleta "Cálida clínica" (verde salvia + terracota + crema) y el logo de huella. Estas son 3 direcciones nuevas para que elija. Las tres se implementan como **tema propio sobre Bootstrap 5** (variables CSS + tokens), no como plantilla.

> **Nota técnica:** Las tipografías propuestas son todas **libres (OFL/SIL)** y se sirven desde Google Fonts o fontsource (autohospedable). Los iconos serán SVG propios o de Lucide (ISC). Nada de assets con licencia restrictiva.

---

## Dirección A — "Verde Cálido" (mi recomendada)

**Sensación:** Profesional y cercana, clínica moderna con calidez humana. Equilibra confianza (verde) con energía (ámbar).

| Token | Valor | Uso |
|-------|-------|-----|
| Primario | `#0d7a5f` (verde esmeralda profundo) | Botones, enlaces, acentos |
| Primario oscuro | `#095c47` | Hover, header |
| Acento | `#f59e0b` (ámbar) | CTAs, badges de urgencia |
| Fondo | `#f8faf9` (gris verdoso) | Body |
| Superficie | `#ffffff` | Tarjetas |
| Tinta | `#1a2f28` (casi negro verdoso) | Texto |
| Éxito | `#16a34a` | Estados positivos |
| Peligro | `#dc2626` | Eliminar, errores |

- **Tipografía:** **Inter** (UI, 400/600/700) + **Fraunces** (titulares display, opcional). Inter es el estándar de facto para interfaces modernas (usada por Linear, Vercel); Fraunces aporta personalidad cálida en titulares grandes.
- **Tono:** Confianza + calidez. Copy cercano pero profesional ("Agenda la cita de Firulais" no "Solicitar turno").
- **Logo:** Marca tipográfica "Mascota Feliz" en Inter 800 + un **icono SVG de estetoscopio simplificado** o **corazón+hoja** (símbolo de cuidado + vida). Sin huella.

**Por qué la recomiendo:** El verde esmeralda transmite salud/medicina (sin cliché del azul clínico), el ámbar da energía sin saturar, y la combinación es accesible (contraste AA en todos los pares). Funciona en modo claro y oscuro.

---

## Dirección B — "Azul Profesional"

**Sensación:** Limpia, institucional, seria. Estilo fintech/SaaS (Linear/Stripe). Más "clínica alta gama", menos "tienda de mascotas".

| Token | Valor | Uso |
|-------|-------|-----|
| Primario | `#1e40af` (azul índigo profundo) | Botones, enlaces |
| Primario oscuro | `#1e3a8a` | Hover |
| Acento | `#06b6d4` (cian) | CTAs secundarios, highlights |
| Fondo | `#f8fafc` | Body |
| Superficie | `#ffffff` | Tarjetas |
| Tinta | `#0f172a` | Texto |
| Éxito | `#16a34a` | |
| Peligro | `#dc2626` | |

- **Tipografía:** **Inter** (todo) + **Sora** (titulares). Sora es geométrica, moderna, un poco "tech".
- **Tono:** Profesional, directo, ligeramente formal ("Solicitar cita", "Gestión clínica").
- **Logo:** Marca tipográfica + icono de **cruz médica minimalista** o **escudo con cruz**.

**Nota:** El usuario indicó "evita índigo/azul salvo que lo pidas". Esta dirección **usa azul** — solo la elijo si Giraldo la pide explícitamente.

---

## Dirección C — "Naturaleza Vibrante"

**Sensación:** Fresca, optimista, enérgica. Pensada para destacar en redes sociales y atraer dueños jóvenes. Más arriesgada.

| Token | Valor | Uso |
|-------|-------|-----|
| Primario | `#059669` (verde esmeralda) | Botones |
| Acento | `#f97316` (naranja vibrante) | CTAs, highlights |
| Secundario | `#8b5cf6` (violeta suave) | Badges, estados especiales |
| Fondo | `#fefce8` (crema muy cálida) | Body |
| Superficie | `#ffffff` | Tarjetas |
| Tinta | `#1c1917` | Texto |
| Éxito | `#16a34a` | |
| Peligro | `#e11d48` (rosa intenso) | |

- **Tipografía:** **Plus Jakarta Sans** (UI, moderna y redondeada) + **Bricolage Grotesque** (titulares display, con personalidad). Ambas libres y muy actuales (2023-2024).
- **Tono:** Cercano, juguetón sin ser infantil ("¡Agenda con Firulais! 🐶" — emojis moderados). Microcopy con personalidad.
- **Logo:** Marca tipográfica + icono de **corazón redondo** o **patita abstracta geométrica** (no huella realista).

**Riesgo:** La combinación naranja+violeta puede saturar; requiere cuidado en densidad. Es la más distintiva pero la más difícil de mantener consistente.

---

## Comparación rápida

| Criterio | A: Verde Cálido | B: Azul Profesional | C: Naturaleza Vibrante |
|----------|----------------|--------------------|-----------------------|
| Sensación | Confianza cálida | Seria/institucional | Optimista/joven |
| Riesgo visual | Bajo | Bajo | Medio |
| Accesibilidad AA | ✓ fácil | ✓ fácil | ⚠ requiere cuidado |
| Modo oscuro | ✓ natural | ✓ natural | ⚠ más trabajo |
| Distintiva | Media | Baja (común) | Alta |
| Apta para App Service/producción | ✓ | ✓ | ✓ |

## Mi recomendación

**Dirección A ("Verde Cálido")**. Razones:
1. El verde esmeralda es **salud/medicina** sin caer en el cliché azul.
2. Es **accesible de fábrica** (contraste AA en todos los pares primarios).
3. El ámbar da energía para CTAs sin competir con el verde.
4. Funciona **igual de bien en claro y oscuro** (importante para el panel interno).
5. No repite la paleta descartada (era verde salvia más apagado + terracota; esta es verde esmeralda más profundo + ámbar).

## Pregunta para Giraldo

**¿Cuál dirección eliges: A, B o C?** (o si quieres un híbrido, dímelo).

Si eliges **A**, arranco enseguida con 3.2 (Bootstrap 5 + tema propio "Verde Cálido") sin tocar infraestructura.
