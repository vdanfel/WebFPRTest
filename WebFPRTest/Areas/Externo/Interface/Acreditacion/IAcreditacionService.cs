using WebFPRTest.Areas.Externo.Models.Acreditacion;
using WebFPRTest.Areas.Externo.Result;

namespace WebFPRTest.Areas.Externo.Interface.Acreditacion
{
    public interface IAcreditacionService
    {
        Task<List<AcreditacionTabla>> JugadoresAcreditados_Bandeja(AcreditacionFiltroViewModel acreditacionFiltroViewModel);
        Task<decimal> EquipoSaldo(int Id_Equipo);
        Task Jugador_SolicitudAcreditacion(int Id_Jugador, int Id_Usuario);
        Task Equipo_ActualizarSaldo(int Id_Equipo, decimal Saldo, int Id_Usuario);
        Task<ComprobanteResult> Comprobante_Insert(AcreditacionFiltroViewModel acreditacionFiltroViewModel, int Id_Usuario);
        Task JugadorComprobante_Insert(int Id_Comprobante, int Id_Jugador, decimal ImporteJugador);
        Task Archivo_Insertar(int Id_Equipo, int Id_Jugador, int Id_013_TipoArchivo, string RutaArchivo, int Usuario);
    }
}
