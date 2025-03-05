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
        public DateTime FechaRegistroActaMedica { get; set; }
        public DateTime FechaVencimientoActaMedica { get; set; }
        public string RutaRugbyReady { get; set; }
        public IFormFile RugbyReady { get; set; }
        public DateTime FechaRegistroRugbyReady { get; set; }
        public DateTime FechaVencimientoRugbyReady { get; set; }
        public string RutaRugbyLaws { get; set; }
        public IFormFile RugbyLaws { get; set; }
        public DateTime FechaRegistroRugbyLaws { get; set; }
        public DateTime FechaVencimientoRugbyLaws { get; set; }
        public string RutaKeepRugbyClean { get; set; }
        public IFormFile KeepRugbyClean{ get; set; }
        public DateTime FechaRegistroKeepRugbyClean { get; set; }
        public DateTime FechaVencimientoKeepRugbyClean { get; set; }
        public string RutaPrimerosAuxilios { get; set; }
        public IFormFile PrimerosAuxilios { get; set; }
        public DateTime FechaRegistroPrimerosAuxilios { get; set; }
        public DateTime FechaVencimientoPrimerosAuxilios { get; set; }
        public string RutaConmocionCerebral { get; set; }
        public IFormFile ConmocionCerebral { get; set; }
        public DateTime FechaRegistroConmocionCerebral { get; set; }
        public DateTime FechaVencimientoConmocionCerebral { get; set; }
    }
}
