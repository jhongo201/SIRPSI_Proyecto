﻿using DataAccess.Models.Module;
using DataAccess.Models.Rols;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SIRPSI.DTOs.Variables
{
    public class ConsultarVariable
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public string? Modulo { get; set; }
        public string? Variable1 { get; set; }
        public string? Variable2 { get; set; }
        public string? Variable3 { get; set; }
        public string? Variable4 { get; set; }
        public IdentityRole? Role { get; set; }
    }
}