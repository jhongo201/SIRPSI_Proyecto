using System.ComponentModel.DataAnnotations;

namespace SIRPSI.DTOs.ModuleUserRole
{
    public class ActualizarModuloUserRole
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
		public string Descripcion { get; set; }
		public string? Ruta { get; set; }
		public bool? TieneHijos { get; set; }
	}
}
