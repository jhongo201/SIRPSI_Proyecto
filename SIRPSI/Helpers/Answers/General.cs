using Twilio.Rest.Api.V2010.Account;

namespace SIRPSI.Helpers.Answers
{
    //Clase de visualización de respuestas al cliente.
    public class General
    {
        public string? title { get; set; }
        public int status { get; set; }
        public string? message { get; set; }
        public string? otherdata { get; set; } = null!;
    }
    public class NotificationMessage : General
    {
        public MessageResource? data { get; set; }
    }
}
