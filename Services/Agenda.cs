using System;
using System.Collections.Generic;
using System.Linq;
using AgendaEmpleados.Models;

namespace AgendaEmpleados.Services
{
    public class Agenda
    {
        private readonly List<Empleado> empleados = new List<Empleado>();
        private int siguienteId = 1;

        public Agenda()
        {
            AgregarEmpleado("Ana", "García López", "612345", "ana.garcia@ejemplo.com");
            AgregarEmpleado("Carlos", "Martín Ruiz", "698765", "carlos.martin@empresa.es");
        }

        public void AgregarEmpleado(string nombre, string apellidos, string telefono,
            string correo)
        {
            empleados.Add(new Empleado(siguienteId++, nombre, apellidos, telefono, correo));
        }

        public List<Empleado> ObtenerOrdenados()
        {
            return empleados.OrderBy(empleado => empleado.IdPersona).ToList();
        }

        public List<Empleado> Buscar(string texto)
        {
            return empleados.Where(empleado =>
                empleado.Nombre.Contains(texto, StringComparison.OrdinalIgnoreCase) ||
                empleado.Apellidos.Contains(texto, StringComparison.OrdinalIgnoreCase) ||
                empleado.Telefono.Contains(texto, StringComparison.OrdinalIgnoreCase) ||
                empleado.Correo.Contains(texto, StringComparison.OrdinalIgnoreCase))
                .OrderBy(empleado => empleado.IdPersona).ToList();
        }

        public Empleado ObtenerPorId(int id)
        {
            return empleados.FirstOrDefault(empleado => empleado.IdPersona == id);
        }

        public bool AsignarEmpresa(Empleado empleado, Empresa empresa)
        {
            if (empleado == null || empresa == null || !empresa.Activa ||
                !empleados.Contains(empleado) || empleado.Empresa != null)
                return false;

            empleado.Empresa = empresa;
            return true;
        }

        public bool DesvincularEmpresa(Empleado empleado)
        {
            if (empleado == null || !empleados.Contains(empleado) || empleado.Empresa == null)
                return false;

            empleado.Empresa = null;
            return true;
        }

        public List<Empleado> ObtenerDeEmpresa(Empresa empresa)
        {
            return empleados.Where(empleado => empleado.Empresa == empresa)
                .OrderBy(empleado => empleado.IdPersona).ToList();
        }

        public bool Eliminar(Empleado empleado)
        {
            return empleados.Remove(empleado);
        }
    }
}