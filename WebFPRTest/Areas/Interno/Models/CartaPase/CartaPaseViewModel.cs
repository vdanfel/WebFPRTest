using WebFPRTest.Result;

namespace WebFPRTest.Areas.Interno.Models.CartaPase
{
    public class CartaPaseViewModel
    {
        public int Id_JugadorActual { get; set; }
        public int Id_JugadorNuevo { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public string Nombres { get; set; }
        public int Id_001_TipoDocumento { get; set; }
        public List<ParametrosTipoResult>TipoDocumentos { get; set; }
        public string Documento { get; set; }
        public string Correo { get; set; }
        public string NombreEquipo { get; set; }
        public int Id_EquipoActual { get; set; }
        public int Id_EquipoNuevo { get; set; }
        public List<EquipoListResult> EquipoList { get; set;}
        public bool JugadorLibre { get; set; }
        public string RutaCartaPase { get; set; }
        public IFormFile CartaPase { get; set; }
        public List<RutasArchivos> RutaArchivos { get; set; }
    }
    public class RutasArchivos
    { 
        public int Id_013_TipoArchivo { get; set; }
        public string RutaArchivo { get; set; }

    }
}
