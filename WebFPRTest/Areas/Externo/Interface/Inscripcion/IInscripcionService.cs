using WebFPRTest.Areas.Externo.Models.Inscripcion;

namespace WebFPRTest.Areas.Externo.Interface.Inscripcion
{
    public interface IInscripcionService
    {
        Task<InscripcionViewModel> Jugador_Select(int Id_Jugador);
        Task Archivo_Insertar(int Id_Equipo, int Id_Jugador, int Id_013_TipoArchivo, string RutaArchivo, int Usuario);
        Task<InscripcionViewModel> ArchivosInscripcion(int Id_Equipo, int Id_Jugador);
    }
}
