using System;
using System.Collections.Generic;

namespace ndaw.Core.Routing
{
    public interface ISignalSink: INamed
    {
        ISignalNode Owner { get; }

        event EventHandler<RoutingEventArgs> ReceivedData;

        void ReceiveDataFromSource(RoutingEventArgs e);

        bool IsMapped { get; }
        ISignalSource Source { get; set; }
    }
}
