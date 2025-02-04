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
        public async Task<List<TipoDocumento>> ListarTipoDocumento()
        {
            var procedure = "usp_ParametrosList";
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id_ParametrosTipo", 1);

                var tipoDocumentoList = await _connection.QueryAsync<TipoDocumento>(
                    procedure,
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return tipoDocumentoList.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar tipos de documento", ex);
            }
            finally
            {
                _connection.Close();
            }
        }
    }
}
