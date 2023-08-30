using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerServices
{
    public class MessengerRequest
    {
        public string? MessageBody { get; set; }
        public string MessageReceiver { get; set; }
    }
    public class MessengerResponse
    {
        //public List<MailboxAddress> To { get; set; }
        //public string Subject { get; set; }
        //public string Content { get; set; }
    }
}
//{
//  "body": "Your appointment is coming up on July 21 at 3PM",
//  "num_segments": "1",
//  "direction": "outbound-api",
//  "from": "whatsapp:+14155238886",
//  "date_updated": "Sat, 29 Jul 2023 05:22:54 +0000",
//  "price": null,
//  "error_message": null,
//  "uri": "/2010-04-01/Accounts/AC0b1bc6555f14eaa33501dac543da620c/Messages/SM2693bc0e31c3201bbb5222ae35f28b02.json",
//  "account_sid": "AC0b1bc6555f14eaa33501dac543da620c",
//  "num_media": "0",
//  "to": "whatsapp:+573116234821",
//  "date_created": "Sat, 29 Jul 2023 05:22:54 +0000",
//  "status": "queued",
//  "sid": "SM2693bc0e31c3201bbb5222ae35f28b02",
//  "date_sent": null,
//  "messaging_service_sid": null,
//  "error_code": null,
//  "price_unit": null,
//  "api_version": "2010-04-01",
//  "subresource_uris": {
//    "media": "/2010-04-01/Accounts/AC0b1bc6555f14eaa33501dac543da620c/Messages/SM2693bc0e31c3201bbb5222ae35f28b02/Media.json"
//  }
//}