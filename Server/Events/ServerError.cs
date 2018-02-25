using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NRelay.Server.Events
{
    public class ServerErrorEventArgs : EventArgs
    {
        private Exception error;
        public Exception Error
        {
            get => error;
        }
        public ServerErrorEventArgs(Exception error) : base ()
        {
            this.error = error;
        }
    }
    public delegate void ServerErrorHandler(object sender, ServerErrorEventArgs args);
}
