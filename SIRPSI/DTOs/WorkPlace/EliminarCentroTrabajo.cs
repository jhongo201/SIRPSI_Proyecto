using System.ComponentModel.DataAnnotations;

namespace SIRPSI.DTOs.WorkPlace
{
    public class EliminarCentroTrabajo
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string? Id { get; set; }
    }
}
