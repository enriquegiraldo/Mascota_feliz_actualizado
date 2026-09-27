/* Mascota Feliz — Command Palette (búsqueda global con Ctrl/Cmd+K).
   Sin dependencias. Degradación: si no hay JS, el botón no aparece
   (se oculta por CSS) y la navegación sigue funcionando. */
(function () {
  "use strict";

  var dialogo = document.getElementById("mf-cmdk");
  if (!dialogo) { return; }

  var input = document.getElementById("mf-cmdk-query");
  var lista = document.getElementById("mf-cmdk-lista");
  var botonAbrir = document.getElementById("mf-abrir-cmdk");
  var indiceActivo = -1;
  var items = [];
  var debounceId = null;

  function abrir() {
    dialogo.hidden = false;
    dialogo.setAttribute("aria-hidden", "false");
    document.body.style.overflow = "hidden";
    setTimeout(function () { input.focus(); }, 30);
  }

  function cerrar() {
    dialogo.hidden = true;
    dialogo.setAttribute("aria-hidden", "true");
    document.body.style.overflow = "";
    input.value = "";
    lista.innerHTML = "";
    items = [];
    indiceActivo = -1;
  }

  function escapar(s) {
    var d = document.createElement("div");
    d.textContent = s;
    return d.innerHTML;
  }

  function iconoSvg(tipo) {
    if (tipo === "Mascota") {
      return '<svg xmlns="http://www.w3.org/2000/svg" width="22" height="22" viewBox="0 0 24 24" fill="currentColor" aria-hidden="true"><path d="M4 10c0-3 2-5 5-5 1 0 2 1 3 1s2-1 3-1c3 0 5 2 5 5v3h-3v6H7v-6H4z"/></svg>';
    }
    if (tipo === "Dueño") {
      return '<svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"/><circle cx="12" cy="7" r="4"/></svg>';
    }
    if (tipo === "Historia") {
      return '<svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"/><polyline points="14 2 14 8 20 8"/><line x1="8" y1="13" x2="16" y2="13"/><line x1="8" y1="17" x2="16" y2="17"/></svg>';
    }
    if (tipo === "Visita") {
      return '<svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><rect x="3" y="4" width="18" height="18" rx="2" ry="2"/><line x1="16" y1="2" x2="16" y2="6"/><line x1="8" y1="2" x2="8" y2="6"/><line x1="3" y1="10" x2="21" y2="10"/></svg>';
    }
    if (tipo === "Cita") {
      return '<svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><path d="M9 11l3 3 8-8"/><path d="M21 12v7a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h11"/></svg>';
    }
    return '<svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"/><circle cx="9" cy="7" r="4"/><path d="M23 21v-2a4 4 0 0 0-3-3.87"/></svg>';
  }

  function render(resultados) {
    lista.innerHTML = "";
    items = [];
    indiceActivo = -1;

    if (!resultados.length) {
      lista.innerHTML = '<li class="mf-cmdk-vacio">No hay resultados. Prueba con otro término.</li>';
      return;
    }

    resultados.forEach(function (r, i) {
      var li = document.createElement("li");
      li.className = "mf-cmdk-item";
      li.setAttribute("role", "option");
      li.setAttribute("id", "mf-cmdk-item-" + i);
      li.setAttribute("data-url", r.url);
      li.innerHTML =
        '<span class="mf-cmdk-item-ico mf-cmdk-item-ico--' + r.tipo.toLowerCase() + '">' + iconoSvg(r.tipo) + "</span>" +
        '<span class="mf-cmdk-item-cuerpo">' +
          '<span class="mf-cmdk-item-titulo">' + escapar(r.titulo) + "</span>" +
          '<span class="mf-cmdk-item-sub">' + escapar(r.subtitulo || "") + "</span>" +
        "</span>" +
        '<span class="mf-cmdk-item-tipo">' + escapar(r.tipo) + "</span>";
      li.addEventListener("click", function () {
        window.location.href = r.url;
      });
      li.addEventListener("mouseenter", function () {
        activar(i);
      });
      lista.appendChild(li);
      items.push(li);
    });
    activar(0);
  }

  function activar(i) {
    if (i < 0 || i >= items.length) { return; }
    items.forEach(function (el, idx) {
      if (idx === i) {
        el.classList.add("mf-cmdk-item--activo");
        el.setAttribute("aria-selected", "true");
        el.scrollIntoView({ block: "nearest" });
      } else {
        el.classList.remove("mf-cmdk-item--activo");
        el.removeAttribute("aria-selected");
      }
    });
    indiceActivo = i;
  }

  function buscar(termino) {
    if (!termino || termino.trim().length < 1) {
      lista.innerHTML = "";
      items = [];
      return;
    }
    fetch("/Buscar?q=" + encodeURIComponent(termino.trim()), { headers: { "Accept": "application/json" } })
      .then(function (r) { return r.json(); })
      .then(function (data) { render(data.resultados || []); })
      .catch(function () { lista.innerHTML = '<li class="mf-cmdk-vacio">Error al buscar.</li>'; });
  }

  // Eventos.
  if (botonAbrir) {
    botonAbrir.addEventListener("click", abrir);
  }
  dialogo.addEventListener("click", function (e) {
    if (e.target.hasAttribute("data-mf-cmdk-cerrar")) { cerrar(); }
  });
  document.addEventListener("keydown", function (e) {
    // Ctrl+K o Cmd+K abre.
    if ((e.ctrlKey || e.metaKey) && (e.key === "k" || e.key === "K")) {
      e.preventDefault();
      if (dialogo.hidden) { abrir(); } else { cerrar(); }
      return;
    }
    if (dialogo.hidden) { return; }
    if (e.key === "Escape") { e.preventDefault(); cerrar(); }
    else if (e.key === "ArrowDown") {
      e.preventDefault();
      if (indiceActivo < items.length - 1) { activar(indiceActivo + 1); }
    } else if (e.key === "ArrowUp") {
      e.preventDefault();
      if (indiceActivo > 0) { activar(indiceActivo - 1); }
    } else if (e.key === "Enter") {
      e.preventDefault();
      if (indiceActivo >= 0 && items[indiceActivo]) {
        var url = items[indiceActivo].getAttribute("data-url");
        if (url) { window.location.href = url; }
        else { items[indiceActivo].click(); }
      }
    }
  });
  input.addEventListener("input", function () {
    clearTimeout(debounceId);
    debounceId = setTimeout(function () { buscar(input.value); }, 200);
  });

  // Mostrar el botón de búsqueda solo si hay JS (progresivo).
  if (botonAbrir) { botonAbrir.style.display = ""; }
})();
