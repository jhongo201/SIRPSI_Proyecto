using System.ComponentModel.DataAnnotations;

namespace SIRPSI.DTOs.ModuleUserRole
{
    public class EliminarModuloUserRole
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string? Id { get; set; }
    }
}
