﻿using AutoMapper;
using DataAccess.Context;
using DataAccess.Models.Documents;
using DataAccess.Models.Estados;
using EmailServices;
using EvertecApi.Log4net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIRPSI.Core.Helper;
using SIRPSI.DTOs.Document;
using SIRPSI.DTOs.User;
using SIRPSI.DTOs.User.Usuario;
using SIRPSI.Helpers.Answers;
using SIRPSI.Settings;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SIRPSI.Controllers.User
{
    [Route("api/usuario")]
    [ApiController]
    public class UsuariosController : ControllerBase
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
        public UsuariosController(AppDbContext context,
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

        #region Consulta
        [HttpGet("ConsultarUsuarios", Name = "consultarUsuarios")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<object>> Get()
        {
            try
            {
                //Consulta el tipo documento
                var usuarioConsultado = (from data in (await context.AspNetUsers.ToListAsync())
                                         orderby data.RegistrationDate ascending
                                         select new ConsultarUsuarios()
                                         {
                                             Id = data.Id,
                                             idTipoDocumento = data.TypeDocument,
                                             tipoDocumento = (context.tiposDocumento.Where(x => x.Id == data.TypeDocument).FirstOrDefault()),
                                             cedula = data.Document,
                                             correo = data.Email,
                                             telefono = data.PhoneNumber,
                                             idEmpresa = data.IdCompany,
                                             empresa = (context.empresas.Where(x => x.Id == data.IdCompany).FirstOrDefault()),
                                             nombreUsuario = data.Names,
                                             apellidosUsuario = data.Surnames,
                                             idEstado = data.Status,
                                             estado = (context.estados.Where(x => x.Id == data.Status).FirstOrDefault()),
                                             IdRol = data.IdRol,
                                             role = (context.Roles.Where(x => x.Id == data.IdRol).FirstOrDefault()),
                                         }).ToList();

                if (usuarioConsultado == null)
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
                return usuarioConsultado;
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

        [HttpGet("ConsultarUsuariosEmpresa", Name = "consultarUsuariosEmpresa")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<object>> GetUserCompany(string? status = null, int? type = 0, string? role = null)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    IEnumerable<Claim> claims = identity.Claims;
                }
                var documento = identity.FindFirst("documento").Value.ToString();
                var roles = identity.FindFirst("rol").Value.ToString();
                var usuario = context.AspNetUsers.Where(u => u.Document.Equals(documento)).FirstOrDefault();
                if (usuario == null)
                {
                    return NotFound(new General()
                    {
                        title = "Actualizar empresa",
                        status = 404,
                        message = "Usuario no encontrado"
                    });
                }
                var empresa = context.empresas.Where(u => u.IdUsuario.Equals(usuario.Id)).FirstOrDefault();
                usuario.IdCompany = empresa != null ? empresa.Id : usuario.IdCompany;
                context.Update(usuario);
                await context.SaveChangesAsync();
                //Consulta el tipo documento
                var usuarioConsultado = (from data in (await context.AspNetUsers.ToListAsync())
                                         where data.IdCompany == (usuario.IdCompany == null || usuario.IdCompany == "" ? empresa.Id : usuario.IdCompany)
                                         && (role == null ? 1 == 1 : data.IdRol == role)
                                         //&& data.Status == statusSettings.Inactivo
                                         select new ConsultarUsuarios()
                                         {
                                             Id = data.Id,
                                             idTipoDocumento = data.TypeDocument,
                                             tipoDocumento = (context.tiposDocumento.Where(x => x.Id == data.TypeDocument).FirstOrDefault()),
                                             cedula = data.Document,
                                             correo = data.Email,
                                             correoAux = data.EmailAux,
                                             telefono = data.PhoneNumber,
                                             telefonoAux = data.PhoneNumberAux,
                                             idEmpresa = data.IdCompany,
                                             empresa = (context.empresas.Where(x => x.Id == data.IdCompany).FirstOrDefault()),
                                             nombreUsuario = data.Names,
                                             apellidosUsuario = data.Surnames,
                                             habilidadesLectoEscritura = data.ReadingWritingSkills,
                                             tieneDiscapacidad = data.HaveDisability,
                                             idEstado = data.Status,
                                             IdPais = data.IdCountry,
                                             pais = (context.pais.Where(x => x.Id == data.IdCountry).FirstOrDefault()),
                                             estado = (context.estados.Where(x => x.Id == data.Status).FirstOrDefault()),
                                             IdRol = data.IdRol,
                                             role = (context.Roles.Where(x => x.Id == data.IdRol).FirstOrDefault()),
                                             IdOcupacionProfesion = data.IdOccupationProfession,
                                             ocupacionProfesion = data.IdOccupationProfession != null ? (context.ocupacionProfesion.Where(x => x.Id == data.IdOccupationProfession).FirstOrDefault()) : null,
                                             workPlaces = context.userWorkPlace.Where(x => x.UserId == data.Id).ToList()
                                         }).ToList();

                if (type == 1) usuarioConsultado = usuarioConsultado.Where(x => x.idEstado == status).ToList();
                else if (type == 2)
                {
                    string[] statusNot = new string[] { "10b28980-jhbd-4dc2-11db-57f4c8780b67", "c22caee5-aba0-4bd8-abf3-cff6305df919" };
                    usuarioConsultado = usuarioConsultado.Where(x => !statusNot.Contains(x.idEstado)).ToList();
                }

                if (usuarioConsultado == null)
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
                return usuarioConsultado;
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

        [HttpGet("ConsultarUsuariosEmpresaSinCentro", Name = "consultarUsuariosEmpresaSinCentro")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<object>> GetUserCompanyNotCenter(string? role = null, int? workCenters = 1)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    IEnumerable<Claim> claims = identity.Claims;
                }
                var documento = identity.FindFirst("documento").Value.ToString();
                var roles = identity.FindFirst("rol").Value.ToString();
                var usuario = context.AspNetUsers.Where(u => u.Document.Equals(documento)).FirstOrDefault();
                if (usuario == null)
                {
                    return NotFound(new General()
                    {
                        title = "Actualizar empresa",
                        status = 404,
                        message = "Usuario no encontrado"
                    });
                }
                var empresa = context.empresas.Where(u => u.IdUsuario.Equals(usuario.Id)).FirstOrDefault();
                usuario.IdCompany = empresa != null ? empresa.Id : usuario.IdCompany;
                context.Update(usuario);
                await context.SaveChangesAsync();
                //Consulta el tipo documento
                var usuarioConsultado = (from data in (await context.AspNetUsers.ToListAsync())
                                         where data.IdCompany == (usuario.IdCompany == null || usuario.IdCompany == "" ? empresa.Id : usuario.IdCompany)
                                         select new ConsultarUsuarios()
                                         {
                                             Id = data.Id,
                                             idTipoDocumento = data.TypeDocument,
                                             tipoDocumento = (context.tiposDocumento.Where(x => x.Id == data.TypeDocument).FirstOrDefault()),
                                             cedula = data.Document,
                                             correo = data.Email,
                                             telefono = data.PhoneNumber,
                                             idEmpresa = data.IdCompany,
                                             empresa = (context.empresas.Where(x => x.Id == data.IdCompany).FirstOrDefault()),
                                             nombreUsuario = data.Names,
                                             apellidosUsuario = data.Surnames,
                                             idEstado = data.Status,
                                             estado = (context.estados.Where(x => x.Id == data.Status).FirstOrDefault()),
                                             IdRol = data.IdRol,
                                             role = (context.Roles.Where(x => x.Id == data.IdRol).FirstOrDefault()),
                                             psicologosCentroTrabajo = (context.psicologosCentroTrabajo.Where(x => x.IdUser == data.Id).FirstOrDefault())
                                         }).ToList();

                if (usuarioConsultado == null)
                {
                    //Visualizacion de mensajes al usuario del aplicativo
                    return NotFound(new General()
                    {
                        title = "Consultar usuario",
                        status = 404,
                        message = "Usuarios no encontrados"
                    });
                }
                if (workCenters == 1)
                    usuarioConsultado = usuarioConsultado.Where(y => context.userWorkPlace.Where(x => x.UserId == y.Id).ToList().Count() == 0).ToList();
                else if (workCenters == 0)
                    usuarioConsultado = usuarioConsultado.Where(y => context.psicologosCentroTrabajo.Where(x => x.IdUser == y.Id).ToList().Count() == 0).ToList();
                if (role != null)
                    usuarioConsultado = usuarioConsultado.Where(x => x.IdRol == role).ToList();
                //Retorno de los datos encontrados
                return usuarioConsultado;
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

        [HttpGet("ConsultarUsuarioDatos", Name = "consultarUsuarioDatos")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<object>> GetValidationUserApi([FromQuery] ValidarUsuarioRequest request)
        {
            try
            {
                List<UsuarioConsultado> Usuarios = new List<UsuarioConsultado>()
                {
                    new UsuarioConsultado()
                    {
                        TipoDocumento = "373a1d14-775b-48f7-b033-6f697401ad6c",
                        Nombre = "Camilo Andres",
                        Apellidos = "Rivas Orobio",
                        Documento = "1017270383",
                        FechaExpedicion = DateTime.Parse("30/05/2017")
                    },
                    new UsuarioConsultado()
                    {
                        TipoDocumento = "373a1d14-775b-48f7-b033-6f697401ad6c",
                        Nombre = "Laura",
                        Apellidos = "Morales",
                        Documento = "1006123456",
                        FechaExpedicion = DateTime.Parse("15/04/2000")
                    },
                    new UsuarioConsultado()
                    {
                        TipoDocumento = "373a1d14-775b-48f7-b033-6f697401ad6c",
                        Nombre = "Joanthan Lion",
                        Apellidos = "Puerta Rodriguez",
                        Documento = "1122334455",
                        FechaExpedicion = DateTime.Parse("15/04/2000")
                    },
                };
                //Consulta el tipo documento
                var usuarioConsultado = (from data in Usuarios
                                         where data.TipoDocumento == request.TypeDocumentId &&
                                            data.Documento == request.Document &&
                                            data.FechaExpedicion.ToString("yyyyMMdd") == request.ExpeditionDate.ToString("yyyyMMdd")
                                         select data).FirstOrDefault();

                if (usuarioConsultado == null)
                {
                    //Visualizacion de mensajes al usuario del aplicativo
                    return NotFound(new General()
                    {
                        title = "Consultar usuario",
                        status = 404,
                        message = "Usuarios no encontrado"
                    });
                }
                //Retorno de los datos encontrados
                return usuarioConsultado;
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

        [HttpGet("ConsultarUsuario", Name = "consultarUsuario")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<object>> GetById(string id)
        {
            try
            {
                //Consulta el tipo documento
                var usuarioConsultado = (from data in (await context.AspNetUsers.ToListAsync())
                                         where data.Id == id
                                         select new ConsultarUsuarios()
                                         {
                                             Id = data.Id,
                                             idTipoDocumento = data.TypeDocument,
                                             tipoDocumento = (context.tiposDocumento.Where(x => x.Id == data.TypeDocument).FirstOrDefault()),
                                             cedula = data.Document,
                                             correo = data.Email,
                                             correoAux = data.EmailAux,
                                             telefono = data.PhoneNumber,
                                             telefonoAux = data.PhoneNumberAux,
                                             idEmpresa = data.IdCompany,
                                             empresa = (context.empresas.Where(x => x.Id == data.IdCompany).FirstOrDefault()),
                                             nombreUsuario = data.Names,
                                             apellidosUsuario = data.Surnames,
                                             habilidadesLectoEscritura = data.ReadingWritingSkills,
                                             tieneDiscapacidad = data.HaveDisability,
                                             idEstado = data.Status,
                                             IdPais = data.IdCountry,
                                             pais = (context.pais.Where(x => x.Id == data.IdCountry).FirstOrDefault()),
                                             estado = (context.estados.Where(x => x.Id == data.Status).FirstOrDefault()),
                                             IdRol = data.IdRol,
                                             role = (context.Roles.Where(x => x.Id == data.IdRol).FirstOrDefault()),
                                             IdOcupacionProfesion = data.IdOccupationProfession,
                                             ocupacionProfesion = data.IdOccupationProfession != null ? (context.ocupacionProfesion.Where(x => x.Id == data.IdOccupationProfession).FirstOrDefault()) : null,
                                             workPlaces = context.userWorkPlace.Where(x => x.UserId == data.Id).ToList()
                                         }).FirstOrDefault();

                if (usuarioConsultado == null)
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
                return usuarioConsultado;
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

        #region Registro
        //[HttpPost("RegistrarUsuario")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //public async Task<ActionResult> Post([FromBody] RegistrarTipoDocumento registrarTipoDocumento)
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
        #endregion

        #region Eliminar
        //[HttpDelete("EliminarUusario")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //public async Task<ActionResult> Delete([FromBody] EliminarTipoDocumento eliminarTipoDocumento)
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
        //                title = "Eliminar tipo documento",
        //                status = 404,
        //                message = "Usuario no encontrado"
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
        //                title = "Eliminar tipo documento",
        //                status = 404,
        //                message = "Roles no encontrados"
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

        //            if (estados == null)
        //            {
        //                return NotFound(new General()
        //                {
        //                    title = "Eliminar tipo documento",
        //                    status = 404,
        //                    message = "Estados no encontrados"
        //                });
        //            }

        //            //Consulta de empresa
        //            var existe = await context.tiposDocumento.Where(x => x.Id.Equals(eliminarTipoDocumento.Id)).FirstOrDefaultAsync();

        //            if (existe == null)
        //            {
        //                return NotFound(new General()
        //                {
        //                    title = "Eliminar tipo documento",
        //                    status = 404,
        //                    message = "Tipo empresa no encontrada"
        //                });
        //            }

        //            //Agregar datos al contexto
        //            context.tiposDocumento.Where(x => x.Id.Equals(eliminarTipoDocumento.Id)).ToList()
        //              .ForEach(r =>
        //              {
        //                  r.IdEstado = estados.Where(x => x.IdConsecutivo.Equals(2)).Select(x => x.Id).First();
        //                  r.UsuarioModifico = usuario.Document;
        //                  r.FechaModifico = DateTime.Now.ToDateTimeZone().DateTime;
        //              });

        //            //Se elimina el regitro de forma logica
        //            await context.SaveChangesAsync();

        //            return Ok(new General()
        //            {
        //                //Visualizacion de mensajes al usuario del aplicativo
        //                title = "Eliminar tipo documento",
        //                status = 200,
        //                message = "Tipo documento eliminado"
        //            });
        //        }
        //        else
        //        {
        //            return BadRequest(new General()
        //            {
        //                title = "Eliminar tipo documento",
        //                status = 400,
        //                message = "No tiene permisos para eliminar tipo documento"
        //            });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //Registro de errores
        //        logger.LogError("Eliminar tipo documento " + ex.Message.ToString() + " - " + ex.StackTrace);
        //        return BadRequest(new General()
        //        {
        //            title = "Eliminar tipo documento",
        //            status = 400,
        //            message = "Contacte con el administrador del sistema"
        //        });
        //    }

        //}
        #endregion

        #region Actualizar
        [HttpPut("ActualizarUsuario")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Put(ActualizarUsuario actualizarUsuario)
        {
            try
            {
                var usuario = context.AspNetUsers.Where(u => u.Document.Equals(actualizarUsuario.Document)).FirstOrDefault();
                if (usuario == null)
                    return NotFound(new General()
                    {
                        title = "Actualizar tipo documento",
                        status = 404,
                        message = "Usuario no encontrado"
                    });
                //public string? IdWorkCenter { get; set; } = null;
                usuario.TypeDocument = actualizarUsuario.IdTypeDocument != null ? (string)actualizarUsuario.IdTypeDocument : usuario.TypeDocument;
                usuario.Document = actualizarUsuario.Document != null ? (string)actualizarUsuario.Document : usuario.Document;
                usuario.IdCompany = actualizarUsuario.IdCompany != null ? (string)actualizarUsuario.IdCompany : usuario.IdCompany;
                usuario.IdCountry = actualizarUsuario.IdCountry != null ? (string)actualizarUsuario.IdCountry : usuario.IdCountry;
                usuario.Names = actualizarUsuario.Names != null ? (string)actualizarUsuario.Names : usuario.Names;
                usuario.Surnames = actualizarUsuario.Surnames != null ? (string)actualizarUsuario.Surnames : usuario.Surnames;
                usuario.Email = actualizarUsuario.Email != null ? (string)actualizarUsuario.Email : usuario.Email;
                usuario.EmailAux = actualizarUsuario.EmailAux != null ? (string)actualizarUsuario.EmailAux : usuario.EmailAux;
                usuario.PhoneNumber = actualizarUsuario.PhoneNumber != null ? (string)actualizarUsuario.PhoneNumber : usuario.PhoneNumber;
                usuario.PhoneNumberAux = actualizarUsuario.PhoneNumberAux != null ? (string)actualizarUsuario.PhoneNumberAux : usuario.PhoneNumberAux;
                usuario.IdRol = actualizarUsuario.IdRol != null ? (string)actualizarUsuario.IdRol : usuario.IdRol;
                usuario.Status = actualizarUsuario.IdEstado != null ? (string)actualizarUsuario.IdEstado : usuario.Status;
                usuario.IdOccupationProfession = actualizarUsuario.IdOccupationProfession != null ? (string)actualizarUsuario.IdOccupationProfession : usuario.IdOccupationProfession;
                usuario.HaveDisability = actualizarUsuario.HaveDisability != null ? (bool)actualizarUsuario.HaveDisability : usuario.HaveDisability;
                usuario.ReadingWritingSkills = actualizarUsuario.ReadingWritingSkills != null ? (bool)actualizarUsuario.ReadingWritingSkills : usuario.ReadingWritingSkills;
                await context.SaveChangesAsync();

                return Ok(new General()
                {
                    title = "Actualizar tipo documento",
                    status = 200,
                    message = "Tipo documento actualizado"
                });
            }
            catch (Exception ex)
            {
                logger.LogError("Actualizar tipo documento" + ex.Message.ToString() + " - " + ex.StackTrace);
                return BadRequest(new General()
                {
                    title = "Actualizar tipo documento",
                    status = 400,
                    message = ""
                });
            }
        }
        #endregion
    }
}
