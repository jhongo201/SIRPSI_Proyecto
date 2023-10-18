﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.PsychologistsCenterWork
{
    [Table("PsicologosCentroTrabajo", Schema = "sirpsi")]
    public class PsicologosCentroTrabajo
    {
        [Key]
        public string Id { get; set; }
        public string IdUser { get; set; }
        public string IdCentroTrabajo { get; set; }
    }
}
