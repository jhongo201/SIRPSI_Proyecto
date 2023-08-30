using Twilio.Rest.Api.V2010.Account;
using static System.Net.Mime.MediaTypeNames;

namespace MessengerServices
{
    public interface IMessengerSender
    {
        Task<MessageResource> SendTextMessageNotification(MessengerRequest messengerRequest);
        Task<MessageResource> SendWhatsappNotification(MessengerRequest messengerRequest);
    }
}
