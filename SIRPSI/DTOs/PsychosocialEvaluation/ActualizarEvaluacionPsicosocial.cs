using DataAccess.Models.Users;
using System.ComponentModel.DataAnnotations;

namespace SIRPSI.DTOs.PsychosocialEvaluation
{
    public class ActualizarEvaluacionPsicosocial
    {
        public string Id { get; set; } = null!;
        public string IdUsuario { get; set; }
        public string IdCentroTrabajo { get; set; }
        public DateTime FechaInicio { get; set; }
        public string IdEstado { get; set; }
        public string IdUsuarioRegistra { get; set; }
        public bool Finalizado { get; set; }
    }
}
