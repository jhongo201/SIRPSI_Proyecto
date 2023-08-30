using System.ComponentModel.DataAnnotations;

namespace SIRPSI.DTOs.Module
{
    public class RegistrarModulo
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string? Ruta { get; set; }
        public bool? TieneHijos { get; set; }
    }
}
