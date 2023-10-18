using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.Schedule
{
    [Table("Horario", Schema = "sirpsi")]
    public partial class Horario
    {
        public string Id { get; set; }
        public string EmpresaId { get; set; }
        public string DiaSemanaId { get; set; } = null!;
        public System.TimeSpan HoraInicio { get; set; }
        public System.TimeSpan HoraFin { get; set; }
        public DateTime? Fecha { get; set; } = null;
        public string? Nota { get; set; } = null!;

    }
}
