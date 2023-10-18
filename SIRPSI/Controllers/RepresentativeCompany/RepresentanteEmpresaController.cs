using AutoMapper;
using DataAccess.Context;
using DataAccess.Models.Ministry;
using DataAccess.Models.RepresentativeCompany;
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
using SIRPSI.Core.Helper;
using SIRPSI.DTOs.Ministry;
using SIRPSI.DTOs.RepresentativeCompany;
using SIRPSI.DTOs.WorkPlace;
using SIRPSI.Helpers.Answers;
using System.Net;
using System.Security.Claims;

namespace SIRPSI.Controllers.RepresentativeCompany
{
    [Route("api/RepresentanteEmpresa")]
    [ApiController]
    [EnableCors("CorsApi")]
    public class RepresentanteEmpresaController : ControllerBase
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
        public RepresentanteEmpresaController(AppDbContext context,
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
        [HttpGet("ConsultarRepresentanteEmpresa", Name = "consultarRepresentanteEmpresa")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<object>> Get()
        {
            try
            {
                //Consulta el estados
                var centrosDeTrabajo = await context.representanteEmpresa.Select(x => new
                {
                    x.Id,
                    //x.Nombre,
                    //x.Descripcion,
                    //x.Nit,
                    //x.FechaRegistro,
                    //x.FechaModifico
                }).ToListAsync();

                if (centrosDeTrabajo == null)
                {
                    //Visualizacion de mensajes al usuario del aplicativo
                    return NotFound(new General()
                    {
                        title = "Consultar RepresentanteEmpresa",
                        status = 404,
                        message = "RepresentanteEmpresa no encontrado"
                    });
                }
                //Retorno de los datos encontrados
                return centrosDeTrabajo;
            }
            catch (Exception ex)
            {
                //Registro de errores
                logger.LogError("RepresentanteEmpresa " + ex.Message.ToString() + " - " + ex.StackTrace);
                return BadRequest(new General()
                {
                    title = "Consultar RepresentanteEmpresa",
                    status = 400,
                    message = "Contacte con el administrador del sistema"
                });
            }
        }
        #endregion

        #region Registro
        [HttpPost("RegistrarRepresentanteEmpresa")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Post([FromBody] RegistrarRepresentanteEmpresa registrarRepresentanteEmpresa)
        {
            try
            {
                //Mapeo de datos en clases
                var centroTrabajo = mapper.Map<RepresentanteEmpresa>(registrarRepresentanteEmpresa);
                //Agregar datos al contexto
                context.Add(centroTrabajo);
                //Guardado de datos 
                await context.SaveChangesAsync();
                return Created("", new General()
                {
                    //Visualizacion de mensajes al usuario del aplicativo
                    title = "Registrar Representante",
                    status = 201,
                    message = "Representante creado"
                });
            }
            catch (Exception ex)
            {
                //Registro de errores
                logger.LogError("Registrar Representante " + ex.Message.ToString() + ex.StackTrace);
                return BadRequest(new General()
                {
                    title = "Registrar Representante",
                    status = 400,
                    message = "Contacte con el administrador del sistema"
                });
            }
        }
        #endregion
    }
}
