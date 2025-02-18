using WebFPRTest.Result;

namespace WebFPRTest.Areas.Externo.Models.Acreditacion
{
    public class AcreditacionFiltroViewModel
    {
        public int Id_Equipo { get; set; }
        public string RutaComprobante { get; set; }
        public IFormFile Comprobante { get; set; }
        public int Id_015_TipoPago { get;set; }
        public List<ParametrosTipoResult>TipoPagos { get; set; }
        public List<AcreditacionTabla> ListaJugadores { get; set; }
        public decimal SaldoAFavor { get; set; }
        public decimal ImporteComprobante { get; set; }
        public string NumeroComprobante { get; set; }
        public decimal ImporteTotal { get; set; }
        public decimal TotalPagoAcreditacion { get; set; }
        public List<int> jugadoresSeleccionados { get; set; }  // IDs de jugadores marcados
        public List<decimal> valoresAcreditacion { get; set; } // Valores de acreditación correspondientes

    }
    public class JugadorSeleccionado
    {
        public int Id_Jugador { get; set; }
        public decimal ValorAcreditacion { get; set; }
    }
    public class AcreditacionTabla
    { 
        public int Indice { get; set; }
        public int Id_Jugador { get; set; }
        public string NombreCompleto { get; set; }
        public string Division { get; set; }
        public string Situacion { get; set; }
        public decimal ValorAcreditacion { get; set; }
    }
}
