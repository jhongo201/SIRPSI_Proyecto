using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.Tests
{
    [Table("Empleados", Schema = "dbo")]
    public partial class Employees
    {
        public string Id { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? nombre_completo { get; set; }
        public string? sexo { get; set; }
        public string? genero { get; set; }
        public string? otro_genero { get; set; }
        public string? etnia { get; set; }
        public string? cual_indigena { get; set; }
        public string? discapacidad { get; set; }
        public string? cual_discapacidad { get; set; }
        public string? anio_nacimiento { get; set; }
        public string? lugar_residencia { get; set; }
        public string? zona { get; set; }
        public string? cual_rural { get; set; }
        public string? estado_civil { get; set; }
        public string? nivel_educativo { get; set; }
        public string? ocupacion { get; set; }
        public string? lugar_reidencia { get; set; }
        public string? estrado { get; set; }
        public string? tipo_vivienda { get; set; }
        public string? dependientes { get; set; }
        public string? lugar_trabajo { get; set; }
        public string? tiempo_laborado { get; set; }
        public string? cargo_empresa { get; set; }
        public string? seleccion_cargo { get; set; }
        public string? tiempoLavorado_Cargo { get; set; }
        public string? departamentoTrabajo { get; set; }
        public string? tipoContrato { get; set; }
        public string? horasTrabajadasDiarias { get; set; }
        public string? tipoSalario { get; set; }
        public string? id_desempleado { get; set; }
   
    }
}
