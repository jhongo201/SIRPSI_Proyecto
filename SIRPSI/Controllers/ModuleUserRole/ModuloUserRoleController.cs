﻿using AutoMapper;
using DataAccess.Context;
using DataAccess.Models.Module;
using DataAccess.Models.ModuleUserRole;
using DataAccess.Models.Rols;
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
using SIRPSI.DTOs.ModuleUserRole;
using SIRPSI.Helpers.Answers;
using System.Security.Claims;

namespace SIRPSI.Controllers.Module
{
    [Route("api/modulosUserRole")]
    [ApiController]
    [EnableCors("CorsApi")]
    public class ModuloUserRoleController : ControllerBase
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
        public ModuloUserRoleController(AppDbContext context,
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
        [HttpGet("ConsultarModulosRole", Name = "consultarModulosRole")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<object>> Get(string? id = null)
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
                var roles = identity.FindFirst("rol").Value.ToString();
                //Consulta de usuarios por documento
                var usuario = context.AspNetUsers.Where(u => u.Document.Equals(documento)).FirstOrDefault();
                if (usuario == null)
                {
                    return NotFound(new General()
                    {
                        title = "Consultar roles",
                        status = 404,
                        message = "Usuario no encontrado"
                    });
                }
                //Obtiene la url del servicio
                var countLast = HttpContext.Request.GetDisplayUrl().Split("/").Last().Count();
                string Url = HttpContext.Request.GetDisplayUrl();
                var getUrl = Url.Remove(Url.Length - (countLast + 1));
                //Consulta de roles por id de usuario
                var rolesList = new List<string>();
                //Verifica los roles
                var list = roles.Split(',').ToList();

                foreach (var i in list)
                {
                    var result = context.AspNetRoles.Where(r => r.Id.Equals(i)).Select(x => x.Description).FirstOrDefault();

                    if (result != null)
                    {
                        rolesList.Add(result.ToString());
                    }
                }
                if (rolesList == null)
                {
                    return NotFound(new General()
                    {
                        title = "Consultar roles",
                        status = 404,
                        message = "Roles no encontrados"
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
                            title = "Consultar roles",
                            status = 404,
                            message = "Estado no encontrado"
                        });
                    }
                    //Consulta el rol
                    var rol = (from data in (await context.moduloUserRole.ToListAsync())
                               where data.RoleId == id
                               select new ConsultarModuloUserRole
                               {
                                   Id = data.Id,
                                   RoleId = data.RoleId,
                                   ModuloId = data.ModuloId,
                                   FechaRegistro = data.FechaRegistro,
                                   UsuarioRegistro = data.UsuarioRegistro,
                                   FechaModifico = data.FechaModifico,
                                   UsuarioModifico = data.UsuarioModifico,
                                   Estado = (context.estados.Where(x => x.Id == data.EstadoId)).FirstOrDefault(),
                               }).ToList();

                    if (rol == null)
                    {
                        //Visualizacion de mensajes al usuario del aplicativo
                        return NotFound(new General()
                        {
                            title = "Consultar roles",
                            status = 404,
                            message = "Rol no encontrado"
                        });
                    }

                    //Retorno de los datos encontrados
                    return rol;
                }
                else
                {
                    return BadRequest(new General()
                    {
                        title = "Consultar roles",
                        status = 400,
                        message = "No tiene permisos para consultar roles"
                    });
                }
            }
            catch (Exception ex)
            {
                //Registro de errores
                logger.LogError("Consultar roles " + ex.Message.ToString() + " - " + ex.StackTrace);
                return BadRequest(new General()
                {
                    title = "Consultar roles",
                    status = 400,
                    message = "Contacte con el administrador del sistema"
                });
            }

        }
        #endregion

        #region Registro
        [HttpPost("RegistrarModulosRole")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Post([FromBody] RegistrarModuloUserRole registrarModuloUserRole)
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
                var roles = identity.FindFirst("rol").Value.ToString();

                //Consulta de usuarios por documento
                var usuario = context.AspNetUsers.Where(u => u.Document.Equals(documento)).FirstOrDefault();

                if (usuario == null)
                {
                    return NotFound(new General()
                    {
                        title = "Registrar roles",
                        status = 404,
                        message = "Usuario no encontrado"
                    });
                }

                //Obtiene la url del servicio
                var countLast = HttpContext.Request.GetDisplayUrl().Split("/").Last().Count();
                string Url = HttpContext.Request.GetDisplayUrl();

                var getUrl = Url.Remove(Url.Length - (countLast + 1));

                //Consulta de roles por id de usuario
                var rolesList = new List<string>();

                //Verifica los roles
                var list = roles.Split(',').ToList();

                foreach (var i in list)
                {
                    var result = context.AspNetRoles.Where(r => r.Id.Equals(i)).Select(x => x.Description).FirstOrDefault();

                    if (result != null)
                    {
                        rolesList.Add(result.ToString());
                    }
                }

                if (rolesList == null)
                {
                    return NotFound(new General()
                    {
                        title = "Registrar roles",
                        status = 404,
                        message = "Roles no encontrados"
                    });
                }

                //Revisa los permisos de usuario
                var permisos = await context.permisosXUsuario.Where(x => x.Vista.Equals(getUrl) && x.IdUsuario.Equals(usuario.Id)).ToListAsync();

                //Consulta si tiene el permiso
                var permitido = permisos.Select(x => x.Registrar.Equals(true)).FirstOrDefault();
                //var roleModulo = await context.moduloUserRole.Where(x => x.RoleId == registrarModuloUserRole.RoleId && x.ModuloId == registrarModuloUserRole.ModuloId).FirstAsync();

                //Si es permitido
                if (true)
                {
                    //Mapeo de datos en clases
                    var rol = mapper.Map<ModuloUserRole>(registrarModuloUserRole);
                    //Valores asignados
                    rol.Id = Guid.NewGuid().ToString();
                    rol.EstadoId = "cab25738-41fe-4989-a115-0ac36325dd6c";
                    rol.UsuarioRegistro = usuario.Id;
                    rol.FechaRegistro = DateTime.Now;

                    //Agregar datos al contexto
                    context.Add(rol);
                    //Guardado de datos 
                    await context.SaveChangesAsync();

                    return Created("", new General()
                    {
                        //Visualizacion de mensajes al usuario del aplicativo
                        title = "Registrar roles",
                        status = 201,
                        message = "Modulo asignado"
                    });
                }
                else
                {
                    return BadRequest(new General()
                    {
                        title = "Registrar roles",
                        status = 400,
                        message = "No tiene permisos para registrar roles"
                    });
                }
            }
            catch (Exception ex)
            {
                //Registro de errores
                logger.LogError("Registrar roles " + ex.Message.ToString() + " - " + ex.StackTrace);
                return BadRequest(new General()
                {
                    title = "Registrar roles",
                    status = 400,
                    message = "Contacte con el administrador del sistema"
                });
            }
        }
        #endregion

        #region Actualizar
        //[HttpPut("ActualizarModulosRole")]
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
        //        var roles = identity.FindFirst("rol").Value.ToString();

        //        //Consulta de usuario
        //        var usuario = context.AspNetUsers.Where(u => u.Document.Equals(documento)).FirstOrDefault();

        //        if (usuario == null)
        //        {
        //            return NotFound(new General()
        //            {
        //                title = "Actualizar roles",
        //                status = 404,
        //                message = "Usuario no encontrado"
        //            });
        //        }

        //        //Obtiene la url del servicio
        //        var countLast = HttpContext.Request.GetDisplayUrl().Split("/").Last().Count();
        //        string Url = HttpContext.Request.GetDisplayUrl();

        //        var getUrl = Url.Remove(Url.Length - (countLast + 1));

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
        //                title = "Actualizar roles",
        //                status = 404,
        //                message = "Roles no encontrados"
        //            });
        //        }

        //        //Revisa los permisos de usuario
        //        var permisos = await context.permisosXUsuario.Where(x => x.Vista.Equals(getUrl) && x.IdUsuario.Equals(usuario.Id)).ToListAsync();

        //        //Consulta si tiene el permiso
        //        var permitido = permisos.Select(x => x.Actualizar.Equals(true)).FirstOrDefault();

        //        //Si es permitido
        //        if (true)
        //        {
        //            //Consulta de roles de usuario
        //            var existeRol = await context.modulo.Where(x => x.Id.Equals(actualizarRol.Id)).FirstOrDefaultAsync();

        //            if (existeRol == null)
        //            {
        //                return NotFound(new
        //                {
        //                    //Visualizacion de mensajes al usuario del aplicativo
        //                    title = "Actualizar roles",
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
        //                title = "Actualizar roles",
        //                status = 200,
        //                message = "Rol actualizado"
        //            });
        //        }
        //        else
        //        {
        //            return BadRequest(new General()
        //            {
        //                title = "Actualizar roles",
        //                status = 400,
        //                message = "No tiene permisos para actualizar roles"
        //            });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogError("Actualizar roles " + ex.Message.ToString() + " - " + ex.StackTrace);
        //        return BadRequest(new General()
        //        {
        //            title = "Actualizar roles",
        //            status = 400,
        //            message = "Contacte con el administrador del sistema"
        //        }); ;
        //    }
        //}
        #endregion

        #region Eliminar
        [HttpPost("EliminarModulosRole")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Delete([FromBody] RegistrarModuloUserRole eliminarRole)
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
                var roles = identity.FindFirst("rol").Value.ToString();

                //Consulta de usuario
                var usuario = context.AspNetUsers.Where(u => u.Document.Equals(documento)).FirstOrDefault();

                if (usuario == null)
                {
                    return NotFound(new General()
                    {
                        title = "Eliminar roles",
                        status = 404,
                        message = "Usuario no encontrado"
                    });
                }

                //Obtiene la url del servicio
                var countLast = HttpContext.Request.GetDisplayUrl().Split("/").Last().Count();
                string Url = HttpContext.Request.GetDisplayUrl();

                var getUrl = Url.Remove(Url.Length - (countLast + 1));

                //Consulta de roles por id de usuario

                var rolesList = new List<string>();

                //Verifica los roles
                var list = roles.Split(',').ToList();

                foreach (var i in list)
                {
                    var result = context.modulo.Where(r => r.Id.Equals(i)).Select(x => x.Nombre).FirstOrDefault();

                    if (result != null)
                    {
                        rolesList.Add(result.ToString());
                    }
                }

                if (rolesList == null)
                {
                    return NotFound(new General()
                    {
                        title = "Eliminar roles",
                        status = 404,
                        message = "Roles no encontrados"
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

                    //Agrega datos al contexto
                    var eliminar = context.moduloUserRole.Where(x => x.RoleId.Equals(eliminarRole.RoleId) && x.ModuloId.Equals(eliminarRole.ModuloId)).First();

                    if (eliminar != null)
                        context.moduloUserRole.Remove(eliminar);

                    //Se elimina el regitro
                    await context.SaveChangesAsync();

                    return Ok(new General()
                    {
                        //Visualizacion de mensajes al usuario del aplicativo
                        title = "Eliminar roles",
                        status = 200,
                        message = "Rol eliminado"
                    });
                }
                else
                {
                    return BadRequest(new General()
                    {
                        title = "Eliminar roles",
                        status = 400,
                        message = "No tiene permisos para eliminar roles"
                    });
                }
            }
            catch (Exception ex)
            {
                //Registro de errores
                logger.LogError("Eliminar roles " + ex.Message.ToString() + " - " + ex.StackTrace);
                return BadRequest(new General()
                {
                    title = "Eliminar roles",
                    status = 400,
                    message = "Contacte con el administrador del sistema"
                });
            }

        }
        #endregion
    }
}