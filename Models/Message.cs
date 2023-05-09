using System;

namespace Chat.Models
{
    public class Message
    {
        public string receiverSignalRConnectionId { get; set; }
        public User sender { get; set; }
        public string message { get; set; }
    }
}