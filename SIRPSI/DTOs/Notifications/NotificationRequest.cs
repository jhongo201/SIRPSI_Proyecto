using System.ComponentModel.DataAnnotations;

namespace SIRPSI.DTOs.Notifications
{
    public class NotificationRequest
    {
        public string MessageCodeActivation { get; set; }
        public string MessageReceiver { get; set; }
    }
}
