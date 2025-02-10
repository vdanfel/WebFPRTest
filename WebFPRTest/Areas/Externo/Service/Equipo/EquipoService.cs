using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using WebFPRTest.Areas.Externo.Interface.Equipo;
using WebFPRTest.Areas.Externo.Models.Equipo;

namespace WebFPRTest.Areas.Externo.Service.Equipo
{
    public class EquipoService:IEquipoService
    {
        public readonly SqlConnection _connection;
        public EquipoService(SqlConnection connection)
        {
            _connection = connection;
        }
        public async Task<int> Equipo_Insertar(EquipoViewModel equipo, int usuario)
        {
            var procedure = "usp_Equipo_Insert";
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Nombre", equipo.Nombre);
                parameters.Add("@Siglas", equipo.Siglas);
                parameters.Add("@RUC", equipo.RUC);
                parameters.Add("@RazonSocial", equipo.RazonSocial);
                parameters.Add("@RepresentanteLegal", equipo.RepresentanteLegal);
                parameters.Add("@UsuarioAdministrativo", equipo.UsuarioAdministrativo);
                parameters.Add("@Contacto", equipo.Contacto);
                parameters.Add("@LugarEntrenamiento", equipo.LugarEntrenamiento);
                parameters.Add("@Usuario", usuario);

                var idEquipo = await _connection.QuerySingleAsync<int>(
                procedure,
                parameters,
                commandType: CommandType.StoredProcedure
                );

                return idEquipo;
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
        public async Task Equipo_Actualizar(EquipoViewModel equipo, int Usuario)
        {
            var procedure = "usp_Equipo_Update";
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id_Equipo", equipo.Id_Equipo);
                parameters.Add("@Nombre", equipo.Nombre);
                parameters.Add("@Siglas", equipo.Siglas);
                parameters.Add("@RUC", equipo.RUC);
                parameters.Add("@RazonSocial", equipo.RazonSocial);
                parameters.Add("@RepresentanteLegal", equipo.RepresentanteLegal);
                parameters.Add("@UsuarioAdministrativo", equipo.UsuarioAdministrativo);
                parameters.Add("@Contacto", equipo.Contacto);
                parameters.Add("@LugarEntrenamiento", equipo.LugarEntrenamiento);
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
        public async Task<int> Equipo_Existe(string nombre, string ruc)
        {
            var procedure = "usp_Equipo_Existe";
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Nombre", nombre);
                parameters.Add("@RUC", ruc);

                var existe = await _connection.QuerySingleAsync<int>(
                procedure,
                parameters,
                commandType: CommandType.StoredProcedure
                );

                return existe;
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
        public async Task<EquipoViewModel> Equipo_Listar(int Id_Equipo)
        {
            var procedure = "usp_Equipo_Select";
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id_Equipo", Id_Equipo);

                var equipo = await _connection.QueryFirstOrDefaultAsync<EquipoViewModel>(
                    procedure,
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return equipo;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error inesperado al obtener el equipo.", ex);
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
        public async Task<string> Archivo_RutaLogo(int idEquipo, int idJugador, int idTipoArchivo)
        {
            var procedure = "usp_Archivos_RutaLogo";
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
        public async Task HorariosEntrenamientos_Insertar(HorariosEntrenamientoModel horarios, int usuario)
        {
            var procedure = "usp_HorariosEntrenamiento_Insert";
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id_Equipo", horarios.Id_Equipo);
                parameters.Add("@Lunes", horarios.Lunes);
                parameters.Add("@Lunes_HI", horarios.Lunes_HI.HasValue ? horarios.Lunes_HI : null);
                parameters.Add("@Lunes_HF", horarios.Lunes_HF.HasValue ? horarios.Lunes_HF : null);
                parameters.Add("@Martes", horarios.Martes);
                parameters.Add("@Martes_HI", horarios.Martes_HI.HasValue ? horarios.Martes_HI : null);
                parameters.Add("@Martes_HF", horarios.Martes_HF.HasValue ? horarios.Martes_HF : null);
                parameters.Add("@Miercoles", horarios.Miercoles);
                parameters.Add("@Miercoles_HI", horarios.Miercoles_HI.HasValue ? horarios.Miercoles_HI : null);
                parameters.Add("@Miercoles_HF", horarios.Miercoles_HF.HasValue ? horarios.Miercoles_HF : null);
                parameters.Add("@Jueves", horarios.Jueves);
                parameters.Add("@Jueves_HI", horarios.Jueves_HI.HasValue ? horarios.Jueves_HI : null);
                parameters.Add("@Jueves_HF", horarios.Jueves_HF.HasValue ? horarios.Jueves_HF : null);
                parameters.Add("@Viernes", horarios.Viernes);
                parameters.Add("@Viernes_HI", horarios.Viernes_HI.HasValue ? horarios.Viernes_HI : null);
                parameters.Add("@Viernes_HF", horarios.Viernes_HF.HasValue ? horarios.Viernes_HF : null);
                parameters.Add("@Sabado", horarios.Sabado);
                parameters.Add("@Sabado_HI", horarios.Sabado_HI.HasValue ? horarios.Sabado_HI : null);
                parameters.Add("@Sabado_HF", horarios.Sabado_HF.HasValue ? horarios.Sabado_HF : null);
                parameters.Add("@Domingo", horarios.Domingo);
                parameters.Add("@Domingo_HI", horarios.Domingo_HI.HasValue ? horarios.Domingo_HI : null);
                parameters.Add("@Domingo_HF", horarios.Domingo_HF.HasValue ? horarios.Domingo_HF : null);

                parameters.Add("@Usuario", usuario);
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
        public async Task<HorariosEntrenamientoModel> HorariosEntrenamiento_Listar(int Id_Equipo)
        {
            var procedure = "usp_HorariosEntrenamiento_List";
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id_Equipo", Id_Equipo);

                var horarios = await _connection.QueryFirstOrDefaultAsync<HorariosEntrenamientoModel>(
                    procedure,
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return horarios;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error inesperado al obtener el equipo.", ex);
            }
            finally
            {
                _connection.Close();
            }
        }
        public async Task AsociarUsuarioEquipo(int Id_Equipo, int Id_Usuario)
        {
            var procedure = "usp_Usuario_AsociarEquipo";
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id_Equipo", Id_Equipo);
                parameters.Add("@Id_Usuario", Id_Usuario);
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
