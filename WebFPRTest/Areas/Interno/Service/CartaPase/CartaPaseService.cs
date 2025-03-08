using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using WebFPRTest.Areas.Interno.Interface.CartaPase;
using WebFPRTest.Areas.Interno.Models.CartaPase;

namespace WebFPRTest.Areas.Interno.Service.CartaPase
{
    public class CartaPaseService:ICartaPaseService
    {
        private readonly SqlConnection _connection;

        public CartaPaseService(SqlConnection connection)
        {
            _connection = connection;
        }
        public async Task<CartaPaseViewModel> Jugador_Datos(int Id_Jugador)
        {
            var procedure = "usp_Jugador_Select";
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id_Jugador", Id_Jugador);
                var jugador = await _connection.QueryFirstOrDefaultAsync<CartaPaseViewModel>(
                    procedure,
                    parameters,
                    commandType: CommandType.StoredProcedure
                    );
                return jugador;
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
    }
}
