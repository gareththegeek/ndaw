using NAudio.Wave;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ndaw.Core.Oscillators
{
    public class SineWave : IOscillator
    {
        public string Name { get { return "Sine Wave"; } [ExcludeFromCodeCoverage]set { } }

        private const float PI2 = (float)(Math.PI * 2D);
        private const int TableSize = 1024 * 128;
        private const float TableSizeF = (float)TableSize;
        private const float Factor = TableSizeF / PI2;
        private static float[] sineTable;

        static SineWave()
        {
            sineTable = new float[TableSize];
            for (int i = 0; i < TableSize; i++)
            {
                float angle = ((float)i / TableSizeF) * PI2;
                sineTable[i] = (float)Math.Sin(angle);
            }
        }

        private float frequencyInSamples;
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
                    frequencyInSamples = format.SampleRate / value;
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
                    throw new ArgumentOutOfRangeException("value", "Amplitude must be between zero and one");
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

                if (value != null)
                {
                    frequencyInSamples = format.SampleRate / frequency;
                }
            }
        }

        public int Time { get; set; }

        public SineWave()
        {
            this.Time = 0;

            this.Frequency = 200f;
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

            var factor = Factor * PI2 * (1f / frequencyInSamples);
            var time = this.Time;

            unsafe
            {
                fixed (float* pSineTable = sineTable, fixedBuffer = buffers[0])
                {
                    var pBuffer = fixedBuffer;

                    for (int i = 0; i < count; i++)
                    {
                        int index = (int)(time++ * factor);
                        index %= TableSize;
                        index = (index >= 0) ? index : TableSize - index;

                        *(pBuffer++) = *(pSineTable + index) * amplitude;
                    }
                }

                for (int j = 1; j < format.Channels; j++)
                {
                    Buffer.BlockCopy(buffers[0], 0, buffers[j], 0, sizeof(float) * count);
                }
            }

            this.Time = time;
        }

        //public float Generate(int time)
        //{
        //    var value = PI2 * (time / frequencyInSamples);
        //    int index = (int)(value * Factor);
        //    index %= TableSize;
        //    index = (index >= 0) ? index : TableSize - index;
        //    return amplitude * sineTable[index];
        //}
    }
}
