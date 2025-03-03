using WebFPRTest.Result;

namespace WebFPRTest.Areas.Interno.Models.ListAcreditacion
{
    public class ComprobanteFiltroViewModel
    {
        public int? Id_Equipo { get; set; }
        public List<EquipoListResult> ListarEquipos { get; set; }
        public int Id_001_TipoDocumento { get; set; }
        public List<ParametrosTipoResult> TipoDocumentos { get; set; }
        public string? Documento { get; set; }
        public string? Paterno { get; set; }
        public string? Materno { get; set; }
        public string? Nombres { get; set; }
        public int? Id_015_TipoPago { get; set; }
        public List<ParametrosTipoResult>TipoPagos { get; set; }
        public string? NumeroOperacion { get; set; }

        
        

        public List<ComprobanteTabla> ListaComprobantes { get; set; }
        
    }
    public class ComprobanteTabla
    {
        public int Indice { get; set; }
        public int Id_Comprobante { get; set; }
        public string TipoPago { get; set; }
        public string NumeroComprobante { get; set; }
        public string NombreEquipo { get; set; }
        public string NombreJugador { get; set; }
    }
}
