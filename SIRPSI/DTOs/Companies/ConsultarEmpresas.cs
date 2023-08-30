using DataAccess.Models.Companies;
using DataAccess.Models.Documents;
using DataAccess.Models.Estados;
using DataAccess.Models.Ministry;
using DataAccess.Models.Users;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIRPSI.DTOs.Companies
{
    public class ConsultarEmpresas
    {
        public string? Id { get; set; }
        public string? DigitoVerificacion { get; set; }
        public string? IdTipoEmpresa { get; set; }
        public string? Documento { get; set; }
        public string? Nombre { get; set; }
        public string? IdMinisterio { get; set; }
        public string? IdEstado { get; set; }
        public int IdConsecutivo { get; set; }
        public string Descripcion { get; set; } = null!;
        public string? IdUsuario { get; set; }
        public AspNetUsers? Usuario { get; set; }
        public Estados? Estado { get; set; }
        public TiposDocumento? TipoDocumento { get; set; }
        public TiposEmpresa? TipoEmpresa { get; set; }
        public Ministerio? Ministerio { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public DateTime? FechaModifico { get; set; }

    }
}
