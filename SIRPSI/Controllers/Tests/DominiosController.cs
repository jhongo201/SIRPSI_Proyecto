using AutoMapper;
using DataAccess.Context;
using DataAccess.Models.Tests;
using EmailServices;
using EvertecApi.Log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIRPSI.DTOs.Tests;
using SIRPSI.DTOs.User;
using SIRPSI.Helpers.Answers;
using SIRPSI.Settings;
using System.Security.Claims;

namespace SIRPSI.Controllers.Tests
{
    [Route("api/[controller]")]
    [ApiController]
    public class DominiosController : ControllerBase
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
        public DominiosController(AppDbContext context,
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
        [HttpGet("ConsultarDominios", Name = "ConsultarDominios")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<object>> GetDomains()
        {
            try
            {
                //Consulta el tipo documento
                var dominioConsultado = (from data in (await context.dominios.ToListAsync())
                                         orderby data.Id ascending
                                         select new ConsultarDominiosDto()
                                         {
                                             Id = data.Id,
                                             Nombre = data.Nombre,
                                             IdEstado = data.IdEstado,

                                         }).ToList();

                if (dominioConsultado == null)
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
                return dominioConsultado;
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
