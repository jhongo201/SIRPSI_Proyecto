using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.Tests
{
    [Table("PreguntasEvaluacion", Schema = "sirpsi")]
    public partial class Preguntas
    {
        [Key]
        public string Id { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Pregunta { get; set; }
        public int Posicion { get; set; }
        public int Siempre { get; set; }
        public int CasiSiempre { get; set; }
        public int AlgunasVeces { get; set; }
        public int CasiNunca { get; set; }
        public int Nunca { get; set; }
        public string? IdForma { get; set; }
        public string IdDimension { get; set; }
        public string grupo { get; set; }

    }
}
