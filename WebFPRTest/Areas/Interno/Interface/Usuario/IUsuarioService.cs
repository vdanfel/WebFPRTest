using WebFPRTest.Areas.Interno.Models.Usuario;

namespace WebFPRTest.Areas.Interno.Interface.Usuario
{
    public interface IUsuarioService
    {
        Task<List<UsuarioViewModelTabla>> Usuario_Bandeja(UsuarioFiltroViewModel index);
        Task<UsuarioViewModel> Usuario_BuscarId_Usuario(int Id_Usuario);
        Task<UsuarioViewModel> Usuario_ValidarPersona(int idTipoDocumento, string documento);
        Task<int> Usuario_Existe(string usuario, int Id_Persona);
        Task<int> Persona_Insertar(UsuarioViewModel usuario, int Id_Usuario);
        Task<int> Usuario_Insertar(UsuarioViewModel usuario, int Id_Usuario);
        Task Usuario_Actualizar(UsuarioViewModel usuario, int Id_Usuario);
    }
}
