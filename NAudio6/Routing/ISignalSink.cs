using System;
using System.Collections.Generic;

namespace ndaw.Routing
{
    public interface ISignalSink
    {
        event EventHandler<RoutingEventArgs> ReceivedData;

        void ReceiveDataFromSource(RoutingEventArgs e);

        bool IsMapped { get; }
        ISignalSource Source { get; set; }
    }
}
