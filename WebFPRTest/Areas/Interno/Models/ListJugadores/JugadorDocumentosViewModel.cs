namespace WebFPRTest.Areas.Interno.Models.ListJugadores
{
    public class JugadorDocumentosViewModel
    {
        public int Id_009_EstadoJugador { get; set; }
        public bool Flag_Aprobacion1 { get; set; }
        public string Observacion { get; set; }
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
        public IFormFile KeepRugbyClean { get; set; }
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
