using System.ComponentModel.DataAnnotations.Schema;

namespace SIRPSI.DTOs.Tests
{

    public class ConsultarDimensionesDto
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string? IdEstado { get; set; }
        public string? IdUsuarioRegistra { get; set; }
        public string? IdDominio { get; set; }
        public string? Dominio { get; set; }
        public int? valorA1 { get; set; }
        public int? valorA2 { get; set; }
    }
}
