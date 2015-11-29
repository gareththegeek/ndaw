using NAudio.Wave;
using ndaw.Routing;
using System;
using System.Linq;
using System.Collections.Generic;
using NAudio.Utils;
using System.Runtime.InteropServices;

namespace ndaw.Soundcard.Wave
{
    public class WaveInputMapper
    {
        private class WaveInDeviceData
        {
            public string Name;
            public WaveIn Driver;
            public int Channels;
            public float[][] Buffers;
        }

        public IEnumerable<ISignalSource> Inputs { get; private set; }

        public WaveFormat Format { get; private set; }
        private WaveFormat formatPerLine;

        private WaveInDeviceData device;
        private WaveIn driver;

        //TODO wrap WaveIn to allow DI
        public void Initialise(WaveFormat format, WaveIn driver)
        {
            if (driver == null)
            {
                throw new ArgumentNullException("driver", "Must specify a WaveIn device instance");
            }

            if (format == null)
            {
                throw new ArgumentNullException("format", "Must specify an audio format");
            }

            this.driver = driver;
            
            driver.DataAvailable += device_DataAvailable;

            var caps = WaveIn.GetCapabilities(driver.DeviceNumber);

            driver.WaveFormat = format;
            device = new WaveInDeviceData
            {
                Driver = driver,
                Name = caps.ProductName,
                Channels = caps.Channels,
                Buffers = new float[caps.Channels][]
            };

            Format = WaveFormat.CreateIeeeFloatWaveFormat(format.SampleRate, device.Channels);
            formatPerLine = WaveFormat.CreateIeeeFloatWaveFormat(format.SampleRate, 1);

            mapInputs(device.Channels);
        }
        
        private void mapInputs(int channelCount)
        {
            var inputs = new List<ISignalSource>();

            for (int i = 0; i < device.Channels; i++)
            {
                inputs.Add(new SignalSource());
            }

            this.Inputs = inputs;
        }

        void device_DataAvailable(object sender, WaveInEventArgs e)
        {
            var waveIn = sender as WaveIn;
            if (waveIn == null)
            {
                throw new InvalidOperationException("Sender must be a WaveIn instance");
            }
            
            //TODO support 16 bit
            //var is16Bit = device.Driver.WaveFormat.BitsPerSample == 16;
            //var size = is16Bit ? sizeof(short) : sizeof(int);
            var size = sizeof(float);
            var sampleCount = e.BytesRecorded / device.Channels / size;
            for (int i = 0; i < device.Channels; i++)
            {
                device.Buffers[i] = BufferHelpers.Ensure(device.Buffers[i], sampleCount);
            }

            copySamplesToManagedMemory(device.Buffers, e.Buffer, sampleCount, device.Channels);

            for (int i = 0; i < device.Channels; i++)
            {
                var input = Inputs.ElementAt(i);

                if (input.IsMapped)
                {
                    (input as SignalSource).RaiseBufferReady(
                        new RoutingEventArgs
                        {
                            Index = i,
                            Format = formatPerLine,
                            Count = sampleCount,
                            Buffer = device.Buffers[i]
                        });
                }
            }
        }

        private void copySamplesToManagedMemory(
            float[][] floatBuffers,
            byte[] byteBuffer,
            int sampleCount,
            int channels)
        {
            var floatBufferHandles = new GCHandle[channels];
            var floatBufferPointers = new IntPtr[channels];

            for (int i = 0; i < channels; i++)
            {
                floatBufferHandles[i] = GCHandle.Alloc(floatBuffers[i], GCHandleType.Pinned);
                floatBufferPointers[i] = floatBufferHandles[i].AddrOfPinnedObject();
            }

            try
            {
                unsafe
                {
                    fixed (byte* fixedByteBuffer = byteBuffer)
                    {
                        float* pInBuffer = (float*)fixedByteBuffer;

                        for (int i = 0; i < sampleCount; i++)
                        {
                            for (int j = 0; j < channels; j++)
                            {
                                float* pFloatBuffer = (float*)floatBufferPointers[j] + i;

                                *pFloatBuffer = *(pInBuffer++);
                            }
                        }

                        //if (is16Bit)
                        //{
                        //    short* pShortBuffer = (short*)fixedByteBuffer;

                        //    for (int i = 0; i < sampleCount; i++)
                        //    {
                        //        for (int j = 0; j < channels; j++)
                        //        {
                        //            float* pFloatBuffer = (float*)floatBufferPointers[j] + i;

                        //            *pFloatBuffer = *(pShortBuffer++) / (float)Int16.MaxValue;
                        //        }
                        //    }
                        //}
                        //else
                        //{
                        //    int* pIntBuffer = (int*)fixedByteBuffer;

                        //    for (int i = 0; i < sampleCount; i++)
                        //    {
                        //        for (int j = 0; j < channels; j++)
                        //        {
                        //            float* pFloatBuffer = (float*)floatBufferPointers[j] + i;

                        //            *pFloatBuffer = *(pIntBuffer++) / (float)Int32.MaxValue;
                        //        }
                        //    }
                        //}
                    }
                }
            }
            finally
            {
                for (int i = 0; i < floatBufferHandles.Length; i++)
                {
                    if (floatBufferHandles[i].IsAllocated)
                    {
                        floatBufferHandles[i].Free();
                    }
                }
            }
        }
    }
}
