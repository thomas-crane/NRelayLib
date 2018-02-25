using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NRelay.Server.Events
{
    public class MessageEventArgs : EventArgs
    {
        private string payload;
        private DateTime timestamp;
        public string Payload
        {
            get => payload;
        }
        public DateTime Timestamp
        {
            get => timestamp;
        }
        public MessageEventArgs(string payload) : base()
        {
            this.payload = payload;
            timestamp = DateTime.UtcNow;
        }
    }
    public delegate void MessageHandler(object sender, MessageEventArgs args);
}
