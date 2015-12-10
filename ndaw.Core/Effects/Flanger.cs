using NAudio.Utils;
using NAudio.Wave;
using ndaw.Core.Oscillators;
using ndaw.Core.Routing;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ndaw.Core.Effects
{
    public class Flanger: ISignalProcess
    {
        public string Name { get { return "Flanger"; } [ExcludeFromCodeCoverage]set { } }

        private class ChannelData
        {
            public int Position;
            public float[] DelayBuffer;
            public int Time;
        }

        private float depth = 1f;
        public float Depth 
        {
            get { return depth; } 
            set 
            {
                if (value < 0f || value > 1f)
                {
                    throw new ArgumentOutOfRangeException("value", "Depth must be between 0 and 1");
                }

                depth = value;
                lfo.Amplitude = depth;
            }
        }

        private float wet = 0.5f;
        private float dry = 0.5f;
        public float Wet
        {
            get { return wet; }
            set
            {
                if (value < 0f || value > 1f)
                {
                    throw new ArgumentOutOfRangeException("value", "Wet must be between 0 and 1");
                }

                wet = value;
                dry = 1f - value;
            }
        }

        private float frequency;
        public float Frequency
        {
            get { return frequency; }
            set
            {
                if (value < 0f)
                {
                    throw new ArgumentOutOfRangeException("value", "Frequency must be a positive value");
                }

                frequency = value;
                lfo.Frequency = value;
            }
        }

        private int bufferLength = 45;
        private float maximumDelay = 1f;
        private float maximumDelaySamples = 44.1f;
        public float MaximumDelay
        {
            get { return maximumDelay; }
            set
            {
                if (value <= 0f)
                {
                    throw new ArgumentOutOfRangeException("value", "Maximum delay must be positive");
                }

                maximumDelay = value;
                
                if (format != null)
                {
                    maximumDelaySamples = Utility.MillisecondsToSamples(format.SampleRate, maximumDelay);
                    bufferLength = (int)Math.Ceiling(maximumDelaySamples);

                    foreach (var channel in channels)
                    {
                        channel.DelayBuffer = BufferHelpers.Ensure(channel.DelayBuffer, bufferLength);
                    }
                }
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
                    maximumDelaySamples = Utility.MillisecondsToSamples(format.SampleRate, maximumDelay);
                    bufferLength = (int)Math.Ceiling(maximumDelaySamples);

                    channels = new ChannelData[format.Channels];
                    for (int i = 0; i < format.Channels; i++)
                    {
                        var channel = new ChannelData();
                        channel.DelayBuffer = new float[bufferLength];
                        channels[i] = channel;
                    }

                    lfo.Format = format;
                }
            }
        }

        private ChannelData[] channels;
        private IOscillator lfo;

        public Flanger(IOscillator lfo)
        {
            if (lfo == null)
            {
                throw new ArgumentNullException("lfo", "Low frequency oscillator cannot be null");
            }

            this.lfo = lfo;
            this.Frequency = 0.25f;
            this.Depth = 1f;
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

            if (buffers.Any(b => b.Length < count || count < 0))
            {
                throw new ArgumentOutOfRangeException("count", "Count must be equal to or less than buffer length");
            }

            for (int i = 0; i < format.Channels; i++)
            {
                processChannel(channels[i], buffers[i], count);
            }
        }

        private void processChannel(ChannelData channel, float[] buffer, int count)
        {
            //TODO unsafe implementation

            for (int i = 0; i < count; i++)
            {
                var sample = buffer[i];
                channel.DelayBuffer[channel.Position] = sample;

                int s = channel.Time++;

                var delay = (lfo.Generate(s) + 1f) * (maximumDelaySamples / 2f);

                float delaySample;

                var delayPosition = channel.Position - (int)delay;

                delayPosition += bufferLength;
                delayPosition %= bufferLength;

                delaySample = (float)(channel.DelayBuffer[delayPosition]);

                buffer[i] = (sample * dry + delaySample * wet);

                channel.Position += 1;
                channel.Position %= bufferLength;
            }
        }
    }
}
