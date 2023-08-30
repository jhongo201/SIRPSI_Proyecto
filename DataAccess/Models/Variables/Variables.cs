using DataAccess.Models.Status;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models.Users;

namespace DataAccess.Models.Variables
{
    [Table("Variables", Schema = "sirpsi")]
    public partial class Variables
    {
        [Key]
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public string? Modulo { get; set; }
        public string? Variable1 { get; set; }
        public string? Variable2 { get; set; }
        public string? Variable3 { get; set; }
        public string? Variable4 { get; set; }
    }
}
