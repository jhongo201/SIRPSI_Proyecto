using System.ComponentModel.DataAnnotations;

namespace SIRPSI.DTOs.Module
{
    public class ActualizarModulo
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
		public string Descripcion { get; set; }
		public string? Ruta { get; set; }
		public bool? TieneHijos { get; set; }
	}
}
