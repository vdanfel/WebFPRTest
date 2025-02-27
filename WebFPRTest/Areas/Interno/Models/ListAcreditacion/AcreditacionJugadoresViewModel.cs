using WebFPRTest.Result;

namespace WebFPRTest.Areas.Interno.Models.ListAcreditacion
{
    public class AcreditacionJugadoresViewModel
    {
        public string RutaComprobante { get; set; }
        public int Id_015_TipoPago { get; set; }
        public List<ParametrosTipoResult> TipoPagos { get; set; }
        public string NumeroComprobante { get; set; }
        public decimal ImporteComprobante { get; set; }
        public List<ListaJugadoresComprobante> ListaJugadores { get; set; }
    }
    public class ListaJugadoresComprobante
    { 
        public int Indice {  get; set; }
        public int Id_Jugador { get; set; }
        public string NombreEquipo { get; set; }
        public string NombreJugador { get; set; }
        public decimal CostoAcreditacion { get; set; }

    }
}
