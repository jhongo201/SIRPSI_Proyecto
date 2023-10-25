using System.ComponentModel.DataAnnotations.Schema;

namespace SIRPSI.DTOs.Tests
{
    public class DetalleEvaluacionPsicosocialDto
    {
        public string IdEvaluacionPsicosocialUsuario { get; set; }
        public string IdPreguntaEvaluacion { get; set; }
        public int Respuesta { get; set; }
        public float Puntuacion { get; set; }
        public string IdUserEvaluacion { get; set; }

    }
}
