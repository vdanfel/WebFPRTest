namespace WebFPRTest.Areas.Externo.Models.Equipo
{
    public class EquipoViewModel
    {
        public int Id_Equipo { get; set; }
        public string Nombre { get; set; }
        public string Siglas { get; set; }
        public string? RutaLogo { get; set; }
        public string RUC { get; set; }
        public string RazonSocial { get; set; }
        public string RepresentanteLegal { get; set; }
        public string UsuarioAdministrativo { get; set; }
        public string Contacto { get; set; }
        public string LugarEntrenamiento { get; set; }
        public HorariosEntrenamientoModel Horarios { get; set; }
        public IFormFile? Logo { get; set; }
    }
}
