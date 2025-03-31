using Dapper;
using System.Data;
using Microsoft.Data.SqlClient;
using WebFPRTest.Interface;
using WebFPRTest.Result;

namespace WebFPRTest.Service
{
    public class TiposService: ITiposService
    {
        private readonly SqlConnection _connection;
        public TiposService(SqlConnection connection)
        { 
            _connection = connection;
        }
        public async Task<List<ParametrosTipoResult>> ParametroTipo_Listar(int parametroTipo)
        {
            var procedure = "usp_ParametrosList";
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id_ParametrosTipo", parametroTipo);

                var parametro = await _connection.QueryAsync<ParametrosTipoResult>(
                    procedure,
                    parameters,
                    commandType: CommandType.StoredProcedure
                    );
                return parametro.ToList();
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
        public async Task<List<EquipoListResult>> Equipo_Listar()
        {
            var procedure = "usp_Equipo_List";
            try 
            {
                var equipos = await _connection.QueryAsync<EquipoListResult>(
                    procedure,
                    commandType: CommandType.StoredProcedure
                    );
                return equipos.ToList();
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
        public async Task<string> TipoArchivo_Descripcion(int Id_003_TipoArchivo)
        {
            var procedure = "usp_TipoArchivo_Descripcion";
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id_003_TipoArchivo", Id_003_TipoArchivo);

                var descripcion = await _connection.QueryFirstOrDefaultAsync<string>(
                    procedure,
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return descripcion;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error al buscar al jugador por documento.", ex);
            }
            finally
            {
                _connection.Close();
            }
        }
        public async Task<bool> ControladorTipoUsuario(int Id_Controlador, int Id_011_TipoUsuario)
        {
            var procedure = "usp_ControladorTipoUsuario_Existe";

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id_Controlador", Id_Controlador);
                parameters.Add("@Id_011_TipoUsuario", Id_011_TipoUsuario);

                var id = await _connection.QueryFirstOrDefaultAsync<int?>(
                    procedure,
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                // Si `id` es distinto de null, significa que existe el registro
                return id.HasValue;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al verificar la existencia del ControladorTipoUsuario.", ex);
            }
            finally
            {
                _connection.Close();
            }
        }

    }
}
