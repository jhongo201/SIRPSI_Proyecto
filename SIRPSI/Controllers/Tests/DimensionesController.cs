using AutoMapper;
using DataAccess.Context;
using EmailServices;
using EvertecApi.Log4net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIRPSI.DTOs.Tests;
using SIRPSI.Helpers.Answers;
using SIRPSI.Settings;

namespace SIRPSI.Controllers.Tests
{

    [Route("api/[controller]")]
    [ApiController]
    public class DimensionesController : Controller
    {
        #region Dependencias
        private readonly UserManager<IdentityUser> userManager;
        private readonly AppDbContext context;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly ILoggerManager logger;
        private readonly IMapper mapper;
        private readonly IEmailSender emailSender;
        private readonly StatusSettings statusSettings;
         
        //Constructor  
        public DimensionesController(AppDbContext context,
            IConfiguration configuration,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILoggerManager logger,
            IMapper mapper,
            IEmailSender emailSender,
            StatusSettings statusSettings)
        {
            this.context = context;
            this.configuration = configuration;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.mapper = mapper;
            this.emailSender = emailSender;
            this.statusSettings = statusSettings;
        }
        #endregion





        #region Consultar
        [HttpGet("ConsultarDimension", Name = "ConsultarDimension")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<object>> GetDimension()
        {
            try
            {
                //Consulta el tipo documento
                //var dimensionConsultada = (from data in (await context.dimensiones.ToListAsync())
                //                         orderby data.Id ascending
                //                         select new ConsultarDimensionesDto()
                //                         {
                //                             Id = data.Id,
                //                             Nombre = data.Nombre,
                //                             IdEstado = data.IdEstado,
                //                             IdUsuarioRegistra = data.IdUsuarioRegistra,
                //                             IdDominio = data.IdDominio,
                //                             Dominio = (context.dominios.Where(x => x.Id == data.IdDominio).FirstOrDefault()),

                //                         }).ToList();


                var dimensionConsultada = (from data in (await context.dimensiones.ToListAsync())
                                           join dominio in context.dominios on data.IdDominio equals dominio.Id
                                           orderby data.Id ascending
                                           select new ConsultarDimensionesDto()
                                           {
                                               Id = data.Id,
                                               Nombre = data.Nombre,
                                               IdEstado = data.IdEstado,
                                               IdUsuarioRegistra = data.IdUsuarioRegistra,
                                               IdDominio = data.IdDominio,
                                               Dominio = dominio.Nombre, // Agregar el nombre del dominio directamente
                                           }).ToList();

                if (dimensionConsultada == null)
                {
                    //Visualizacion de mensajes al usuario del aplicativo
                    return NotFound(new General()
                    {
                        title = "Consultar usuario",
                        status = 404,
                        message = "Usuarios no encontrados"
                    });
                }
                //Retorno de los datos encontrados
                return dimensionConsultada;
            }
            catch (Exception ex)
            {
                //Registro de errores
                logger.LogError("Consultar usuario " + ex.Message.ToString() + " - " + ex.StackTrace);
                return BadRequest(new General()
                {
                    title = "Consultar usuario",
                    status = 400,
                    message = "Contacte con el administrador del sistema"
                });
            }
        }
        #endregion


    }
}
