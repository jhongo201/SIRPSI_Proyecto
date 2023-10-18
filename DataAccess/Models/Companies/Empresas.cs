using DataAccess.Models.Ministry;
using DataAccess.Models.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.Status
{
    [Table("Empresas", Schema = "sirpsi")]
    public partial class Empresas
    {
        [Key]
        public string Id { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaModifico { get; set; }
        public string? UsuarioModifico { get; set; }
        public string? IdMinisterio { get; set; }
        [ForeignKey("Id")]
        public Ministerio Ministerio { get; set; }
        public string UsuarioRegistro { get; set; } = null!;
        public string? IdUsuario { get; set; }
        public string IdTipoPersona { get; set; }
        public string IdRegimenTributario { get; set; }
        public string? IdRepresentanteEmpresa { get; set; }
    }
}
