using WebFPRTest.Areas.Externo.Models.Acreditacion;

namespace WebFPRTest.Areas.Externo.Interface.Acreditacion
{
    public interface IAcreditacionService
    {
        Task<List<AcreditacionTabla>> JugadoresAcreditados_Bandeja(AcreditacionFiltroViewModel acreditacionFiltroViewModel);
    }
}
