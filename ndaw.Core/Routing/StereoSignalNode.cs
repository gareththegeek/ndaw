﻿using NAudio.Wave;
using System;
using System.Collections.Generic;

namespace ndaw.Core.Routing
{
    public class StereoSignalNode: ISignalProcessNode
    {
        public string Name { get; set; }

        public bool Bypass { get; set; }
        public ISignalProcess SignalProcess { get; private set; }

        public ISignalSink LeftIn { get; private set; }
        public ISignalSink RightIn { get; private set; }

        public ISignalSource LeftOut { get; private set; }
        public ISignalSource RightOut { get; private set; }

        IEnumerable<ISignalSource> ISignalNode.Sources { get { return new[] { LeftOut, RightOut }; } }
        IEnumerable<ISignalSink> ISignalNode.Sinks { get { return new[] { LeftIn, RightIn }; } }

        private float[] leftData;
        private float[] rightData;

        private WaveFormat format;

        public StereoSignalNode(WaveFormat format, ISignalProcess process)
        {
            if (process == null)
            {
                throw new ArgumentNullException("process", "Process cannot be null");
            }

            if (format == null)
            {
                throw new ArgumentNullException("format", "Must specify audio format");
            }

            this.format = WaveFormat.CreateIeeeFloatWaveFormat(format.SampleRate, 2);

            SignalProcess = process;
            SignalProcess.Format = this.format;

            Name = string.Format("{0} (Stereo)", SignalProcess.Name);

            LeftIn = new SignalSink(this);
            RightIn = new SignalSink(this);

            LeftIn.ReceivedData += LeftIn_ReceivedData;
            RightIn.ReceivedData += RightIn_ReceivedData;

            LeftOut = new SignalSource(this);
            RightOut = new SignalSource(this);
        }
        
        private void LeftIn_ReceivedData(object sender, RoutingEventArgs e)
        {
            leftData = e.Buffer;

            if (rightData != null || !RightIn.IsMapped)
            {
                processFrame(e.Count, e.Format);
            }
        }

        private void RightIn_ReceivedData(object sender, RoutingEventArgs e)
        {
            rightData = e.Buffer;

            if (leftData != null || !LeftIn.IsMapped)
            {
                processFrame(e.Count, e.Format);
            }
        }

        private void processFrame(int count, WaveFormat format)
        {
            if (!Bypass)
            {
                SignalProcess.Process(new float[][] { leftData, rightData }, count);
            }

            if (LeftOut != null)
            {
                LeftOut.RaiseBufferReady(new RoutingEventArgs
                {
                    Count = count,
                    Buffer = leftData,
                    Format = format,
                    Index = 0
                });
            }

            if (RightOut != null)
            {
                RightOut.RaiseBufferReady(new RoutingEventArgs
                {
                    Count = count,
                    Buffer = rightData,
                    Format = format,
                    Index = 1
                });
            }

            leftData = null;
            rightData = null;
        }
    }
}
