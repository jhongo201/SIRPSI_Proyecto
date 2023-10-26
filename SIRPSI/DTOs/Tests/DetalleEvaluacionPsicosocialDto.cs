using System.ComponentModel.DataAnnotations.Schema;

namespace SIRPSI.DTOs.Tests
{
    public class DetalleEvaluacionPsicosocialDto
    {
        public string IdEvaluacionPsicosocialUsuario { get; set; }
        public string IdPreguntaEvaluacion { get; set; }
        public int Respuesta { get; set; }
        public string Puntuacion { get; set; }
        public string? IdUserEvaluacion { get; set; }
        public string? IdDimension { get; set; }
        public string? IdDominio { get; set; }
        public string? Forma { get; set; }
        public string? NombreDimension { get; set; }
        


    }
}
