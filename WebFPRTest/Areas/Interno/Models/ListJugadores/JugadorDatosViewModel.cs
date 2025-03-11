using WebFPRTest.Result;

namespace WebFPRTest.Areas.Interno.Models.ListJugadores
{
    public class JugadorDatosViewModel
    { 
        public int Id_Jugador { get; set; }
        public int Id_Equipo { get; set; }
        public int Id_Persona { get; set; }
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
        public string Correo { get; set; }
        public int Id_005_TipoSeguro { get; set; }
        public List<ParametrosTipoResult> TipoSeguros { get; set; }
        public string NumeroPoliza { get; set; }
        public DateTime? FechaPoliza { get; set; }
        public DateTime? FechaVencimientoPoliza { get; set; }
        public int? Id_006_TipoVehiculo { get; set; }
        public List<ParametrosTipoResult> TipoVehiculos { get; set; }
        public string? NumeroPlaca { get; set; }
        public int Id_007_Division { get; set; }
        public List<ParametrosTipoResult> DivisionList { get; set; }
        public int Id_008_Situacion { get; set; }
        public List<ParametrosTipoResult> SituacionList { get; set; }
        public int Id_009_EstadoJugador { get; set; }
        public string Observacion { get; set; }
        public string MotivoAnulacion { get; set; }
        public int Id_014_TipoSangre { get; set; }
        public List<ParametrosTipoResult> TipoSangre { get; set; }
        public JugadorApoderado DatosApoderado { get; set; }
        public string? RutaFoto { get; set; }
        public IFormFile? Foto { get; set; }
        public string? RutaDeslinde { get; set; }
        public IFormFile? Deslinde { get; set; }
        public bool Flag_Aprobacion1 { get; set; }
        public int Id_016_CostoAcreditacion { get; set; }
    }
    public class JugadorApoderado
    {
        public string ApoderadoPaterno { get; set; }
        public string ApoderadoMaterno { get; set; }
        public string ApoderadoNombres { get; set; }
        public int ApoderadoId_001_TipoDocumento { get; set; }
        public string ApoderadoDocumento { get; set; }
    }
}
