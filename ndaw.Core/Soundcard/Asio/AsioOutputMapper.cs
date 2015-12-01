using NAudio.Utils;
using NAudio.Wave;
using ndaw.Core.Routing;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ndaw.Core.Soundcard.Asio
{
    public class AsioOutputMapper
    {
        // TODO There is an overlap in logic between this and the WaveOutputMapper

        public BufferedWaveProvider OutputBuffer { get; private set; }

        public WaveFormat Format { get; private set; }

        public IEnumerable<ISignalSink> Outputs { get; private set; }

        private byte[] rawBuffer;
        private float[][] receivedData;

        public void Initialise(WaveFormat format, AsioOut driver)
        {
            if (driver == null)
            {
                throw new ArgumentNullException("driver", "Must specify an asio interface");
            }

            if (format == null)
            {
                throw new ArgumentNullException("format", "Must specify an audio format");
            }

            Format = WaveFormat.CreateIeeeFloatWaveFormat(format.SampleRate, driver.DriverOutputChannelCount);

            OutputBuffer = new BufferedWaveProvider(Format);

            mapOutputs(driver);
        }

        private void mapOutputs(AsioOut driver)
        {
            var channelCount = driver.DriverOutputChannelCount;

            receivedData = new float[channelCount][];

            var outputs = new List<ISignalSink>();
            for (int i = 0; i < channelCount; i++)
            {
                var output = new SignalSink(i);
                output.ReceivedData += output_ReceivedData;
                output.Name = driver.AsioOutputChannelName(i);

                receivedData[i] = null;

                outputs.Add(output);
            }
            Outputs = outputs;
        }

        private void output_ReceivedData(object sender, RoutingEventArgs e)
        {
            if (e.Index < 0 || e.Index > receivedData.Length)
            {
                throw new ArgumentOutOfRangeException("e", "Output index exceeds number of outputs on the Asio device");
            }

            receivedData[e.Index] = e.Buffer;

            if (isAllDataReceived())
            {
                processFrame();
            }
        }

        private bool isAllDataReceived()
        {
            for (int i = 0; i < receivedData.Length; i++)
            {
                if (receivedData[i] == null && Outputs.ElementAt(i).IsMapped)
                {
                    return false;
                }
            }

            return true;
        }

        private void processFrame()
        {
            var count = receivedData.First(d => d != null).Length;

            if (receivedData.Any(d => d != null && d.Length != count))
            {
                throw new NotSupportedException("Processing buffers of varying lengths is not currently supported");
            }

            interleaveChannelsToByteArray(count);

            OutputBuffer.AddSamples(rawBuffer, 0, rawBuffer.Length);

            for (int i = 0; i < receivedData.Length; i++)
            {
                receivedData[i] = null;
            }
        }

        private void interleaveChannelsToByteArray(int count)
        {
            rawBuffer = BufferHelpers.Ensure(rawBuffer, Format.Channels * count * sizeof(float));

            var channels = Format.Channels;

            GCHandle[] receivedDataHandles = new GCHandle[channels];
            IntPtr[] receivedDataPointers = new IntPtr[channels];
            for (int i = 0; i < channels; i++)
            {
                if (receivedData[i] != null)
                {
                    receivedDataHandles[i] = GCHandle.Alloc(receivedData[i], GCHandleType.Pinned);
                    receivedDataPointers[i] = receivedDataHandles[i].AddrOfPinnedObject();
                }
                else
                {
                    receivedDataPointers[i] = IntPtr.Zero;
                }
            }

            try
            {
                unsafe
                {
                    fixed (byte* fixedTargetBuffer = rawBuffer)
                    {
                        byte* pTargetBuffer = fixedTargetBuffer;

                        for (int i = 0; i < count; i++)
                        {
                            for (int j = 0; j < channels; j++)
                            {
                                byte* pReceivedData = (byte*)receivedDataPointers[j];

                                if (pReceivedData != null)
                                {
                                    var i4 = i * 4;
                                    *(pTargetBuffer++) = *(pReceivedData + i4 + 0);
                                    *(pTargetBuffer++) = *(pReceivedData + i4 + 1);
                                    *(pTargetBuffer++) = *(pReceivedData + i4 + 2);
                                    *(pTargetBuffer++) = *(pReceivedData + i4 + 3);
                                }
                                else
                                {
                                    *(pTargetBuffer++) = 0;
                                    *(pTargetBuffer++) = 0;
                                    *(pTargetBuffer++) = 0;
                                    *(pTargetBuffer++) = 0;
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                for (int i = 0; i < receivedDataHandles.Length; i++)
                {
                    if (receivedDataHandles[i].IsAllocated)
                    {
                        receivedDataHandles[i].Free();
                    }
                }
            }
        }
    }
}
