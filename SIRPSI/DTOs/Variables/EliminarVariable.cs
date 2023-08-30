using System.ComponentModel.DataAnnotations;

namespace SIRPSI.DTOs.Variables
{
    public class EliminarVariable
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string? Id { get; set; }
    }
}
