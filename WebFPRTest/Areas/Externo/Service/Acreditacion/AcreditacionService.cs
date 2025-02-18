using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Runtime.CompilerServices;
using WebFPRTest.Areas.Externo.Interface.Acreditacion;
using WebFPRTest.Areas.Externo.Models.Acreditacion;
using WebFPRTest.Areas.Externo.Result;

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
        public async Task Jugador_SolicitudAcreditacion(int Id_Jugador, int Id_Usuario)
        {
            var procedure = "usp_Jugador_Update";
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id_Jugador", Id_Jugador, DbType.Int32);
                parameters.Add("@Id_009_EstadoJugador", 443, DbType.Int32);
                parameters.Add("@Id_UsuarioModificacion", Id_Usuario, DbType.Int32);
                await _connection.QueryAsync(
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
                await _connection.QueryAsync(
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
        public async Task<ComprobanteResult> Comprobante_Insert(AcreditacionFiltroViewModel acreditacionFiltroViewModel, int Id_Usuario)
        {
            var procedure = "usp_Comprobante_Insert";
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id_Equipo", acreditacionFiltroViewModel.Id_Equipo, DbType.Int32); // Asegurar que el Id_Equipo se envía
                parameters.Add("@Id_015_TipoPago", acreditacionFiltroViewModel.Id_015_TipoPago, DbType.Int32);
                parameters.Add("@NumeroComprobante", acreditacionFiltroViewModel.NumeroComprobante, DbType.String);
                parameters.Add("@ImporteComprobante", acreditacionFiltroViewModel.ImporteComprobante, DbType.Decimal);
                parameters.Add("@Id_UsuarioCreacion", Id_Usuario, DbType.Int32);

                var comprobante = await _connection.QueryFirstOrDefaultAsync<ComprobanteResult>(
                    procedure,
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return comprobante; // Retorna el objeto con Id_Comprobante y Correlativo
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error al insertar el comprobante.", ex);
            }
            finally
            {
                _connection.Close();
            }
        }
        public async Task JugadorComprobante_Insert(int Id_Comprobante, int Id_Jugador, decimal ImporteJugador)
        {
            var procedure = "usp_JugadorComprobante_insert";
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id_Comprobante", Id_Comprobante, DbType.Int32);
                parameters.Add("@Id_Jugador", Id_Jugador, DbType.Int32);
                parameters.Add("@ImporteJugador", ImporteJugador, DbType.Decimal);
                await _connection.QueryAsync(
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
        public async Task Archivo_Insertar(int Id_Equipo, int Id_Jugador, int Id_013_TipoArchivo, string RutaArchivo, int Usuario)
        {
            var procedure = "usp_Archivos_Insert";
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id_Equipo", Id_Equipo);
                parameters.Add("@Id_Jugador", Id_Jugador);
                parameters.Add("@Id_013_TipoArchivo", Id_013_TipoArchivo);
                parameters.Add("@RutaArchivo", RutaArchivo);
                parameters.Add("@Usuario", Usuario);
                await _connection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
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
    }
}
