using NAudio.Utils;
using ndaw.Core.Routing;
using System;
using System.Collections.Generic;

namespace ndaw.Core.Routing
{
    public class SignalSource : ISignalSource
    {
        public string Name { get; set; }

        public ISignalNode Owner { get; private set; }

        private List<ISignalSink> sinks = new List<ISignalSink>();
        private float[][] sinkBuffers;

        public bool IsMapped
        {
            get { return sinks.Count > 0; }
        }

        public SignalSource(ISignalNode owner)
        {
            if (owner == null)
            {
                throw new ArgumentNullException("owner", "SignalSource must have an owner");
            }

            this.Owner = owner;
        }

        public void AddSink(ISignalSink sink)
        {
            sinks.Add(sink);
            sinkBuffers = new float[sinks.Count][];
        }

        public void RemoveSink(ISignalSink sink)
        {
            sinks.Remove(sink);
            sinkBuffers = new float[sinks.Count][];
        }

        public void RaiseBufferReady(RoutingEventArgs e)
        {
            if (sinks.Count == 0) return;

            for (int i = 0; i < sinks.Count; i++)
            {
                float[] buffer;
                if (i == 0)
                {
                    // The first (or only) receiver can just reuse the buffer
                    // so there is no need to clone it
                    buffer = e.Buffer;
                }
                else
                {
                    sinkBuffers[i] = BufferHelpers.Ensure(sinkBuffers[i], e.Count);
                    Buffer.BlockCopy(e.Buffer, 0, sinkBuffers[i], 0, e.Count * sizeof(float));

                    buffer = sinkBuffers[i];
                }

                sinks[i].ReceiveDataFromSource(
                    new RoutingEventArgs
                    {
                        Buffer = buffer,
                        Count = e.Count,
                        Format = e.Format,
                        Index = i
                    });
            }
        }
    }
}
