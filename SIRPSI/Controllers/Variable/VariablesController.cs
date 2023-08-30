using AutoMapper;
using DataAccess.Context;
using DataAccess.Models.Module;
using DataAccess.Models.Variables;
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
using SIRPSI.DTOs.Variables;
using SIRPSI.Helpers.Answers;
using System.Security.Claims;

namespace SIRPSI.Controllers.Variable
{
    [Route("api/variables")]
    [ApiController]
    [EnableCors("CorsApi")]
    public class VariablesController : ControllerBase
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
        public VariablesController(AppDbContext context,
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
        [HttpGet("ConsultarVariables", Name = "consultarVariables")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<object>> Get(string? modulo = null, string? variable1 = null)
        {
            try
            {
                //Claims de usuario - Enviados por token
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    IEnumerable<Claim> claims = identity.Claims;
                }
                //Consulta el documento con los claims
                var documento = identity.FindFirst("documento").Value.ToString();
                //Consulta el rol con los claims
                var variables = identity.FindFirst("rol").Value.ToString();
                //Consulta de usuarios por documento
                var usuario = context.AspNetUsers.Where(u => u.Document.Equals(documento)).FirstOrDefault();
                if (usuario == null)
                {
                    return NotFound(new General()
                    {
                        title = "Consultar variables",
                        status = 404,
                        message = "Usuario no encontrado"
                    });
                }
                //Obtiene la url del servicio
                var countLast = HttpContext.Request.GetDisplayUrl().Split("/").Last().Count();
                string Url = HttpContext.Request.GetDisplayUrl();
                var getUrl = Url.Remove(Url.Length - (countLast + 1));
                //Consulta de variables por id de usuario
                var variablesList = new List<string>();
                //Verifica los variables
                var list = variables.Split(',').ToList();

                foreach (var i in list)
                {
                    var result = context.AspNetRoles.Where(r => r.Id.Equals(i)).Select(x => x.Description).FirstOrDefault();

                    if (result != null)
                    {
                        variablesList.Add(result.ToString());
                    }
                }
                if (variablesList == null)
                {
                    return NotFound(new General()
                    {
                        title = "Consultar variables",
                        status = 404,
                        message = "variables no encontrados"
                    });
                }
                //Revisa los permisos de usuario
                var permisos = await context.permisosXUsuario.Where(x => x.Vista.Equals(getUrl) && x.IdUsuario.Equals(usuario.Id)).ToListAsync();
                //Consulta si tiene el permiso
                var permitido = permisos.Select(x => x.Consulta.Equals(true)).FirstOrDefault();
                //Si es permitido
                if (true)
                {
                    //Consulta estado
                    var estado = await context.estados.Where(x => x.IdConsecutivo.Equals(1)).FirstOrDefaultAsync();
                    if (estado == null)
                    {
                        return NotFound(new General()
                        {
                            title = "Consultar variables",
                            status = 404,
                            message = "Estado no encontrado"
                        });
                    }
                    //Consulta la variable
                    var rol = (from data in (await context.variables.ToListAsync())
                               select new ConsultarVariable
                               {
                                   Id = data.Id,
                                   Nombre = data.Nombre,
                                   Descripcion = data.Descripcion,
                                   Modulo = data.Modulo,
                                   Variable1 = data.Variable1,
                                   Variable2 = data.Variable2,
                                   Variable3 = data.Variable3,
                                   Variable4 = data.Variable4,
                                   Role = (data.Variable1 != null ? context.Roles.Where(x => x.Id.Equals(data.Variable2)).FirstOrDefault() : null)
                               }).ToList();

                    if (rol == null)
                    {
                        //Visualizacion de mensajes al usuario del aplicativo
                        return NotFound(new General()
                        {
                            title = "Consultar variables",
                            status = 404,
                            message = "Rol no encontrado"
                        });
                    }
                    if (modulo != null) rol = rol.Where(x => x.Modulo == modulo).ToList();
                    if (variable1 != null) rol = rol.Where(x => x.Variable1 == variable1).ToList();
                    //Retorno de los datos encontrados
                    return rol;
                }
                else
                {
                    return BadRequest(new General()
                    {
                        title = "Consultar variables",
                        status = 400,
                        message = "No tiene permisos para consultar variables"
                    });
                }
            }
            catch (Exception ex)
            {
                //Registro de errores
                logger.LogError("Consultar variables " + ex.Message.ToString() + " - " + ex.StackTrace);
                return BadRequest(new General()
                {
                    title = "Consultar variables",
                    status = 400,
                    message = "Contacte con el administrador del sistema"
                });
            }

        }
        #endregion

        #region Registro
        [HttpPost("RegistrarVariables")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Post([FromBody] RegistrarVariable registrarVariable)
        {
            try
            {
                //Claims de usuario - Enviados por token
                var identity = HttpContext.User.Identity as ClaimsIdentity;

                if (identity != null)
                {
                    IEnumerable<Claim> claims = identity.Claims;
                }
                //Consulta el documento con los claims
                var documento = identity.FindFirst("documento").Value.ToString();

                //Consulta el rol con los claims
                var variables = identity.FindFirst("rol").Value.ToString();

                //Consulta de usuarios por documento
                var usuario = context.AspNetUsers.Where(u => u.Document.Equals(documento)).FirstOrDefault();

                if (usuario == null)
                {
                    return NotFound(new General()
                    {
                        title = "Registrar variables",
                        status = 404,
                        message = "Usuario no encontrado"
                    });
                }

                //Obtiene la url del servicio
                var countLast = HttpContext.Request.GetDisplayUrl().Split("/").Last().Count();
                string Url = HttpContext.Request.GetDisplayUrl();

                var getUrl = Url.Remove(Url.Length - (countLast + 1));

                //Consulta de variables por id de usuario
                var variablesList = new List<string>();

                //Verifica los variables
                var list = variables.Split(',').ToList();

                foreach (var i in list)
                {
                    var result = context.AspNetRoles.Where(r => r.Id.Equals(i)).Select(x => x.Description).FirstOrDefault();

                    if (result != null)
                    {
                        variablesList.Add(result.ToString());
                    }
                }

                if (variablesList == null)
                {
                    return NotFound(new General()
                    {
                        title = "Registrar variables",
                        status = 404,
                        message = "variables no encontrados"
                    });
                }

                //Revisa los permisos de usuario
                var permisos = await context.permisosXUsuario.Where(x => x.Vista.Equals(getUrl) && x.IdUsuario.Equals(usuario.Id)).ToListAsync();

                //Consulta si tiene el permiso
                var permitido = permisos.Select(x => x.Registrar.Equals(true)).FirstOrDefault();

                //Si es permitido
                if (true)
                {
                    //Mapeo de datos en clases
                    var variable = mapper.Map<Variables>(registrarVariable);
                    //Valores asignados
                    variable.Id = Guid.NewGuid().ToString();

                    //Agregar datos al contexto
                    context.Add(variable);
                    //Guardado de datos 
                    await context.SaveChangesAsync();

                    return Created("", new General()
                    {
                        //Visualizacion de mensajes al usuario del aplicativo
                        title = "Registrar variables",
                        status = 201,
                        message = "Rol creado"
                    });
                }
                else
                {
                    return BadRequest(new General()
                    {
                        title = "Registrar variables",
                        status = 400,
                        message = "No tiene permisos para registrar variables"
                    });
                }
            }
            catch (Exception ex)
            {
                //Registro de errores
                logger.LogError("Registrar variables " + ex.Message.ToString() + " - " + ex.StackTrace);
                return BadRequest(new General()
                {
                    title = "Registrar variables",
                    status = 400,
                    message = "Contacte con el administrador del sistema"
                });
            }
        }
        #endregion

        #region Actualizar
        [HttpPut("ActualizarVariables")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Put(ActualizarVariable actualizarVariable)
        {
            try
            {
                //Claims de usuario - Enviados por token
                var identity = HttpContext.User.Identity as ClaimsIdentity;

                if (identity != null)
                {
                    IEnumerable<Claim> claims = identity.Claims;
                }

                //Consulta el documento con los claims
                var documento = identity.FindFirst("documento").Value.ToString();

                //Consulta el rol con los claims
                var variables = identity.FindFirst("rol").Value.ToString();

                //Consulta de usuario
                var usuario = context.AspNetUsers.Where(u => u.Document.Equals(documento)).FirstOrDefault();

                if (usuario == null)
                {
                    return NotFound(new General()
                    {
                        title = "Actualizar variables",
                        status = 404,
                        message = "Usuario no encontrado"
                    });
                }

                //Obtiene la url del servicio
                var countLast = HttpContext.Request.GetDisplayUrl().Split("/").Last().Count();
                string Url = HttpContext.Request.GetDisplayUrl();

                var getUrl = Url.Remove(Url.Length - (countLast + 1));

                //Consulta de variables por id de usuario

                var variablesList = new List<string>();

                //Verifica los variables
                var list = variables.Split(',').ToList();

                foreach (var i in list)
                {
                    var result = context.AspNetRoles.Where(r => r.Id.Equals(i)).Select(x => x.Description).FirstOrDefault();

                    if (result != null)
                    {
                        variablesList.Add(result.ToString());
                    }
                }

                if (variablesList == null)
                {
                    return NotFound(new General()
                    {
                        title = "Actualizar variables",
                        status = 404,
                        message = "variables no encontrados"
                    });
                }

                //Revisa los permisos de usuario
                var permisos = await context.permisosXUsuario.Where(x => x.Vista.Equals(getUrl) && x.IdUsuario.Equals(usuario.Id)).ToListAsync();

                //Consulta si tiene el permiso
                var permitido = permisos.Select(x => x.Actualizar.Equals(true)).FirstOrDefault();

                //Si es permitido
                if (true)
                {
                    //Consulta de variables de usuario
                    var existeRol = await context.variables.Where(x => x.Id.Equals(actualizarVariable.Id)).FirstOrDefaultAsync();

                    if (existeRol == null)
                    {
                        return NotFound(new
                        {
                            //Visualizacion de mensajes al usuario del aplicativo
                            title = "Actualizar variables",
                            status = 404,
                            message = "Variable no encontrada"
                        });
                    }

                    //Consulta de estados
                    var estados = await context.estados.ToListAsync();

                    //Registro de datos
                    context.variables.Where(x => x.Id.Equals(actualizarVariable.Id)).ToList()
                        .ForEach(r =>
                        {
                            r.Nombre = actualizarVariable.Nombre;
                            r.Descripcion = actualizarVariable.Descripcion;
                            r.Modulo = actualizarVariable.Modulo;
                            r.Variable1 = actualizarVariable.Variable1;
                            r.Variable2 = actualizarVariable.Variable2;
                            r.Variable3 = actualizarVariable.Variable3;
                            r.Variable4 = actualizarVariable.Variable4;
                        });

                    //Guardado de datos
                    await context.SaveChangesAsync();

                    return Ok(new General()
                    {
                        //Visualizacion de mensajes al usuario del aplicativo
                        title = "Actualizar variables",
                        status = 200,
                        message = "Variable actualizada"
                    });
                }
                else
                {
                    return BadRequest(new General()
                    {
                        title = "Actualizar variables",
                        status = 400,
                        message = "No tiene permisos para actualizar variables"
                    });
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Actualizar variables " + ex.Message.ToString() + " - " + ex.StackTrace);
                return BadRequest(new General()
                {
                    title = "Actualizar variables",
                    status = 400,
                    message = "Contacte con el administrador del sistema"
                }); ;
            }
        }
        #endregion

        #region Eliminar
        [HttpDelete("EliminarVariables")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Delete([FromBody] EliminarVariable eliminarVariable)
        {
            try
            {
                //Claims de usuario - Enviados por token
                var identity = HttpContext.User.Identity as ClaimsIdentity;

                if (identity != null)
                {
                    IEnumerable<Claim> claims = identity.Claims;
                }

                //Consulta el documento con los claims
                var documento = identity.FindFirst("documento").Value.ToString();

                //Consulta el rol con los claims
                var variables = identity.FindFirst("rol").Value.ToString();

                //Consulta de usuario
                var usuario = context.AspNetUsers.Where(u => u.Document.Equals(documento)).FirstOrDefault();

                if (usuario == null)
                {
                    return NotFound(new General()
                    {
                        title = "Eliminar variables",
                        status = 404,
                        message = "Usuario no encontrado"
                    });
                }

                //Obtiene la url del servicio
                var countLast = HttpContext.Request.GetDisplayUrl().Split("/").Last().Count();
                string Url = HttpContext.Request.GetDisplayUrl();

                var getUrl = Url.Remove(Url.Length - (countLast + 1));

                //Consulta de variables por id de usuario

                var variablesList = new List<string>();

                //Verifica los variables
                var list = variables.Split(',').ToList();

                foreach (var i in list)
                {
                    var result = context.modulo.Where(r => r.Id.Equals(i)).Select(x => x.Nombre).FirstOrDefault();

                    if (result != null)
                    {
                        variablesList.Add(result.ToString());
                    }
                }

                if (variablesList == null)
                {
                    return NotFound(new General()
                    {
                        title = "Eliminar variables",
                        status = 404,
                        message = "variables no encontrados"
                    });
                }

                //Revisa los permisos de usuario
                var permisos = await context.permisosXUsuario.Where(x => x.Vista.Equals(getUrl) && x.IdUsuario.Equals(usuario.Id)).ToListAsync();

                //Consulta si tiene el permiso
                var permitido = permisos.Select(x => x.Eliminar.Equals(true)).FirstOrDefault();

                //Si es permitido
                if (true)
                {
                    //Consulta estados
                    var estados = await context.estados.ToListAsync();

                    //Consulta de variables
                    var existeRol = context.variables.Where(x => x.Id.Equals(eliminarVariable.Id)).FirstOrDefault();

                    if (existeRol == null)
                    {
                        return NotFound(new General()
                        {
                            title = "Eliminar variables",
                            status = 404,
                            message = "Variable no encontrada"
                        });
                    }

                    //Agrega datos al contexto
                    context.variables.Remove(existeRol);

                    //Se elimina el regitro
                    await context.SaveChangesAsync();

                    return Ok(new General()
                    {
                        //Visualizacion de mensajes al usuario del aplicativo
                        title = "Eliminar variables",
                        status = 200,
                        message = "Variable eliminada"
                    });
                }
                else
                {
                    return BadRequest(new General()
                    {
                        title = "Eliminar variables",
                        status = 400,
                        message = "No tiene permisos para eliminar variables"
                    });
                }
            }
            catch (Exception ex)
            {
                //Registro de errores
                logger.LogError("Eliminar variables " + ex.Message.ToString() + " - " + ex.StackTrace);
                return BadRequest(new General()
                {
                    title = "Eliminar variables",
                    status = 400,
                    message = "Contacte con el administrador del sistema"
                });
            }

        }
        #endregion
    }
}