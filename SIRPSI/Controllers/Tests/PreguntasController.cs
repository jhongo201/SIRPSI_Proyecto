using AutoMapper;
using DataAccess.Context;
using DataAccess.Models.Documents;
using DataAccess.Models.Estados;
using DataAccess.Models.PsychosocialEvaluation;
using DataAccess.Models.Tests;
using EmailServices;
using EvertecApi.Log4net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIRPSI.Core.Helper;
using SIRPSI.DTOs.Document;
using SIRPSI.DTOs.Tests;
using SIRPSI.DTOs.User.Usuario;
using SIRPSI.Helpers.Answers;
using SIRPSI.Settings;

namespace SIRPSI.Controllers.Tests
{
    [Route("api/[controller]")]
    [ApiController]
    public class PreguntasController : Controller
    {
        #region Dependencias
        private readonly AppDbContext context;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly ILoggerManager logger;
        private readonly IMapper mapper;
        private readonly IEmailSender emailSender;
        private readonly StatusSettings statusSettings;

        //Constructor  
        public PreguntasController(AppDbContext context,
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
            this.signInManager = signInManager;
            this.logger = logger;
            this.mapper = mapper;
            this.emailSender = emailSender;
            this.statusSettings = statusSettings;
        }
        #endregion





        #region Consultar
        [HttpGet("ConsultarPreguntas", Name = "ConsultarPreguntas")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<object>> GetPreguntas()
        {
            try
            {
                var dimensionConsultada = (from data in (await context.preguntas.ToListAsync())
                                           join dimensiones in context.dimensiones on data.IdDimension equals dimensiones.Id
                                           join forma in context.forma on data.IdForma equals forma.Id
                                           //join dominios in context.dominios on dimensiones.IdDominio equals dominios.Id

                                           orderby data.Id ascending
                                           select new ConsultarPreguntasDto()
                                           {
                                               Id = data.Id,
                                               Pregunta = data.Pregunta,
                                               Posicion = data.Posicion,
                                               Siempre = data.Siempre,
                                               CasiSiempre = data.CasiSiempre,
                                               AlgunasVeces = data.AlgunasVeces,
                                               CasiNunca = data.CasiNunca,
                                               Nunca = data.Nunca,
                                               IdDimension = data.IdDimension,
                                               Dimension = dimensiones.Nombre,
                                               Forma = forma.Nombre,
                                               Dominio = dimensiones.IdDominio,
                                               //DominioId = dominios.Nombre,
                                           }).ToList();

                if (dimensionConsultada == null)
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
                return dimensionConsultada;
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
        [HttpPost("GuardarRespuestasPreguntas")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> PostSaveResponse([FromBody] DetalleEvaluacionPsicosocialDto detalle)
        {
            try
            {
                //Consulta de usuarios por documento
                var detalleEvluacion = context.evaluacionPsicosocialUsuario.Where(u => u.IdUsuario.Equals(detalle.IdUserEvaluacion) && u.IdEstado.Equals("cab25738-41fe-4989-a115-0ac36325dd6c")).FirstOrDefault();

                if (detalleEvluacion == null)
                {
                    return NotFound(new General()
                    {
                        title = "Evaluación no autorizada",
                        status = 404,
                        message = "Usuario no Autorizado para realizar la evaluación"
                    });
                }

                //Mapeo de datos en clases
                var detalleEvaluacion = mapper.Map<DetalleEvaluacionPsicosocial>(detalle);
                //Valores asignados
                detalleEvaluacion.Id = Guid.NewGuid().ToString();
                detalleEvaluacion.IdPreguntaEvaluacion = detalle.IdPreguntaEvaluacion;
                detalleEvaluacion.Respuesta = detalle.Respuesta;
                detalleEvaluacion.Puntuacion = detalle.Puntuacion;
                detalleEvaluacion.IdUserEvaluacion = detalle.IdUserEvaluacion;

                //Agregar datos al contexto
                context.Add(detalleEvaluacion);
                //Guardado de datos
                await context.SaveChangesAsync();

                return Created("", new General()
                {
                    //Visualizacion de mensajes al usuario del aplicativo
                    title = "Registrar respuesta",
                    status = 200,
                    message = "Respuesta registrada"
                });


            }
            catch (Exception ex)
            {
                //Registro de errores
                logger.LogError("Registrar tipo documento " + ex.Message.ToString() + " - " + ex.StackTrace);
                return BadRequest(new General()
                {
                    title = "Registrar tipo documento",
                    status = 400,
                    message = "Contacte con el administrador del sistema"
                });
            }
        }









        [HttpPost("RegitroEmpleados")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> PostSaveEmployee([FromBody] EmployeeDataDto employe)
        {
            try
            {
                //Consulta de usuarios por documento
                var empleado = context.empleado.Where(u => u.id_desempleado.Equals(employe.id_desempleado)).FirstOrDefault();

                if (empleado != null)
                {
                    return NotFound(new General()
                    {
                        title = "Empleado registrado",
                        status = 404,
                        message = "El empleado ya existe!"
                    });
                }

                //Mapeo de datos en clases
                var empleadoMap = mapper.Map<Employees>(employe);
                //Valores asignados
                empleadoMap.Id = Guid.NewGuid().ToString();
                empleadoMap.nombre_completo = employe.nombre_completo;
                empleadoMap.sexo = employe.sexo;
                empleadoMap.genero = employe.genero;
                empleadoMap.otro_genero = employe.otro_genero;
                empleadoMap.etnia = employe.etnia;
                empleadoMap.cual_indigena = employe.cual_indigena;
                empleadoMap.discapacidad = employe.discapacidad;
                empleadoMap.cual_discapacidad = employe.cual_discapacidad;
                empleadoMap.anio_nacimiento = employe.anio_nacimiento;
                empleadoMap.lugar_residencia = employe.lugar_residencia;
                empleadoMap.zona = employe.zona;
                empleadoMap.cual_rural = employe.cual_rural;
                empleadoMap.estado_civil = employe.estado_civil;
                empleadoMap.nivel_educativo = employe.nivel_educativo;
                empleadoMap.ocupacion = employe.ocupacion;
                empleadoMap.lugar_reidencia = employe.lugar_reidencia;
                empleadoMap.estrado = employe.estrado;
                empleadoMap.tipo_vivienda = employe.tipo_vivienda;
                empleadoMap.dependientes = employe.dependientes;
                empleadoMap.lugar_trabajo = employe.lugar_trabajo;
                empleadoMap.tiempo_laborado = employe.tiempo_laborado;
                empleadoMap.cargo_empresa = employe.cargo_empresa;
                empleadoMap.seleccion_cargo = employe.seleccion_cargo;
                empleadoMap.tiempoLavorado_Cargo = employe.tiempoLavorado_Cargo;
                empleadoMap.departamentoTrabajo = employe.departamentoTrabajo;
                empleadoMap.tipoContrato = employe.tipoContrato;
                empleadoMap.horasTrabajadasDiarias = employe.horasTrabajadasDiarias;
                empleadoMap.tipoSalario = employe.tipoSalario;
                empleadoMap.id_desempleado = employe.id_desempleado;

                //Agregar datos al contexto
                context.Add(empleadoMap);
                //Guardado de datos
                await context.SaveChangesAsync();

                return Created("", new General()
                {
                    //Visualizacion de mensajes al usuario del aplicativo
                    title = "Registro exitoso",
                    status = 200,
                    message = "Empleado registrado"
                });


        }
            catch (Exception ex)
            {
                //Registro de errores
                logger.LogError("Registrar tipo documento " + ex.Message.ToString() + " - " + ex.StackTrace);
                return BadRequest(new General()
        {
            title = "Registrar tipo documento",
                    status = 400,
                    message = "Contacte con el administrador del sistema"
                });
            }
}
        #endregion


        #region Actualizar
        [HttpPut("ActualizarRespuestasPreguntas")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> PutSaveResponse(string id_pregunta, int responseData)
        {
            try
            {
                //Consulta de usuarios por documento
                var detalleEvluacion = context.detalleEvaluacionPsicosocial.Where(u => u.IdPreguntaEvaluacion.Equals(id_pregunta)).FirstOrDefault();

                if (detalleEvluacion == null)
                {
                    return NotFound(new General()
                    {
                        title = "Actualización no autorizada",
                        status = 404,
                        message = "La pregunta no existe"
                    });
                }

                //Mapeo de datos en clases
                detalleEvluacion.Respuesta = responseData;
                await context.SaveChangesAsync();


                return Created("", new General()
                {
                    //Visualizacion de mensajes al usuario del aplicativo
                    title = "Registrar respuesta",
                    status = 200,
                    message = "Respuesta registrada"
                });


            }
            catch (Exception ex)
            {
                //Registro de errores
                logger.LogError("Registrar tipo documento " + ex.Message.ToString() + " - " + ex.StackTrace);
                return BadRequest(new General()
                {
                    title = "Registrar tipo documento",
                    status = 400,
                    message = "Contacte con el administrador del sistema"
                });
            }
        }
        #endregion

    }
}
