using WebFPRTest.Areas.Externo.Models.Jugador;
using WebFPRTest.Models;

namespace WebFPRTest.Areas.Externo.Interface.Jugador
{
    public interface IJugadorService
    {
        Task<PersonaModel> Persona_Existe(int idTipoDocumento, string documento);
        Task<int> Persona_Insertar(PersonaModel persona, int Id_Usuario);
        Task Persona_Actualizar(JugadorViewModel jugadorViewModel, int Id_Usuario);
        Task<int> Jugador_Existe(int Id_Persona, int Id_Equipo);
        Task<int> Jugador_Insertar(JugadorModel jugador, int Id_Usuario);
        Task<List<JugadorTablaViewModel>> Jugador_Bandeja(JugadorFiltroViewModel jugadorFiltroViewModel);
        Task<JugadorViewModel> Jugador_Select(int Id_Jugador);
        Task<JugadorApoderado> Apoderado_Select(int Id_Persona);
        Task<string> Archivo_Ruta(int idEquipo, int idJugador, int idTipoArchivo);
        Task Archivo_Insertar(int Id_Equipo, int Id_Jugador, int Id_013_TipoArchivo, string RutaArchivo, int Usuario);
        Task<JugadorViewModel> Jugador_ValidarPersona(int Id_001_TipoDocumento, string Documento);
        Task Jugador_Actualizar(JugadorViewModel jugadorViewModel, int Id_Usuario);
        Task<int> Jugador_BuscarPorDocumento(int Id_Equipo, int Id_001_TipoDocumento, string Documento);
        Task Apoderado_Insertar(JugadorViewModel jugadorViewModel, int Id_Usuario);
        Task Jugador_CambioEstado441(int Id_Equipo, int Id_Jugador, int Usuario);


    }
}
