using WebFPRTest.Result;

namespace WebFPRTest.Interface
{
    public interface ITiposService
    {
        Task<List<ParametrosTipoResult>> ParametroTipo_Listar(int parametroTipo);
        Task<List<TipoDocumento>> ListarTipoDocumento();
    }
}
