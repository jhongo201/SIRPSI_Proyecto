using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.Clasificacion
{
    [Table("ClasificacionTrabajador", Schema = "dbo")]
    public partial class Clasificacion
    {
        [Key]
        public string Id { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Nombre { get; set; }

    }
}
