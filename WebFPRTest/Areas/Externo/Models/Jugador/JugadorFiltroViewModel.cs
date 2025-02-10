using System.Drawing;
using WebFPRTest.Result;

namespace WebFPRTest.Areas.Externo.Models.Jugador
{
    public class JugadorFiltroViewModel
    {
        public int? Id_Equipo {  get; set; }
        public string? Paterno { get; set; }
        public string? Materno { get; set; }
        public string? Nombres{ get; set; }
        public string? Documento { get; set; }
        public int? Id_007_Division { get; set; }
        public List<JugadorTablaViewModel> ListaJugadores { get; set; }
        public List<ParametrosTipoResult> ListaDivisiones { get; set; }
        public JugadorFiltroViewModel() 
        {
            ListaJugadores = new List<JugadorTablaViewModel>();
        }
        
        
    }
    public class JugadorTablaViewModel
    { 
        public int Indice { get; set; }
        public int Id_Jugador { get; set; }
        public int Id_Persona{ get; set; }
        public string ApellidosYNombre { get; set; }
        public string TipoDocumento { get; set; }
        public string Documento {  get; set; }
        public string Division {  get; set; }
        public string Situacion {  get; set; }
        public int Id_009_EstadoJugador {  get; set; }
    }
}
