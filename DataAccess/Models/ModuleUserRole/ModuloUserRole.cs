using DataAccess.Models.Estados;
using DataAccess.Models.Module;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.ModuleUserRole
{
    [Table("ModuloUserRole", Schema = "sirpsi")]
    public partial class ModuloUserRole
    {
        public string Id { get; set; }
        public string RoleId { get; set; }
        public string ModuloId { get; set; }
        public string? EstadoId { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string UsuarioRegistro { get; set; } = null!;
        public DateTime? FechaModifico { get; set; }
        public string? UsuarioModifico { get; set; }
    }
}