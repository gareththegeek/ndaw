using NAudio.Utils;
using NAudio.Wave;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ndaw.Core.Filters.Implementations
{
    public class FirFilter: IFilterImplementation
    {
        //TODO unroll loops for fixed filter lengths (orders)?
        // e.g. one read implementation for 10 coefficients or fewer, one for 20 or fewer etc.

        // TODO implement coefficient calculator
        // Kaiser

        public string Name { get { return "FIR Filter"; } [ExcludeFromCodeCoverage]set { } }

        private class ChannelData
        {
            public int Position;
            public float[] InputHistory;
        }

        private float[] coefficients;
        public float[] Coefficients 
        {
            get { return coefficients; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Coefficients cannot be null");
                }

                coefficients = value;
            }
        }

        private WaveFormat format;
        public WaveFormat Format
        {
            get { return format; }
            set
            {
                format = value;

                if (format != null)
                {
                    channels = new ChannelData[format.Channels];
                    for (int i = 0; i < format.Channels; i++)
                    {
                        channels[i] = new ChannelData();
                        channels[i].InputHistory = new float[0];
                    }
                }
            }
        }

        private ChannelData[] channels;

        public void Process(float[][] buffers, int count)
        {
            if (buffers == null || buffers.Any(b => b == null))
            {
                throw new ArgumentNullException("buffers", "Buffer cannot be null");
            }

            if (format == null)
            {
                throw new InvalidOperationException("Format cannot be null");
            }

            if (format.Channels != buffers.Length)
            {
                throw new ArgumentOutOfRangeException("buffers", "There must be one buffer per channel");
            }

            if (buffers.Any(b => b.Length < count || count < 0))
            {
                throw new ArgumentOutOfRangeException("count", "Count must be equal to or less than buffer length");
            }

            if (this.coefficients == null)
            {
                throw new InvalidOperationException("Fir filter requires coefficients to be defined in order to operate");
            }

            var inputHistoryLength = Math.Max(count, coefficients.Length);
            
            for (int i = 0; i < format.Channels; i++)
            {
                var channel = channels[i];
                var buffer = buffers[i];

                if (inputHistoryLength > channel.InputHistory.Length)
                {
                    channel.InputHistory = BufferHelpers.Ensure(channel.InputHistory, inputHistoryLength);
                }
                
                // Unsafe equivalent was approximately 3 times faster in testing
                processUnsafe(channel, buffer, count);
                //processSafe(channel, buffer, count);
            }
        }

        private unsafe void processUnsafe(ChannelData channel, float[] buffer, int count)
        {
            var coefficientsLength = this.coefficients.Length;

            var inputHistoryLength = channel.InputHistory.Length;
            var position = channel.Position;

            fixed (float* fixedBuffer = buffer, 
                fixedInputHistory = channel.InputHistory, 
                fixedCoefficients = coefficients)
            {
                float* pBuffer = fixedBuffer;
                float* pInputHistory = fixedInputHistory + position;

                for (int i = 0; i < count; i++)
                {
                    *pInputHistory = *pBuffer;

                    float sample = 0f;

                    float* ph = pInputHistory;
                    float* pCoefficients = fixedCoefficients;
                    for (int j = 0; j < coefficientsLength; j++)
                    {
                        sample += *ph * *(pCoefficients++);

                        if (--ph < fixedInputHistory)
                        {
                            ph += inputHistoryLength;
                        }
                    }
                    *(pBuffer++) = sample;

                    position++;
                    pInputHistory++;
                    if (position >= inputHistoryLength)
                    {
                        position = 0;
                        pInputHistory = fixedInputHistory;
                    }
                }
            }

            channel.Position = position;
        }

        [ExcludeFromCodeCoverage]
        private void processSafe(ChannelData channel, float[] buffer, int count)
        {
            var inputHistoryLength = channel.InputHistory.Length;

            for (var i = 0; i < count; i++)
            {
                channel.InputHistory[channel.Position] = buffer[i];

                float sample = 0f;
                int index = channel.Position + inputHistoryLength;
                for (var j = 0; j < Coefficients.Length; j++)
                {
                    sample += channel.InputHistory[index % inputHistoryLength] * Coefficients[j];
                }
                buffer[i] = sample;

                channel.Position += 1;
                channel.Position %= inputHistoryLength;
            }
        }
    }
}
