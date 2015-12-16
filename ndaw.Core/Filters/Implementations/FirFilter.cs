using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using NAudio.Utils;
using NAudio.Wave;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ndaw.Core.Filters.Implementations
{
    public class FirFilter : IFilterImplementation
    {
        // TODO implement coefficient calculator Kaiser
        
        public string Name { get { return "FIR Filter"; } [ExcludeFromCodeCoverage]set { } }

        private class ChannelData
        {
            public int Position;
            public float[] InputHistory;
        }

        private float[] coefficients;
        private Matrix<float> C;
        public float[] Coefficients
        {
            get { return coefficients; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value", "Coefficients cannot be null");
                }
                if (value.Length == 0)
                {
                    throw new ArgumentOutOfRangeException("value", "Length of coefficients must be greater than zero");
                }

                coefficients = value;

                C = new DenseMatrix(1, coefficients.Length, coefficients.Reverse().ToArray());
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
                    }
                }
            }
        }

        private float[] matrix;
        private float[] history;
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

            if (coefficients.Length > count)
            {
                throw new InvalidOperationException("Count must be greater than or equal to coefficient length");
            }

            for (int i = 0; i < format.Channels; i++)
            {
                var channel = channels[i];
                var buffer = buffers[i];

                // Using BlockCopy and Math.Net Numerics Matrix is 12 times faster in testing and 4 times faster with small buffers
                processMatrix(channel, buffer, count);
                // Unsafe equivalent was approximately 3 times faster in testing
                //processUnsafe(channel, buffer, count);
                //processSafe(channel, buffer, count);
            }
        }

        private void processMatrix(ChannelData channel, float[] buffer, int count)
        {
            var c = coefficients.Length;
            var src = 1 - c;
            var dst = 0;
            var offset = c - 1;

            matrix = BufferHelpers.Ensure(matrix, count * c);
            history = BufferHelpers.Ensure(history, c);
            channel.InputHistory = BufferHelpers.Ensure(channel.InputHistory, c);

            Buffer.BlockCopy(channel.InputHistory, 0, history, 0, sizeof(float) * c);

            for (int i = 0; i < count; i++)
            {
                Buffer.BlockCopy(
                    buffer,
                    sizeof(float) * (src + offset),
                    matrix,
                    sizeof(float) * (dst + offset),
                    sizeof(float) * (c - offset));

                if (offset > 0)
                {
                    Buffer.BlockCopy(
                        history,
                        sizeof(float) * (c - offset),
                        matrix,
                        sizeof(float) * dst,
                        sizeof(float) * offset);

                    offset -= 1;
                }

                src += 1;
                dst += c;
            }

            Buffer.BlockCopy(buffer, sizeof(float) * (count - c), channel.InputHistory, 0, sizeof(float) * c);

            var X = new DenseMatrix(coefficients.Length, count, matrix);

            var B = new DenseMatrix(1, count, buffer);
            C.Multiply(X, B);
        }

        [ExcludeFromCodeCoverage]
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
