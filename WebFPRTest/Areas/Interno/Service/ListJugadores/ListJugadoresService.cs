using ClosedXML.Excel;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using WebFPRTest.Areas.Interno.Interface.ListJugadores;
using WebFPRTest.Areas.Interno.Models.ListJugadores;
using WebFPRTest.Areas.Interno.Result;

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
                parameters.Add("@Id_Persona", jugadorDatosViewModel.Id_Persona, DbType.Int32);
                parameters.Add("@Id_Equipo", jugadorDatosViewModel.Id_Equipo, DbType.Int32);
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
        public async Task<byte[]> DescargarExcel(int idEquipo, string paterno, string materno, string nombres, string documento, int idDivision, int idEstadoJugador)
        {
            var procedure = "usp_Jugador_Reporte";

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id_Equipo", idEquipo, DbType.Int32);
                parameters.Add("@Paterno", paterno, DbType.String);
                parameters.Add("@Materno", materno, DbType.String);
                parameters.Add("@Nombres", nombres, DbType.String);
                parameters.Add("@Documento", documento, DbType.String);
                parameters.Add("@Id_007_Division", idDivision, DbType.Int32);
                parameters.Add("@Id_009_EstadoJugador", idEstadoJugador, DbType.Int32);

                var jugadores = await _connection.QueryAsync<ReporteJugadorResult>(
                    procedure,
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                if (jugadores == null || !jugadores.Any())
                {
                    throw new Exception("No se encontraron datos para exportar.");
                }

                return GenerarExcel(jugadores);
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error al generar el reporte de jugadores.", ex);
            }
            finally
            {
                _connection.Close();
            }
        }
        private byte[] GenerarExcel(IEnumerable<ReporteJugadorResult> jugadores)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Reporte de Jugadores");

                // Agregar encabezados
                worksheet.Cell(1, 1).Value = "Índice";
                worksheet.Cell(1, 2).Value = "Paterno";
                worksheet.Cell(1, 3).Value = "Materno";
                worksheet.Cell(1, 4).Value = "Nombres";
                worksheet.Cell(1, 5).Value = "Nombre Equipo";
                worksheet.Cell(1, 6).Value = "Tipo Documento";
                worksheet.Cell(1, 7).Value = "Documento";
                worksheet.Cell(1, 8).Value = "Fecha Nacimiento";
                worksheet.Cell(1, 9).Value = "Género";
                worksheet.Cell(1, 10).Value = "Tipo Sangre";
                worksheet.Cell(1, 11).Value = "División";
                worksheet.Cell(1, 12).Value = "Situación";
                worksheet.Cell(1, 13).Value = "Estado Jugador";
                worksheet.Cell(1, 14).Value = "Fecha Inscripción";
                worksheet.Cell(1, 15).Value = "Última Modificación";

                int row = 2;
                foreach (var jugador in jugadores)
                {
                    worksheet.Cell(row, 1).Value = jugador.Indice;
                    worksheet.Cell(row, 2).Value = jugador.Paterno;
                    worksheet.Cell(row, 3).Value = jugador.Materno ;
                    worksheet.Cell(row, 4).Value = jugador.Nombres;
                    worksheet.Cell(row, 5).Value = jugador.NombreEquipo;
                    worksheet.Cell(row, 6).Value = jugador.TipoDocumento;
                    worksheet.Cell(row, 7).Value = jugador.Documento;
                    worksheet.Cell(row, 8).Value = jugador.FechaNacimiento;
                    worksheet.Cell(row, 9).Value = jugador.Genero;
                    worksheet.Cell(row, 10).Value = jugador.TipoSangre;
                    worksheet.Cell(row, 11).Value = jugador.Division;
                    worksheet.Cell(row, 12).Value = jugador.Situacion;
                    worksheet.Cell(row, 13).Value = jugador.EstadoJugador;
                    worksheet.Cell(row, 14).Value = jugador.FechaInscripcion;
                    worksheet.Cell(row, 15).Value = jugador.UltimaModificacion;
                    row++;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

    }
}
