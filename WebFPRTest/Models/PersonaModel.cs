namespace WebFPRTest.Models
{
    public class PersonaModel
    {
        public int Id_Persona { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public string Nombres { get; set; }
        public int Id_001_TipoDocumento { get; set; }
        public string Documento { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public int Id_003_Pais { get; set; }
        public int Id_004_Nacionalidad { get; set; }
        public int Id_002_Sexo { get; set; }
        public string Celular { get; set; }
        public int Id_014_TipoSangre { get; set; }
        public string Correo { get; set; }
        public int Id_005_TipoSeguro { get; set; }
        public string NumeroPoliza { get; set; }
        public DateTime? FechaPoliza { get; set; }
        public DateTime? FechaVencimientoPoliza { get; set; }
        public int Id_006_TipoVehiculo { get; set; }
        public string NumeroPlaca { get; set; }
    }
}
