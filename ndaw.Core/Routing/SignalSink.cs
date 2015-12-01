using NAudio.Utils;
using ndaw.Core.Routing;
using System;

namespace ndaw.Core.Routing
{
    public class SignalSink: ISignalSink
    {
        public string Name { get; set; }

        public ISignalNode Owner { get; private set; }

        private int index;

        public SignalSink(ISignalNode owner, int index = 0)
        {
            if (owner == null)
            {
                throw new ArgumentNullException("owner", "SignalSink must have an owner");
            }

            this.index = index;
            this.Owner = owner;
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
