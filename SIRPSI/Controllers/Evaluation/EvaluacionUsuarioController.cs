//using AutoMapper;
//using DataAccess.Context;
//using DataAccess.Models.Evaluation;
//using DataAccess.Models.Module;
//using DataAccess.Models.Users;
//using EmailServices;
//using EvertecApi.Log4net;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Cors;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Http.Extensions;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using SIRPSI.DTOs.Evaluation;
//using SIRPSI.DTOs.Module;
//using SIRPSI.DTOs.User;
//using SIRPSI.DTOs.WorkPlace;
//using SIRPSI.Helpers.Answers;
//using System.Security.Claims;

//namespace SIRPSI.Controllers.Evaluation
//{
//    [Route("api/evaluacionUsuario")]
//    [ApiController]
//    [EnableCors("CorsApi")]
//    public class EvaluacionUsuarioController : ControllerBase
//    {
//        #region Dependencias
//        private readonly UserManager<IdentityUser> userManager;
//        private readonly AppDbContext context;
//        private readonly IConfiguration configuration;
//        private readonly SignInManager<IdentityUser> signInManager;
//        private readonly ILoggerManager logger;
//        private readonly IMapper mapper;
//        private readonly IEmailSender emailSender;

//        //Constructor 
//        public EvaluacionUsuarioController(AppDbContext context,
//            IConfiguration configuration,
//            UserManager<IdentityUser> userManager,
//            SignInManager<IdentityUser> signInManager,
//            ILoggerManager logger,
//            IMapper mapper,
//            IEmailSender emailSender)
//        {
//            this.context = context;
//            this.configuration = configuration;
//            this.userManager = userManager;
//            this.signInManager = signInManager;
//            this.logger = logger;
//            this.mapper = mapper;
//            this.emailSender = emailSender;
//        }
//        #endregion

//        #region Consulta
//        [HttpGet("ConsultarUsuariosCentroDeTrabajo")]
//        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
//        public async Task<ActionResult<object>> GetByWorkCenter(string? workCenter = null)
//        {
//            var usersWorkCenter = (from data in await context.userWorkPlace.Where(x => (workCenter != null ? x.WorkPlaceId == workCenter : 1 == 1)).ToListAsync()
//                                   select new ConsultarUsuariosCentroTrabajo()
//                                   {
//                                       WorkplaceId = data.WorkPlaceId,
//                                       UserId = data.UserId,
//                                       User = (context.AspNetUsers.Where(x => x.Id == data.UserId).FirstOrDefault()),
//                                       Workplace = (context.centroTrabajo.Where(x => x.Id == data.WorkPlaceId).FirstOrDefault()),
//                                   });

//            var usuariosConsultados = (from data in usersWorkCenter
//                                       select new ConsultarUsuarios()
//                                       {
//                                           Id = data.User.Id,
//                                           idTipoDocumento = data.User.TypeDocument,
//                                           tipoDocumento = (context.tiposDocumento.Where(x => x.Id == data.User.TypeDocument).FirstOrDefault()),
//                                           cedula = data.User.Document,
//                                           correo = data.User.Email,
//                                           telefono = data.User.PhoneNumber,
//                                           idEmpresa = data.User.IdCompany,
//                                           empresa = (context.empresas.Where(x => x.Id == data.User.IdCompany).FirstOrDefault()),
//                                           nombreUsuario = data.User.Names,
//                                           apellidosUsuario = data.User.Surnames,
//                                           idEstado = data.User.Status,
//                                           estado = (context.estados.Where(x => x.Id == data.User.Status).FirstOrDefault()),
//                                           IdRol = data.User.IdRol,
//                                           role = (context.Roles.Where(x => x.Id == data.User.IdRol).FirstOrDefault()),
//                                           trabajadorCentroTrabajo = (context.userWorkPlace.Where(x => x.UserId == data.User.Id).FirstOrDefault())
//                                       }).ToList();

//            return Ok(usuariosConsultados);
//        }
//        #endregion

//        #region Registro
//        [HttpPost("RegistrarEvaluacion")]
//        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
//        public async Task<ActionResult> Post([FromBody] RegistrarEvaluacion registrarEvaluacion)
//        {
//            try
//            {
//                var identity = HttpContext.User.Identity as ClaimsIdentity;
//                if (identity != null)
//                {
//                    IEnumerable<Claim> claims = identity.Claims;
//                }
//                var documento = identity.FindFirst("documento").Value.ToString();
//                var Evaluacion = identity.FindFirst("rol").Value.ToString();
//                var usuario = context.AspNetUsers.Where(u => u.Document.Equals(documento)).FirstOrDefault();
//                if (usuario == null)
//                {
//                    return NotFound(new General()
//                    {
//                        title = "Registrar Evaluacion",
//                        status = 404,
//                        message = "Usuario no encontrado"
//                    });
//                }
//                var evaluacion = mapper.Map<Evaluacion>(registrarEvaluacion);
//                evaluacion.Id = Guid.NewGuid().ToString();
//                evaluacion.UsuarioRegistra = usuario.Id;
//                evaluacion.FechaCreacion = DateTime.Now;

//                context.Add(evaluacion);
//                await context.SaveChangesAsync();

//                return Created("", new General()
//                {
//                    title = "Registrar evaluación",
//                    status = 201,
//                    message = "Evaluación creada"
//                });
//            }
//            catch (Exception ex)
//            {
//                //Registro de errores
//                logger.LogError("Registrar evaluación " + ex.Message.ToString() + " - " + ex.StackTrace);
//                return BadRequest(new General()
//                {
//                    title = "Registrar evaluación",
//                    status = 400,
//                    message = "Contacte con el administrador del sistema"
//                });
//            }
//        }
//        #endregion
//    }
//}