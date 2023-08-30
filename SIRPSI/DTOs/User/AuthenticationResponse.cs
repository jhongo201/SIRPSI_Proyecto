using DataAccess.Models.Status;
using DataAccess.Models.Users;
using SIRPSI.DTOs.User.Roles;

namespace SIRPSI.DTOs.User
{
    //Response login user
    public class AuthenticationResponse
    {
        public string? Token { get; set; }
        public DateTime Expiration { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string EmpresaId { get; set; }
        public Empresas Empresa { get; set; }
        public string Id { get; set; }
        public string EstadoId { get; set; }
        public UserCredentials User { get; set; }
        public List<RutasAsignadas> RutasAsignadas { get; set; }
    }
}
