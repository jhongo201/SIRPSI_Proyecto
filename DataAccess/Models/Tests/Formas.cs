using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.Tests
{
    [Table("Formas", Schema = "sirpsi")]
    public partial class Formas
    {
        [Key]
        public string Id { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Nombre { get; set; }
        public string IdEstado { get; set; }
        public string IdUsuarioRegistra { get; set; }
    }
}
