
using WebFPRTest.Areas.Interno.Models.ListJugadores;

namespace WebFPRTest.Areas.Interno.Interface.ListJugadores
{
    public interface IListJugadoresService
    {
        Task<List<JugadoresTablaViewModel>> Jugador_Bandeja(JugadoresFiltroViewModel jugadorFiltroViewModel);
    }
}
