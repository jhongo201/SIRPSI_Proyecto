﻿using System.ComponentModel.DataAnnotations;

namespace SIRPSI.DTOs.Module
{
    public class EliminarModulo
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string? Id { get; set; }
    }
}
