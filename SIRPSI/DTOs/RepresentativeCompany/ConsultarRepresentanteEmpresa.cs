using DataAccess.Models.Documents;
using System.ComponentModel.DataAnnotations;

namespace SIRPSI.DTOs.RepresentativeCompany
{
    public class ConsultarRepresentanteEmpresa
    {
        public string Id { get; set; } = null!;
        public string PrimerNombre { get; set; } = null!;
        public string? SegundoNombre { get; set; }
        public string PrimerApellido { get; set; } = null!;
        public string? SegundoApellido { get; set; }
        public string IdTipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public TiposDocumento? TipoDocumento { get; set; }
    }
}
