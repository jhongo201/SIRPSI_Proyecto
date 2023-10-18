using System.ComponentModel.DataAnnotations;

namespace SIRPSI.DTOs.PsychosocialEvaluation
{
    public class RegistrarEvaluacionPsicosocial
    {
        public string IdUsuario { get; set; }
        public string IdCentroTrabajo { get; set; }
        public DateTime FechaInicio { get; set; }
    }
}

