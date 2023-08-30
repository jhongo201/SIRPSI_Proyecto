using DataAccess.Models.Module;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SIRPSI.DTOs.Module
{
    public class ConsultarModulo
    {
        public string? Id { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public string? IdEstado { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public string? UsuarioRegistro { get; set; } = null!;
        public DateTime? FechaModifico { get; set; }
        public string? UsuarioModifico { get; set; }
        public string? Ruta { get; set; }
        public bool? TieneHijos { get; set; }
        public DataAccess.Models.Estados.Estados? Estado { get; set; }
    }
}