using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NRelay.Server.Events
{
    public class ConnectedToServerEventArgs : EventArgs
    {
        public ConnectedToServerEventArgs() : base() { }
    }
    public delegate void ConnectedToServerHandler(object sender, ConnectedToServerEventArgs args);
    
}
