using AgendaEmpleados.UI;
using AgendaEmpleados.Services;

namespace AgendaEmpleados
{
    internal static class Program
    {
        private static void Main()
        {
            Agenda agenda = new Agenda();
            Empresas empresas = new Empresas();
            new ConsolaMenuPrincipal(
                new ConsolaAgenda(agenda),
                new ConsolaEmpresas(empresas)).Ejecutar();
        }
    }
}