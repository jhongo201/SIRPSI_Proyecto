using DataAccess.Models.RepresentativeCompany;
using DataAccess.Models.Status;
using DataAccess.Models.Users;
using DataAccess.Models.WorkPlace;
using System.ComponentModel.DataAnnotations;

namespace SIRPSI.DTOs.Companies
{
    public class ActualizarEmpresasAssign
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        //public string? TipoDocumento { get; set; }
        //[Required(ErrorMessage = "El campo {0} es requerido")]
        public string? Documento { get; set; } = null!;
        public string? DigitoVerificacion { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string? IdTipoEmpresa { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public string? Observacion { get; set; }
        public string? IdUsuario { get; set; }
    }
    public class ActualizarEmpresas
    {
        public EmpresasAct Empresa { get; set; }
        public CentroTrabajoAct CentroTrabajo { get; set; }
        public AspNetUsers Usuario { get; set; }
        public RepresentanteEmpresa RepresentanteEmpresa { get; set; }
    }
    public class ActualizarDatosComplementariosEmpresas
    {
        public string Id { get; set; }
        public int? NumeroTrabajadores { get; set; }
        public string? ClaseRiesgo { get; set; } = null!;
        public string? IdSectorEconomico { get; set; }
    }
    public class EmpresasAct
    {
        [Key]
        public string Id { get; set; }
        public int IdConsecutivo { get; set; }
        public string? TipoDocumento { get; set; }
        public string? DigitoVerificacion { get; set; }
        public string? IdTipoEmpresa { get; set; }
        public string Documento { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string? Observacion { get; set; }
        public string? IdEstado { get; set; }
        public string IdActividadEconomica { get; set; }
        public int? NumeroTrabajadores { get; set; }
        public string? ClaseRiesgo { get; set; }
        public string? IdSectorEconomico { get; set; }
        public string? IdMinisterio { get; set; }
        public string? IdUsuario { get; set; }
        public string IdTipoPersona { get; set; }
        public string IdRegimenTributario { get; set; }
    }
    public class CentroTrabajoAct
    {
        [Key]
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public string IdEmpresa { get; set; } = null!;
        public string? IdEstado { get; set; }
        public bool? Principal { get; set; }
        public string? IdUsuario { get; set; }
        public int? IdDepartamento { get; set; }
        public int? IdMunicipio { get; set; }
        public string? Direccion { get; set; }
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public string? Celular { get; set; }
    }
}
