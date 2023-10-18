using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.ReportsRole
{
    [Table("ReportesRole", Schema = "sirpsi")]
    public partial class ReportesRole
    {
        public string Id { get; set; }
        public string ReporteId { get; set; }
        public string RoleId { get; set; }
    }
}