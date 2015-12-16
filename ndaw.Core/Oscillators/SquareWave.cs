using NAudio.Wave;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ndaw.Core.Oscillators
{
    public class SquareWave : IOscillator
    {
        public string Name { get { return "Square Wave"; } [ExcludeFromCodeCoverage]set { } }

        private float halfFrequencyInSamples;
        private float frequency;
        public float Frequency
        {
            get { return frequency; }
            set
            {
                if (value <= 0f)
                {
                    throw new ArgumentOutOfRangeException("value", "Frequency cannot be less than or equal to zero");
                }

                frequency = value;
                if (format != null)
                {
                    halfFrequencyInSamples = (format.SampleRate / value) / 2f;
                }
            }
        }

        private float amplitude;
        public float Amplitude
        {
            get { return amplitude; }
            set
            {
                if (value < 0f || value > 1f)
                {
                    throw new ArgumentOutOfRangeException("value", "Amplitude must be between 0 and 1");
                }

                amplitude = value;
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
                    halfFrequencyInSamples = (format.SampleRate / frequency) / 2f;
                }
            }
        }

        public int Time { get; set; }

        public SquareWave()
        {
            this.Time = 0;

            this.Frequency = 400f;
            this.Amplitude = 0.125f;
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

            var time = Time;
            var channels = format.Channels;

            unsafe
            {
                fixed (float* fixedBuffer = buffers[0])
                {
                    var pBuffer = fixedBuffer;

                    for (int i = 0; i < count; i++)
                    {
                        var sample = (2f * (float)((int)(time++ / halfFrequencyInSamples) % 2) - 1f) * amplitude;

                        *pBuffer++ = sample;
                    }
                }

                for (int j = 1; j < format.Channels; j++)
                {
                    Buffer.BlockCopy(buffers[0], 0, buffers[j], 0, sizeof(float) * count);
                }
            }

            Time = time;
        }
    }
}
