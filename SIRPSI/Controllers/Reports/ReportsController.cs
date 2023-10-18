using AutoMapper;
using DataAccess.Context;
using DataAccess.Models.Estados;
using DataAccess.Models.Module;
using DataAccess.Models.ModuleUserRole;
using DataAccess.Models.Rols;
using DataAccess.Models.Users;
using DataAccess.Models.WorkPlace;
using EmailServices;
using EvertecApi.Log4net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIRPSI.DTOs.Companies;
using SIRPSI.DTOs.Module;
using SIRPSI.DTOs.ModuleUserRole;
using SIRPSI.DTOs.Reports;
using SIRPSI.DTOs.User;
using SIRPSI.Helpers.Answers;
using System.Security.Claims;

namespace SIRPSI.Controllers.Reports
{
    [Route("api/reportes")]
    [ApiController]
    [EnableCors("CorsApi")]
    public class ReportsController : ControllerBase
    {
        #region Dependencias
        private readonly UserManager<IdentityUser> userManager;
        private readonly AppDbContext context;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly ILoggerManager logger;
        private readonly IMapper mapper;
        private readonly IEmailSender emailSender;

        //Constructor 
        public ReportsController(AppDbContext context,
            IConfiguration configuration,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILoggerManager logger,
            IMapper mapper,
            IEmailSender emailSender)
        {
            this.context = context;
            this.configuration = configuration;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.mapper = mapper;
            this.emailSender = emailSender;
        }
        #endregion

        #region Consulta
        [HttpGet("ConsultarTiposReportesRole", Name = "ConsultarTiposReportesRole")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<object>> GetByTypeReports(string role)
        {
            try
            {
                //Consulta el rol
                var rol = (from data in (await context.reportesRole.ToListAsync())
                           where data.RoleId == role
                           select new ConsultarReportes
                           {
                               Id = data.Id,
                               RoleId = data.RoleId,
                               ReporteId = data.ReporteId,
                               ReporteName = (context.reportes.Where(x => x.Id == data.ReporteId).FirstOrDefault().Nombre),
                               //Role = (context.AspNetRoles.Where(x => x.Id == data.RoleId).FirstOrDefault()),
                           }).ToList();

                if (rol == null)
                {
                    return NotFound(new General()
                    {
                        title = "Consultar tipos de reporte",
                        status = 404,
                        message = "Reporte no encontrado"
                    });
                }
                return rol;
            }
            catch (Exception ex)
            {
                logger.LogError("Consultar tipos de reporte " + ex.Message.ToString() + " - " + ex.StackTrace);
                return BadRequest(new General()
                {
                    title = "Consultar tipos de reporte",
                    status = 400,
                    message = "Contacte con el administrador del sistema"
                });
            }
        }

        [HttpGet("ConsultarHistorialRetirosReintegros", Name = "ConsultarHistorialRetirosReintegros")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<object>> GetHistoryWithdrawalsWithdrawals(int? type = 2)
        {
            try
            {
                //Consulta el rol
                var rol = (from data in (await context.centroTrabajoHistorial.ToListAsync())
                           where data.Tipo == type
                           select new ConsultarHistorialRetirosReintegros
                           {
                               Id = data.Id,
                               IdUser = data.IdUser,
                               IdCentroTrabajo = data.IdCentroTrabajo,
                               UserRegister = data.UserRegister,
                               Fecha = data.Fecha,
                               IdEstado = data.IdEstado,
                               Tipo = data.Tipo,
                               User = (from user in (context.AspNetUsers)
                                       where user.Id == data.IdUser
                                       select new UsuariosHistorialRetirosReintegros
                                       {
                                           Id = user.Id,
                                           IdTipoDocumento = user.TypeDocument,
                                           TipoDocumento = (context.tiposDocumento.Where(x => x.Id == user.TypeDocument).FirstOrDefault().Nombre),
                                           Documento = user.Document,
                                           Correo = user.Email,
                                           Telefono = user.PhoneNumber,
                                           IdEmpresa = user.IdCompany,
                                           Empresa = (context.empresas.Where(x => x.Id == user.IdCompany).FirstOrDefault().Nombre),
                                           Nombres = user.Names,
                                           Apellidos = user.Surnames,
                                           NombreCompleto = user.Names + ' ' + user.Surnames,
                                           IdRol = user.IdRol,
                                           Role = (context.Roles.Where(x => x.Id == user.IdRol).FirstOrDefault().Name),
                                       }).FirstOrDefault(),
                               CentroTrabajo = (context.centroTrabajo.Where(x => x.Id == data.IdCentroTrabajo).FirstOrDefault()),
                               Estado = (context.estados.Where(x => x.Id == data.IdEstado).FirstOrDefault()),
                               RegisterUser = (from user in (context.AspNetUsers)
                                               where user.Id == data.UserRegister
                                               select new UsuariosHistorialRetirosReintegros
                                               {
                                                   Id = user.Id,
                                                   IdTipoDocumento = user.TypeDocument,
                                                   TipoDocumento = (context.tiposDocumento.Where(x => x.Id == user.TypeDocument).FirstOrDefault().Nombre),
                                                   Documento = user.Document,
                                                   Correo = user.Email,
                                                   Telefono = user.PhoneNumber,
                                                   IdEmpresa = user.IdCompany,
                                                   Empresa = (context.empresas.Where(x => x.Id == user.IdCompany).FirstOrDefault().Nombre),
                                                   Nombres = user.Names,
                                                   Apellidos = user.Surnames,
                                                   NombreCompleto = user.Names + ' ' + user.Surnames,
                                                   IdRol = user.IdRol,
                                                   Role = (context.Roles.Where(x => x.Id == user.IdRol).FirstOrDefault().Name),
                                               }).FirstOrDefault(),
                           }).ToList();

                if (rol == null)
                {
                    return NotFound(new General()
                    {
                        title = "Consultar Historial de retiros y reintegros de Trabajadores",
                        status = 404,
                        message = "Reporte no encontrado"
                    });
                }
                return rol;
            }
            catch (Exception ex)
            {
                logger.LogError("Consultar Historial de retiros y reintegros de Trabajadores " + ex.Message.ToString() + " - " + ex.StackTrace);
                return BadRequest(new General()
                {
                    title = "Consultar Historial de retiros y reintegros de Trabajadores",
                    status = 400,
                    message = "Contacte con el administrador del sistema"
                });
            }
        }
        #endregion
    }
}