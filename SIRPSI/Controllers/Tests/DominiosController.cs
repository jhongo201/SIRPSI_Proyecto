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
                                             IdUsuarioRegistra = data.IdUsuarioRegistra,
                                             Forma = data.Forma,

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









        //#region Registro
        //[HttpPost("RegistrarUsuario")]
        ////[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //public async Task<ActionResult> Post([FromBody] Dominios dominio)
        //{
        //    try
        //    {
        //        //Claims de usuario - Enviados por token
        //        var identity = HttpContext.User.Identity as ClaimsIdentity;

        //        if (identity != null)
        //        {
        //            IEnumerable<Claim> claims = identity.Claims;
        //        }

        //        //Consulta el documento con los claims
        //        var documento = identity.FindFirst("documento").Value.ToString();

        //        //Consulta el rol con los claims
        //        var roles = identity.FindFirst("rol").Value.ToString();

        //        //Consulta de usuarios por documento
        //        var usuario = context.AspNetUsers.Where(u => u.Document.Equals(documento)).FirstOrDefault();

        //        if (usuario == null)
        //        {
        //            return NotFound(new General()
        //            {
        //                title = "Registrar tipo documento",
        //                status = 404,
        //                message = "Usuario no encontrado"
        //            });
        //        }

        //        //Consultar estados
        //        var estado = await context.estados.Where(x => x.Id.Equals(registrarTipoDocumento.IdEstado)).FirstOrDefaultAsync();

        //        if (estado == null)
        //        {
        //            return NotFound(new General()
        //            {
        //                title = "Registrar tipo documento",
        //                status = 404,
        //                message = "Estado no encontrado"
        //            });
        //        }

        //        //Obtiene la url del servicio
        //        string getUrl = HttpContext.Request.GetDisplayUrl();

        //        //Consulta de roles por id de usuario

        //        var rolesList = new List<string>();

        //        //Verifica los roles
        //        var list = roles.Split(',').ToList();

        //        foreach (var i in list)
        //        {
        //            var result = context.AspNetRoles.Where(r => r.Id.Equals(i)).Select(x => x.Description).FirstOrDefault();

        //            if (result != null)
        //            {
        //                rolesList.Add(result.ToString());
        //            }
        //        }

        //        if (rolesList == null)
        //        {
        //            return NotFound(new General()
        //            {
        //                title = "Registrar tipo documento",
        //                status = 404,
        //                message = "Roles no encontrados"
        //            });
        //        }

        //        //Revisa los permisos de usuario
        //        var permisos = await context.permisosXUsuario.Where(x => x.Vista.Equals(getUrl) && x.IdUsuario.Equals(usuario.Id)).ToListAsync();

        //        //Consulta si tiene el permiso
        //        var permitido = permisos.Select(x => x.Registrar.Equals(true)).FirstOrDefault();

        //        //Si es permitido
        //        if (true)
        //        {
        //            //Mapeo de datos en clases
        //            var tipoEmpresa = mapper.Map<TiposDocumento>(registrarTipoDocumento);
        //            //Valores asignados
        //            tipoEmpresa.Id = Guid.NewGuid().ToString();
        //            tipoEmpresa.Nombre = registrarTipoDocumento.Nombre != null ? registrarTipoDocumento.Nombre : "";
        //            tipoEmpresa.Descripcion = registrarTipoDocumento.Descripcion;
        //            tipoEmpresa.IdEstado = estado.Id;
        //            tipoEmpresa.UsuarioRegistro = usuario.Document != null ? usuario.Document : ""; ;
        //            tipoEmpresa.FechaRegistro = DateTime.Now.ToDateTimeZone().DateTime;
        //            tipoEmpresa.FechaModifico = null;
        //            tipoEmpresa.UsuarioModifico = null;

        //            //Agregar datos al contexto
        //            context.Add(tipoEmpresa);
        //            //Guardado de datos 
        //            await context.SaveChangesAsync();

        //            return Created("", new General()
        //            {
        //                //Visualizacion de mensajes al usuario del aplicativo
        //                title = "Registrar tipo documento",
        //                status = 201,
        //                message = "Tipo documento creado"
        //            });
        //        }
        //        else
        //        {
        //            return BadRequest(new General()
        //            {
        //                title = "Registrar tipo documento",
        //                status = 400,
        //                message = "No tiene permisos para registrar tipo documento"
        //            });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //Registro de errores
        //        logger.LogError("Registrar tipo documento " + ex.Message.ToString() + " - " + ex.StackTrace);
        //        return BadRequest(new General()
        //        {
        //            title = "Registrar tipo documento",
        //            status = 400,
        //            message = "Contacte con el administrador del sistema"
        //        });
        //    }
        //}
        //#endregion
    }
}
