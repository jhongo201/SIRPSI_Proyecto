using DataAccess.Models.Users;
using System.ComponentModel.DataAnnotations;

namespace SIRPSI.DTOs.OccupationalLicense
{
    public class ConsultarLicenciaOcupacional
    {
        public string Id { get; set; }
        public string UsuarioId { get; set; }
        public string Numero { get; set; } = null!;
        public DateTime FechaExpedicion { get; set; }
        public AspNetUsers? Usuario { get; set; }
    }
}
