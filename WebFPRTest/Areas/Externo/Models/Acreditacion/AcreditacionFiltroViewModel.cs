namespace WebFPRTest.Areas.Externo.Models.Acreditacion
{
    public class AcreditacionFiltroViewModel
    {
        public int Id_Equipo { get; set; }
        public string? Paterno { get; set; }
        public string? Materno{ get; set; }
        public string? Nombres { get; set; }
        public List<AcreditacionTabla> ListaJugadores { get; set; }
    }
    public class AcreditacionTabla
    { 
        public int Indice { get; set; }
        public int Id_Jugador { get; set; }
        public string NombreCompleto { get; set; }
        public string Division { get; set; }
        public string Situacion { get; set; }
    }
}
