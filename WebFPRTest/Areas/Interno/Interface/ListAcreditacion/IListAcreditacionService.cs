using WebFPRTest.Areas.Interno.Models.ListAcreditacion;

namespace WebFPRTest.Areas.Interno.Interface.ListAcreditacion
{
    public interface IListAcreditacionService
    {
        Task<List<ComprobanteTabla>> Comprobante_Bandeja(ComprobanteFiltroViewModel comprobanteFiltroViewModel);
        Task<List<ListaJugadoresComprobante>> JugadorComprobante_Jugadores(int Id_Comprobante);
        Task<AcreditacionJugadoresViewModel> Comprobante_Select(int Id_Comprobante);
    }
}
