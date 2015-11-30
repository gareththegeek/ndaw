using NAudio.Wave;
using ndaw.Core.Routing;
using System;
using System.Linq;
using System.Collections.Generic;
using NAudio.Utils;
using System.Runtime.InteropServices;

namespace ndaw.Core.Soundcard.Wave
{
    public class WaveOutputMapper
    {
        private class WaveOutDeviceData
        {
            public string Name;
            public WaveOut Driver;
            public int Channels;
            public float[][] Buffers;
        }

        public BufferedWaveProvider OutputBuffer { get; private set; }

        public WaveFormat Format { get; private set; }

        public IEnumerable<ISignalSink> Outputs { get; private set; }

        private WaveOutDeviceData device;

        private byte[] rawBuffer;

        public void Initialise(WaveFormat format, WaveOut driver)
        {
            if (driver == null)
            {
                throw new ArgumentNullException("driver", "Must specify a WaveIn device instance");
            }

            if (format == null)
            {
                throw new ArgumentNullException("format", "Must specify an audio format");
            }

            var caps = WaveOut.GetCapabilities(driver.DeviceNumber);
            
            device = new WaveOutDeviceData
            {
                Driver = driver,
                Name = caps.ProductName,
                Channels = caps.Channels,
                Buffers = new float[caps.Channels][]
            };

            Format = WaveFormat.CreateIeeeFloatWaveFormat(format.SampleRate, caps.Channels);
            OutputBuffer = new BufferedWaveProvider(Format);
            OutputBuffer.DiscardOnBufferOverflow = true;

            driver.Init(OutputBuffer);

            mapOutputs();
        }

        private void mapOutputs()
        {
            var outputs = new List<ISignalSink>();
            for (int i = 0; i < device.Channels; i++)
            {
                var output = new SignalSink(i);
                output.ReceivedData += output_ReceivedData;

                device.Buffers[i] = null;

                outputs.Add(output);
            }
            Outputs = outputs;
        }

        private void output_ReceivedData(object sender, RoutingEventArgs e)
        {
            if (e.Index < 0 || e.Index > device.Buffers.Length)
            {
                throw new ArgumentOutOfRangeException("e", "Output index exceeds number of outputs on the Asio device");
            }

            device.Buffers[e.Index] = e.Buffer;

            if (isAllDataReceived())
            {
                processFrame();
            }
        }

        private bool isAllDataReceived()
        {
            for (int i = 0; i < device.Buffers.Length; i++)
            {
                if (device.Buffers[i] == null && Outputs.ElementAt(i).IsMapped)
                {
                    return false;
                }
            }

            return true;
        }

        private void processFrame()
        {
            var count = device.Buffers.First(d => d != null).Length;

            if (device.Buffers.Any(d => d != null && d.Length != count))
            {
                throw new NotSupportedException("Processing buffers of varying lengths is not currently supported");
            }

            interleaveChannelsToByteArray(count);

            OutputBuffer.AddSamples(rawBuffer, 0, rawBuffer.Length);

            for (int i = 0; i < device.Buffers.Length; i++)
            {
                device.Buffers[i] = null;
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
                if (device.Buffers[i] != null)
                {
                    receivedDataHandles[i] = GCHandle.Alloc(device.Buffers[i], GCHandleType.Pinned);
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
