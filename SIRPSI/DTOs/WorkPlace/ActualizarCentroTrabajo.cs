using DataAccess.Models.Status;

namespace SIRPSI.DTOs.WorkPlace
{
    public class ActualizarCentroTrabajo
    {
        public string? Id { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public string? IdEmpresa { get; set; } = null!;
        public string? IdEstado { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public string? UsuarioRegistro { get; set; } = null!;
        public DateTime? FechaModifico { get; set; }
        public string? UsuarioModifico { get; set; }
        public string? IdUsuario { get; set; }
    }
}
