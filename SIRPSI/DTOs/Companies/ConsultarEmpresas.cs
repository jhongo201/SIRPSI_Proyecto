using DataAccess.Models.Companies;
using DataAccess.Models.Documents;
using DataAccess.Models.Estados;
using DataAccess.Models.Ministry;
using DataAccess.Models.RepresentativeCompany;
using DataAccess.Models.TaxRegimes;
using DataAccess.Models.TypesPerson;
using DataAccess.Models.Users;
using DataAccess.Models.WorkPlace;
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
        public string? Observacion { get; set; }
        public string IdTipoPersona { get; set; }
        public string IdRegimenTributario { get; set; }
        public string IdActividadEconomica { get; set; }
        public int? NumeroTrabajadores { get; set; }
        public string? ClaseRiesgo { get; set; }
        public string? IdSectorEconomico { get; set; }
        public AspNetUsers? Usuario { get; set; }
        public Estados? Estado { get; set; }
        public TiposDocumento? TipoDocumento { get; set; }
        public TiposEmpresa? TipoEmpresa { get; set; }
        public Ministerio? Ministerio { get; set; }
        public TiposPersona? TipoPersona { get; set; }
        public RegimenesTributario? RegimenTributario { get; set; }
        public CentroTrabajo? CentroTrabajo { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public DateTime? FechaModifico { get; set; }
        public string? IdRepresentanteEmpresa { get; set; }
        public RepresentanteEmpresa? RepresentanteEmpresa { get; set; }
    }
}
