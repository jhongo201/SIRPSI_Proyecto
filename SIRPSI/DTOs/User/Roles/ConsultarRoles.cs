using DataAccess.Models.Module;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SIRPSI.DTOs.User.Roles
{
    public class ConsultarRoles
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Status { get; set; }
        public string? Description { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string? UserRegistration { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? UserModify { get; set; }
        public DataAccess.Models.Estados.Estados? Estado { get; set; }
        public List<RutasAsignadas> RutasAsignadas { get; set; }
        public List<RutasAsignadasRole> RutasAsignadasRole { get; set; }
    }
    public class RutasAsignadas
    {
        public string Id { get; set; }
        public string ModuloId { get; set; }
        public string? Ruta { get; set; }
    }
    public class RutasAsignadasRole
    {
        public string Id { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public string? IdEstado { get; set; }
        public string? Ruta { get; set; }
        public bool? TieneHijos { get; set; }
        public bool? InUse { get; set; }
    }

}