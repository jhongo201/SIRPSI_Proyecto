using System.ComponentModel.DataAnnotations.Schema;

namespace SIRPSI.DTOs.Tests
{
    public class ConsultarPreguntasDto
    {
        public string Id { get; set; }
        public string? Pregunta { get; set; }
        public int? Posicion { get; set; }
        public int? Siempre { get; set; }
        public int? CasiSiempre { get; set; }
        public int? AlgunasVeces { get; set; }
        public int? CasiNunca { get; set; }
        public int? Nunca { get; set; }
        public string? IdDimension { get; set; }
        public string? Dimension { get; set; }
        public string? Forma { get; set; }
        public string? Dominio { get; set; }
        public string? DominioId { get; set; }




    }
}
