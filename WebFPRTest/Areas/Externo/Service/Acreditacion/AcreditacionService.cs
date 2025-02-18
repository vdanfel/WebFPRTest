using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Runtime.CompilerServices;
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
        public async Task<decimal> EquipoSaldo(int Id_Equipo)
        {
            var procedure = "usp_Equipo_Saldo";
            try 
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id_Equipo", Id_Equipo, DbType.String);
                var saldo = await _connection.QueryFirstOrDefaultAsync<decimal>(
                    procedure,
                   parameters,
                   commandType: CommandType.StoredProcedure
                   );
                return saldo;
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
        public async Task Jugador_SolicitudAcreditacion(int Id_Jugador)
        {
            var procedure = "usp_Jugador_SolicitudAcreditacion";
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id_Jugador", Id_Jugador, DbType.Int32);
                var saldo = await _connection.QueryAsync<decimal>(
                    procedure,
                   parameters,
                   commandType: CommandType.StoredProcedure
                   );
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error al actualizar el estado del jugador.", ex);
            }
            finally
            {
                _connection.Close();
            }
        }
        
        public async Task Equipo_ActualizarSaldo(int Id_Equipo, decimal Saldo)
        {
            var procedure = "usp_Equipo_Update";
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id_Equipo", Id_Equipo, DbType.Int32);
                parameters.Add("@Saldo", Id_Equipo, DbType.Decimal);
                var saldo = await _connection.QueryAsync<decimal>(
                   procedure,
                   parameters,
                   commandType: CommandType.StoredProcedure
                );
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error al actualizar el estado del jugador.", ex);
            }
            finally
            {
                _connection.Close();
            }
        }
    }
}
