using AutoMapper;
using DataAccess.Context;
using DataAccess.Models.Module;
using DataAccess.Models.PsychosocialEvaluation;
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
using SIRPSI.DTOs.PsychosocialEvaluation;
using SIRPSI.DTOs.User;
using SIRPSI.DTOs.WorkPlace;
using SIRPSI.Helpers.Answers;
using SIRPSI.Settings;
using System.Security.Claims;

namespace SIRPSI.Controllers.Evaluation
{
    [Route("api/evaluacionPsicosocial")]
    [ApiController]
    [EnableCors("CorsApi")]
    public class EvaluacionController : ControllerBase
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
        public EvaluacionController(AppDbContext context,
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
                                       where (context.evaluacionPsicosocialUsuario.Where(x => x.IdUsuario == data.UserId && x.Finalizado == false).FirstOrDefault() == null)
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

        [HttpGet("ConsultarUsuariosEvaluacion")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<object>> GetByWorkCenterEvaluation(string workCenter)
        {
            var users = (from data in await context.evaluacionPsicosocialUsuario.ToListAsync()
                         where data.IdCentroTrabajo == workCenter && data.Finalizado == false
                         select new ConsultarEvaluacionPsicosocial()
                         {
                             Id = data.Id,
                             IdCentroTrabajo = data.IdCentroTrabajo,
                             IdUsuario = data.IdUsuario,
                             FechaInicio = data.FechaInicio,
                             IdEstado = data.IdEstado,
                             IdUsuarioRegistra = data.IdUsuarioRegistra,
                             Finalizado = data.Finalizado,
                             Radicado = "RD0000002",
                             Consentimiento = "Si",
                             Porcentaje = 12,
                             Usuario = (from user in context.AspNetUsers.Where(x => x.Id == data.IdUsuario)
                                        select new ConsultarUsuarios()
                                        {
                                            Id = user.Id,
                                            idTipoDocumento = user.TypeDocument,
                                            tipoDocumento = (context.tiposDocumento.Where(x => x.Id == user.TypeDocument).FirstOrDefault()),
                                            cedula = user.Document,
                                            correo = user.Email,
                                            correoAux = user.EmailAux,
                                            telefono = user.PhoneNumber,
                                            telefonoAux = user.PhoneNumberAux,
                                            idEmpresa = user.IdCompany,
                                            empresa = (context.empresas.Where(x => x.Id == user.IdCompany).FirstOrDefault()),
                                            nombreUsuario = user.Names,
                                            apellidosUsuario = user.Surnames,
                                            habilidadesLectoEscritura = user.ReadingWritingSkills,
                                            tieneDiscapacidad = user.HaveDisability,
                                            idEstado = user.Status,
                                            IdPais = user.IdCountry,
                                            pais = (context.pais.Where(x => x.Id == user.IdCountry).FirstOrDefault()),
                                            estado = (context.estados.Where(x => x.Id == user.Status).FirstOrDefault()),
                                            IdRol = user.IdRol,
                                            role = (context.Roles.Where(x => x.Id == user.IdRol).FirstOrDefault()),
                                            IdOcupacionProfesion = user.IdOccupationProfession,
                                            ocupacionProfesion = user.IdOccupationProfession != null ? (context.ocupacionProfesion.Where(x => x.Id == user.IdOccupationProfession).FirstOrDefault()) : null,
                                            workPlaces = context.userWorkPlace.Where(x => x.UserId == user.Id).ToList()
                                        }).FirstOrDefault(),
                             CentroTrabajo = (context.centroTrabajo.Where(x => x.Id == data.IdCentroTrabajo).FirstOrDefault()),
                         });

            return Ok(users);
        }



        [HttpGet("ConsultarEvaluacionUsuarioId", Name = "consultarEvaluacionUsuarioId")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<object>> Get(string idUsers)
        {
            try
            {
                //var identity = HttpContext.User.Identity as ClaimsIdentity;
                //if (identity != null)
                //{
                //    IEnumerable<Claim> claims = identity.Claims;
                //}
                //var documento = identity.FindFirst("documento").Value.ToString();
                //var Evaluacion = identity.FindFirst("rol").Value.ToString();
                //var usuario = context.AspNetUsers.Where(u => u.Document.Equals(documento)).FirstOrDefault();
                //if (usuario == null)
                //{
                //    return NotFound(new General()
                //    {
                //        title = "Consultar Evaluacion",
                //        status = 404,
                //        message = "Usuario no encontrado"
                //    });
                //}
                var rol = (from data in (await context.evaluacionPsicosocialUsuario.ToListAsync())
                           where data.IdUsuario == idUsers && data.Finalizado == false
                           select new ConsultarEvaluacionPsicosocial
                           {
                               Id = data.Id,
                               Finalizado = data.Finalizado,
                               FechaInicio = data.FechaInicio,
                              
                           }).ToList();

                if (rol == null)
                {
                    return NotFound(new General()
                    {
                        title = "Consultar Evaluacion",
                        status = 404,
                        message = "Rol no encontrado"
                    });
                }
                return rol;
            }
            catch (Exception ex)
            {
                logger.LogError("Consultar Evaluacion " + ex.Message.ToString() + " - " + ex.StackTrace);
                return BadRequest(new General()
                {
                    title = "Consultar Evaluacion",
                    status = 400,
                    message = "Contacte con el administrador del sistema"
                });
            }

        }
        #endregion

        #region Registro
        [HttpPost("RegistrarEvaluacion")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Post([FromBody] RegistrarEvaluacionPsicosocial registrarEvaluacion)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    IEnumerable<Claim> claims = identity.Claims;
                }
                var documento = identity.FindFirst("documento").Value.ToString();
                var Evaluacion = identity.FindFirst("rol").Value.ToString();
                var usuario = context.AspNetUsers.Where(u => u.Document.Equals(documento)).FirstOrDefault();
                if (usuario == null)
                {
                    return NotFound(new General()
                    {
                        title = "Convocar evaluación",
                        status = 404,
                        message = "Usuario no encontrado"
                    });
                }
                var evaluacion = mapper.Map<EvaluacionPsicosocialUsuario>(registrarEvaluacion);
                evaluacion.Id = Guid.NewGuid().ToString();
                evaluacion.IdUsuarioRegistra = usuario.Id;
                evaluacion.Finalizado = false;
                evaluacion.IdEstado = statusSettings.Activo;

                context.Add(evaluacion);
                await context.SaveChangesAsync();

                return Created("", new General()
                {
                    title = "Convocar evaluación",
                    status = 201,
                    message = "Usuario(s) convocado(s) a evaluación psicosocial."
                });
            }
            catch (Exception ex)
            {
                //Registro de errores
                logger.LogError("Convocar evaluación " + ex.Message.ToString() + " - " + ex.StackTrace);
                return BadRequest(new General()
                {
                    title = "Convocar evaluación",
                    status = 400,
                    message = "Contacte con el administrador del sistema"
                });
            }
        }
        #endregion

        #region Actualizar
        //[HttpPut("ActualizarEvaluacion")]
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
        //        var Evaluacion = identity.FindFirst("rol").Value.ToString();

        //        //Consulta de usuario
        //        var usuario = context.AspNetUsers.Where(u => u.Document.Equals(documento)).FirstOrDefault();

        //        if (usuario == null)
        //        {
        //            return NotFound(new General()
        //            {
        //                title = "Actualizar Evaluacion",
        //                status = 404,
        //                message = "Usuario no encontrado"
        //            });
        //        }

        //        //Obtiene la url del servicio
        //        var countLast = HttpContext.Request.GetDisplayUrl().Split("/").Last().Count();
        //        string Url = HttpContext.Request.GetDisplayUrl();

        //        var getUrl = Url.Remove(Url.Length - (countLast + 1));

        //        //Consulta de Evaluacion por id de usuario

        //        var EvaluacionList = new List<string>();

        //        //Verifica los Evaluacion
        //        var list = Evaluacion.Split(',').ToList();

        //        foreach (var i in list)
        //        {
        //            var result = context.AspNetEvaluacion.Where(r => r.Id.Equals(i)).Select(x => x.Description).FirstOrDefault();

        //            if (result != null)
        //            {
        //                EvaluacionList.Add(result.ToString());
        //            }
        //        }

        //        if (EvaluacionList == null)
        //        {
        //            return NotFound(new General()
        //            {
        //                title = "Actualizar Evaluacion",
        //                status = 404,
        //                message = "Evaluacion no encontrados"
        //            });
        //        }

        //        //Revisa los permisos de usuario
        //        var permisos = await context.permisosXUsuario.Where(x => x.Vista.Equals(getUrl) && x.IdUsuario.Equals(usuario.Id)).ToListAsync();

        //        //Consulta si tiene el permiso
        //        var permitido = permisos.Select(x => x.Actualizar.Equals(true)).FirstOrDefault();

        //        //Si es permitido
        //        if (true)
        //        {
        //            //Consulta de Evaluacion de usuario
        //            var existeRol = await context.modulo.Where(x => x.Id.Equals(actualizarRol.Id)).FirstOrDefaultAsync();

        //            if (existeRol == null)
        //            {
        //                return NotFound(new
        //                {
        //                    //Visualizacion de mensajes al usuario del aplicativo
        //                    title = "Actualizar Evaluacion",
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
        //                title = "Actualizar Evaluacion",
        //                status = 200,
        //                message = "Rol actualizado"
        //            });
        //        }
        //        else
        //        {
        //            return BadRequest(new General()
        //            {
        //                title = "Actualizar Evaluacion",
        //                status = 400,
        //                message = "No tiene permisos para actualizar Evaluacion"
        //            });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogError("Actualizar Evaluacion " + ex.Message.ToString() + " - " + ex.StackTrace);
        //        return BadRequest(new General()
        //        {
        //            title = "Actualizar Evaluacion",
        //            status = 400,
        //            message = "Contacte con el administrador del sistema"
        //        }); ;
        //    }
        //}
        #endregion

        #region Eliminar
        //[HttpDelete("EliminarEvaluacion")]
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
        //        var Evaluacion = identity.FindFirst("rol").Value.ToString();

        //        //Consulta de usuario
        //        var usuario = context.AspNetUsers.Where(u => u.Document.Equals(documento)).FirstOrDefault();

        //        if (usuario == null)
        //        {
        //            return NotFound(new General()
        //            {
        //                title = "Eliminar Evaluacion",
        //                status = 404,
        //                message = "Usuario no encontrado"
        //            });
        //        }

        //        //Obtiene la url del servicio
        //        var countLast = HttpContext.Request.GetDisplayUrl().Split("/").Last().Count();
        //        string Url = HttpContext.Request.GetDisplayUrl();

        //        var getUrl = Url.Remove(Url.Length - (countLast + 1));

        //        //Consulta de Evaluacion por id de usuario

        //        var EvaluacionList = new List<string>();

        //        //Verifica los Evaluacion
        //        var list = Evaluacion.Split(',').ToList();

        //        foreach (var i in list)
        //        {
        //            var result = context.modulo.Where(r => r.Id.Equals(i)).Select(x => x.Nombre).FirstOrDefault();

        //            if (result != null)
        //            {
        //                EvaluacionList.Add(result.ToString());
        //            }
        //        }

        //        if (EvaluacionList == null)
        //        {
        //            return NotFound(new General()
        //            {
        //                title = "Eliminar Evaluacion",
        //                status = 404,
        //                message = "Evaluacion no encontrados"
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

        //            //Consulta de Evaluacion
        //            var existeRol = context.AspNetEvaluacion.Where(x => x.Id.Equals(eliminarRol.Id)).FirstOrDefault();

        //            if (existeRol == null)
        //            {
        //                return NotFound(new General()
        //                {
        //                    title = "Eliminar Evaluacion",
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
        //                title = "Eliminar Evaluacion",
        //                status = 200,
        //                message = "Rol eliminado"
        //            });
        //        }
        //        else
        //        {
        //            return BadRequest(new General()
        //            {
        //                title = "Eliminar Evaluacion",
        //                status = 400,
        //                message = "No tiene permisos para eliminar Evaluacion"
        //            });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //Registro de errores
        //        logger.LogError("Eliminar Evaluacion " + ex.Message.ToString() + " - " + ex.StackTrace);
        //        return BadRequest(new General()
        //        {
        //            title = "Eliminar Evaluacion",
        //            status = 400,
        //            message = "Contacte con el administrador del sistema"
        //        });
        //    }

        //}
        #endregion
    }
}