using DataAccess.Models.Users;
using Microsoft.AspNetCore.Identity;
using SIRPSI.DTOs.RepresentativeCompany;
using SIRPSI.DTOs.User;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIRPSI.DTOs.Companies
{
    public class RegistrarEmpresas
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string? TipoDocumento { get; set; }
        public string? DigitoVerificacion { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string? IdTipoEmpresa { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Documento { get; set; } = null!;

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Nombre { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public string? Observacion { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string? IdEstado { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string IdMinisterio { get; set; }
        public string? IdUsuario { get; set; }
        public UserCredentials? Usuario { get; set; } = null;
        public string? IdRepresentanteEmpresa { get; set; }
    }
    public class RegistrarEmpresaRequest
    {
        public EmpresasAct Empresa { get; set; }
        public CentroTrabajoAct CentroTrabajo { get; set; }
        public User Usuario { get; set; }
        public RegistrarRepresentanteEmpresa RepresentanteEmpresa { get; set; }
    }
    public partial class User : AspNetUsers
    {
        public string Password { get; set; }
    }
}
