using NAudio.Dsp;
using NAudio.Wave;
using ndaw.Core.Routing;
using System;
using System.Linq;

namespace ndaw.Core.Fourier
{
    public class FourierTransform : ISignalProcess
    {
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
        private ChannelData[] channels;
        private int m;
        
        public event EventHandler<FourierTransformEventArgs> DataReady;

        public FourierTransform(int transformLength)
        {
            if (!IsPowerOfTwo(transformLength))
            {
                throw new ArgumentException("Transform length must be a power of two", "transformLength");
            }
            
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
                    X = (float)(channel.InputHistory[i] 
                        * FastFourierTransform.BlackmannHarrisWindow(i, transformLength)),
                    Y = 0f
                };
            }

            FastFourierTransform.FFT(true, m, channel.Complex);

            var e = new FourierTransformEventArgs
            {
                TranformLength = transformLength,
                Channel = channel.Index,
                Real = channel.Complex.Select(c => (float)c.X).ToArray(),
                Imaginary = channel.Complex.Select(c => (float)c.Y).ToArray()
            };

            if (DataReady != null)
            {
                DataReady.Invoke(this, e);
            }
        }
    }
}
