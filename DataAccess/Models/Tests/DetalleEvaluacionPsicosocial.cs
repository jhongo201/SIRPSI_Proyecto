using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.Tests
{
    [Table("DetalleEvaluacionPsicosocial", Schema = "sirpsi")]
    public partial class DetalleEvaluacionPsicosocial
    {
        [Key]
        public string Id { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string IdEvaluacionPsicosocialUsuario { get; set; }
        public string IdPreguntaEvaluacion { get; set; }
        public int Respuesta { get; set; }
        public float Puntuacion { get; set; }
        public string IdUserEvaluacion { get; set; }

    }
}
