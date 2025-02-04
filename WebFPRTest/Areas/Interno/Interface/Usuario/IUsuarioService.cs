using WebFPRTest.Areas.Interno.Models.Usuario;

namespace WebFPRTest.Areas.Interno.Interface.Usuario
{
    public interface IUsuarioService
    {
        Task<List<UsuarioViewModelTabla>> Usuario_Bandeja(UsuarioFiltroViewModel index);
        Task<UsuarioViewModel> Usuario_BuscarId_Usuario(int Id_Usuario);
        Task<UsuarioViewModel> Usuario_ValidarPersona(int idTipoDocumento, string documento);
    }
}
