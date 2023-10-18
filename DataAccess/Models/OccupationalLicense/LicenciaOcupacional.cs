using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.OccupationalLicense
{
    [Table("LicenciaOcupacional", Schema = "sirpsi")]
    public partial class LicenciaOcupacional
    {
        public string Id { get; set; }
        public string UsuarioId { get; set; }
        public string Numero { get; set; } = null!;
        public DateTime FechaExpedicion { get; set; }
    }
}
