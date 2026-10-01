namespace AgendaEmpleados.Models
{
    public class Empleado
    {
        public int IdPersona { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string Empresa { get; set; }

        public Empleado(int idPersona, string nombre, string apellidos,
            string telefono, string correo, string empresa)
        {
            IdPersona = idPersona;
            Nombre = nombre;
            Apellidos = apellidos;
            Telefono = telefono;
            Correo = correo;
            Empresa = empresa;
        }
    }
}