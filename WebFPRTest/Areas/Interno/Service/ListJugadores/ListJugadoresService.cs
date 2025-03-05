using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
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
                parameters.Add("@Id_009_EstadoJugador", jugadorFiltroViewModel.Id_009_EstadoJugador, DbType.Int32);

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
        public async Task<JugadorDatosViewModel> Jugador_Select(int Id_Jugador)
        {
            var procedure = "usp_Jugador_Select";
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id_Jugador", Id_Jugador, DbType.Int32);

                var jugador = await _connection.QueryFirstOrDefaultAsync<JugadorDatosViewModel>(
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
        public async Task<JugadorApoderado> Apoderado_Select(int Id_Persona)
        {
            var procedure = "usp_Apoderado_Select";
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id_Persona", Id_Persona, DbType.Int32);

                var apoderado = await _connection.QueryFirstOrDefaultAsync<JugadorApoderado>(
                    procedure,
                    parameters,
                    commandType: CommandType.StoredProcedure
                    );
                return apoderado;
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
        public async Task<string> Archivo_Ruta(int idEquipo, int idJugador, int idTipoArchivo)
        {
            var procedure = "usp_Archivos_Ruta";
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id_Equipo", idEquipo);
                parameters.Add("@Id_Jugador", idJugador);
                parameters.Add("@Id_013_TipoArchivo", idTipoArchivo);

                var rutaArchivo = await _connection.QuerySingleOrDefaultAsync<string>(
                    procedure,
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return rutaArchivo ?? string.Empty; // Retorna cadena vacía si no hay resultado
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error al obtener la ruta del archivo.", ex);
            }
            finally
            {
                _connection.Close();
            }
        }
        public async Task Jugador_Actualizar(JugadorDatosViewModel jugadorDatosViewModel, int Id_Usuario)
        {
            var procedure = "usp_Jugador_Update";
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@Id_Jugador", jugadorDatosViewModel.Id_Jugador, DbType.Int32);
                parameters.Add("@Id_009_EstadoJugador", jugadorDatosViewModel.Id_009_EstadoJugador, DbType.Int32);
                parameters.Add("@Observacion", jugadorDatosViewModel.Observacion, DbType.String);
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
        public async Task<JugadorDocumentosViewModel> ArchivosInscripcion(int Id_Equipo, int Id_Jugador)
        {
            var procedure = "usp_ArchivosInscripcion";
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id_Equipo", Id_Equipo, DbType.Int32);
                parameters.Add("@Id_Jugador", Id_Jugador, DbType.Int32);

                var archivos = await _connection.QueryFirstOrDefaultAsync<JugadorDocumentosViewModel>(
                   procedure,
                    parameters,
                    commandType: CommandType.StoredProcedure
                    );
                return archivos;
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
