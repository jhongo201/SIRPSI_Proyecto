using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace DataAccess.Models.Tests
{
    [Table("Dominios", Schema = "sirpsi")]
    public partial class Dominios 
    {
        [Key]
        public string Id { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Nombre { get; set; }
        public string IdEstado { get; set; }
        public string IdUsuarioRegistra { get; set; }
        public string Forma { get; set; }

        public static implicit operator string?(Dominios? v)
        {
            throw new NotImplementedException();
        }
    }
}
