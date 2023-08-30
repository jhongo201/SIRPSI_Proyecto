﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.Estados
{
    [Table("Estados", Schema = "sirpsi")]
    public partial class Estados
    {
        
        public string Id { get; set; } = null!;

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdConsecutivo { get; set; }

        public string Nombre { get; set; } = null!;

        public string? Descripcion { get; set; }

        public DateTime FechaRegistro { get; set; }

        public string UsuarioRegistro { get; set; } = null!;

        public DateTime? FechaModifico { get; set; }

        public string? UsuarioModifico { get; set; }
    }
}
