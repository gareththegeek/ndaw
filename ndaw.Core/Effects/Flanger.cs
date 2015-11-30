using NAudio.Wave;
using ndaw.Core.Routing;
using System;
using System.Linq;

namespace ndaw.Core.Effects
{
    public class Flanger: ISignalProcess
    {
        //TODO Flanger appears to be special case of delay unit, with no decay - allow the 
        // option for delay value to be constant or to be driver by external LFO unit so that
        // phase and delay effects are same class?

        // Note that current sample is always overwritten instead of being aggregated in a normal delay unit

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
                if (depth < 0f || depth > 1f)
                {
                    throw new ArgumentOutOfRangeException("value", "Depth must be between 0 and 1");
                }

                depth = value; 
            }
        }

        private float wet = 0.5f;
        private float dry = 0.5f;
        public float Wet
        {
            get { return wet; }
            set
            {
                if (wet < 0f || wet > 1f)
                {
                    throw new ArgumentOutOfRangeException("value", "Wet must be between 0 and 1");
                }

                wet = value;
                dry = 1f - value;
            }
        }

        private float volume = 1f;
        public float Volume
        {
            get { return volume; }
            set
            {
                if (volume < 0f || volume > 1f)
                {
                    throw new ArgumentOutOfRangeException("value", "Volume must be between 0 and 1");
                }

                volume = value;
            }
        }

        private float frequency;
        private float lfoFactor;
        public float Frequency
        {
            get { return frequency; }
            set
            {
                if (frequency < 0f)
                {
                    throw new ArgumentOutOfRangeException("value", "Frequency must be a positive value");
                }

                frequency = value;
                if (format != null)
                {
                    lfoFactor = frequency / format.SampleRate;
                }
            }
        }

        private const float maximumDelay = 88.2f;
        public float MaximumDelay
        {
            get { return maximumDelay; }
            set
            {
                if (maximumDelay > BufferLength * 2 || maximumDelay < 1f)
                {
                    throw new ArgumentOutOfRangeException("value", string.Format("MaximumDelay must be between 1 and {0} sample(s)", BufferLength));
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
                    channels = new ChannelData[format.Channels];
                    for (int i = 0; i < format.Channels; i++)
                    {
                        var channel = new ChannelData();
                        channel.DelayBuffer = new float[BufferLength];
                        channels[i] = channel;
                    }

                    lfoFactor = frequency / format.SampleRate;
                }
            }
        }

        private const int BufferLength = 88200;

        private ChannelData[] channels;

        public Flanger()
        {
            this.Frequency = 0.25f;
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
            for (int i = 0; i < count; i++)
            {
                //TODO Math.Sin could be speeded up using lookup table?

                var sample = buffer[i];
                channel.DelayBuffer[channel.Position] = sample;

                int s = channel.Time++;

                var delay = ((Math.Sin(2f * Math.PI * s * lfoFactor) * depth + 1f) * maximumDelay / 2f);

                float delaySample;

                var delayPosition = channel.Position - (int)delay;

                delayPosition += BufferLength;
                delayPosition %= BufferLength;

                delaySample = (float)(channel.DelayBuffer[delayPosition]);

                buffer[i] = (sample * dry + delaySample * wet) * volume;

                channel.Position += 1;
                channel.Position %= BufferLength;
            }
        }
    }
}
