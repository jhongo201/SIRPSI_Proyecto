﻿using AutoMapper;
using DataAccess.Context;
using DataAccess.Models.Companies;
using DataAccess.Models.Documents;
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
using SIRPSI.Core.Helper;
using SIRPSI.DTOs.Companies;
using SIRPSI.DTOs.Document;
using SIRPSI.Helpers.Answers;
using System.Security.Claims;

namespace SIRPSI.Controllers.OccupationProfession
{
    [Route("api/ocupacionProfesion")]
    [ApiController]
    [EnableCors("CorsApi")]
    public class OcupacionProfesionController : ControllerBase
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
        public OcupacionProfesionController(AppDbContext context,
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
        [HttpGet("ConsultarOcupacionProfesion", Name = "consultarOcupacionProfesion")]
        public async Task<ActionResult<object>> Get()
        {
            try
            {
                var tipoEmpresa = context.ocupacionProfesion.ToList();
                if (tipoEmpresa == null)
                {
                    //Visualizacion de mensajes al usuario del aplicativo
                    return NotFound(new General()
                    {
                        title = "Consultar ocupación o presión",
                        status = 404,
                        message = "ocupación o presión no encontrada"
                    });
                }
                return tipoEmpresa;
            }
            catch (Exception ex)
            {
                //Registro de errores
                logger.LogError("Consultar ocupación o presión " + ex.Message.ToString() + " - " + ex.StackTrace);
                return BadRequest(new General()
                {
                    title = "Consultar ocupación o presión",
                    status = 400,
                    message = "Contacte con el administrador del sistema"
                });
            }
        }
        #endregion
    }
}