﻿using System.ComponentModel.DataAnnotations;

namespace SIRPSI.DTOs.User
{
    public class RecoverPassword
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string UserId { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string CodePassword { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string ConfirmPassword { get; set; }
   
    }
    public class ActivateUserRequest
    {
        public string Company { get; set; }
        public string Document { get; set; }
        public string Code { get; set; }
    }
}
