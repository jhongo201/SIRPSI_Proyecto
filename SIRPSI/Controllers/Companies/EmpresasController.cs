using AutoMapper;
using DataAccess.Context;
using DataAccess.Models.Companies;
using DataAccess.Models.Documents;
using DataAccess.Models.Estados;
using DataAccess.Models.Ministry;
using DataAccess.Models.Rols;
using DataAccess.Models.Status;
using DataAccess.Models.Users;
using EmailServices;
using EvertecApi.Log4net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIRPSI.Controllers.User;
using SIRPSI.Core.Helper;
using SIRPSI.DTOs.Companies;
using SIRPSI.DTOs.Document;
using SIRPSI.DTOs.User;
using SIRPSI.Helpers.Answers;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;

namespace SIRPSI.Controllers.Companies
{
    [Route("api/empresas")]
    [ApiController]
    [EnableCors("CorsApi")]
    public class EmpresasController : ControllerBase
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
        public EmpresasController(AppDbContext context,
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
        [HttpGet("ConsultarEmpresas", Name = "consultarEmpresas")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<object>> Get()
        {
            try
            {
                //Consulta estados
                var estado = await context.estados.Where(x => x.IdConsecutivo.Equals(1)).FirstOrDefaultAsync();

                if (estado == null)
                {
                    return NotFound(new General()
                    {
                        title = "Consultar empresas",
                        status = 404,
                        message = "Estado no encontrado"
                    });
                }

                //Consulta el empresa
                var empresa = context.empresas.
                    Join(context.tiposDocumento,
                    e => e.TipoDocumento,
                    td => td.Id,
                    (e, td) => new { empresa = e, tipoDoc = td }
                    ).
                    Join(context.tiposEmpresas,
                    er => er.empresa.IdTipoEmpresa,
                    te => te.Id,
                    (er, te) => new { resulEmpre = er, tipoEmp = te })
                    .Join(context.ministerio,
                    r => r.resulEmpre.empresa.IdMinisterio,
                    m => m.Id,
                    (r, m) => new { restipoEmp = r, ministerio = m })
                    .Join(context.estados,
                    rt => rt.restipoEmp.resulEmpre.empresa.IdEstado,
                    es => es.Id,
                    (rt, es) => new { rTotal = rt, estado = es })
                    .Where(x => x.rTotal.restipoEmp.resulEmpre.empresa.IdEstado.Equals(estado.Id)).Select(x => new
                    {
                        x.rTotal.restipoEmp.resulEmpre.empresa.Id,
                        x.rTotal.restipoEmp.resulEmpre.empresa.TipoDocumento,
                        tipoDocNombre = x.rTotal.restipoEmp.resulEmpre.tipoDoc.Nombre,
                        x.rTotal.restipoEmp.resulEmpre.empresa.Documento,
                        x.rTotal.restipoEmp.resulEmpre.empresa.DigitoVerificacion,
                        x.rTotal.restipoEmp.resulEmpre.empresa.IdTipoEmpresa,
                        tipoEmpNombre = x.rTotal.restipoEmp.tipoEmp.Nombre,
                        x.rTotal.restipoEmp.resulEmpre.empresa.Nombre,
                        x.rTotal.restipoEmp.resulEmpre.empresa.IdMinisterio,
                        ministerioNombre = x.rTotal.ministerio.Nombre,
                        x.rTotal.restipoEmp.resulEmpre.empresa.IdEstado,
                        estadoNombre = x.estado.Nombre,
                        x.rTotal.restipoEmp.resulEmpre.empresa.IdConsecutivo,
                        x.rTotal.restipoEmp.resulEmpre.empresa.Descripcion,
                        usuario = x.rTotal.restipoEmp.resulEmpre.empresa.IdUsuario != null ?
                            (context.AspNetUsers.Where(d => d.Id == x.rTotal.restipoEmp.resulEmpre.empresa.IdUsuario).FirstOrDefault()) : null,
                    }).ToList();
                if (empresa == null)
                {
                    //Visualizacion de mensajes al usuario del aplicativo
                    return NotFound(new General()
                    {
                        title = "Consultar empresas",
                        status = 404,
                        message = "Empresa no encontrada"
                    });
                }
                //Retorno de los datos encontrados
                return empresa;
            }
            catch (Exception ex)
            {
                //Registro de errores
                logger.LogError("Consultar empresa " + ex.Message.ToString() + " - " + ex.StackTrace);
                return BadRequest(new General()
                {
                    title = "Consultar empresa",
                    status = 400,
                    message = "Contacte con el administrador del sistema"
                });
            }
        }

        [HttpGet("ConsultarEmpresasUsuario", Name = "consultarEmpresasUsuario")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<object>> GetByUser(string? user)
        {
            try
            {
                var empresa = new List<ConsultarEmpresas>();
                //Consulta estados
                var estado = await context.estados.Where(x => x.IdConsecutivo.Equals(1)).FirstOrDefaultAsync();
                if (estado == null)
                {
                    return NotFound(new General()
                    {
                        title = "Consultar empresas",
                        status = 404,
                        message = "Estado no encontrado"
                    });
                }
                //Consulta el empresa
                empresa = (from data in await context.empresas.ToListAsync()
                           where data.IdUsuario == user
                           //&& data.IdEstado.Equals(estado.Id)
                           select new ConsultarEmpresas()
                           {
                               Id = data.Id,
                               DigitoVerificacion = data.DigitoVerificacion,
                               IdTipoEmpresa = data.IdTipoEmpresa,
                               Documento = data.Documento,
                               Nombre = data.Nombre,
                               IdMinisterio = data.IdMinisterio,
                               IdEstado = data.IdEstado,
                               IdConsecutivo = data.IdConsecutivo,
                               Descripcion = data.Descripcion,
                               IdUsuario = data.IdUsuario,
                               Usuario = data.IdUsuario != null ? (context.AspNetUsers.Where(x => x.Id == data.IdUsuario).FirstOrDefault()) : null,
                               Estado = data.IdEstado != null ? (context.estados.Where(x => x.Id == data.IdEstado).FirstOrDefault()) : null,
                               TipoDocumento = data.TipoDocumento != null ? (context.tiposDocumento.Where(x => x.Id == data.TipoDocumento).FirstOrDefault()) : null,
                               TipoEmpresa = data.IdTipoEmpresa != null ? (context.tiposEmpresas.Where(x => x.Id == data.IdTipoEmpresa).FirstOrDefault()) : null,
                               Ministerio = data.IdMinisterio != null ? (context.ministerio.Where(x => x.Id == data.IdMinisterio).FirstOrDefault()) : null,
                               FechaRegistro = data.FechaRegistro,
                               FechaModifico = data.FechaModifico,
                           }).ToList();
                return Ok(empresa);
            }
            catch (Exception ex)
            {
                //Registro de errores
                logger.LogError("Consultar empresa " + ex.Message.ToString() + " - " + ex.StackTrace);
                return BadRequest(new General()
                {
                    title = "Consultar empresa",
                    status = 400,
                    message = "Contacte con el administrador del sistema"
                });
            }
        }

        #endregion

        #region Registro
        [HttpPost("RegistrarEmpresa")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Post([FromBody] RegistrarEmpresas registrarEmpresas)
        {
            try
            {
                //Consulta estados
                var estados = await context.estados.Where(x => x.Id.Equals(registrarEmpresas.IdEstado)).FirstOrDefaultAsync();

                if (estados == null)
                {
                    return NotFound(new General()
                    {
                        title = "Registrar empresa",
                        status = 404,
                        message = "Estado no encontrado"
                    });
                }

                //Consulta tipo documento
                var tipoDocumento = await context.tiposDocumento.Where(x => x.Id.Equals(registrarEmpresas.TipoDocumento)).FirstOrDefaultAsync();

                if (tipoDocumento == null)
                {
                    return NotFound(new General()
                    {
                        title = "Registrar empresa",
                        status = 404,
                        message = "Tipo documento no encontrado"
                    });
                }

                //Consulta tipo empresa
                var tipoEmpresa = await context.tiposEmpresas.Where(x => x.Id.Equals(registrarEmpresas.IdTipoEmpresa)).FirstOrDefaultAsync();

                if (tipoEmpresa == null)
                {
                    return NotFound(new General()
                    {
                        title = "Registrar empresa",
                        status = 404,
                        message = "Tipo empresa no encontrado"
                    });
                }
                var empresas = await context.empresas.Where(x => x.Documento.Equals(registrarEmpresas.Documento) && x.DigitoVerificacion.Equals(registrarEmpresas.DigitoVerificacion)).FirstOrDefaultAsync();

                if (empresas != null)
                {
                    return NotFound(new General()
                    {
                        title = "Registrar empresa",
                        status = 404,
                        message = "Ya existe una empresa con este documento y digito de verificación"
                    });
                }
                var user = new AspNetUsers();
                if (registrarEmpresas.Usuario != null)
                {
                    #region Validaciones
                    var existTypeDoc = await context.tiposDocumento.Where(x => x.Id.Equals(registrarEmpresas.Usuario.IdTypeDocument)).FirstOrDefaultAsync();
                    if (existTypeDoc == null)
                        return BadRequest(new General()
                        {
                            title = "usuario",
                            status = 400,
                            message = "Tipo de documento NO existe."
                        });

                    var existRoles = await context.AspNetRoles.Where(x => x.Id.Equals(registrarEmpresas.Usuario.IdRol)).FirstOrDefaultAsync();
                    if (existRoles == null)
                        return BadRequest(new General()
                        {
                            title = "usuario",
                            status = 400,
                            message = "Estado del usuario NO existe."
                        });

                    var existCountry = await context.pais.Where(x => x.Id.Equals(registrarEmpresas.Usuario.IdCountry)).FirstOrDefaultAsync();
                    if (existCountry == null)
                        return BadRequest(new General()
                        {
                            title = "usuario",
                            status = 400,
                            message = "Pais NO existe."
                        });

                    if (!string.IsNullOrEmpty(registrarEmpresas.Usuario.IdCompany))
                    {
                        var existCompany = await context.empresas.Where(x => x.Id.Equals(registrarEmpresas.Usuario.IdCompany)).FirstOrDefaultAsync();
                        if (existCompany == null)
                            return BadRequest(new General()
                            {
                                title = "usuario",
                                status = 400,
                                message = "Empresa NO existe."
                            });
                    }

                    var existStatus = await context.estados.Where(x => x.Id.Equals(registrarEmpresas.Usuario.IdEstado)).FirstOrDefaultAsync();
                    if (existStatus == null)
                        return BadRequest(new General()
                        {
                            title = "usuario",
                            status = 400,
                            message = "Estado NO existe."
                        });

                    var emailValidation = registrarEmpresas.Usuario.Email.Split('@')[1].Split('.')[0];
                    var rolesValidationMinisterio = await context.variables
                        .Where(x => x.Modulo == "Users" && x.Variable1 == "ValidacionMinisterio").ToListAsync();

                    var validarionRoleEspecial = rolesValidationMinisterio.Where(x => x.Variable2 == existRoles.Id).FirstOrDefault();
                    if (emailValidation != "ministerio" && validarionRoleEspecial != null)
                        return BadRequest(new General()
                        {
                            title = "usuario",
                            status = 400,
                            message = "El email no tiene el dominio correcto (Ejm: ministerio.xxx.xx)."
                        });
                    #endregion
                    var userNet = mapper.Map<AspNetUsers>(registrarEmpresas.Usuario);

                    userNet.Document = registrarEmpresas.Usuario.Document;
                    userNet.IdCompany = registrarEmpresas.Usuario.IdCompany;
                    userNet.IdCountry = registrarEmpresas.Usuario.IdCountry;
                    userNet.TypeDocument = registrarEmpresas.Usuario.IdTypeDocument;

                    if (string.IsNullOrEmpty(userNet.Email))
                    {
                        userNet.UserName = registrarEmpresas.Usuario.Document + "@sirpsi.com";
                        userNet.Email = registrarEmpresas.Usuario.Document + "@sirpsi.com";
                    }
                    else
                    {
                        userNet.UserName = registrarEmpresas.Usuario.Email;
                        userNet.Email = registrarEmpresas.Usuario.Email;
                    }
                    userNet.Surnames = registrarEmpresas.Usuario.Surnames;
                    userNet.Names = registrarEmpresas.Usuario.Names;
                    userNet.PhoneNumber = registrarEmpresas.Usuario.PhoneNumber;
                    userNet.UserRegistration = registrarEmpresas.Usuario.Document;
                    userNet.RegistrationDate = DateTime.Now.ToDateTimeZone().DateTime;
                    userNet.UserModify = null;
                    userNet.ModifiedDate = null;
                    userNet.Status = registrarEmpresas.Usuario.IdEstado;
                    userNet.Id = Guid.NewGuid().ToString();
                    if (emailValidation != "ministerio" && validarionRoleEspecial != null)
                        return BadRequest(new General()
                        {
                            title = "usuario",
                            status = 400,
                            message = "El email no tiene el dominio correcto (Ejm: ministerio.xxx.xx)."
                        });

                    var ministerioValidationUsersDisabled = await context.variables
                        .Where(x => x.Variable1 == "ValidationInternalActivation").ToListAsync();

                    var valministerioValidationUsersDisabled = ministerioValidationUsersDisabled.Where(x => x.Variable2 == userNet.IdCompany).FirstOrDefault();
                    var StatusCodeChange = context.variables.Where(x => x.Nombre == "SendChangeStatusCode" && x.Variable1 == registrarEmpresas.Usuario.IdEstado).FirstOrDefault();

                    if (StatusCodeChange != null && valministerioValidationUsersDisabled == null)
                        registrarEmpresas.Usuario.CodeActivation = userNet.CodeActivation = new Random().Next(10000, 99999).ToString();

                    context.Add(userNet);
                    var result = await userManager.CreateAsync(userNet, registrarEmpresas.Usuario.Password);
                    user = userNet;
                }

                //Mapeo de datos en clases
                var empresa = mapper.Map<Empresas>(registrarEmpresas);
                //Valores asignados
                empresa.Id = Guid.NewGuid().ToString();
                empresa.TipoDocumento = registrarEmpresas.TipoDocumento;
                empresa.Documento = registrarEmpresas.Documento;
                empresa.Nombre = registrarEmpresas.Nombre;
                empresa.Descripcion = registrarEmpresas.Descripcion;
                empresa.IdTipoEmpresa = registrarEmpresas.IdTipoEmpresa;
                empresa.DigitoVerificacion = registrarEmpresas.DigitoVerificacion;
                empresa.IdEstado = estados.Id;
                empresa.IdMinisterio = registrarEmpresas.IdMinisterio;
                empresa.UsuarioRegistro = context.AspNetUsers.FirstOrDefault().Id;
                empresa.FechaRegistro = DateTime.Now.ToDateTimeZone().DateTime;
                empresa.FechaModifico = null;
                empresa.UsuarioModifico = null;
                empresa.IdUsuario = user.Id;

                //Agregar datos al contexto
                user.IdCompany = empresa.Id;
                context.Add(empresa);
                //Guardado de datos 
                await context.SaveChangesAsync();
                if (registrarEmpresas.Usuario != null)
                {
                    context.Update(user);
                    await context.SaveChangesAsync();
                }

                return Created("", new General()
                {
                    //Visualizacion de mensajes al usuario del aplicativo
                    title = "Registrar empresa",
                    status = 201,
                    message = "Empresa creada",
                    otherdata = empresa.Id
                });
            }
            catch (Exception ex)
            {
                //Registro de errores
                logger.LogError("Registrar empresa " + ex.Message.ToString() + " - " + ex.StackTrace);
                return BadRequest(new General()
                {
                    title = "Registrar empresa",
                    status = 400,
                    message = "Contacte con el administrador del sistema"
                });
            }
        }
        #endregion

        #region Actualizar
        [HttpPut("ActualizarEmpresa")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Put(ActualizarEmpresas actualizarEmpresas)
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
                        title = "Actualizar empresa",
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
                        title = "Actualizar empresas",
                        status = 404,
                        message = "Roles no encontrados"
                    });
                }

                //Revisa los permisos de usuario
                var permisos = await context.permisosXUsuario.Where(x => x.Vista.Equals(getUrl) && x.IdUsuario.Equals(usuario.Id)).ToListAsync();

                //Consulta si tiene el permiso
                var permitido = permisos.Select(x => x.Actualizar.Equals(true)).FirstOrDefault();

                //Si es permitido
                if (true)
                {
                    //Consulta tipo documento
                    var tipoDocumento = await context.tiposDocumento.Where(x => x.Id.Equals(actualizarEmpresas.TipoDocumento)).FirstOrDefaultAsync();

                    if (tipoDocumento == null)
                    {
                        return NotFound(new General()
                        {
                            title = "Actualizar empresa",
                            status = 404,
                            message = "Tipo documento no encontrado"
                        });
                    }

                    //Consulta tipo empresa
                    var tipoEmpresa = await context.tiposEmpresas.Where(x => x.Id.Equals(actualizarEmpresas.IdTipoEmpresa)).FirstOrDefaultAsync();

                    if (tipoEmpresa == null)
                    {
                        return NotFound(new General()
                        {
                            title = "Actualizar empresa",
                            status = 404,
                            message = "Tipo empresa no encontrado"
                        });
                    }

                    //Consulta de empresa del usuario
                    var existe = await context.empresas.Where(x => x.Id.Equals(actualizarEmpresas.Id)).FirstOrDefaultAsync();

                    if (existe == null)
                    {
                        //Visualizacion de mensajes al usuario del aplicativo
                        return NotFound(new
                        {
                            //Visualizacion de mensajes al usuario del aplicativo
                            title = "Actualizar empresa",
                            status = 404,
                            message = "Empresa no encontrada"
                        });
                    }
                    //Consulta de estados
                    var estados = await context.estados.ToListAsync();
                    //Registro de datos
                    context.empresas.Where(x => x.Id.Equals(existe.Id)).ToList()
                        .ForEach(r =>
                        {
                            r.TipoDocumento = actualizarEmpresas.TipoDocumento;
                            r.Documento = actualizarEmpresas.Documento != null ? actualizarEmpresas.Documento : "";
                            r.Nombre = actualizarEmpresas.Nombre != null ? actualizarEmpresas.Nombre : "";
                            r.Descripcion = actualizarEmpresas.Descripcion != null ? actualizarEmpresas.Descripcion : "";
                            r.IdTipoEmpresa = actualizarEmpresas.IdTipoEmpresa;
                            r.DigitoVerificacion = actualizarEmpresas.DigitoVerificacion;
                            r.UsuarioModifico = usuario.Document;
                            r.FechaModifico = DateTime.Now.ToDateTimeZone().DateTime;
                            r.IdUsuario = actualizarEmpresas.IdUsuario;
                        });
                    //Guardado de datos
                    await context.SaveChangesAsync();

                    return Ok(new General()
                    {
                        //Visualizacion de mensajes al usuario del aplicativo
                        title = "Actualizar empresa",
                        status = 200,
                        message = "Empresa actualizada"
                    });
                }
                else
                {
                    return BadRequest(new General()
                    {
                        title = "Actualizar empresa",
                        status = 400,
                        message = "No tiene permisos para actualizar empresas"
                    });
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Actualizar empresa " + ex.Message.ToString() + " - " + ex.StackTrace);
                return BadRequest(new General()
                {
                    title = "Actualizar empresa",
                    status = 400,
                    message = ""
                });
            }
        }
        #endregion

        #region Eliminar
        [HttpDelete("EliminarEmpresa")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Delete([FromBody] EliminarEmpresas eliminarEmpresas)
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
                        title = "Eliminar empresa",
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
                        title = "Eliminar empresa",
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

                    if (estados == null)
                    {
                        return NotFound(new General()
                        {
                            title = "Eliminar empresa",
                            status = 404,
                            message = "Estados no encontrado"
                        });
                    }

                    //Consulta de empresa
                    var existe = await context.empresas.Where(x => x.Id.Equals(eliminarEmpresas.Id)).FirstOrDefaultAsync();

                    if (existe == null)
                    {
                        return NotFound(new General()
                        {
                            title = "Eliminar empresa",
                            status = 404,
                            message = "Empresa no encontrada"
                        });
                    }

                    //Agregar datos al contexto
                    context.empresas.Where(x => x.Id.Equals(eliminarEmpresas.Id)).ToList()
                  .ForEach(r =>
                  {
                      r.IdEstado = estados.Where(x => x.IdConsecutivo.Equals(2)).Select(x => x.Id).First();
                      r.UsuarioModifico = usuario.Document;
                      r.FechaModifico = DateTime.Now.ToDateTimeZone().DateTime;
                  });

                    //Se elimina el regitro de forma logica
                    await context.SaveChangesAsync();

                    return Ok(new General()
                    {
                        //Visualizacion de mensajes al usuario del aplicativo
                        title = "Eliminar empresa",
                        status = 200,
                        message = "Empresa eliminada"
                    });
                }
                else
                {
                    return BadRequest(new General()
                    {
                        title = "Eliminar empresa",
                        status = 400,
                        message = "No tiene permisos para eliminar empresas"
                    });
                }
            }
            catch (Exception ex)
            {
                //Registro de errores
                logger.LogError("Eliminar empresa " + ex.Message.ToString() + " - " + ex.StackTrace);
                return BadRequest(new General()
                {
                    title = "Eliminar empresa",
                    status = 400,
                    message = "Contacte con el administrador del sistema"
                });
            }

        }
        #endregion
    }
}
