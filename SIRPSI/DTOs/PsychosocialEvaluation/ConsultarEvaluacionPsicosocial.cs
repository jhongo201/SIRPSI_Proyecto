using DataAccess.Models.Users;
using DataAccess.Models.WorkPlace;
using SIRPSI.DTOs.User;
using System.ComponentModel.DataAnnotations;

namespace SIRPSI.DTOs.PsychosocialEvaluation
{
    public class ConsultarEvaluacionPsicosocial
    {
        public string Id { get; set; } = null!;
        public string IdUsuario { get; set; }
        public string IdCentroTrabajo { get; set; }
        public DateTime FechaInicio { get; set; }
        public string IdEstado { get; set; }
        public string IdUsuarioRegistra { get; set; }
        public bool Finalizado { get; set; }
        public string Radicado { get; set; }
        public string Consentimiento { get; set; }
        public float Porcentaje { get; set; }
        public ConsultarUsuarios? Usuario { get; set; }
        public CentroTrabajo? CentroTrabajo { get; set; }
    }
}
