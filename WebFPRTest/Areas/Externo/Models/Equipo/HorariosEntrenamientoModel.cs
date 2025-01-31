namespace WebFPRTest.Areas.Externo.Models.Equipo
{
    public class HorariosEntrenamientoModel
    {
        public int Id_HorariosEntrenamiento { get; set; }
        public int Id_Equipo { get; set; }
        public bool Lunes { get; set; }
        public TimeSpan? Lunes_HI { get; set; }
        public TimeSpan? Lunes_HF { get; set; }
        public bool Martes { get; set; }
        public TimeSpan? Martes_HI { get; set; }
        public TimeSpan? Martes_HF { get; set; }
        public bool Miercoles { get; set; }
        public TimeSpan? Miercoles_HI { get; set; }
        public TimeSpan? Miercoles_HF { get; set; }
        public bool Jueves { get; set; }
        public TimeSpan? Jueves_HI { get; set; }
        public TimeSpan? Jueves_HF { get; set; }
        public bool Viernes { get; set; }
        public TimeSpan? Viernes_HI { get; set; }
        public TimeSpan? Viernes_HF { get; set; }
        public bool Sabado { get; set; }
        public TimeSpan? Sabado_HI { get; set; }
        public TimeSpan? Sabado_HF { get; set; }
        public bool Domingo { get; set; }
        public TimeSpan? Domingo_HI { get; set; }
        public TimeSpan? Domingo_HF { get; set; }
    }
}
