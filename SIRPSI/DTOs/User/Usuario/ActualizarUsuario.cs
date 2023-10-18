using System.ComponentModel.DataAnnotations;

namespace SIRPSI.DTOs.User.Usuario
{
    public class ActualizarUsuario
    {
        public string? IdTypeDocument { get; set; } = null;
        public string? Document { get; set; } = null;
        public string? IdCompany { get; set; } = null;
        public string? IdCountry { get; set; } = null;
        public string? Names { get; set; } = null;
        public string? Surnames { get; set; } = null;
        public string? Email { get; set; } = null;
        public string? EmailAux { get; set; } = null;
        public string? PhoneNumber { get; set; } = null;
        public string? PhoneNumberAux { get; set; } = null;
        public string? IdRol { get; set; } = null;
        public string? IdEstado { get; set; } = null;
        public string? Role { get; set; } = null;
        public string? Id { get; set; }
        public string? IdWorkCenter { get; set; } = null;
        public string? CodeActivation { get; set; }
        public string? IdOccupationProfession { get; set; } = null;
        public bool? HaveDisability { get; set; } = null;
        public bool? ReadingWritingSkills { get; set; } = null;
    }
}
