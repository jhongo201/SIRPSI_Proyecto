using DataAccess.Models.Status;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models.Users;

namespace DataAccess.Models.TypesPerson
{
    [Table("TiposPersona", Schema = "sirpsi")]
    public partial class TiposPersona
    {
        [Key]
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string? Descripcion { get; set; }
    }
}
