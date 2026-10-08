using System;
using System.Collections.Generic;
using System.Linq;
using AgendaEmpleados.Models;
using AgendaEmpleados.Services;

namespace AgendaEmpleados.UI
{
    public class ConsolaEmpresas
    {
        private readonly Empresas empresas;

        public ConsolaEmpresas(Empresas empresas)
        {
            this.empresas = empresas;
        }

        public void Ejecutar()
        {
            bool volver = false;
            while (!volver)
            {
                MostrarMenu();
                switch (LeerOpcion())
                {
                    case 1: Alta(); break;
                    case 2: Listado(empresas.ObtenerActivas(), "LISTADO DE EMPRESAS"); break;
                    case 3: Busqueda(); break;
                    case 4: Modificar(); break;
                    case 5: Baja(); break;
                    case 6: Recuperar(); break;
                    case 0: volver = true; break;
                    default:
                        Console.WriteLine("\nOpcion no valida. Elige un numero del menu.");
                        Pausar();
                        break;
                }
            }
        }

        private static void MostrarMenu()
        {
            LimpiarPantalla();
            Console.WriteLine("========================================");
            Console.WriteLine("             MENU DE EMPRESAS");
            Console.WriteLine("========================================");
            Console.WriteLine("1. Alta de empresa");
            Console.WriteLine("2. Listado de empresas");
            Console.WriteLine("3. Buscar empresa");
            Console.WriteLine("4. Modificar empresa");
            Console.WriteLine("5. Dar de baja empresa");
            Console.WriteLine("6. Recuperar empresa dada de baja");
            Console.WriteLine("0. Volver al menu principal");
            Console.WriteLine("----------------------------------------");
        }

        private static int LeerOpcion()
        {
            Console.Write("Opcion: ");
            return int.TryParse(Console.ReadLine(), out int opcion) ? opcion : -1;
        }

        private void Alta()
        {
            LimpiarPantalla();
            Console.WriteLine("--- ALTA DE EMPRESA ---");
            string cif = LeerObligatorio("CIF: ");
            string nombre = LeerObligatorio("Nombre comercial: ");
            empresas.Agregar(cif, nombre);
            Console.WriteLine("\nEmpresa añadida correctamente.");
            Pausar();
        }

        private static void Listado(List<Empresa> lista, string titulo)
        {
            LimpiarPantalla();
            Console.WriteLine("--- " + titulo + " ---");
            if (lista.Count == 0) Console.WriteLine("No hay empresas para mostrar.");
            else MostrarFilas(lista);
            Pausar();
        }

        private void Busqueda()
        {
            LimpiarPantalla();
            Console.WriteLine("--- BUSQUEDA DE EMPRESAS ---");
            string texto = LeerObligatorio("Escribe el CIF o nombre comercial: ");
            List<Empresa> resultados = empresas.Buscar(texto);
            if (resultados.Count == 0) Console.WriteLine("No se encontraron coincidencias.");
            else MostrarFilas(resultados);
            Pausar();
        }

        private void Modificar()
        {
            Empresa empresa = LocalizarActiva("--- MODIFICAR EMPRESA ---");
            if (empresa == null) return;
            Console.WriteLine("Pulsa Enter para conservar el valor actual.");
            empresa.Cif = LeerEditableObligatorio("CIF", empresa.Cif);
            empresa.NombreComercial = LeerEditableObligatorio("Nombre comercial", empresa.NombreComercial);
            Console.WriteLine("\nEmpresa modificada correctamente.");
            Pausar();
        }

        private void Baja()
        {
            Empresa empresa = LocalizarActiva("--- BAJA DE EMPRESA ---");
            if (empresa == null) return;
            Console.Write("Confirma la baja (S/N): ");
            if ((Console.ReadLine() ?? string.Empty).Trim()
                .Equals("s", StringComparison.OrdinalIgnoreCase))
            {
                empresas.DarDeBaja(empresa);
                Console.WriteLine("Empresa dada de baja. Se conserva para poder recuperarla.");
            }
            else Console.WriteLine("Operacion cancelada.");
            Pausar();
        }

        private void Recuperar()
        {
            List<Empresa> inactivas = empresas.ObtenerInactivas();
            LimpiarPantalla();
            Console.WriteLine("--- RECUPERAR EMPRESA ---");
            if (inactivas.Count == 0)
            {
                Console.WriteLine("No hay empresas dadas de baja.");
                Pausar();
                return;
            }

            MostrarFilas(inactivas);
            int id = LeerEnteroPositivo("Introduce el ID de la empresa que quieres recuperar: ");
            Empresa elegida = inactivas.FirstOrDefault(empresa => empresa.Id == id);
            if (elegida == null)
            {
                Console.WriteLine("El ID no pertenece a una empresa dada de baja.");
            }
            else
            {
                empresas.Recuperar(elegida);
                Console.WriteLine("Empresa recuperada correctamente.");
            }
            Pausar();
        }

        private Empresa LocalizarActiva(string titulo)
        {
            LimpiarPantalla();
            Console.WriteLine(titulo);
            string texto = LeerObligatorio("Busca por CIF, nombre comercial o ID: ");
            List<Empresa> resultados;
            if (int.TryParse(texto, out int id))
            {
                Empresa empresa = empresas.ObtenerActivaPorId(id);
                resultados = empresa == null ? new List<Empresa>() : new List<Empresa> { empresa };
            }
            else resultados = empresas.Buscar(texto);

            if (resultados.Count == 0)
            {
                Console.WriteLine("No existe ninguna empresa activa con ese dato.");
                Pausar();
                return null;
            }
            MostrarFilas(resultados);
            if (resultados.Count == 1) return resultados[0];

            int idElegido = LeerEnteroPositivo("Introduce el ID exacto de la empresa: ");
            Empresa elegida = resultados.FirstOrDefault(empresa => empresa.Id == idElegido);
            if (elegida == null) Console.WriteLine("El ID no pertenece a los resultados.");
            return elegida;
        }

        private static void MostrarFilas(List<Empresa> lista)
        {
            Console.WriteLine("{0,-5} {1,-18} {2}", "ID", "CIF", "Nombre comercial");
            Console.WriteLine(new string('-', 55));
            foreach (Empresa empresa in lista)
            {
                Console.WriteLine("{0,-5} {1,-18} {2}", empresa.Id,
                    Ajustar(empresa.Cif, 18), empresa.NombreComercial);
            }
        }

        private static string LeerObligatorio(string mensaje)
        {
            while (true)
            {
                Console.Write(mensaje);
                string valor = (Console.ReadLine() ?? string.Empty).Trim();
                if (!string.IsNullOrWhiteSpace(valor)) return valor;
                Console.WriteLine("El dato no puede estar vacio.");
            }
        }

        private static string LeerEditableObligatorio(string etiqueta, string actual)
        {
            while (true)
            {
                Console.Write(etiqueta + " [" + actual + "]: ");
                string valor = (Console.ReadLine() ?? string.Empty).Trim();
                if (valor.Length == 0) return actual;
                if (!string.IsNullOrWhiteSpace(valor)) return valor;
                Console.WriteLine("El dato no puede estar vacio.");
            }
        }

        private static int LeerEnteroPositivo(string mensaje)
        {
            while (true)
            {
                Console.Write(mensaje);
                if (int.TryParse(Console.ReadLine(), out int valor) && valor > 0) return valor;
                Console.WriteLine("Introduce un ID entero positivo.");
            }
        }

        private static string Ajustar(string texto, int longitud)
        {
            return texto.Length <= longitud ? texto : texto.Substring(0, longitud - 3) + "...";
        }

        private static void Pausar()
        {
            Console.WriteLine("\nPulsa Enter para continuar...");
            Console.ReadLine();
        }

        private static void LimpiarPantalla()
        {
            if (!Console.IsOutputRedirected) Console.Clear();
        }
    }
}
