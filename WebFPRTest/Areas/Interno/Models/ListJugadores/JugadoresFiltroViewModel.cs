using System.Drawing;
using WebFPRTest.Result;

namespace WebFPRTest.Areas.Interno.Models.ListJugadores
{
    public class JugadoresFiltroViewModel
    {
        public int? Id_Equipo { get; set; }
        public string? Paterno { get; set; }
        public string? Materno { get; set; }
        public string? Nombres { get; set; }
        public string? Documento { get; set; }
        public int? Id_007_Division { get; set; }
        public List<EquipoListResult> EquiposList { get; set; }
        public List<JugadoresTablaViewModel> ListaJugadores { get; set; }
        public List<ParametrosTipoResult> ListaDivisiones { get; set; }
        public int? Id_009_EstadoJugador { get; set; }
        public List<ParametrosTipoResult> ListaEstadoJugador { get; set; }
        public JugadoresFiltroViewModel()
        {
            ListaJugadores = new List<JugadoresTablaViewModel>();
        }
        // Nuevas propiedades para la paginación
        public int PaginaActual { get; set; } = 1;
        public int FilasPorPagina { get; set; } = 100;
        public int TotalRegistros { get; set; }
        public int TotalPaginas => (int)Math.Ceiling((double)TotalRegistros / FilasPorPagina);
    }
    public class JugadoresTablaViewModel
    {
        public int Indice { get; set; }
        public int Id_Jugador { get; set; }
        public int Id_Equipo { get; set; }
        public int Id_Persona { get; set; }
        public string ApellidosYNombre { get; set; }
        public string TipoDocumento { get; set; }
        public string Documento { get; set; }
        public string Division { get; set; }
        public string Situacion { get; set; }
        public int Id_009_EstadoJugador { get; set; }
        public string NombreEquipo {get;set;}
    }
}
