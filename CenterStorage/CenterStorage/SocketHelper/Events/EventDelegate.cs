using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketHelper.Events
{
    public delegate void ReceiveCompletedHandler(object sender, ReceiveEventArgs args);
    public delegate void SendCompletedHandler(object sender, SendEventArgs args);
    public delegate void ErrorOccuredHandler(object sender, ErrorEventArgs args);
    public delegate void ClientAcceptedHandler(object sender, ClientAcceptedEventArgs args);
}
