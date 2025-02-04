using WebFPRTest.Result;

namespace WebFPRTest.Areas.Interno.Models.Usuario
{
    public class UsuarioViewModel
    {
        public int Id_Usuario { get; set; }
        public int Id_Persona { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public string Nombres { get; set; }
        public int? Id_011_TipoUsuario { get; set; }
        public List<ParametrosTipoResult> TipoUsuarioList { get; set; }
        public string TipoUsuario { get; set; }
        public int? Id_001_TipoDocumento { get; set; }
        public List<ParametrosTipoResult> TipoDocumentoList { get; set; }
        public string TipoDocumento { get; set; }
        public string Documento { get; set; }
        public string Celular { get; set; }
        public string Correo { get; set; }
        public string Usuario { get; set; }
        public string ClaveHash { get; set; }
        public string Clave { get; set; }
        public string ClaveConfirmacion { get; set; }
    }
}
