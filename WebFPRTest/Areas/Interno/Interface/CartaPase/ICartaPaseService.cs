using WebFPRTest.Areas.Interno.Models.CartaPase;

namespace WebFPRTest.Areas.Interno.Interface.CartaPase
{
    public interface ICartaPaseService
    {
        Task<CartaPaseViewModel> Jugador_Datos(int Id_Jugador);
        Task<int> Jugador_CartaPase(int Id_Jugador, int Id_EquipoNuevo, int Id_Usuario);
        Task Archivo_Insertar(int Id_Equipo, int Id_Jugador, int Id_013_TipoArchivo, string RutaArchivo, int Usuario);
        Task<bool> JugadorLibre(int Id_Jugador);
        Task<List<RutasArchivos>> Archivos_RutasJugador(int Id_Jugador, int Id_Equipo);
    }
}
