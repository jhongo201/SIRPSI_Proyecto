using Microsoft.IdentityModel.Tokens;
using System;
using System.ComponentModel.DataAnnotations;

namespace SIRPSI.DTOs.User
{
    //Clase que recibe los datos del cliente, cuando se envian las credenciales
    public class UserCredentials
    {
        public string? IdTypeDocument { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string? Document { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string? Password { get; set; }
        public string? IdCompany { get; set; }
        public string? IdCountry{ get; set; }    
        public string? Names { get; set; }
        public string? Surnames { get; set; }
        public string? Email { get; set; }
        public string? EmailAux { get; set; }
        public string? PhoneNumber { get; set; }
        public string? PhoneNumberAux { get; set; }
        public string? IdRol { get; set; }
        public string? IdEstado { get; set; }
        public string? Role { get; set; }
        public string? Id { get; set; }
        public string? IdWorkCenter { get; set; } = null;
        public string? CodeActivation { get; set; }
        public string? IdOccupationProfession { get; set; } = null;
        public bool? HaveDisability { get; set; } = null;
        public bool? ReadingWritingSkills { get; set; } = null;
        public string? Clasificacion { get; set; }

    }
}