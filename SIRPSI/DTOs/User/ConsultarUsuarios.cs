using DataAccess.Models.Country;
using DataAccess.Models.Documents;
using DataAccess.Models.Estados;
using DataAccess.Models.OccupationProfession;
using DataAccess.Models.PsychologistsCenterWork;
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
        public string? correoAux { get; set; }
        public string? telefono { get; set; }
        public string? telefonoAux { get; set; }
        public string? idEmpresa { get; set; }
        public Empresas? empresa { get; set; }
        public string? nombreUsuario { get; set; }
        public string? apellidosUsuario { get; set; }
        public bool? tieneDiscapacidad { get; set; }
        public bool? habilidadesLectoEscritura { get; set; }
        public string? idEstado { get; set; }
        public Estados? estado { get; set; }
        public string? IdRol { get; set; }
        public string? IdPais { get; set; }
        public string? IdOcupacionProfesion { get; set; }
        public OcupacionProfesion? ocupacionProfesion { get; set; }
        public Pais? pais { get; set; }
        public IdentityRole? role { get; set; }
        public List<UserWorkPlace>? workPlaces { get; set; }
        public UserWorkPlace? trabajadorCentroTrabajo { get; set; }
        public PsicologosCentroTrabajo? psicologosCentroTrabajo { get; set; }
    }
    public class ValidarUsuarioRequest
    {
        public string TypeDocumentId { get; set; }
        public string Document { get; set; }
        public DateTime ExpeditionDate { get; set; }
    }
    public class UsuarioConsultado
    {
        public string? Id { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string TipoDocumento { get; set; }
        public string Documento { get; set; }
        public DateTime FechaExpedicion { get; set; }

    }
}
