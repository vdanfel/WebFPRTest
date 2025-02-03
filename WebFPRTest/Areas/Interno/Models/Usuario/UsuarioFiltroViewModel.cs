namespace WebFPRTest.Areas.Interno.Models.Usuario
{
    public class UsuarioFiltroViewModel
    {
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public string Nombres { get; set; }
        public string Documento { get; set; }
        public string Correo { get; set; }

        public List<UsuarioViewModelTabla> ListaUsuarios { get; set; }

        public UsuarioFiltroViewModel()
        {
            ListaUsuarios = new List<UsuarioViewModelTabla>();
        }
    }
    public class UsuarioViewModelTabla
    {
        public int Indice { get; set; }  // Para numerar la tabla
        public int Id_Usuario { get; set; }
        public string ApellidosYNombre { get; set; }  // "Paterno Materno, Nombres"
        public string Usuario { get; set; }
        public string TipoUsuario { get; set; }
        public string NombreEquipo { get; set; }
    }
}
