using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.EconomicActivity
{
    [Table("ActividadEconomica", Schema = "sirpsi")]
    public partial class ActividadEconomica
    {
        public string Id { get; set; } = null!;
        public int ClaseRiesgo { get; set; }
        public string CodigoCIIU { get; set; } = null!;
        public string CodigoAdicional { get; set; }
        public string Descripcion { get; set; }
    }
}