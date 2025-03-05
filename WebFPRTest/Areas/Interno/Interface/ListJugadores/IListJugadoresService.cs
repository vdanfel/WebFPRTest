
using WebFPRTest.Areas.Interno.Models.ListJugadores;

namespace WebFPRTest.Areas.Interno.Interface.ListJugadores
{
    public interface IListJugadoresService
    {
        Task<List<JugadoresTablaViewModel>> Jugador_Bandeja(JugadoresFiltroViewModel jugadorFiltroViewModel);
        Task<string> Archivo_Ruta(int idEquipo, int idJugador, int idTipoArchivo);
        Task<JugadorApoderado> Apoderado_Select(int Id_Persona);
        Task<JugadorDatosViewModel> Jugador_Select(int Id_Jugador);
        Task Jugador_Actualizar(JugadorDatosViewModel jugadorDatosViewModel, int Id_Usuario);
        Task<JugadorDocumentosViewModel> ArchivosInscripcion(int Id_Equipo, int Id_Jugador);
    }
}
