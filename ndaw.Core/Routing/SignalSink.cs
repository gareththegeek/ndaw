using NAudio.Utils;
using ndaw.Core.Routing;
using System;

namespace ndaw.Core.Routing
{
    public class SignalSink: ISignalSink
    {
        public string Name { get; set; }

        private int index;

        public SignalSink() { }

        public SignalSink(int index)
        {
            this.index = index;
        }

        public bool IsMapped { get { return source != null; } }

        private ISignalSource source;
        public ISignalSource Source 
        {
            get { return source; }
            set
            {
                if (source != null)
                {
                    source.RemoveSink(this);
                }

                source = value;

                if (source != null)
                {
                    source.AddSink(this);
                }
            }
        }

        public event EventHandler<RoutingEventArgs> ReceivedData;

        public void ReceiveDataFromSource(RoutingEventArgs e)
        {
            if (ReceivedData != null)
            {
                ReceivedData.Invoke(this, new RoutingEventArgs
                {
                    Buffer = e.Buffer,
                    Count = e.Count,
                    Format = e.Format,
                    Index = index
                });
            }
        }
    }
}
