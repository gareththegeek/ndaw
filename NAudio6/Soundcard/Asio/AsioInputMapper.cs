using NAudio.Utils;
using NAudio.Wave;
using NAudio.Wave.Asio;
using ndaw.Routing;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ndaw.Soundcard.Asio
{
    public class AsioInputMapper
    {
        public IEnumerable<ISignalSource> Inputs { get; private set; }

        public WaveFormat Format { get; private set; }
        private WaveFormat formatPerLine;

        private float[][] floatBuffer;
        
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

            driver.AudioAvailable += asioDriver_AudioAvailable;

            Format = WaveFormat.CreateIeeeFloatWaveFormat(format.SampleRate, driver.DriverInputChannelCount);
            formatPerLine = WaveFormat.CreateIeeeFloatWaveFormat(format.SampleRate, 1);

            mapInputs(driver);
        }

        private void mapInputs(AsioOut driver)
        {
            var inputCount = driver.DriverInputChannelCount;

            var inputs = new List<ISignalSource>();
            floatBuffer = new float[inputCount][];
            
            for (int i = 0; i < inputCount; i++)
            {
                inputs.Add(new SignalSource());
            }
            this.Inputs = inputs;
        }

        private void asioDriver_AudioAvailable(object sender, AsioAudioAvailableEventArgs e)
        {
            var index = 0;
            
            foreach (var input in Inputs)
            {
                if (input.IsMapped)
                {
                    floatBuffer[index] = BufferHelpers.Ensure(floatBuffer[index], e.SamplesPerBuffer);

                    copySamplesToManagedMemory(index, e);

                    (input as SignalSource).RaiseBufferReady(
                        new RoutingEventArgs
                        {
                            Index = index,
                            Format = formatPerLine,
                            Count = e.SamplesPerBuffer,
                            Buffer = floatBuffer[index]
                        });
                }
            }
        }

        private unsafe void copySamplesToManagedMemory(int index, AsioAudioAvailableEventArgs e)
        {
            fixed (float* fixedFloatBuffer = floatBuffer[index])
            {
                float* pFloatBuffer = fixedFloatBuffer;

                switch (e.AsioSampleType)
                {
                    case AsioSampleType.Int32LSB:
                        int* pInt = (int*)e.InputBuffers[index++];
                        for (int i = 0; i < e.SamplesPerBuffer; i++)
                        {
                            *(pFloatBuffer++) = *(pInt++) / (float)Int32.MaxValue;
                        }
                        break;
                    case AsioSampleType.Int16LSB:
                        short* pShort = (short*)e.InputBuffers[index++];
                        for (int i = 0; i < e.SamplesPerBuffer; i++)
                        {
                            *(pFloatBuffer++) = *(pShort++) / (float)Int16.MaxValue;
                        }
                        break;
                    case AsioSampleType.Int24LSB:
                        byte* pByte = (byte*)e.InputBuffers[index++];
                        for (int i = 0; i < e.SamplesPerBuffer; i++)
                        {
                            int sample = pByte[0] | (pByte[1] << 8) | ((sbyte)pByte[2] << 16);
                            *(pFloatBuffer++) = sample / 8388608f;

                            pByte += 3;
                        }
                        break;
                    case AsioSampleType.Float32LSB:
                        float* pFloat = (float*)e.InputBuffers[index++];
                        for (int i = 0; i < e.SamplesPerBuffer; i++)
                        {
                            *(pFloatBuffer++) = *(pFloat++);
                        }
                        break;
                    default:
                        throw new NotSupportedException("Unsupported audio format detected");
                }
            }
        }
    }
}
