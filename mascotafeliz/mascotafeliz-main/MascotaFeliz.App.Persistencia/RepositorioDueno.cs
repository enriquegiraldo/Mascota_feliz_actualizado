using MascotaFeliz.App.Dominio;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace MascotaFeliz.App.Persistencia
{
    public class RepositorioDueno : IRepositorioDueno
    {
        private readonly AppContext _appContext;

        public RepositorioDueno(AppContext appContext)
        {
            _appContext = appContext;
        }

        public Dueno AddDueno(Dueno dueno)
        {
            var duenoAdicionado = _appContext.Duenos.Add(dueno);
            _appContext.SaveChanges();
            return duenoAdicionado.Entity;
        }

        public Dueno GetDueno(int idDueno)
        {
            return _appContext.Duenos.Find(idDueno);
        }

        public Dueno UpdateDueno(Dueno dueno)
        {
            var duenoEncontrado = _appContext.Duenos.Find(dueno.Id);
            if (duenoEncontrado != null)
            {
                duenoEncontrado.Nombres = dueno.Nombres;
                duenoEncontrado.Apellidos = dueno.Apellidos;
                duenoEncontrado.Direccion = dueno.Direccion;
                duenoEncontrado.Telefono = dueno.Telefono;
                duenoEncontrado.Correo = dueno.Correo;
                _appContext.SaveChanges();
            }
            return duenoEncontrado;
        }

        public IEnumerable<Dueno> GetAllDuenos()
        {
            return _appContext.Duenos;
        }

        public IEnumerable<Dueno> GetDuenosPorFiltro(string filtro)
        {
            var f = Normalizador.Normalizar(filtro);
            return _appContext.Duenos
                .AsEnumerable()
                .Where(d => Normalizador.Normalizar(d.Nombres).Contains(f)
                         || Normalizador.Normalizar(d.Apellidos).Contains(f));
        }

        public void DeleteDueno(int idDueno)
        {
            var duenoEncontrado = _appContext.Duenos.Find(idDueno);
            if (duenoEncontrado == null)
                return;
            _appContext.Duenos.Remove(duenoEncontrado);
            _appContext.SaveChanges();
        }
    }
}
