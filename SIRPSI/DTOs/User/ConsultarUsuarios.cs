using DataAccess.Models.Documents;
using DataAccess.Models.Estados;
using DataAccess.Models.Status;
using DataAccess.Models.WorkPlace;
using Microsoft.AspNetCore.Identity;

namespace SIRPSI.DTOs.User
{
    public class ConsultarUsuarios
    {
        public string? Id { get; set; }
        public string? idTipoDocumento { get; set; }
        public TiposDocumento? tipoDocumento { get; set; }
        public string? cedula { get; set; }
        public string? correo { get; set; }
        public string? telefono { get; set; }
        public string? idEmpresa { get; set; }
        public Empresas? empresa { get; set; }
        public string? nombreUsuario { get; set; }
        public string? apellidosUsuario { get; set; }
        public string? idEstado { get; set; }
        public Estados? estado { get; set; }
        public string? IdRol { get; set; }
        public IdentityRole? role { get; set; }
        public List<UserWorkPlace>? workPlaces { get; set; }
    }
}
