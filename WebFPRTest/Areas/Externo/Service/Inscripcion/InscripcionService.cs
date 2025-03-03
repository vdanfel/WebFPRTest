using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using WebFPRTest.Areas.Externo.Interface.Inscripcion;
using WebFPRTest.Areas.Externo.Models.Inscripcion;

namespace WebFPRTest.Areas.Externo.Service.Inscripcion
{
    public class InscripcionService: IInscripcionService
    {
        public readonly SqlConnection _connection;
        public InscripcionService(SqlConnection connection)
        {
            _connection = connection;
        }
        public async Task<InscripcionViewModel> Jugador_Select(int Id_Jugador)
        {
            var procedure = "usp_Jugador_Select";
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id_Jugador", Id_Jugador, DbType.Int32);

                var jugador = await _connection.QueryFirstOrDefaultAsync<InscripcionViewModel>(
                    procedure,
                    parameters,
                    commandType: CommandType.StoredProcedure
                    );
                return jugador;
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
        public async Task<InscripcionViewModel> ArchivosInscripcion(int Id_Equipo, int Id_Jugador)
        {
            var procedure = "usp_ArchivosInscripcion";
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id_Equipo", Id_Equipo, DbType.Int32);
                parameters.Add("@Id_Jugador", Id_Jugador, DbType.Int32);

                var jugador = await _connection.QueryFirstOrDefaultAsync<InscripcionViewModel>(
                    procedure,
                    parameters,
                    commandType: CommandType.StoredProcedure
                    );
                return jugador;
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
