using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.OccupationProfession
{
    [Table("OcupacionProfesion", Schema = "sirpsi")]
    public class OcupacionProfesion
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
    }
}
