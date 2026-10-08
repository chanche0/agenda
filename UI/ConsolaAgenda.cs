using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using AgendaEmpleados.Models;
using AgendaEmpleados.Services;

namespace AgendaEmpleados.UI
{
    public class ConsolaAgenda
    {
        private readonly Agenda agenda;
        private readonly Empresas empresas;

        public ConsolaAgenda(Agenda agenda, Empresas empresas)
        {
            this.agenda = agenda;
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
                    case 2: Listado(agenda.ObtenerOrdenados()); break;
                    case 3: Busqueda(); break;
                    case 4: Modificar(); break;
                    case 5: Baja(); break;
                    case 6: AsignarEmpresa(); break;
                    case 7: DesvincularEmpresa(); break;
                    case 8: VerEmpresa(); break;
                    case 0:
                        volver = true;
                        break;
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
            Console.WriteLine("          AGENDA DE EMPLEADOS");
            Console.WriteLine("========================================");
            Console.WriteLine("1. Alta de empleado");
            Console.WriteLine("2. Listado de empleados");
            Console.WriteLine("3. Buscar empleado");
            Console.WriteLine("4. Modificar empleado");
            Console.WriteLine("5. Dar de baja empleado");
            Console.WriteLine("6. Asignar empresa");
            Console.WriteLine("7. Desvincular empresa");
            Console.WriteLine("8. Ver empresa de una persona");
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
            Console.WriteLine("--- ALTA DE EMPLEADO ---");
            string nombre = LeerTextoObligatorio("Nombre: ");
            string apellidos = LeerTextoObligatorio("Apellidos: ");
            string telefono = LeerTelefono();
            string correo = LeerCorreo();
            agenda.AgregarEmpleado(nombre, apellidos, telefono, correo);
            Console.WriteLine("\nEmpleado añadido correctamente.");
            Pausar();
        }

        private static void Listado(List<Empleado> empleados)
        {
            LimpiarPantalla();
            Console.WriteLine("--- LISTADO DE EMPLEADOS ---");
            if (empleados.Count == 0) Console.WriteLine("No hay empleados para mostrar.");
            else MostrarFilas(empleados);
            Pausar();
        }

        private void Busqueda()
        {
            LimpiarPantalla();
            Console.WriteLine("--- BUSQUEDA ---");
            string texto = LeerTextoObligatorio("Escribe nombre, apellido, telefono o correo: ");
            List<Empleado> resultados = agenda.Buscar(texto);
            if (resultados.Count == 0) Console.WriteLine("No se encontraron coincidencias.");
            else MostrarFilas(resultados);
            Pausar();
        }

        private void Modificar()
        {
            Empleado empleado = LocalizarEmpleado("--- MODIFICAR EMPLEADO ---");
            if (empleado == null) return;
            Console.WriteLine("Pulsa Enter para conservar el valor actual.");
            empleado.Nombre = LeerTextoEditable("Nombre", empleado.Nombre);
            empleado.Apellidos = LeerTextoEditable("Apellidos", empleado.Apellidos);
            empleado.Telefono = LeerTelefono(empleado.Telefono);
            empleado.Correo = LeerCorreo(empleado.Correo);
            Console.WriteLine("\nEmpleado modificado correctamente.");
            Pausar();
        }

        private void Baja()
        {
            Empleado empleado = LocalizarEmpleado("--- BAJA DE EMPLEADO ---");
            if (empleado == null) return;
            Console.Write("Confirma la baja (S/N): ");
            if ((Console.ReadLine() ?? string.Empty).Trim().Equals("s", StringComparison.OrdinalIgnoreCase))
            {
                agenda.Eliminar(empleado);
                Console.WriteLine("Empleado eliminado correctamente.");
            }
            else Console.WriteLine("Operacion cancelada.");
            Pausar();
        }

        private void AsignarEmpresa()
        {
            do
            {
                AsignarEmpresaAEmpleado();
            }
            while (PreguntarSiContinuar("¿Quieres asignar una empresa a otra persona? (S/N): "));
        }

        private void AsignarEmpresaAEmpleado()
        {
            Empleado empleado = LocalizarEmpleado("--- ASIGNAR EMPRESA ---");
            if (empleado == null) return;
            if (empleado.Empresa != null)
            {
                Console.WriteLine("La persona ya pertenece a " + empleado.Empresa.NombreComercial +
                    ". Desvinculala primero para asignarle otra empresa.");
                Pausar();
                return;
            }

            List<Empresa> activas = empresas.ObtenerActivas();
            if (activas.Count == 0)
            {
                Console.WriteLine("No hay empresas activas para asignar.");
                Pausar();
                return;
            }

            Console.WriteLine("\nEmpresas disponibles:");
            MostrarEmpresas(activas);
            int idEmpresa = LeerEnteroPositivo("Introduce el ID de la empresa: ");
            Empresa empresa = empresas.ObtenerActivaPorId(idEmpresa);
            if (empresa == null)
            {
                Console.WriteLine("El ID no corresponde a una empresa activa.");
            }
            else if (agenda.AsignarEmpresa(empleado, empresa))
            {
                Console.WriteLine("Empresa asignada correctamente.");
            }
            else
            {
                Console.WriteLine("No se pudo asignar la empresa. Comprueba que la persona no tenga ya una.");
            }
            Pausar();
        }

        private void DesvincularEmpresa()
        {
            do
            {
                DesvincularEmpleado();
            }
            while (PreguntarSiContinuar("¿Quieres desvincular a otra persona? (S/N): "));
        }

        private void DesvincularEmpleado()
        {
            Empleado empleado = LocalizarEmpleado("--- DESVINCULAR PERSONA ---");
            if (empleado == null) return;
            if (empleado.Empresa == null)
            {
                Console.WriteLine("La persona no tiene ninguna empresa asignada.");
                Pausar();
                return;
            }

            Console.WriteLine("La persona está vinculada a " + empleado.Empresa.NombreComercial + ".");
            Console.Write("¿Quieres desvincularla? (S/N): ");
            if ((Console.ReadLine() ?? string.Empty).Trim()
                .Equals("s", StringComparison.OrdinalIgnoreCase))
            {
                agenda.DesvincularEmpresa(empleado);
                Console.WriteLine("Persona desvinculada. Sus datos se conservan.");
            }
            else Console.WriteLine("Operacion cancelada.");
            Pausar();
        }

        private static bool PreguntarSiContinuar(string mensaje)
        {
            Console.Write(mensaje);
            return (Console.ReadLine() ?? string.Empty).Trim()
                .Equals("s", StringComparison.OrdinalIgnoreCase);
        }

        private void VerEmpresa()
        {
            Empleado empleado = LocalizarEmpleado("--- EMPRESA DE UNA PERSONA ---");
            if (empleado == null) return;
            if (empleado.Empresa == null)
                Console.WriteLine("La persona no está vinculada a ninguna empresa.");
            else
            {
                Console.WriteLine("Empresa vinculada:");
                MostrarEmpresas(new List<Empresa> { empleado.Empresa });
            }
            Pausar();
        }

        private static void MostrarEmpresas(List<Empresa> lista)
        {
            Console.WriteLine("{0,-5} {1}", "ID", "Nombre comercial");
            Console.WriteLine(new string('-', 45));
            foreach (Empresa empresa in lista)
                Console.WriteLine("{0,-5} {1}", empresa.Id, empresa.NombreComercial);
        }

        private Empleado LocalizarEmpleado(string titulo)
        {
            LimpiarPantalla();
            Console.WriteLine(titulo);
            string texto = LeerTextoObligatorio("Busca por nombre, apellido o ID: ");
            List<Empleado> resultados;
            if (int.TryParse(texto, out int id))
            {
                Empleado empleado = agenda.ObtenerPorId(id);
                resultados = empleado == null ? new List<Empleado>() : new List<Empleado> { empleado };
            }
            else resultados = agenda.Buscar(texto);

            if (resultados.Count == 0)
            {
                Console.WriteLine("No existe ningun empleado con ese dato.");
                Pausar();
                return null;
            }
            MostrarFilas(resultados);
            if (resultados.Count == 1) return resultados[0];
            int idElegido = LeerEnteroPositivo("Introduce el ID exacto del empleado: ");
            Empleado elegido = resultados.FirstOrDefault(empleado => empleado.IdPersona == idElegido);
            if (elegido == null) Console.WriteLine("El ID no pertenece a los resultados.");
            return elegido;
        }

        private static void MostrarFilas(List<Empleado> empleados)
        {
            Console.WriteLine("{0,-4} {1,-22} {2,-10} {3}", "ID", "Nombre y apellidos", "Telefono", "Correo");
            Console.WriteLine(new string('-', 68));
            foreach (Empleado empleado in empleados)
            {
                Console.WriteLine("{0,-4} {1,-22} {2,-10} {3}", empleado.IdPersona,
                    Ajustar(empleado.Nombre + " " + empleado.Apellidos, 22), empleado.Telefono,
                    empleado.Correo);
            }
        }

        private static string LeerTextoObligatorio(string mensaje)
        {
            while (true)
            {
                Console.Write(mensaje);
                string valor = (Console.ReadLine() ?? string.Empty).Trim();
                if (!string.IsNullOrWhiteSpace(valor)) return valor;
                Console.WriteLine("El dato no puede estar vacio.");
            }
        }

        private static string LeerTelefono(string actual = null)
        {
            while (true)
            {
                Console.Write("Telefono" + (actual == null ? ": " : " [" + actual + "]: "));
                string valor = (Console.ReadLine() ?? string.Empty).Trim();
                if (actual != null && valor == string.Empty) return actual;
                if (Regex.IsMatch(valor, "^\\d{6}$")) return valor;
                Console.WriteLine("Telefono no valido: debe contener exactamente 6 digitos.");
            }
        }

        private static string LeerCorreo(string actual = null)
        {
            while (true)
            {
                Console.Write("Correo" + (actual == null ? ": " : " [" + actual + "]: "));
                string valor = (Console.ReadLine() ?? string.Empty).Trim();
                if (actual != null && valor == string.Empty) return actual;
                try
                {
                    MailAddress correo = new MailAddress(valor);
                    if (correo.Address == valor && valor.Contains("@") && valor.IndexOf('@') > 0) return valor;
                }
                catch (FormatException) { }
                Console.WriteLine("Correo no valido: debe tener un formato como nombre@dominio.com.");
            }
        }

        private static string LeerTextoEditable(string etiqueta, string actual)
        {
            Console.Write(etiqueta + " [" + actual + "]: ");
            string valor = (Console.ReadLine() ?? string.Empty).Trim();
            return valor == string.Empty ? actual : valor;
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