using AutoMapper;
using DataAccess.Context;
using DataAccess.Models.Estados;
using DataAccess.Models.PsychologistsCenterWork;
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
using SIRPSI.Core.Helper;
using SIRPSI.DTOs.Status;
using SIRPSI.DTOs.User;
using SIRPSI.DTOs.Variables;
using SIRPSI.DTOs.WorkPlace;
using SIRPSI.Helpers.Answers;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace SIRPSI.Controllers.WorkPlace
{
    [Route("api/userWorkPlace")]
    [ApiController]
    [EnableCors("CorsApi")]
    public class UserWorkPlaceController : ControllerBase
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
        public UserWorkPlaceController(AppDbContext context,
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
        [HttpGet("ConsultarCentroDeTrabajoUsuario")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<object>> Get(string? user)
        {
            var centrosDeTrabajoUsuario = (from data in (await context.centroTrabajo.ToListAsync())
                                           where context.psicologosCentroTrabajo.Where(x => x.IdCentroTrabajo == data.Id && x.IdUser == user).Count() > 0
                                           select new CentroTrabajo
                                           {
                                               Id = data.Id,
                                               Nombre = data.Nombre,
                                               Descripcion = data.Descripcion,
                                               IdEmpresa = data.IdEmpresa,
                                               FechaModifico = data.FechaModifico,
                                               UsuarioModifico = data.UsuarioModifico,
                                               UsuarioRegistro = data.UsuarioRegistro,
                                               FechaRegistro = data.FechaRegistro,
                                               IdEstado = data.IdEstado,
                                               IdUsuario = data.IdUsuario,
                                               Empresa = (context.empresas.Where(x => x.Id == data.IdEmpresa)).FirstOrDefault(),
                                               Estados = (context.estados.Where(x => x.Id == data.IdEstado)).FirstOrDefault(),
                                               //Usuario = (context.AspNetUsers.Where(x => x.Id == data.IdUsuario)).FirstOrDefault(),
                                           }).ToList();
            return centrosDeTrabajoUsuario;
        }

        [HttpGet("ConsultarUsuariosCentroDeTrabajo")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<object>> GetByWorkCenter(string? workCenter = null)
        {
            var usersWorkCenter = (from data in await context.userWorkPlace.Where(x => (workCenter != null ? x.WorkPlaceId == workCenter : 1 == 1)).ToListAsync()
                                   select new ConsultarUsuariosCentroTrabajo()
                                   {
                                       WorkplaceId = data.WorkPlaceId,
                                       UserId = data.UserId,
                                       User = (context.AspNetUsers.Where(x => x.Id == data.UserId).FirstOrDefault()),
                                       Workplace = (context.centroTrabajo.Where(x => x.Id == data.WorkPlaceId).FirstOrDefault()),
                                   });

            var usuariosConsultados = (from data in usersWorkCenter
                                       select new ConsultarUsuarios()
                                       {
                                           Id = data.User.Id,
                                           idTipoDocumento = data.User.TypeDocument,
                                           tipoDocumento = (context.tiposDocumento.Where(x => x.Id == data.User.TypeDocument).FirstOrDefault()),
                                           cedula = data.User.Document,
                                           correo = data.User.Email,
                                           telefono = data.User.PhoneNumber,
                                           idEmpresa = data.User.IdCompany,
                                           empresa = (context.empresas.Where(x => x.Id == data.User.IdCompany).FirstOrDefault()),
                                           nombreUsuario = data.User.Names,
                                           apellidosUsuario = data.User.Surnames,
                                           idEstado = data.User.Status,
                                           estado = (context.estados.Where(x => x.Id == data.User.Status).FirstOrDefault()),
                                           IdRol = data.User.IdRol,
                                           role = (context.Roles.Where(x => x.Id == data.User.IdRol).FirstOrDefault()),
                                           trabajadorCentroTrabajo = (context.userWorkPlace.Where(x => x.UserId == data.User.Id).FirstOrDefault())
                                       }).ToList();

            return Ok(usuariosConsultados);
        }
        #endregion

        #region Registro
        [HttpPost("RegistrarCentroDeTrabajoUsuario")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Post([FromBody] RegistrarCentroTrabajoUsuario registrarCentroTrabajoUsuario)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    IEnumerable<Claim> claims = identity.Claims;
                }
                var documento = identity.FindFirst("documento").Value.ToString();
                var usuario = context.AspNetUsers.Where(u => u.Document.Equals(documento)).FirstOrDefault();

                var userWorkCenter = await context.userWorkPlace.Where(x => x.UserId == registrarCentroTrabajoUsuario.User && x.WorkPlaceId == registrarCentroTrabajoUsuario.Workplace).FirstOrDefaultAsync();
                if (userWorkCenter != null)
                {
                    //Visualizacion de mensajes al usuario del aplicativo
                    return NotFound(new General()
                    {
                        title = "Asignar centro de trabajo",
                        status = 404,
                        message = "Este usuario ya esta asignado a ese centro de trabajo"
                    });
                }
                var centroTrabajoUser = mapper.Map<UserWorkPlace>(registrarCentroTrabajoUsuario);
                //Valores asignados
                centroTrabajoUser.Id = Guid.NewGuid().ToString();
                centroTrabajoUser.UserId = registrarCentroTrabajoUsuario.User;
                centroTrabajoUser.WorkPlaceId = registrarCentroTrabajoUsuario.Workplace;

                //Agregar datos al contexto
                context.Add(centroTrabajoUser);
                context.centroTrabajoHistorial.Add(new CentroTrabajoHistorial()
                {
                    Id = Guid.NewGuid().ToString(),
                    IdUser = centroTrabajoUser.UserId,
                    IdCentroTrabajo = centroTrabajoUser.WorkPlaceId,
                    Fecha = DateTime.Now,
                    UserRegister = usuario.Id,
                    IdEstado = "fc6b0f2f-0641-40e5-b7ac-3eed551f5531",
                    Tipo = 2,
                });
                //Guardado de datos 
                await context.SaveChangesAsync();
                return Created("", new General()
                {
                    //Visualizacion de mensajes al usuario del aplicativo
                    title = "Registrar centro de trabajo",
                    status = 201,
                    message = "Centro de trabajo creado"
                });
            }
            catch (Exception e)
            {
                return Created("", new General()
                {
                    //Visualizacion de mensajes al usuario del aplicativo
                    title = "Registrar centro de trabajo",
                    status = 401,
                    message = "Ha ocurrido un error inesperado"
                });
            }
        }
        #endregion

        #region Eliminar
        [HttpDelete("EliminarCentroTrabajoUsuario")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<object>> Delete(string? centroTrabajo, string? user)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    IEnumerable<Claim> claims = identity.Claims;
                }
                var documento = identity.FindFirst("documento").Value.ToString();
                var usuario = context.AspNetUsers.Where(u => u.Document.Equals(documento)).FirstOrDefault();

                var centrosDeTrabajo = await context.userWorkPlace.Where(x => x.WorkPlaceId == centroTrabajo && x.UserId == user).FirstOrDefaultAsync();
                if (centrosDeTrabajo == null)
                {
                    return NotFound(new General()
                    {
                        title = "Consultar centro de trabajo",
                        status = 404,
                        message = "Centros de trabajo no encontrados"
                    });
                }
                context.userWorkPlace.Remove(centrosDeTrabajo);
                context.centroTrabajoHistorial.Add(new CentroTrabajoHistorial()
                {
                    Id = Guid.NewGuid().ToString(),
                    IdUser = centrosDeTrabajo.UserId,
                    IdCentroTrabajo = centrosDeTrabajo.WorkPlaceId,
                    Fecha = DateTime.Now,
                    UserRegister = usuario.Id,
                    IdEstado = "62aea53c-ea76-4994-8d44-129936574bf5",
                    Tipo = 2,
                });
                await context.SaveChangesAsync();

                return Ok(new General()
                {
                    title = "Eliminar centro de trabajo",
                    status = 200,
                    message = "El usuario ya no esta asignado a este centro de trabajo"
                });
            }
            catch (Exception ex)
            {
                logger.LogError("Consultar centro de trabajo " + ex.Message.ToString() + " - " + ex.StackTrace);
                return BadRequest(new General()
                {
                    title = "Consultar centro de trabajo",
                    status = 400,
                    message = "Contacte con el administrador del sistema"
                });
            }
        }
        #endregion

    }
}
