using AutoMapper;
using DataAccess.Context;
using DataAccess.Models.Companies;
using DataAccess.Models.Documents;
using DataAccess.Models.Estados;
using DataAccess.Models.Ministry;
using DataAccess.Models.RepresentativeCompany;
using DataAccess.Models.Rols;
using DataAccess.Models.Status;
using DataAccess.Models.Users;
using DataAccess.Models.WorkPlace;
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
using SIRPSI.DTOs.RepresentativeCompany;
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
                               Observacion = data.Observacion,
                               IdUsuario = data.IdUsuario,
                               IdActividadEconomica = data.IdActividadEconomica,
                               NumeroTrabajadores = data.NumeroTrabajadores,
                               ClaseRiesgo = data.ClaseRiesgo,
                               IdSectorEconomico = data.IdSectorEconomico,
                               IdTipoPersona = data.IdTipoPersona,
                               IdRegimenTributario = data.IdRegimenTributario,
                               Usuario = data.IdUsuario != null ? (context.AspNetUsers.Where(x => x.Id == data.IdUsuario).FirstOrDefault()) : null,
                               Estado = data.IdEstado != null ? (context.estados.Where(x => x.Id == data.IdEstado).FirstOrDefault()) : null,
                               TipoDocumento = data.TipoDocumento != null ? (context.tiposDocumento.Where(x => x.Id == data.TipoDocumento).FirstOrDefault()) : null,
                               TipoEmpresa = data.IdTipoEmpresa != null ? (context.tiposEmpresas.Where(x => x.Id == data.IdTipoEmpresa).FirstOrDefault()) : null,
                               Ministerio = data.IdMinisterio != null ? (context.ministerio.Where(x => x.Id == data.IdMinisterio).FirstOrDefault()) : null,
                               TipoPersona = data.IdTipoPersona != null ? (context.tiposPersona.Where(x => x.Id == data.IdTipoPersona).FirstOrDefault()) : null,
                               RegimenTributario = data.IdRegimenTributario != null ? (context.regimenesTributario.Where(x => x.Id == data.IdRegimenTributario).FirstOrDefault()) : null,
                               CentroTrabajo = (context.centroTrabajo.Where(x => x.IdEmpresa == data.Id && x.Principal == true).FirstOrDefault()),
                               FechaRegistro = data.FechaRegistro,
                               FechaModifico = data.FechaModifico,
                               IdRepresentanteEmpresa = data.IdRepresentanteEmpresa,
                               RepresentanteEmpresa = (context.representanteEmpresa.Where(x => x.Id == data.IdRepresentanteEmpresa).FirstOrDefault()),
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
        public async Task<ActionResult> Post([FromBody] RegistrarEmpresaRequest registrarEmpresas)
        {
            try
            {
                //Consulta estados
                var estados = await context.estados.Where(x => x.Id.Equals(registrarEmpresas.Empresa.IdEstado)).FirstOrDefaultAsync();

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
                var tipoDocumento = await context.tiposDocumento.Where(x => x.Id.Equals(registrarEmpresas.Empresa.TipoDocumento)).FirstOrDefaultAsync();

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
                var tipoEmpresa = await context.tiposEmpresas.Where(x => x.Id.Equals(registrarEmpresas.Empresa.IdTipoEmpresa)).FirstOrDefaultAsync();

                if (tipoEmpresa == null)
                {
                    return NotFound(new General()
                    {
                        title = "Registrar empresa",
                        status = 404,
                        message = "Tipo empresa no encontrado"
                    });
                }
                var empresas = await context.empresas.Where(x => x.Documento.Equals(registrarEmpresas.Empresa.Documento) && x.DigitoVerificacion.Equals(registrarEmpresas.Empresa.DigitoVerificacion)).FirstOrDefaultAsync();

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
                    var existTypeDoc = await context.tiposDocumento.Where(x => x.Id.Equals(registrarEmpresas.Usuario.TypeDocument)).FirstOrDefaultAsync();
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

                    var existStatus = await context.estados.Where(x => x.Id.Equals(registrarEmpresas.Usuario.Status)).FirstOrDefaultAsync();
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
                    userNet.TypeDocument = registrarEmpresas.Usuario.TypeDocument;

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
                    userNet.Status = registrarEmpresas.Usuario.Status;
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
                    var StatusCodeChange = context.variables.Where(x => x.Nombre == "SendChangeStatusCode" && x.Variable1 == registrarEmpresas.Usuario.Status).FirstOrDefault();

                    if (StatusCodeChange != null && valministerioValidationUsersDisabled == null)
                        registrarEmpresas.Usuario.CodeActivation = userNet.CodeActivation = new Random().Next(10000, 99999).ToString();

                    context.Add(userNet);
                    var result = await userManager.CreateAsync(userNet, registrarEmpresas.Usuario.Password);
                    user = userNet;
                }
                var representante = new RepresentanteEmpresa();
                if (registrarEmpresas.RepresentanteEmpresa != null)
                {
                    representante = mapper.Map<RepresentanteEmpresa>(registrarEmpresas.RepresentanteEmpresa);
                    representante.Id = Guid.NewGuid().ToString();
                    context.Add(representante);
                    await context.SaveChangesAsync();
                }
                //Mapeo de datos en clases
                var empresa = mapper.Map<Empresas>(registrarEmpresas.Empresa);
                //Valores asignados
                empresa.Id = Guid.NewGuid().ToString();
                empresa.TipoDocumento = registrarEmpresas.Empresa.TipoDocumento;
                empresa.Documento = registrarEmpresas.Empresa.Documento;
                empresa.Nombre = registrarEmpresas.Empresa.Nombre;
                empresa.Descripcion = registrarEmpresas.Empresa.Descripcion;
                empresa.IdTipoEmpresa = registrarEmpresas.Empresa.IdTipoEmpresa;
                empresa.DigitoVerificacion = registrarEmpresas.Empresa.DigitoVerificacion;
                empresa.IdEstado = estados.Id;
                empresa.IdMinisterio = registrarEmpresas.Empresa.IdMinisterio;
                empresa.UsuarioRegistro = context.AspNetUsers.FirstOrDefault().Id;
                empresa.FechaRegistro = DateTime.Now.ToDateTimeZone().DateTime;
                empresa.FechaModifico = null;
                empresa.UsuarioModifico = null;
                empresa.IdUsuario = user.Id;
                empresa.IdConsecutivo = 0;
                empresa.IdRepresentanteEmpresa = representante.Id;

                //Agregar datos al contexto
                user.IdCompany = empresa.Id;
                context.Add(empresa);
                await context.SaveChangesAsync();
                var centroTrabajo = mapper.Map<CentroTrabajo>(registrarEmpresas.CentroTrabajo);
                //Valores asignados
                centroTrabajo.Id = Guid.NewGuid().ToString();
                centroTrabajo.Nombre = registrarEmpresas.CentroTrabajo.Nombre != null ? registrarEmpresas.CentroTrabajo.Nombre : "";
                centroTrabajo.Descripcion = registrarEmpresas.CentroTrabajo.Descripcion;
                centroTrabajo.IdEmpresa = empresa.Id;
                centroTrabajo.IdEstado = estados.Id;
                centroTrabajo.UsuarioRegistro = user.Id;
                centroTrabajo.FechaRegistro = DateTime.Now.ToDateTimeZone().DateTime;
                centroTrabajo.UsuarioModifico = null;
                centroTrabajo.UsuarioModifico = null;
                centroTrabajo.Principal = true;

                //Agregar datos al contexto
                context.Add(centroTrabajo);
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
        [HttpPut("ActualizarEmpresaAsignar")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> PutAssign(ActualizarEmpresasAssign actualizarEmpresas)
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
                    var tipoDocumento = await context.tiposDocumento.Where(x => x.Id.Equals(actualizarEmpresas.Empresa.TipoDocumento)).FirstOrDefaultAsync();

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
                    var tipoEmpresa = await context.tiposEmpresas.Where(x => x.Id.Equals(actualizarEmpresas.Empresa.IdTipoEmpresa)).FirstOrDefaultAsync();

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
                    var existe = await context.empresas.Where(x => x.Id.Equals(actualizarEmpresas.Empresa.Id)).FirstOrDefaultAsync();

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
                    var centroTrabajo = await context.centroTrabajo.Where(x => x.Id.Equals(actualizarEmpresas.CentroTrabajo.Id)).FirstOrDefaultAsync();
                    var admin = await context.AspNetUsers.Where(x => x.Id.Equals(actualizarEmpresas.Usuario.Id)).FirstOrDefaultAsync();
                    //Registro de datos
                    existe.IdTipoPersona = actualizarEmpresas.Empresa.IdTipoPersona;
                    existe.IdRegimenTributario = actualizarEmpresas.Empresa.IdRegimenTributario;
                    existe.TipoDocumento = actualizarEmpresas.Empresa.TipoDocumento;
                    existe.DigitoVerificacion = actualizarEmpresas.Empresa.DigitoVerificacion;
                    existe.IdTipoEmpresa = actualizarEmpresas.Empresa.IdTipoEmpresa;
                    existe.IdActividadEconomica = actualizarEmpresas.Empresa.IdActividadEconomica;
                    existe.Documento = actualizarEmpresas.Empresa.Documento;
                    existe.Nombre = actualizarEmpresas.Empresa.Nombre;
                    existe.IdMinisterio = actualizarEmpresas.Empresa.IdMinisterio;
                    existe.IdEstado = actualizarEmpresas.Empresa.IdEstado;
                    existe.IdUsuario = actualizarEmpresas.Empresa.IdUsuario;

                    centroTrabajo.Nombre = actualizarEmpresas.CentroTrabajo.Nombre;
                    centroTrabajo.Descripcion = actualizarEmpresas.CentroTrabajo.Descripcion;
                    centroTrabajo.Principal = actualizarEmpresas.CentroTrabajo.Principal;
                    centroTrabajo.IdDepartamento = actualizarEmpresas.CentroTrabajo.IdDepartamento;
                    centroTrabajo.IdMunicipio = actualizarEmpresas.CentroTrabajo.IdMunicipio;
                    centroTrabajo.Direccion = actualizarEmpresas.CentroTrabajo.Direccion;
                    centroTrabajo.Email = actualizarEmpresas.CentroTrabajo.Email;
                    centroTrabajo.Telefono = actualizarEmpresas.CentroTrabajo.Telefono;
                    centroTrabajo.Celular = actualizarEmpresas.CentroTrabajo.Celular;

                    admin.TypeDocument = actualizarEmpresas.Usuario.TypeDocument;
                    admin.Document = actualizarEmpresas.Usuario.Document;
                    admin.IdCountry = actualizarEmpresas.Usuario.IdCountry;
                    admin.IdCompany = actualizarEmpresas.Usuario.IdCompany;
                    admin.Names = actualizarEmpresas.Usuario.Names;
                    admin.Surnames = actualizarEmpresas.Usuario.Surnames;
                    admin.IdRol = actualizarEmpresas.Usuario.IdRol;
                    admin.PhoneNumber = actualizarEmpresas.Usuario.PhoneNumber;
                    admin.Email = actualizarEmpresas.Usuario.Email;

                    context.Update(existe);
                    await context.SaveChangesAsync();
                    context.Update(centroTrabajo);
                    await context.SaveChangesAsync();
                    context.Update(admin);
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

        [HttpPut("ActualizarDatosComplementariosEmpresa")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> PutDataComplement(ActualizarDatosComplementariosEmpresas actualizarEmpresas)
        {
            try
            {
                //Claims de usuario - Enviados por token
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
                var countLast = HttpContext.Request.GetDisplayUrl().Split("/").Last().Count();
                string Url = HttpContext.Request.GetDisplayUrl();
                var getUrl = Url.Remove(Url.Length - (countLast + 1));
                var rolesList = new List<string>();
                var list = roles.Split(',').ToList();

                foreach (var i in list)
                {
                    var result = context.AspNetRoles.Where(r => r.Id.Equals(i)).Select(x => x.Description).FirstOrDefault();
                    if (result != null) rolesList.Add(result.ToString());
                }

                if (rolesList == null)
                    return NotFound(new General()
                    {
                        title = "Actualizar empresas",
                        status = 404,
                        message = "Roles no encontrados"
                    });

                var permisos = await context.permisosXUsuario.Where(x => x.Vista.Equals(getUrl) && x.IdUsuario.Equals(usuario.Id)).ToListAsync();
                var permitido = permisos.Select(x => x.Actualizar.Equals(true)).FirstOrDefault();
                if (true)
                {
                    var existe = await context.empresas.Where(x => x.Id.Equals(actualizarEmpresas.Id)).FirstOrDefaultAsync();
                    if (existe == null)
                    {
                        return NotFound(new
                        {
                            title = "Actualizar empresa",
                            status = 404,
                            message = "Empresa no encontrada"
                        });
                    }
                    existe.NumeroTrabajadores = actualizarEmpresas.NumeroTrabajadores;
                    existe.ClaseRiesgo = actualizarEmpresas.ClaseRiesgo;
                    existe.IdSectorEconomico = actualizarEmpresas.IdSectorEconomico;
                    existe.IdConsecutivo = 0;
                    //Guardado de datos
                    context.Update(existe);
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
