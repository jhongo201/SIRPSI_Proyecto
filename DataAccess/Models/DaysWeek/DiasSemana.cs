using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.DaysWeek
{
    [Table("DiasSemana", Schema = "sirpsi")]
    public partial class DiasSemana
    {
        public string Id { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
    }
}