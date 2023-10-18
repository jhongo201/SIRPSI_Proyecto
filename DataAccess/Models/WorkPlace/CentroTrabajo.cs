using DataAccess.Models.Status;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models.Users;

namespace DataAccess.Models.WorkPlace
{
    [Table("CentroTrabajo", Schema = "sirpsi")]
    public partial class CentroTrabajo
    {
        [Key]
        public string Id { get; set; }
        [Required(ErrorMessage = "El {0} es Requerido")]
        public string Nombre { get; set; }
        public string? Descripcion { get; set; }
        [Required(ErrorMessage = "Empresa es Requerida")]
        public string IdEmpresa { get; set; } = null!;
        [ForeignKey("Id")]
        public Empresas Empresa { get; set; }
        public string? IdEstado { get; set; }
        [ForeignKey("Id")]
        public DataAccess.Models.Estados.Estados Estados { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string UsuarioRegistro { get; set; } = null!;
        public DateTime? FechaModifico { get; set; }
        public string? UsuarioModifico { get; set; }
        public bool? Principal { get; set; }
        public string? IdUsuario { get; set; }
        [ForeignKey("Id")]
        public AspNetUsers Usuario { get; set; }
        public int? IdDepartamento { get; set; }
        public int? IdMunicipio { get; set; }
        public string? Direccion { get; set; }
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public string? Celular { get; set; }
    }
}
