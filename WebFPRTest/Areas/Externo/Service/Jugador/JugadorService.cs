using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using WebFPRTest.Areas.Externo.Interface.Jugador;
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
        public async Task<int?> Persona_Existe(int idTipoDocumento, string documento)
        {
            var procedure = "usp_Persona_BuscarPorDocumento";
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id_001_TipoDocumento", idTipoDocumento);
                parameters.Add("@Documento", documento);

                // Solo obtener el Id_Persona
                var idPersona = await _connection.QueryFirstOrDefaultAsync<int?>(
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
                parameters.Add("@Telefono", string.IsNullOrEmpty(persona.Telefono) ? (object)DBNull.Value : persona.Telefono, DbType.String);
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
    }
}
