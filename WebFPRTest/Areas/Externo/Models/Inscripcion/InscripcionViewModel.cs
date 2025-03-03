using WebFPRTest.Result;

namespace WebFPRTest.Areas.Externo.Models.Inscripcion
{
    public class InscripcionViewModel
    {
        public int Id_Jugador { get; set; }
        public int Id_Equipo { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public string Nombres { get; set; }
        public int Id_001_TipoDocumento { get; set; }
        public string Documento { get; set; }
        public string Correo { get; set; }
        public List<ParametrosTipoResult> TipoDocumentos { get; set; }
        public string RutaActaMedica { get; set; }
        public IFormFile ActaMedica { get; set; }
        public string RutaRugbyReady { get; set; }
        public IFormFile RugbyReady { get; set; }
        public string RutaRugbyLaws { get; set; }
        public IFormFile RugbyLaws { get; set; }
        public string RutaKeepRugbyClean { get; set; }
        public IFormFile KeepRugbyClean{ get; set; }
        public string RutaPrimerosAuxilios { get; set; }
        public IFormFile PrimerosAuxilios { get; set; }
        public string RutaConmocionCerebral { get; set; }
        public IFormFile ConmocionCerebral { get; set; } 
    }
}
