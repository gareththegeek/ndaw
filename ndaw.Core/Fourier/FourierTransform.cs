using NAudio.Dsp;
using NAudio.Wave;
using ndaw.Core.Filters.WindowFunctions;
using ndaw.Core.Routing;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ndaw.Core.Fourier
{
    public class FourierTransform : ISignalProcess
    {
        private readonly IFastFourierTransformProvider transformProvider;

        public string Name { get { return "Fourier Transform"; } [ExcludeFromCodeCoverage]set { } }

        private class ChannelData
        {
            public int Index;
            public Complex[] Complex;
            public float[] InputHistory;
            public int Position;
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
                        var channel = new ChannelData();
                        channel.Index = i;
                        channel.Complex = new Complex[transformLength];
                        channel.InputHistory = new float[transformLength];
                        channels[i] = channel;
                    }
                }
            }
        }

        private int transformLength;
        private float[] window;
        private ChannelData[] channels;
        private int m;
        
        public event EventHandler<FourierTransformEventArgs> DataReady;

        public FourierTransform(
            IFastFourierTransformProvider transformProvider, 
            IWindowFunction windowFunction,
            int transformLength)
        {
            if (!IsPowerOfTwo(transformLength))
            {
                throw new ArgumentException("Transform length must be a power of two", "transformLength");
            }
            if (transformProvider == null)
            {
                throw new ArgumentNullException("transformProvider", "Transform provider cannot be null");
            }
            if (windowFunction == null)
            {
                throw new ArgumentNullException("windowFunction", "Window function cannot be null");
            }
            
            this.transformProvider = transformProvider;
            this.window = windowFunction.CalculateCoefficients(transformLength);
            this.transformLength = transformLength;
            this.m = (int)Math.Log(transformLength, 2d);
        }

        private bool IsPowerOfTwo(int x)
        {
            return (x & (x - 1)) == 0;
        }

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

            if (buffers.Any(b => b.Length < count))
            {
                throw new ArgumentOutOfRangeException("count", "Count must be equal to or less than buffer length");
            }

            for (int c = 0; c < format.Channels; c++)
            {
                var channel = channels[c];
                var buffer = buffers[c];

                for (int i = 0; i < count; i++)
                {
                    channel.InputHistory[channel.Position] = buffer[i];

                    channel.Position += 1;

                    if (channel.Position == transformLength)
                    {
                        calculateTransform(channel);
                        channel.Position = 0;
                    }
                }
            }
        }

        private void calculateTransform(ChannelData channel)
        {
            for (int i = 0; i < transformLength; i++)
            {
                channel.Complex[i] = new Complex
                {
                    X = channel.InputHistory[i] * window[i],
                    Y = 0f
                };
            }

            transformProvider.FFT(true, m, channel.Complex);

            var e = new FourierTransformEventArgs(
                transformLength, 
                channel.Index, 
                channel.Complex);
            
            if (DataReady != null)
            {
                DataReady.Invoke(this, e);
            }
        }
    }
}
