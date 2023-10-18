using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.Reports
{
    [Table("Reportes", Schema = "sirpsi")]
    public partial class Reportes
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public int Tipo { get; set; }
    }
}