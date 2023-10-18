using DataAccess.Models.Country;
using DataAccess.Models.Documents;
using DataAccess.Models.Estados;
using DataAccess.Models.Module;
using DataAccess.Models.OccupationProfession;
using DataAccess.Models.PsychologistsCenterWork;
using DataAccess.Models.Reports;
using DataAccess.Models.Rols;
using DataAccess.Models.Status;
using DataAccess.Models.Users;
using DataAccess.Models.WorkPlace;
using Microsoft.AspNetCore.Identity;
using SIRPSI.DTOs.User;
using System.ComponentModel.DataAnnotations;

namespace SIRPSI.DTOs.Reports
{
    public class ConsultarHistorialRetirosReintegros
    {
        public string Id { get; set; }
        public string IdUser { get; set; }
        public string? IdCentroTrabajo { get; set; } = null;
        public string UserRegister { get; set; }
        public DateTime Fecha { get; set; }
        public string IdEstado { get; set; }
        public int Tipo { get; set; }
        public UsuariosHistorialRetirosReintegros? User { get; set; }
        public CentroTrabajo? CentroTrabajo { get; set; }
        public UsuariosHistorialRetirosReintegros? RegisterUser { get; set; }
        public Estados? Estado { get; set; }
    }
    public class UsuariosHistorialRetirosReintegros
    {
        public string? Id { get; set; }
        public string? IdTipoDocumento { get; set; }
        public string? TipoDocumento { get; set; }
        public string? Documento { get; set; }
        public string? Correo { get; set; }
        public string? Telefono { get; set; }
        public string? IdEmpresa { get; set; }
        public string? Empresa { get; set; }
        public string? Nombres { get; set; }
        public string? Apellidos { get; set; }
        public string? NombreCompleto { get; set; }
        public string? IdRol { get; set; }
        public string? Role { get; set; }
    }
}