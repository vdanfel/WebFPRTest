using Dapper;
using System.Data;
using System.Data.SqlClient;
using WebFPRTest.Areas.Interno.Interface.Usuario;
using WebFPRTest.Areas.Interno.Models.Usuario;

namespace WebFPRTest.Areas.Interno.Service.Usuario
{
    public class UsuarioService:IUsuarioService
    {
        private readonly SqlConnection _connection;
        public UsuarioService(SqlConnection connection) 
        {
            _connection = connection;
        }
        public async Task<List<UsuarioViewModelTabla>> Usuario_Bandeja(UsuarioFiltroViewModel index)
        {
            var procedure = "usp_Usuario_Bandeja";
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Paterno", index.Paterno);
                parameters.Add("@Materno", index.Materno);
                parameters.Add("@Nombres", index.Nombres);
                parameters.Add("@Documento", index.Documento);
                parameters.Add("@Correo", index.Correo);

                var usuarios = await _connection.QueryAsync<UsuarioViewModelTabla>(
                    procedure,
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return usuarios.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error inesperado al buscar los usuarios.", ex);
            }
            finally
            {
                _connection.Close();
            }
        }
        public async Task<UsuarioViewModel> Usuario_BuscarId_Usuario(int Id_Usuario)
        {
            var procedure = "usp_Usuario_ObtenerXId_Usuario"; // Nombre del procedimiento almacenado
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id_Usuario", Id_Usuario); // Parámetro de entrada

                var usuario = await _connection.QueryFirstOrDefaultAsync<UsuarioViewModel>(
                    procedure,
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return usuario;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error al buscar el usuario por ID.", ex);
            }
            finally
            {
                _connection.Close();
            }
        }

    }
}
