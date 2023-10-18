using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.RepresentativeCompany
{
    [Table("RepresentanteEmpresa", Schema = "sirpsi")]
    public partial class RepresentanteEmpresa
    {
        public string Id { get; set; } = null!;
        public string PrimerNombre { get; set; } = null!;
        public string? SegundoNombre { get; set; }
        public string PrimerApellido { get; set; } = null!;
        public string? SegundoApellido { get; set; }
        public string IdTipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
    }
}