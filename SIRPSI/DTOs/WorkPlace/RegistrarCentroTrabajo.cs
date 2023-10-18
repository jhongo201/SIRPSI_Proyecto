namespace SIRPSI.DTOs.WorkPlace
{
    public class RegistrarCentroTrabajo
    {
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public string IdEmpresa { get; set; } = null!;
        public int? IdDepartamento { get; set; }
        public int? IdMunicipio { get; set; }
        public string? Direccion { get; set; }
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public string? Celular { get; set; }
    }
    public class RegistrarCentroTrabajoUsuario
    {
        public string Workplace { get; set; }
        public string User { get; set; }
    }
}
