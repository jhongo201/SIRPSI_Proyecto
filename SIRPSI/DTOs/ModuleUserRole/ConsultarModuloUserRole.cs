using DataAccess.Models.Module;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SIRPSI.DTOs.ModuleUserRole
{
    public partial class ConsultarModuloUserRole
    {
        public string Id { get; set; }
        public string RoleId { get; set; }
        public string ModuloId { get; set; }
        public string? EstadoId { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string UsuarioRegistro { get; set; } = null!;
        public DateTime? FechaModifico { get; set; }
        public string? UsuarioModifico { get; set; }
        public Modulo Modulo { get; set; }
        public DataAccess.Models.Estados.Estados? Estado { get; set; }
    }
}