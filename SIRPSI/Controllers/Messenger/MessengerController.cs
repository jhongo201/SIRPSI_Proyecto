using AutoMapper;
using DataAccess.Context;
using DataAccess.Models.Rols;
using DataAccess.Models.Status;
using EmailServices;
using EvertecApi.Log4net;
using MessengerServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIRPSI.Core.Helper;
using SIRPSI.DTOs.Companies;
using SIRPSI.DTOs.Document;
using SIRPSI.DTOs.Notifications;
using SIRPSI.Helpers.Answers;
using System.Security.Claims;

namespace SIRPSI.Controllers.Messenger
{
    [Route("api/mensajes")]
    [ApiController]
    [EnableCors("CorsApi")]
    public class MessengerController : ControllerBase
    {
        #region Dependencias
        private readonly IMessengerSender messengerSender;

        //Constructor 
        public MessengerController(IMessengerSender messengerSender)
        {
            this.messengerSender = messengerSender;
        }
        #endregion

        #region Notification
        [HttpPost("EnviarNotificaciónMensajeWhatsApp", Name = "enviarNotificaciónMensajeWhatsApp")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<object>> SendNotificationMessageWhatsApp(NotificationRequest request)
        {
            MessengerRequest requestNotification = new MessengerRequest();
            requestNotification.MessageReceiver = request.MessageReceiver;
            requestNotification.MessageBody = $"¡Bienvenido al Sistema de Información de Riesgo Psicosocial (SIPRSI)! El código de activación de cuenta es: " +
                $"*{request.MessageCodeActivation}*. Por favor, ingrese este código en el sitio web de SIPRSI para activar su cuenta de usuario."
                + "\r\n\r\nSi tiene dudas o inquietudes, por favor contacte al Psicólogo Especialista SST asignado por su empresa.";

            var data = await this.messengerSender.SendWhatsappNotification(requestNotification);
            return Ok(new NotificationMessage()
            {
                title = "Notificación enviada",
                status = 200,
                message = "Se ha enviado la notificación exitosamente!",
                data = data
            });
        }

        #endregion

    }
}
