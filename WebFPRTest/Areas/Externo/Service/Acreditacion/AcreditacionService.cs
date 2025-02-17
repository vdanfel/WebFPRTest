using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using WebFPRTest.Areas.Externo.Interface.Acreditacion;
using WebFPRTest.Areas.Externo.Models.Acreditacion;

namespace WebFPRTest.Areas.Externo.Service.Acreditacion
{
    public class AcreditacionService:IAcreditacionService
    {
        public readonly SqlConnection _connection;
        public AcreditacionService(SqlConnection connection)
        {
            _connection = connection;
        }
        public async Task<List<AcreditacionTabla>>JugadoresAcreditados_Bandeja(AcreditacionFiltroViewModel acreditacionFiltroViewModel)
        {
            var procedure = "usp_Jugadores_AcreditacionBandeja";
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id_Equipo", acreditacionFiltroViewModel.Id_Equipo, DbType.String);
                var jugadores = await _connection.QueryAsync<AcreditacionTabla>(
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
