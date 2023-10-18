using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.PsychologistsCenterWork
{
    [Table("CentroTrabajoHistorial", Schema = "sirpsi")]
    public class CentroTrabajoHistorial
    {
        [Key]
        public string Id { get; set; }
        public string IdUser { get; set; }
        public string? IdCentroTrabajo { get; set; } = null;
        public string UserRegister { get; set; }
        public DateTime Fecha { get; set; }
        public string IdEstado { get; set; }
        public int Tipo { get; set; }
    }
}
