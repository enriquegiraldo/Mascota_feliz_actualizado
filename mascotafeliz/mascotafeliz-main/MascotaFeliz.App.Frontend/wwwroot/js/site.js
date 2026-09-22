/* Mascota Feliz — JS ligero de mejora progresiva.
   - Confirmación de borrado.
   - Toast de feedback.
   - Sin dependencias; todo degrada si no hay JS. */
(function () {
  "use strict";

  function confirmarBorrado(form) {
    var nombre = form.getAttribute("data-nombre") || "este registro";
    if (!window.confirm("¿Eliminar " + nombre + "? Esta acción no se puede deshacer.")) {
      return false;
    }
    mostrarToast("Registro eliminado.");
    return true;
  }

  function mostrarToast(mensaje) {
    var existente = document.querySelector(".mf-toast");
    if (existente) { existente.remove(); }
    var toast = document.createElement("div");
    toast.className = "mf-toast";
    toast.setAttribute("role", "status");
    toast.setAttribute("aria-live", "polite");
    toast.textContent = mensaje;
    document.body.appendChild(toast);
    requestAnimationFrame(function () { toast.classList.add("mf-show"); });
    setTimeout(function () {
      toast.classList.remove("mf-show");
      setTimeout(function () { toast.remove(); }, 250);
    }, 2600);
  }

  document.addEventListener("submit", function (e) {
    var form = e.target;
    if (form.getAttribute("data-mf-borrar") === "true") {
      if (!confirmarBorrado(form)) { e.preventDefault(); }
    }
  });

  // Marca el link de navegación activo según la ruta.
  document.addEventListener("DOMContentLoaded", function () {
    var ruta = window.location.pathname.toLowerCase();
    document.querySelectorAll(".mf-navbar .nav-link").forEach(function (link) {
      var href = (link.getAttribute("href") || "").toLowerCase();
      if (!href || href === "/") { return; }
      if (ruta.indexOf(href) === 0) { link.classList.add("active"); }
    });
  });

  // Expone el toast por si otras páginas quieren usarlo.
  window.MascotaFeliz = { mostrarToast: mostrarToast };
})();
