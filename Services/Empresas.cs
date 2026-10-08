using System;
using System.Collections.Generic;
using System.Linq;
using AgendaEmpleados.Models;

namespace AgendaEmpleados.Services
{
    public class Empresas
    {
        private readonly List<Empresa> empresas = new List<Empresa>();
        private readonly Agenda agenda;
        private int siguienteId = 1;

        public Empresas(Agenda agenda)
        {
            this.agenda = agenda;
            Agregar("B12345678", "Tecnología Ibérica");
            Agregar("A87654321", "Servicios del Norte");
        }

        public void Agregar(string cif, string nombreComercial)
        {
            if (string.IsNullOrWhiteSpace(cif))
                throw new ArgumentException("El CIF no puede estar vacio.", nameof(cif));
            if (string.IsNullOrWhiteSpace(nombreComercial))
                throw new ArgumentException("El nombre comercial no puede estar vacio.", nameof(nombreComercial));

            empresas.Add(new Empresa(siguienteId++, cif, nombreComercial));
        }

        public List<Empresa> ObtenerActivas()
        {
            return empresas.Where(empresa => empresa.Activa)
                .OrderBy(empresa => empresa.Id).ToList();
        }

        public List<Empresa> Buscar(string texto)
        {
            return empresas.Where(empresa => empresa.Activa &&
                (empresa.Cif.Contains(texto, System.StringComparison.OrdinalIgnoreCase) ||
                 empresa.NombreComercial.Contains(texto, System.StringComparison.OrdinalIgnoreCase)))
                .OrderBy(empresa => empresa.Id).ToList();
        }

        public Empresa ObtenerActivaPorId(int id)
        {
            return empresas.FirstOrDefault(empresa => empresa.Id == id && empresa.Activa);
        }

        public List<Empleado> ObtenerEmpleados(Empresa empresa)
        {
            return agenda.ObtenerDeEmpresa(empresa);
        }

        public List<Empresa> ObtenerInactivas()
        {
            return empresas.Where(empresa => !empresa.Activa)
                .OrderBy(empresa => empresa.Id).ToList();
        }

        public bool DarDeBaja(Empresa empresa)
        {
            if (empresa == null || !empresa.Activa || agenda.ObtenerDeEmpresa(empresa).Count > 0)
                return false;
            empresa.Activa = false;
            return true;
        }

        public bool Recuperar(Empresa empresa)
        {
            if (empresa == null || empresa.Activa) return false;
            empresa.Activa = true;
            return true;
        }
    }
}
