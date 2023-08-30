using DataAccess.Models.Users;
using DataAccess.Models.WorkPlace;

namespace SIRPSI.DTOs.WorkPlace
{
    public class ConsultarCentroTrabajo
    {
        public string Nombre { get; set; }
        public string? Descripcion { get; set; }
    }
    public class ConsultarCentroTrabajoUsuario
    {
        public string Id { get; set; }
        public string Workplace { get; set; }
        public string User { get; set; }
        public List<AspNetUsers> Users { get; set; }
        public List<CentroTrabajo> Workplaces { get; set; }
    }
    public class ConsultarUsuariosCentroTrabajo
    {
        public string Id { get; set; }
        public string WorkplaceId { get; set; }
        public string UserId { get; set; }
        public AspNetUsers User { get; set; }
        public CentroTrabajo Workplace { get; set; }
    }
}
