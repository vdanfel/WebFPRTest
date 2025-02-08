using WebFPRTest.Models;

namespace WebFPRTest.Areas.Externo.Interface.Jugador
{
    public interface IJugadorService
    {
        Task<int?> Persona_Existe(int idTipoDocumento, string documento);
        Task<int> Persona_Insertar(PersonaModel persona, int Id_Usuario);
        Task<int> Jugador_Existe(int Id_Persona, int Id_Equipo);
        Task<int> Jugador_Insertar(JugadorModel jugador, int Id_Usuario);
    }
}
