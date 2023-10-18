using AutoMapper;
using DataAccess.Context;
using DataAccess.Models.Module;
using DataAccess.Models.OccupationalLicense;
using DataAccess.Models.Users;
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
using SIRPSI.DTOs.Module;
using SIRPSI.DTOs.OccupationalLicense;
using SIRPSI.Helpers.Answers;
using System.Security.Claims;

namespace SIRPSI.Controllers.OccupationalLicense
{
    [Route("api/licenciaOcupacional")]
    [ApiController]
    [EnableCors("CorsApi")]
    public class LicenciaOcupacionalController : ControllerBase
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
        public LicenciaOcupacionalController(AppDbContext context,
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
        [HttpGet("ConsultarLicenciaOcupacional", Name = "consultarLicenciaOcupacional")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<object>> Get()
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    IEnumerable<Claim> claims = identity.Claims;
                }
                var documento = identity.FindFirst("documento").Value.ToString();
                var LicenciaOcupacional = identity.FindFirst("rol").Value.ToString();
                var usuario = context.AspNetUsers.Where(u => u.Document.Equals(documento)).FirstOrDefault();
                if (usuario == null)
                {
                    return NotFound(new General()
                    {
                        title = "Consultar Licencia ocupacional",
                        status = 404,
                        message = "Licencia no encontrado"
                    });
                }
                var licencia = (from data in (await context.licenciaOcupacional.ToListAsync())
                                select new ConsultarLicenciaOcupacional
                                {
                                    Id = data.Id,
                                    Numero = data.Numero,
                                    UsuarioId = data.UsuarioId,
                                    FechaExpedicion = data.FechaExpedicion
                                }).ToList();

                if (licencia == null)
                {
                    return NotFound(new General()
                    {
                        title = "Consultar Licencia ocupacional",
                        status = 404,
                        message = "Licencia no encontrado"
                    });
                }
                return licencia;
            }
            catch (Exception ex)
            {
                logger.LogError("Consultar LicenciaOcupacional " + ex.Message.ToString() + " - " + ex.StackTrace);
                return BadRequest(new General()
                {
                    title = "Consultar LicenciaOcupacional",
                    status = 400,
                    message = "Contacte con el administrador del sistema"
                });
            }

        }

        [HttpGet("ConsultarLicenciaOcupacionalUsuario", Name = "consultarLicenciaOcupacionalUsuario")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<object>> GetByUser(string id)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    IEnumerable<Claim> claims = identity.Claims;
                }
                var documento = identity.FindFirst("documento").Value.ToString();
                var LicenciaOcupacional = identity.FindFirst("rol").Value.ToString();
                var usuario = context.AspNetUsers.Where(u => u.Document.Equals(documento)).FirstOrDefault();
                if (usuario == null)
                {
                    return NotFound(new General()
                    {
                        title = "Consultar Licencia ocupacional",
                        status = 404,
                        message = "Licencia no encontrado"
                    });
                }
                var licencia = (from data in (await context.licenciaOcupacional.ToListAsync())
                                where data.UsuarioId == id
                                select new ConsultarLicenciaOcupacional
                                {
                                    Id = data.Id,
                                    Numero = data.Numero,
                                    UsuarioId = data.UsuarioId,
                                    FechaExpedicion = data.FechaExpedicion
                                }).FirstOrDefault();

                if (licencia == null)
                {
                    return NotFound(new General()
                    {
                        title = "Consultar Licencia ocupacional",
                        status = 404,
                        message = "Licencia no encontrado"
                    });
                }
                return licencia;
            }
            catch (Exception ex)
            {
                logger.LogError("Consultar LicenciaOcupacional " + ex.Message.ToString() + " - " + ex.StackTrace);
                return BadRequest(new General()
                {
                    title = "Consultar LicenciaOcupacional",
                    status = 400,
                    message = "Contacte con el administrador del sistema"
                });
            }

        }
        #endregion

        #region Registro
        [HttpPost("ActualizarLicenciaOcupacional")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Post([FromBody] ActualizarLicenciaOcupacional actualizarLicenciaOcupacional)
        {
            try
            {
                var licencia = context.licenciaOcupacional.Where(x => x.UsuarioId == actualizarLicenciaOcupacional.UsuarioId).FirstOrDefault();
                if (licencia == null)
                {
                    var licenciaOcupacional = mapper.Map<LicenciaOcupacional>(actualizarLicenciaOcupacional);
                    licenciaOcupacional.Id = Guid.NewGuid().ToString();
                    context.Add(licenciaOcupacional);
                }
                else
                {
                    licencia.Numero = actualizarLicenciaOcupacional.Numero;
                    licencia.FechaExpedicion = actualizarLicenciaOcupacional.FechaExpedicion;
                    context.Update(licencia);
                }
                await context.SaveChangesAsync();

                return Created("", new General()
                {
                    title = "Actualizar licencia ocupacional",
                    status = 201,
                    message = "licencia ocupacional actualizada"
                });
            }
            catch (Exception ex)
            {
                //Registro de errores
                logger.LogError("Actualizar licencia ocupacional " + ex.Message.ToString() + " - " + ex.StackTrace);
                return BadRequest(new General()
                {
                    title = "Actualizar licencia ocupacional",
                    status = 400,
                    message = "Contacte con el administrador del sistema"
                });
            }
        }
        #endregion

        #region Actualizar
        //[HttpPut("ActualizarLicenciaOcupacional")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //public async Task<ActionResult> Put(ActualizarModulo actualizarRol)
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
        //        var LicenciaOcupacional = identity.FindFirst("rol").Value.ToString();

        //        //Consulta de usuario
        //        var usuario = context.AspNetUsers.Where(u => u.Document.Equals(documento)).FirstOrDefault();

        //        if (usuario == null)
        //        {
        //            return NotFound(new General()
        //            {
        //                title = "Actualizar LicenciaOcupacional",
        //                status = 404,
        //                message = "Usuario no encontrado"
        //            });
        //        }

        //        //Obtiene la url del servicio
        //        var countLast = HttpContext.Request.GetDisplayUrl().Split("/").Last().Count();
        //        string Url = HttpContext.Request.GetDisplayUrl();

        //        var getUrl = Url.Remove(Url.Length - (countLast + 1));

        //        //Consulta de LicenciaOcupacional por id de usuario

        //        var LicenciaOcupacionalList = new List<string>();

        //        //Verifica los LicenciaOcupacional
        //        var list = LicenciaOcupacional.Split(',').ToList();

        //        foreach (var i in list)
        //        {
        //            var result = context.AspNetLicenciaOcupacional.Where(r => r.Id.Equals(i)).Select(x => x.Description).FirstOrDefault();

        //            if (result != null)
        //            {
        //                LicenciaOcupacionalList.Add(result.ToString());
        //            }
        //        }

        //        if (LicenciaOcupacionalList == null)
        //        {
        //            return NotFound(new General()
        //            {
        //                title = "Actualizar LicenciaOcupacional",
        //                status = 404,
        //                message = "LicenciaOcupacional no encontrados"
        //            });
        //        }

        //        //Revisa los permisos de usuario
        //        var permisos = await context.permisosXUsuario.Where(x => x.Vista.Equals(getUrl) && x.IdUsuario.Equals(usuario.Id)).ToListAsync();

        //        //Consulta si tiene el permiso
        //        var permitido = permisos.Select(x => x.Actualizar.Equals(true)).FirstOrDefault();

        //        //Si es permitido
        //        if (true)
        //        {
        //            //Consulta de LicenciaOcupacional de usuario
        //            var existeRol = await context.modulo.Where(x => x.Id.Equals(actualizarRol.Id)).FirstOrDefaultAsync();

        //            if (existeRol == null)
        //            {
        //                return NotFound(new
        //                {
        //                    //Visualizacion de mensajes al usuario del aplicativo
        //                    title = "Actualizar LicenciaOcupacional",
        //                    status = 404,
        //                    message = "Rol no encontrado"
        //                });
        //            }

        //            //Consulta de estados
        //            var estados = await context.estados.ToListAsync();

        //            //Registro de datos
        //            context.modulo.Where(x => x.Id.Equals(actualizarRol.Id)).ToList()
        //                .ForEach(r =>
        //                {
        //                    r.Nombre = actualizarRol.Nombre;
        //                    r.Descripcion = actualizarRol.Descripcion;
        //                    r.UsuarioModifico = usuario.Document;
        //                    r.FechaModifico = DateTime.Now;
        //                });

        //            //Guardado de datos
        //            await context.SaveChangesAsync();

        //            return Ok(new General()
        //            {
        //                //Visualizacion de mensajes al usuario del aplicativo
        //                title = "Actualizar LicenciaOcupacional",
        //                status = 200,
        //                message = "Rol actualizado"
        //            });
        //        }
        //        else
        //        {
        //            return BadRequest(new General()
        //            {
        //                title = "Actualizar LicenciaOcupacional",
        //                status = 400,
        //                message = "No tiene permisos para actualizar LicenciaOcupacional"
        //            });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogError("Actualizar LicenciaOcupacional " + ex.Message.ToString() + " - " + ex.StackTrace);
        //        return BadRequest(new General()
        //        {
        //            title = "Actualizar LicenciaOcupacional",
        //            status = 400,
        //            message = "Contacte con el administrador del sistema"
        //        }); ;
        //    }
        //}
        #endregion

        #region Eliminar
        //[HttpDelete("EliminarLicenciaOcupacional")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //public async Task<ActionResult> Delete([FromBody] EliminarModulo eliminarRol)
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
        //        var LicenciaOcupacional = identity.FindFirst("rol").Value.ToString();

        //        //Consulta de usuario
        //        var usuario = context.AspNetUsers.Where(u => u.Document.Equals(documento)).FirstOrDefault();

        //        if (usuario == null)
        //        {
        //            return NotFound(new General()
        //            {
        //                title = "Eliminar LicenciaOcupacional",
        //                status = 404,
        //                message = "Usuario no encontrado"
        //            });
        //        }

        //        //Obtiene la url del servicio
        //        var countLast = HttpContext.Request.GetDisplayUrl().Split("/").Last().Count();
        //        string Url = HttpContext.Request.GetDisplayUrl();

        //        var getUrl = Url.Remove(Url.Length - (countLast + 1));

        //        //Consulta de LicenciaOcupacional por id de usuario

        //        var LicenciaOcupacionalList = new List<string>();

        //        //Verifica los LicenciaOcupacional
        //        var list = LicenciaOcupacional.Split(',').ToList();

        //        foreach (var i in list)
        //        {
        //            var result = context.modulo.Where(r => r.Id.Equals(i)).Select(x => x.Nombre).FirstOrDefault();

        //            if (result != null)
        //            {
        //                LicenciaOcupacionalList.Add(result.ToString());
        //            }
        //        }

        //        if (LicenciaOcupacionalList == null)
        //        {
        //            return NotFound(new General()
        //            {
        //                title = "Eliminar LicenciaOcupacional",
        //                status = 404,
        //                message = "LicenciaOcupacional no encontrados"
        //            });
        //        }

        //        //Revisa los permisos de usuario
        //        var permisos = await context.permisosXUsuario.Where(x => x.Vista.Equals(getUrl) && x.IdUsuario.Equals(usuario.Id)).ToListAsync();

        //        //Consulta si tiene el permiso
        //        var permitido = permisos.Select(x => x.Eliminar.Equals(true)).FirstOrDefault();

        //        //Si es permitido
        //        if (true)
        //        {
        //            //Consulta estados
        //            var estados = await context.estados.ToListAsync();

        //            //Consulta de LicenciaOcupacional
        //            var existeRol = context.AspNetLicenciaOcupacional.Where(x => x.Id.Equals(eliminarRol.Id)).FirstOrDefault();

        //            if (existeRol == null)
        //            {
        //                return NotFound(new General()
        //                {
        //                    title = "Eliminar LicenciaOcupacional",
        //                    status = 404,
        //                    message = "Rol no encontrado"
        //                });
        //            }

        //            //Agrega datos al contexto
        //            context.modulo.Where(x => x.Id.Equals(eliminarRol.Id)).ToList()
        //              .ForEach(r =>
        //              {
        //                  r.IdEstado = estados.Where(x => x.IdConsecutivo.Equals(2)).Select(x => x.Id).First();
        //                  r.UsuarioModifico = usuario.Document;
        //                  r.FechaModifico = DateTime.Now;
        //              });

        //            //Se elimina el regitro
        //            await context.SaveChangesAsync();

        //            return Ok(new General()
        //            {
        //                //Visualizacion de mensajes al usuario del aplicativo
        //                title = "Eliminar LicenciaOcupacional",
        //                status = 200,
        //                message = "Rol eliminado"
        //            });
        //        }
        //        else
        //        {
        //            return BadRequest(new General()
        //            {
        //                title = "Eliminar LicenciaOcupacional",
        //                status = 400,
        //                message = "No tiene permisos para eliminar LicenciaOcupacional"
        //            });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //Registro de errores
        //        logger.LogError("Eliminar LicenciaOcupacional " + ex.Message.ToString() + " - " + ex.StackTrace);
        //        return BadRequest(new General()
        //        {
        //            title = "Eliminar LicenciaOcupacional",
        //            status = 400,
        //            message = "Contacte con el administrador del sistema"
        //        });
        //    }

        //}
        #endregion
    }
}