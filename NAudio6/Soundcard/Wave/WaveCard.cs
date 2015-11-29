using NAudio.Wave;
using ndaw.Routing;
using System;
using System.Collections.Generic;

namespace ndaw.Soundcard.Wave
{
    public class WaveCard: ISoundcard
    {
        public IEnumerable<ISignalSource> Inputs { get { return inputMapper.Inputs; } }
        public IEnumerable<ISignalSink> Outputs { get { return outputMapper.Outputs; } }

        private WaveOut outDriver;
        private WaveIn inDriver;
        private WaveInputMapper inputMapper;
        private WaveOutputMapper outputMapper;
        private WaveFormat format;

        private bool initialised = false;

        public WaveCard(
            WaveFormat format,
            WaveIn inDriver,
            WaveOut outDriver,
            WaveInputMapper inputMapper,
            WaveOutputMapper outputMapper)
        {
            if (format == null)
            {
                throw new ArgumentNullException("format", "Must specify an audio format");
            }

            if (inDriver == null)
            {
                throw new ArgumentNullException("inDriver", "WaveIn driver cannot be null");
            }

            if (outDriver == null)
            {
                throw new ArgumentNullException("outDriver", "WaveOut driver cannot be null");
            }

            if (inputMapper == null)
            {
                throw new ArgumentNullException("inputMapper", "Wave input mapper cannot be null");
            }

            if (outputMapper == null)
            {
                throw new ArgumentNullException("outputMapper", "Wave output mapper cannot be null");
            }

            this.format = format;
            this.inDriver = inDriver;
            this.outDriver = outDriver;
            this.inputMapper = inputMapper;
            this.outputMapper = outputMapper;
        }

        private void initialse()
        {
            inputMapper.Initialise(format, inDriver);
            outputMapper.Initialise(format, outDriver);

            initialised = true;
        }

        public void Start()
        {
            if (!initialised)
            {
                initialse();
            }

            outDriver.Play();
            inDriver.StartRecording();
        }

        public void Stop()
        {
            outDriver.Stop();
            inDriver.StopRecording();
        }
    }
}
