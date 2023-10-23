using AutoMapper;
using DataAccess.Context;
using DataAccess.Models.Tests;
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
    public class PreguntasController : Controller
    {
        #region Dependencias
        private readonly AppDbContext context;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly ILoggerManager logger;
        private readonly IMapper mapper;
        private readonly IEmailSender emailSender;
        private readonly StatusSettings statusSettings;

        //Constructor  
        public PreguntasController(AppDbContext context,
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
            this.signInManager = signInManager;
            this.logger = logger;
            this.mapper = mapper;
            this.emailSender = emailSender;
            this.statusSettings = statusSettings;
        }
        #endregion





        #region Consultar
        [HttpGet("ConsultarPreguntas", Name = "ConsultarPreguntas")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<object>> GetPreguntas()
        {
            try
            {
                var dimensionConsultada = (from data in (await context.preguntas.ToListAsync())
                    join dimensiones in context.dimensiones on data.IdDimension equals dimensiones.Id
                    join forma in context.forma on data.IdForma equals forma.Id
                    //join dominios in context.dominios on dimensiones.IdDominio equals dominios.Id

                    orderby data.Id ascending
                    select new ConsultarPreguntasDto()
                    {
                        Id = data.Id,
                        Pregunta = data.Pregunta,
                        Posicion = data.Posicion,
                        Siempre = data.Siempre,
                        CasiSiempre = data.CasiSiempre,
                        AlgunasVeces = data.AlgunasVeces,
                        CasiNunca = data.CasiNunca,
                        Nunca = data.Nunca,
                        IdDimension = data.IdDimension,
                        Dimension = dimensiones.Nombre,
                        Forma = forma.Nombre,
                        Dominio = dimensiones.IdDominio,
                        //DominioId = dominios.Nombre,
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
