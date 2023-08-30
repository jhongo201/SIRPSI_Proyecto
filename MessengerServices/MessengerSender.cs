using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace MessengerServices
{
    public class MessengerSender : IMessengerSender
    {
        private readonly MessengerConfiguration messengerConfiguration;

        public MessengerSender(MessengerConfiguration messengerConfiguration)
        {
            this.messengerConfiguration = messengerConfiguration;
        }

        public async Task<MessageResource> SendWhatsappNotification(MessengerRequest messengerRequest)
        {
            try
            {
                TwilioClient.Init(this.messengerConfiguration.AccountSid, this.messengerConfiguration.AuthToken);
                var messageOptions = new CreateMessageOptions(
                  new PhoneNumber($"whatsapp:{messengerRequest.MessageReceiver}"));
                messageOptions.From = new PhoneNumber($@"whatsapp:{this.messengerConfiguration.From}");
                messageOptions.Body = messengerRequest.MessageBody;

                return await MessageResource.CreateAsync(messageOptions);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<MessageResource> SendTextMessageNotification(MessengerRequest messengerRequest)
        {
            try
            {
                TwilioClient.Init(this.messengerConfiguration.AccountSid, this.messengerConfiguration.AuthToken);
                var messageOptions = new CreateMessageOptions(
                  new PhoneNumber(messengerRequest.MessageReceiver));
                messageOptions.From = new PhoneNumber(this.messengerConfiguration.From);
                messageOptions.Body = messengerRequest.MessageBody;

                return await MessageResource.CreateAsync(messageOptions);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
