using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using WebFPRTest.Areas.Interno.Interface.ListAcreditacion;
using WebFPRTest.Areas.Interno.Models.ListAcreditacion;

namespace WebFPRTest.Areas.Interno.Service.ListAcreditacion
{
    public class ListAcreditacionService: IListAcreditacionService
    {
        private readonly SqlConnection _connection;
        public ListAcreditacionService(SqlConnection connection)
        {
            _connection = connection;
        }
        public async Task<List<ComprobanteTabla>> Comprobante_Bandeja(ComprobanteFiltroViewModel comprobanteFiltroViewModel)
        {
            var procedure = "usp_JugadorComprobante_Bandeja";
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id_Equipo", comprobanteFiltroViewModel.Id_Equipo);
                parameters.Add("@Documento", comprobanteFiltroViewModel.Documento);
                parameters.Add("@Paterno", comprobanteFiltroViewModel.Paterno);
                parameters.Add("@Materno", comprobanteFiltroViewModel.Materno);
                parameters.Add("@Nombres", comprobanteFiltroViewModel.Nombres);
                parameters.Add("@Id_015_TipoPago", comprobanteFiltroViewModel.Id_015_TipoPago);
                parameters.Add("@NumeroOperacion", comprobanteFiltroViewModel.NumeroOperacion);

                var usuarios = await _connection.QueryAsync<ComprobanteTabla>(
                    procedure,
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return usuarios.ToList();
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
        public async Task<List<ListaJugadoresComprobante>> JugadorComprobante_Jugadores(int Id_Comprobante)
        {
            var procedure = "usp_JugadorComprobante_Datos ";
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id_Comprobante", Id_Comprobante);

                var usuarios = await _connection.QueryAsync<ListaJugadoresComprobante>(
                    procedure,
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return usuarios.ToList();
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
        public async Task<AcreditacionJugadoresViewModel> Comprobante_Select(int Id_Comprobante)
        {
            var procedure = "usp_Comprobante_Select ";
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id_Comprobante", Id_Comprobante);

                var usuarios = await _connection.QueryFirstOrDefaultAsync<AcreditacionJugadoresViewModel>(
                    procedure,
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                return usuarios;
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
        public async Task<string> Archivo_RutaLogo(int Id_Equipo, int Id_Comprobante, int Id_013_TipoArchivo)
        {
            var procedure = "usp_Archivos_Ruta";
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id_Equipo", Id_Equipo);
                parameters.Add("@Id_Comprobante", Id_Comprobante);
                parameters.Add("@Id_013_TipoArchivo", Id_013_TipoArchivo);

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
    }
}
