using System;

namespace Chat.Models
{
    public class User
    {
        //public string userId { get; set; }
        public string name { get; set; }
        public string signalRConnectionId { get; set; }
        public string oneSignalId { get; set; }
        public DateTime dtConnection { get; set; }
    }
}