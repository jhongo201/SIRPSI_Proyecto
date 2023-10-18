using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.EconomicSectors
{
    [Table("SectoresEconomicos", Schema = "sirpsi")]
    public partial class SectoresEconomicos
    {
        public string Id { get; set; } = null!;
        public string Nombre { get; set; } = null!;
    }
}