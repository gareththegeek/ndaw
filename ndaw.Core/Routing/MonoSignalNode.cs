using NAudio.Wave;
using System;
using System.Collections.Generic;

namespace ndaw.Core.Routing
{
    public class MonoSignalNode: ISignalProcessNode
    {
        public string Name { get; set; }

        public bool Bypass { get; set; }
        public ISignalProcess SignalProcess { get; private set; }

        public ISignalSink CentreIn { get; private set; }
        public ISignalSource CentreOut { get; private set; }

        IEnumerable<ISignalSource> ISignalNode.Sources { get { return new[] { CentreOut }; } }
        IEnumerable<ISignalSink> ISignalNode.Sinks { get { return new[] { CentreIn }; } }

        private WaveFormat format;

        private void input_ReceivedData(object sender, RoutingEventArgs e)
        {
            if (!Bypass)
            {
                SignalProcess.Process(new float[][] { e.Buffer }, e.Count);
            }

            if (CentreOut != null)
            {
                CentreOut.RaiseBufferReady(new RoutingEventArgs
                {
                    Buffer = e.Buffer,
                    Count = e.Count,
                    Format = format,
                    Index = 0
                });
            }
        }

        public MonoSignalNode(WaveFormat format, ISignalProcess process)
        {
            if (process == null)
            {
                throw new ArgumentNullException("process", "Process cannot be null");
            }

            if (format == null)
            {
                throw new ArgumentNullException("format", "Must specify audio format");
            }

            this.format = WaveFormat.CreateIeeeFloatWaveFormat(format.SampleRate, 1);

            SignalProcess = process;
            SignalProcess.Format = this.format;

            Name = string.Format("{0} (Mono)", SignalProcess.Name);

            CentreIn = new SignalSink();
            CentreIn.ReceivedData += input_ReceivedData;

            CentreOut = new SignalSource();
        }
    }
}
