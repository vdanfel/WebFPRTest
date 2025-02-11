using WebFPRTest.Result;

namespace WebFPRTest.Areas.Externo.Models.Jugador
{
    public class JugadorViewModel
    {
        public int Id_Jugador { get; set; }
        public int Id_Equipo { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public string Nombres { get; set; }
        public int Id_001_TipoDocumento { get; set; }
        public List<ParametrosTipoResult> TipoDocumentos { get; set; }
        public string Documento { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public int Id_003_Pais { get; set; }
        public List<ParametrosTipoResult> Paises { get; set; }
        public int Id_004_Nacionalidad { get; set; }
        public List<ParametrosTipoResult> Nacionalidades { get; set; }
        public int Id_002_Sexo { get; set; }
        public List<ParametrosTipoResult> Sexos { get; set; }
        public string Celular { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public int Id_005_TipoSeguro { get; set; }
        public List<ParametrosTipoResult> TipoSeguros { get; set; }
        public string NumeroPoliza { get; set; }
        public DateTime? FechaPoliza { get; set; }
        public DateTime? FechaVencimientoPoliza { get; set; }
        public int Id_006_TipoVehiculos { get; set; }
        public List<ParametrosTipoResult> TipoVehiculos { get; set; }
        public string NumeroPlaca { get; set; }
        public int Id_007_Division { get; set; }
        public List<ParametrosTipoResult> DivisionList { get; set; }
        public int Id_008_Situacion { get; set; }
        public List<ParametrosTipoResult> SituacionList { get; set; }
        public int Id_009_EstadoJugador { get; set; }
        public string Observacion { get; set; }
        public string MotivoAnulacion { get; set; }
        public JugadorApoderado DatosApoderado { get; set; }
    }
    public class JugadorApoderado
    {
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public string Nombres { get; set; }
        public int Id_001_TipoDocumento { get; set; }
        public List<ParametrosTipoResult> TipoDocumentos { get; set; }
        public string Documento { get; set; }
    }
}
