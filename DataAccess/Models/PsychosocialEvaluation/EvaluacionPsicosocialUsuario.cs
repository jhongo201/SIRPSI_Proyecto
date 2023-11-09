using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models.PsychosocialEvaluation
{
    [Table("EvaluacionPsicosocialUsuario", Schema = "sirpsi")]
    public partial class EvaluacionPsicosocialUsuario
    {
        public string Id { get; set; } = null!;
        public string IdUsuario { get; set; }
        public string IdCentroTrabajo { get; set; }
        public DateTime FechaInicio { get; set; }
        public string IdEstado { get; set; }
        public string IdUsuarioRegistra { get; set; }
        public bool Finalizado { get; set; }
        public string? radicadoEvaluacion { get; set; }
    }
}