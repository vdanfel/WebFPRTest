using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using WebFPRTest.Areas.Externo.Interface.Jugador;
using WebFPRTest.Areas.Externo.Models.Jugador;
using WebFPRTest.Areas.Interno.Models.Usuario;
using WebFPRTest.Models;

namespace WebFPRTest.Areas.Externo.Service.Jugador
{
    public class JugadorService: IJugadorService
    {
        public readonly SqlConnection _connection;
        public JugadorService(SqlConnection connection)
        {
            _connection = connection;
        }
        public async Task<PersonaModel> Persona_Existe(int idTipoDocumento, string documento)
        {
            var procedure = "usp_Persona_BuscarPorDocumento";
            try
            {   
                var parameters = new DynamicParameters();
                parameters.Add("@Id_001_TipoDocumento", idTipoDocumento);
                parameters.Add("@Documento", documento);

                // Solo obtener el Id_Persona
                var idPersona = await _connection.QueryFirstOrDefaultAsync<PersonaModel>(
                    procedure,
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return idPersona;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error al buscar el Id_Persona por documento.", ex);
            }
            finally
            {
                _connection.Close();
            }
        }
        public async Task<int> Persona_Insertar(PersonaModel persona, int Id_Usuario)
        {
            var procedure = "usp_Persona_Insert"; // Nombre del procedimiento almacenado
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Paterno", persona.Paterno, DbType.String);
                parameters.Add("@Materno", persona.Materno, DbType.String);
                parameters.Add("@Nombres", persona.Nombres, DbType.String);
                parameters.Add("@Id_001_TipoDocumento", persona.Id_001_TipoDocumento, DbType.Int32);
                parameters.Add("@Documento", persona.Documento, DbType.String);
                parameters.Add("@FechaNacimiento", persona.FechaNacimiento != DateTime.MinValue ? persona.FechaNacimiento : (object)DBNull.Value, DbType.DateTime);
                parameters.Add("@Id_003_Pais", persona.Id_003_Pais != 0 ? persona.Id_003_Pais : (object)DBNull.Value, DbType.Int32);
                parameters.Add("@Id_004_Nacionalidad", persona.Id_004_Nacionalidad != 0 ? persona.Id_004_Nacionalidad : (object)DBNull.Value, DbType.Int32);
                parameters.Add("@Id_002_Sexo", persona.Id_002_Sexo != 0 ? persona.Id_002_Sexo : (object)DBNull.Value, DbType.Int32);
                parameters.Add("@Celular", string.IsNullOrEmpty(persona.Celular) ? (object)DBNull.Value : persona.Celular, DbType.String);
                parameters.Add("@Id_014_TipoSangre", persona.Id_014_TipoSangre != 0 ? persona.Id_014_TipoSangre : (object)DBNull.Value, DbType.Int32);
                parameters.Add("@Correo", string.IsNullOrEmpty(persona.Correo) ? (object)DBNull.Value : persona.Correo, DbType.String);
                parameters.Add("@Id_005_TipoSeguro", persona.Id_005_TipoSeguro != 0 ? persona.Id_005_TipoSeguro : (object)DBNull.Value, DbType.Int32);
                parameters.Add("@NumeroPoliza", string.IsNullOrEmpty(persona.NumeroPoliza) ? (object)DBNull.Value : persona.NumeroPoliza, DbType.String);
                parameters.Add("@FechaPoliza", persona.FechaPoliza != DateTime.MinValue ? persona.FechaPoliza : (object)DBNull.Value, DbType.DateTime);
                parameters.Add("@FechaVencimientoPoliza", persona.FechaVencimientoPoliza != DateTime.MinValue ? persona.FechaVencimientoPoliza : (object)DBNull.Value, DbType.DateTime);
                parameters.Add("@Id_006_TipoVehiculo", persona.Id_006_TipoVehiculo != 0 ? persona.Id_006_TipoVehiculo : (object)DBNull.Value, DbType.Int32);
                parameters.Add("@NumeroPlaca", string.IsNullOrEmpty(persona.NumeroPlaca) ? (object)DBNull.Value : persona.NumeroPlaca, DbType.String);
                parameters.Add("@Usuario", Id_Usuario, DbType.Int32);

                var idPersona = await _connection.ExecuteScalarAsync<int>(
                    procedure,
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return idPersona;
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
        public async Task<int> Jugador_Existe(int Id_Persona, int Id_Equipo)
        {
            var procedure = "usp_Jugador_Existe";
            try 
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id_Persona", Id_Persona, DbType.Int32);
                parameters.Add("@Id_Equipo", Id_Equipo, DbType.Int32);

                var idJugador = await _connection.ExecuteScalarAsync<int>(
                    procedure,
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                return idJugador;
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
        public async Task<int> Jugador_Insertar(JugadorModel jugador, int Id_Usuario)
        {
            var procedure = "usp_Jugador_Insert"; // Nombre del procedimiento almacenado
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id_Persona", jugador.Id_Persona, DbType.String);
                parameters.Add("@Id_Equipo", jugador.Id_Equipo, DbType.String);
                parameters.Add("@Id_007_Division", jugador.Id_007_Division, DbType.Int32);
                parameters.Add("@Id_008_Situacion", jugador.Id_008_Situacion, DbType.Int32);
                parameters.Add("@Id_009_EstadoJugador", jugador.Id_009_EstadoJugador, DbType.Int32);
                parameters.Add("@Id_UsuarioCreacion", Id_Usuario, DbType.Int32);
                var idJugador = await _connection.ExecuteScalarAsync<int>(
                    procedure,
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return idJugador;
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
        public async Task<List<JugadorTablaViewModel>> Jugador_Bandeja(JugadorFiltroViewModel jugadorFiltroViewModel)
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
                
                var jugadores = await _connection.QueryAsync<JugadorTablaViewModel>(
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
        public async Task<JugadorViewModel> Jugador_Select(int Id_Jugador)
        {
            var procedure = "usp_Jugador_Select";
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id_Jugador", Id_Jugador, DbType.Int32);

                var jugador = await _connection.QueryFirstOrDefaultAsync<JugadorViewModel>(
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
        public async Task Persona_Actualizar(JugadorViewModel jugadorViewModel, int Id_Usuario)
        {
            var procedure = "usp_Persona_Update";
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id_Persona", jugadorViewModel.Id_Persona, DbType.Int32);
                parameters.Add("@Paterno", jugadorViewModel.Paterno, DbType.String);
                parameters.Add("@Materno", jugadorViewModel.Materno, DbType.String);
                parameters.Add("@Nombres", jugadorViewModel.Nombres, DbType.String);
                parameters.Add("@Id_001_TipoDocumento", jugadorViewModel.Id_001_TipoDocumento, DbType.Int32);
                parameters.Add("@Documento", jugadorViewModel.Documento, DbType.String);
                parameters.Add("@FechaNacimiento", jugadorViewModel.FechaNacimiento, DbType.DateTime);
                parameters.Add("@Id_003_Pais", jugadorViewModel.Id_003_Pais, DbType.Int32);
                parameters.Add("@Id_004_Nacionalidad", jugadorViewModel.Id_004_Nacionalidad, DbType.Int32);
                parameters.Add("@Id_002_Sexo", jugadorViewModel.Id_002_Sexo, DbType.Int32);
                parameters.Add("@Celular", jugadorViewModel.Celular, DbType.String);
                parameters.Add("@Id_014_TipoSangre", jugadorViewModel.Id_014_TipoSangre, DbType.Int32);
                parameters.Add("@Correo", jugadorViewModel.Correo, DbType.String);
                parameters.Add("@Id_005_TipoSeguro", jugadorViewModel.Id_005_TipoSeguro, DbType.Int32);
                parameters.Add("@NumeroPoliza", jugadorViewModel.NumeroPoliza, DbType.String);
                parameters.Add("@FechaPoliza", jugadorViewModel.FechaPoliza, DbType.DateTime);
                parameters.Add("@FechaVencimientoPoliza", jugadorViewModel.FechaVencimientoPoliza, DbType.DateTime);
                parameters.Add("@Id_006_TipoVehiculo", jugadorViewModel.Id_006_TipoVehiculo, DbType.Int32);
                parameters.Add("@NumeroPlaca", jugadorViewModel.NumeroPlaca, DbType.String);
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
        public async Task Jugador_Actualizar(JugadorViewModel jugadorViewModel, int Id_Usuario)
        {
            var procedure = "usp_Jugador_Update";
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@Id_Jugador", jugadorViewModel.Id_Jugador, DbType.Int32);
                parameters.Add("@Id_Persona", jugadorViewModel.Id_Persona, DbType.Int32);
                parameters.Add("@Id_Equipo", jugadorViewModel.Id_Equipo, DbType.Int32);
                parameters.Add("@Id_007_Division", jugadorViewModel.Id_007_Division, DbType.Int32);
                parameters.Add("@Id_008_Situacion", jugadorViewModel.Id_008_Situacion, DbType.Int32);
                parameters.Add("@Id_009_EstadoJugador", jugadorViewModel.Id_009_EstadoJugador, DbType.Int32);
                parameters.Add("@Observacion", jugadorViewModel.Observacion, DbType.Int32);
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


        public async Task<JugadorViewModel> Jugador_ValidarPersona(int Id_001_TipoDocumento, string Documento)
        {
            var procedure = "usp_Jugador_ObtenerPorDocumento";
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id_001_TipoDocumento", Id_001_TipoDocumento);
                parameters.Add("@Documento", Documento);

                var persona = await _connection.QueryFirstOrDefaultAsync<JugadorViewModel>(
                    procedure,
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return persona;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error al buscar la persona por documento.", ex);
            }
            finally
            {
                _connection.Close();
            }
        }
        public async Task<int> Jugador_BuscarPorDocumento(int Id_Equipo, int Id_001_TipoDocumento, string Documento)
        {
            var procedure = "usp_Jugador_BuscarPorDocumento";
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id_Equipo", Id_Equipo);
                parameters.Add("@Id_001_TipoDocumento", Id_001_TipoDocumento);
                parameters.Add("@Documento", Documento);

                var estado = await _connection.QueryFirstOrDefaultAsync<int>(
                    procedure,
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return estado;
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
        public async Task Apoderado_Insertar(JugadorViewModel jugadorViewModel, int Id_Usuario)
        {
            var procedure = "usp_Apoderado_Insert";
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@Id_Persona", jugadorViewModel.Id_Persona, DbType.Int32);
                parameters.Add("@Paterno", jugadorViewModel.DatosApoderado.ApoderadoPaterno, DbType.String);
                parameters.Add("@Materno", jugadorViewModel.DatosApoderado.ApoderadoMaterno, DbType.String);
                parameters.Add("@Nombres", jugadorViewModel.DatosApoderado.ApoderadoNombres, DbType.String);
                parameters.Add("@Id_001_TipoDocumento", jugadorViewModel.DatosApoderado.ApoderadoId_001_TipoDocumento, DbType.Int32);
                parameters.Add("@Documento", jugadorViewModel.DatosApoderado.ApoderadoDocumento, DbType.String);
                parameters.Add("@Id_Usuario", Id_Usuario, DbType.Int32);
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
        public async Task Jugador_CambioEstado441(int Id_Equipo, int Id_Jugador, int Usuario)
        {
            var procedure = "usp_Jugador_CambioEstado441";
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id_Equipo", Id_Equipo);
                parameters.Add("@Id_Jugador", Id_Jugador);
                parameters.Add("@Id_UsuarioModificacion", Usuario);
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
