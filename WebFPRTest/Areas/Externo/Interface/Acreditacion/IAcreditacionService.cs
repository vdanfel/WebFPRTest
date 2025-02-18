using WebFPRTest.Areas.Externo.Models.Acreditacion;

namespace WebFPRTest.Areas.Externo.Interface.Acreditacion
{
    public interface IAcreditacionService
    {
        Task<List<AcreditacionTabla>> JugadoresAcreditados_Bandeja(AcreditacionFiltroViewModel acreditacionFiltroViewModel);
        Task<decimal> EquipoSaldo(int Id_Equipo);
        Task Jugador_SolicitudAcreditacion(int Id_Jugador);
        Task Equipo_ActualizarSaldo(int Id_Equipo, decimal Saldo);
    }
}
