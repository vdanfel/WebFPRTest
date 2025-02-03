namespace WebFPRTest.Areas.Interno.Models.Usuario
{
    public class IndexViewModel
    {
        public int Id_Usuario { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public string Nombres { get; set; }
        public string Documento { get; set; }
        public string Correo { get; set; }
        public List<UsuarioViewModel> Usuarios { get; set; }
    }
}
