namespace AgendaEmpleados.Models
{
    public class Empresa
    {
        public int Id { get; set; }
        public string Cif { get; set; }
        public string NombreComercial { get; set; }
        public bool Activa { get; set; }

        public Empresa(int id, string cif, string nombreComercial)
        {
            Id = id;
            Cif = cif;
            NombreComercial = nombreComercial;
            Activa = true;
        }
    }
}
