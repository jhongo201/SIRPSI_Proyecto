using DataAccess.Models.Module;
using DataAccess.Models.Reports;
using DataAccess.Models.Rols;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SIRPSI.DTOs.Reports
{
    public class ConsultarReportes
    {
        public string Id { get; set; }
        public string ReporteId { get; set; }
        public string RoleId { get; set; }
        public string? ReporteName { get; set; }
        public Reportes? Reporte { get; set; }
        public Roles? Role { get; set; }
    }
}