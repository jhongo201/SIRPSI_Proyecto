namespace SIRPSI.DTOs.WorkPlace
{
    public class RegistrarCentroTrabajo
    {
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public string IdEmpresa { get; set; } = null!;
    }
    public class RegistrarCentroTrabajoUsuario
    {
        public string Workplace { get; set; }
        public string User { get; set; }
    }
}
