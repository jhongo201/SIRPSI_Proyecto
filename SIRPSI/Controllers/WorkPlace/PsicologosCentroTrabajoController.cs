using AutoMapper;
using DataAccess.Context;
using DataAccess.Models.Estados;
using DataAccess.Models.PsychologistsCenterWork;
using DataAccess.Models.Status;
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
using SIRPSI.DTOs.Companies;
using SIRPSI.DTOs.Status;
using SIRPSI.DTOs.User;
using SIRPSI.DTOs.WorkPlace;
using SIRPSI.Helpers.Answers;
using System.Security.Claims;

namespace SIRPSI.Controllers.WorkPlace
{
    [Route("api/psicologosCentroTrabajo")]
    [ApiController]
    [EnableCors("CorsApi")]
    public class PsicologosCentroTrabajoController : ControllerBase
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
        public PsicologosCentroTrabajoController(AppDbContext context,
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
        //[HttpGet("ConsultarCentroDeTrabajo", Name = "consultarCentroDeTrabajo")]
        ////[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //public async Task<ActionResult<object>> Get(int? companie)
        //{
        //    try
        //    {
        //        //Consulta el estados
        //        var centrosDeTrabajo = (from data in (await context.centroTrabajo.ToListAsync())
        //                                select new CentroTrabajo
        //                                {
        //                                    Id = data.Id,
        //                                    Nombre = data.Nombre,
        //                                    Descripcion = data.Descripcion,
        //                                    IdEmpresa = data.IdEmpresa,
        //                                    FechaModifico = data.FechaModifico,
        //                                    UsuarioModifico = data.UsuarioModifico,
        //                                    UsuarioRegistro = data.UsuarioRegistro,
        //                                    FechaRegistro = data.FechaRegistro,
        //                                    IdEstado = data.IdEstado,
        //                                    IdUsuario = data.IdUsuario,
        //                                    Empresa = (context.empresas.Where(x => x.Id == data.IdEmpresa)).FirstOrDefault(),
        //                                    Estados = (context.estados.Where(x => x.Id == data.IdEstado)).FirstOrDefault(),
        //                                    Usuario = (context.AspNetUsers.Where(x => x.Id == data.IdUsuario)).FirstOrDefault(),
        //                                }).ToList();

        //        if (companie != 0 && companie != null)
        //            centrosDeTrabajo = centrosDeTrabajo.Where(x => x.IdEmpresa == context.empresas.Where(x => x.IdConsecutivo == companie).FirstOrDefault().Id).ToList();

        //        if (centrosDeTrabajo == null)
        //        {
        //            //Visualizacion de mensajes al usuario del aplicativo
        //            return NotFound(new General()
        //            {
        //                title = "Consultar centro de trabajo",
        //                status = 404,
        //                message = "Centros de trabajo no encontrados"
        //            });
        //        }
        //        //Retorno de los datos encontrados
        //        return centrosDeTrabajo;
        //    }
        //    catch (Exception ex)
        //    {
        //        //Registro de errores
        //        logger.LogError("Consultar centro de trabajo " + ex.Message.ToString() + " - " + ex.StackTrace);
        //        return BadRequest(new General()
        //        {
        //            title = "Consultar centro de trabajo",
        //            status = 400,
        //            message = "Contacte con el administrador del sistema"
        //        });
        //    }
        //}

        //[HttpGet("ConsultarCentroTrabajoUsuario", Name = "consultarCentroTrabajoUsuario")]
        ////[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //public async Task<ActionResult<object>> GetByUserWorkCenter(string? centroTrabajo, string? user)
        //{
        //    try
        //    {
        //        //Consulta el estados
        //        var centrosDeTrabajo = await context.userWorkPlace.Where(x => x.WorkPlaceId == centroTrabajo && x.UserId == user).FirstOrDefaultAsync();

        //        if (centrosDeTrabajo == null)
        //        {
        //            //Visualizacion de mensajes al usuario del aplicativo
        //            return NotFound(new General()
        //            {
        //                title = "Consultar centro de trabajo",
        //                status = 404,
        //                message = "Centros de trabajo no encontrados"
        //            });
        //        }
        //        //Retorno de los datos encontrados
        //        return centrosDeTrabajo;
        //    }
        //    catch (Exception ex)
        //    {
        //        //Registro de errores
        //        logger.LogError("Consultar centro de trabajo " + ex.Message.ToString() + " - " + ex.StackTrace);
        //        return BadRequest(new General()
        //        {
        //            title = "Consultar centro de trabajo",
        //            status = 400,
        //            message = "Contacte con el administrador del sistema"
        //        });
        //    }
        //}

        [HttpGet("ConsultarPsicologosCentroDeTrabajo")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<object>> GetByWorkCenter(string? workCenter)
        {
            var psyWorkCenter = (from data in await context.psicologosCentroTrabajo.ToListAsync()
                                 select new ConsultarUsuariosCentroTrabajo()
                                 {
                                     WorkplaceId = data.IdCentroTrabajo,
                                     UserId = data.IdUser,
                                     User = (context.AspNetUsers.Where(x => x.Id == data.IdUser).FirstOrDefault()),
                                     Workplace = (context.centroTrabajo.Where(x => x.Id == data.IdCentroTrabajo).FirstOrDefault()),
                                 });

            var usuariosConsultados = (from data in psyWorkCenter
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
                                           psicologosCentroTrabajo = (context.psicologosCentroTrabajo.Where(x => x.IdUser == data.User.Id).FirstOrDefault())
                                       }).ToList();


            return Ok(usuariosConsultados);
        }
        #endregion

        #region Registro
        [HttpPost("RegistrarCentroDeTrabajo")]
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

                var userWorkCenter = await context.psicologosCentroTrabajo.Where(x => x.IdUser == registrarCentroTrabajoUsuario.User && x.IdCentroTrabajo == registrarCentroTrabajoUsuario.Workplace).FirstOrDefaultAsync();
                if (userWorkCenter != null)
                {
                    //Visualizacion de mensajes al usuario del aplicativo
                    return NotFound(new General()
                    {
                        title = "Asignar centro de trabajo",
                        status = 404,
                        message = "Este psicologo ya esta asignado a ese centro de trabajo"
                    });
                }
                var centroTrabajoUser = new PsicologosCentroTrabajo();
                //Valores asignados
                centroTrabajoUser.Id = Guid.NewGuid().ToString();
                centroTrabajoUser.IdUser = registrarCentroTrabajoUsuario.User;
                centroTrabajoUser.IdCentroTrabajo = registrarCentroTrabajoUsuario.Workplace;

                //Agregar datos al contexto
                context.psicologosCentroTrabajo.Add(centroTrabajoUser);
                context.centroTrabajoHistorial.Add(new CentroTrabajoHistorial()
                {
                    Id = Guid.NewGuid().ToString(),
                    IdUser = centroTrabajoUser.IdUser,
                    IdCentroTrabajo = centroTrabajoUser.IdCentroTrabajo,
                    Fecha = DateTime.Now,
                    UserRegister = usuario.Id,
                    IdEstado = "fc6b0f2f-0641-40e5-b7ac-3eed551f5531",
                    Tipo = 1,
                });
                //Guardado de datos 
                await context.SaveChangesAsync();
                return Created("", new General()
                {
                    //Visualizacion de mensajes al usuario del aplicativo
                    title = "Asignar psicologo a centro de trabajo",
                    status = 201,
                    message = "Usuario(s) asignado(s)"
                });
            }
            catch (Exception e)
            {
                throw;
            }
        }
        #endregion

        #region Actualizar
        [HttpPut("RetirarReintegrarPsicologoCentroTrabajo")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> RemoveOrReInstate(RetirarReintegrarPsicologoCentroTrabajo retirarReintegrar)
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

                var centrosDeTrabajo = await context.psicologosCentroTrabajo.Where(x => x.IdUser == retirarReintegrar.User).FirstOrDefaultAsync();
                if (centrosDeTrabajo != null)
                {
                    return NotFound(new General()
                    {
                        title = "Retirar o reintegrar centro de trabajo",
                        status = 404,
                        message = "Antes de retirar al usuario: " + usuario.Names + ", Debe retirarlo del centro de trabajo asociado."
                    });
                }
                var user = context.AspNetUsers.Where(u => u.Id.Equals(retirarReintegrar.User)).FirstOrDefault();
                user.Status = retirarReintegrar.Status;
                context.AspNetUsers.Update(user);
                context.centroTrabajoHistorial.Add(new CentroTrabajoHistorial()
                {
                    Id = Guid.NewGuid().ToString(),
                    IdUser = retirarReintegrar.User,
                    IdCentroTrabajo = null,
                    Fecha = DateTime.Now,
                    UserRegister = usuario.Id,
                    IdEstado = retirarReintegrar.Status,
                    Tipo = retirarReintegrar.Tipo,
                });
                await context.SaveChangesAsync();

                return Ok(new General()
                {
                    title = "Retirar o reintegrar centro de trabajo",
                    status = 200,
                    message = "El usuario ya ha sido retirado o reintegrado"
                });
            }
            catch (Exception ex)
            {
                logger.LogError("Retirar o reintegrar centro de trabajo " + ex.Message.ToString() + " - " + ex.StackTrace);
                return BadRequest(new General()
                {
                    title = "Retirar o reintegrar centro de trabajo",
                    status = 400,
                    message = "Contacte con el administrador del sistema"
                });
            }
        }

        #endregion

        #region Eliminar
        [HttpDelete("DesvincularPsicologoCentroTrabajo")]
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

                var centrosDeTrabajo = await context.psicologosCentroTrabajo.Where(x => x.IdCentroTrabajo == centroTrabajo && x.IdUser == user).FirstOrDefaultAsync();
                if (centrosDeTrabajo == null)
                {
                    return NotFound(new General()
                    {
                        title = "Consultar centro de trabajo",
                        status = 404,
                        message = "Centros de trabajo no encontrados"
                    });
                }
                context.psicologosCentroTrabajo.Remove(centrosDeTrabajo);
                context.centroTrabajoHistorial.Add(new CentroTrabajoHistorial()
                {
                    Id = Guid.NewGuid().ToString(),
                    IdUser = centrosDeTrabajo.IdUser,
                    IdCentroTrabajo = centrosDeTrabajo.IdCentroTrabajo,
                    Fecha = DateTime.Now,
                    UserRegister = usuario.Id,
                    IdEstado = "62aea53c-ea76-4994-8d44-129936574bf5",
                    Tipo = 1,
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
