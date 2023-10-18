using System.ComponentModel.DataAnnotations;

namespace SIRPSI.DTOs.OccupationalLicense
{
    public class ActualizarLicenciaOcupacional
    {
        public string UsuarioId { get; set; }
        public string Numero { get; set; } = null!;
        public DateTime FechaExpedicion { get; set; }
    }
}

