using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using WebFPRTest.Areas.Externo.Models.Acreditacion;
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
        public async Task<int> Jugador_CartaPase(int Id_Jugador, int Id_EquipoNuevo, int Id_Usuario)
        {
            var procedure = "usp_Jugador_CartaPase";
            try 
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id_Jugador", Id_Jugador);
                parameters.Add("@Id_EquipoNuevo", Id_EquipoNuevo);
                parameters.Add("@Id_UsuarioModificacion", Id_Usuario);
                var nuevoId = await _connection.QueryFirstOrDefaultAsync<int>(
                    procedure,
                    parameters,
                    commandType: CommandType.StoredProcedure
                    );
                return nuevoId;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error al hacer el pase de jugador a otro equipo.", ex);
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
        public async Task<bool> JugadorLibre(int Id_Jugador)
        {
            var procedure = "usp_JugadorLibre";
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id_Jugador", Id_Jugador);
                var respuesta = await _connection.QueryFirstOrDefaultAsync<bool>(
                    procedure, 
                    parameters, 
                    commandType: CommandType.StoredProcedure);
                return respuesta;
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
        public async Task<List<RutasArchivos>> Archivos_RutasJugador(int Id_Jugador, int Id_Equipo)
        {
            var procedure = "usp_Archivos_RutasJugador";
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id_Jugador", Id_Jugador);
                parameters.Add("@Id_Equipo", Id_Equipo);
                var rutaArchivos = await _connection.QueryAsync<RutasArchivos>(
                    procedure,
                    parameters,
                   commandType: CommandType.StoredProcedure
                    );
                return rutaArchivos.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error al obtener la ruta de los archivos.", ex);
            }
            finally
            {
                _connection.Close();
            }
        }
    }
}
