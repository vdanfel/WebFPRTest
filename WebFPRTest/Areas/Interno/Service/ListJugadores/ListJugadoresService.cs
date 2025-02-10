using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using WebFPRTest.Areas.Externo.Models.Jugador;
using WebFPRTest.Areas.Interno.Interface.ListJugadores;
using WebFPRTest.Areas.Interno.Models.ListJugadores;

namespace WebFPRTest.Areas.Interno.Service.ListJugadores
{
    public class ListJugadoresService: IListJugadoresService
    {
        public readonly SqlConnection _connection;
        public ListJugadoresService(SqlConnection connection)
        {
            _connection = connection;
        }
        public async Task<List<JugadoresTablaViewModel>> Jugador_Bandeja(JugadoresFiltroViewModel jugadorFiltroViewModel)
        {
            var procedure = "usp_Jugador_Bandeja";
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id_Equipo", jugadorFiltroViewModel.Id_Equipo, DbType.Int32);
                parameters.Add("@Paterno", jugadorFiltroViewModel.Paterno, DbType.String);
                parameters.Add("@Materno", jugadorFiltroViewModel.Materno, DbType.String);
                parameters.Add("@Nombres", jugadorFiltroViewModel.Nombres, DbType.String);
                parameters.Add("@Documento", jugadorFiltroViewModel.Documento, DbType.String);
                parameters.Add("@Id_007_Division", jugadorFiltroViewModel.Id_007_Division, DbType.Int32);

                var jugadores = await _connection.QueryAsync<JugadoresTablaViewModel>(
                    procedure,
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return jugadores.ToList();
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
    }
}
