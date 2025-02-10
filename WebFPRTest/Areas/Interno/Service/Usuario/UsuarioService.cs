using Dapper;
using System.Data;
using Microsoft.Data.SqlClient;
using WebFPRTest.Areas.Interno.Interface.Usuario;
using WebFPRTest.Areas.Interno.Models.Usuario;
using WebFPRTest.Result;
using System.Text;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

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
        public async Task<UsuarioViewModel> Usuario_ValidarPersona(int idTipoDocumento, string documento)
        {
            var procedure = "usp_Persona_BuscarPorDocumento";
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id_001_TipoDocumento", idTipoDocumento);
                parameters.Add("@Documento", documento);

                var persona = await _connection.QueryFirstOrDefaultAsync<UsuarioViewModel>(
                    procedure,
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return persona;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error al buscar la persona por documento.", ex);
            }
            finally
            {
                _connection.Close();
            }
        }
        public async Task<int> Usuario_Existe(string usuario, int Id_Persona)
        {
            var procedure = "usp_Usuario_Existe";
            try 
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Usuario ", usuario);
                parameters.Add("@Id_Persona", Id_Persona);

                var existe = await _connection.QuerySingleAsync<int>(
                procedure,
                parameters,
                commandType: CommandType.StoredProcedure
                );

                return existe;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error inesperado. Intenta nuevamente.", ex);
            }
            finally
            {
                _connection.Close();
            }
        
        }
        public async Task<int> Persona_Insertar(UsuarioViewModel usuario, int Id_Usuario)
        {
            var procedure = "usp_Persona_Insert"; // Nombre del procedimiento almacenado
            try
            {
                // Crear los parámetros dinámicos
                var parameters = new DynamicParameters();
                parameters.Add("@Paterno", usuario.Paterno, DbType.String);
                parameters.Add("@Materno", usuario.Materno, DbType.String);
                parameters.Add("@Nombres", usuario.Nombres, DbType.String);
                parameters.Add("@Id_001_TipoDocumento", usuario.Id_001_TipoDocumento, DbType.Int32);
                parameters.Add("@Documento", usuario.Documento, DbType.String);
                parameters.Add("@Correo", usuario.Correo, DbType.String);
                parameters.Add("@Celular", usuario.Celular, DbType.String);
                parameters.Add("@Usuario", Id_Usuario);

                // Ejecutar el procedimiento almacenado
                var idPersona = await _connection.ExecuteScalarAsync<int>(
                    procedure,
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return idPersona;  // Devuelves el Id de la persona recién insertada
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error al insertar la persona.", ex);
            }
            finally
            {
                _connection.Close();
            }
        }
        public async Task Usuario_Eliminar(int Id_Usuario)
        {
            var procedure = "usp_Usuario_Delete";

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id_Usuario", Id_Usuario, DbType.String);
                parameters.Add("@Id_UsuarioModificacion", Id_Usuario, DbType.Int32);

                // Ejecutar el procedimiento almacenado
                await _connection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error al actualizar el usuario.", ex);
            }
            finally
            {
                _connection.Close();
            }
        }
        public async Task<int> Usuario_Insertar(UsuarioViewModel usuario, int Id_Usuario)
        {
            var claveHash = EncriptarClave(usuario.ClaveConfirmacion);
            var procedure = "usp_Usuario_Insert"; // Nombre del procedimiento almacenado
            try
            {
                // Crear los parámetros dinámicos
                var parameters = new DynamicParameters();
                parameters.Add("@Id_Persona", usuario.Id_Persona, DbType.Int32);
                parameters.Add("@Id_Equipo", usuario.Id_Equipo, DbType.Int32);
                parameters.Add("@Usuario", usuario.Usuario, DbType.String);
                parameters.Add("@ClaveHash", claveHash, DbType.String);
                parameters.Add("@Id_011_TipoUsuario", usuario.Id_011_TipoUsuario, DbType.Int32);
                parameters.Add("@UsuarioCreacion", Id_Usuario);

                // Ejecutar el procedimiento almacenado
                var idPersona = await _connection.ExecuteScalarAsync<int>(
                    procedure,
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return idPersona;  // Devuelves el Id de la persona recién insertada
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error al insertar la persona.", ex);
            }
            finally
            {
                _connection.Close();
            }
        }
        public async Task Usuario_Actualizar(UsuarioViewModel usuario, int Id_Usuario)
        {
            string claveHash = "";
            if (!usuario.ClaveConfirmacion.IsNullOrEmpty())
            {
                claveHash = EncriptarClave(usuario.ClaveConfirmacion);
            }
            
            var procedure = "usp_Usuario_Update";

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id_Usuario", usuario.Id_Usuario, DbType.Int32);
                parameters.Add("@Id_Equipo", usuario.Id_Equipo, DbType.Int32);
                parameters.Add("@ClaveHash", claveHash, DbType.String);
                parameters.Add("@Id_011_TipoUsuario", usuario.Id_011_TipoUsuario, DbType.Int32);
                parameters.Add("@Id_UsuarioModificacion", Id_Usuario, DbType.Int32);

                // Ejecutar el procedimiento almacenado
                await _connection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error al actualizar el usuario.", ex);
            }
            finally
            {
                _connection.Close();
            }
        }
        public async Task Persona_Actualizar(UsuarioViewModel usuario, int Id_Usuario)
        {
            var procedure = "usp_Persona_Update";
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id_Persona", usuario.Id_Persona, DbType.Int32);
                parameters.Add("@Paterno", usuario.Paterno, DbType.String);
                parameters.Add("@Materno", usuario.Materno, DbType.String);
                parameters.Add("@Nombres", usuario.Nombres, DbType.String);
                parameters.Add("@Id_001_TipoDocumento", usuario.Id_001_TipoDocumento, DbType.Int32);
                parameters.Add("@Documento", usuario.Documento, DbType.String);
                parameters.Add("@Celular", usuario.Celular, DbType.String);
                parameters.Add("@Correo", usuario.Correo, DbType.String);
                parameters.Add("@Id_UsuarioModificacion", Id_Usuario, DbType.Int32);
                await _connection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error al actualizar el usuario.", ex);
            }
            finally
            {
                _connection.Close();
            }
        }
        private string EncriptarClave(string password)
        {
            if (password == null)
            {
                return password;
            }
            else
            {
                using (var sha256 = SHA256.Create())
                {
                    byte[] data = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                    StringBuilder sBuilder = new StringBuilder();
                    for (int i = 0; i < data.Length; i++)
                    {
                        sBuilder.Append(data[i].ToString("x2"));
                    }

                    return sBuilder.ToString();
                }
            }
        }
    }
}
