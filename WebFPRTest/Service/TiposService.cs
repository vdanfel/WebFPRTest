using Dapper;
using System.Data;
using System.Data.SqlClient;
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
    }
}
