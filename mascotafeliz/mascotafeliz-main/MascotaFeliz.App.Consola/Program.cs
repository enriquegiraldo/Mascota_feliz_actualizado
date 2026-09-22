using System;
using MascotaFeliz.App.Dominio;
using MascotaFeliz.App.Persistencia;
using System.Collections.Generic;

namespace MascotaFeliz.App.Consola
{
    class Program
    {

        private static IRepositorioDueno _repoDueno = new RepositorioDueno(new Persistencia.AppContext());
        private static IRepositorioVeterinario _repoVeterinario = new RepositorioVeterinario(new Persistencia.AppContext());
        private static IRepositorioMascota _repoMascota = new RepositorioMascota(new Persistencia.AppContext());
        //private static IRepositorioHistoria _repoHistoria = new RepositorioHistoria(new Persistencia.AppContext());
        //private static IRepositorioVisitaPyP _repoVisitaPyP = new RepositorioVisitaPyP(new Persistencia.AppContext());
        static void Main(string[] args)
        {
            Console.WriteLine("Hola amigos vamos a empezar a trabajar con las tablas");
                       
            //ListarDuenosFiltro();      
            //AddDueno();
            //BuscarDueno(5);

            //ListarVeterinariosFiltro();
            //AddVeterinario();
            //BuscarVeterinario(1);
            //AddHistoria();

                       
            //AddMascota();
            //BuscarMascota(2);
            //ListarMascotas();

        }

        private static void AddDueno()
        {
            var dueno = new Dueno
            {
                Nombres = "Maria",
                Apellidos = "Moreno", 
                Direccion = "Calle 121",
                Telefono = "123456",
                Correo = "maria@gmail.com"
            };
            _repoDueno.AddDueno(dueno);
        }

         private static void AddVeterinario()
        {
            var veterinario = new Veterinario
            {
                Nombres = "Jaime",
                Apellidos = "Duque", 
                Direccion = "Transversal 5 # 17A-100",
                Telefono = "1234577",
                TarjetaProfesional = "TP0003"
            };
            _repoVeterinario.AddVeterinario(veterinario);
        }

        private static void AddMascota()
        {
            var mascota = new Mascota
            {
                Nombre = "susi",
                Color = "Blanca", 
                Especie = "perro",
                Raza = "chihuahua",
                
            };
            _repoMascota.AddMascota(mascota);
        }


        private static void AddHistoria()
        {
            var historia = new Historia
            {
                FechaInicial = new DateTime(1990, 04, 12)
                
            };
           // _repoHistoria.AddHistoria(historia);
        }


        private static void BuscarDueno(int idDueno)
        {
            var dueno = _repoDueno.GetDueno(idDueno);
            Console.WriteLine(dueno.Nombres + " " + dueno.Apellidos+" "+dueno.Direccion+" "+dueno.Telefono+" "+dueno.Correo);
        }

        private static void BuscarVeterinario(int idVeterinario)
        {
            var veterinario = _repoVeterinario.GetVeterinario(idVeterinario);
            Console.WriteLine(veterinario.Nombres + " " + veterinario.Apellidos);
        }

        private static void BuscarMascota(int idMascota)
        {
            var mascota = _repoMascota.GetMascota(idMascota);
            Console.WriteLine(mascota.Nombre + " " + mascota.Color+" "+mascota.Especie+" "+mascota.Raza+" "+mascota.Veterinario);
        }

        private static void ListarDuenosFiltro()
        {
            var duenosM = _repoDueno.GetDuenosPorFiltro("nar");
            foreach (Dueno p in duenosM)
            {
                Console.WriteLine(p.Nombres + " " + p.Apellidos);
            }

        }
      


        private static void ListarVeterinariosFiltro()
        {
            var veterinariosM = _repoVeterinario.GetVeterinariosPorFiltro("e");
            foreach (Veterinario p in veterinariosM)
            {
                Console.WriteLine(p.Nombres + " " + p.Apellidos);
            }

        }

        private static void ListarMascotas()
        {
            var mascotas = _repoMascota.GetAllMascotas();
            foreach (Mascota p in mascotas)
            {
                Console.WriteLine(p.Nombre + " " + p.Raza);
            }

        }

        
        
    }
}