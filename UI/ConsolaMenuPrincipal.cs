using System;

namespace AgendaEmpleados.UI
{
    public class ConsolaMenuPrincipal
    {
        private readonly ConsolaAgenda menuEmpleados;
        private readonly ConsolaEmpresas menuEmpresas;

        public ConsolaMenuPrincipal(ConsolaAgenda menuEmpleados, ConsolaEmpresas menuEmpresas)
        {
            this.menuEmpleados = menuEmpleados;
            this.menuEmpresas = menuEmpresas;
        }

        public void Ejecutar()
        {
            bool salir = false;
            while (!salir)
            {
                if (!Console.IsOutputRedirected) Console.Clear();
                Console.WriteLine("========================================");
                Console.WriteLine("             MENU PRINCIPAL");
                Console.WriteLine("========================================");
                Console.WriteLine("1. Menu de empleados");
                Console.WriteLine("2. Menu de empresas");
                Console.WriteLine("0. Salir");
                Console.WriteLine("----------------------------------------");
                Console.Write("Opcion: ");

                switch (int.TryParse(Console.ReadLine(), out int opcion) ? opcion : -1)
                {
                    case 1:
                        menuEmpleados.Ejecutar();
                        break;
                    case 2:
                        menuEmpresas.Ejecutar();
                        break;
                    case 0:
                        salir = true;
                        Console.WriteLine("\nPrograma finalizado. Hasta pronto.");
                        break;
                    default:
                        Console.WriteLine("\nOpcion no valida. Elige un numero del menu.");
                        Console.WriteLine("Pulsa Enter para continuar...");
                        Console.ReadLine();
                        break;
                }
            }
        }
    }
}
