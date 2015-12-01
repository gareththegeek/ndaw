using NAudio.Wave;
using ndaw.Core.Routing;
using System;
using System.Collections.Generic;

namespace ndaw.Core.Soundcard.Wave
{
    public class WaveCard: ISoundcard
    {
        public string Name { get; set; }

        public IEnumerable<ISignalSource> Sources { get { return inputMapper.Inputs; } }
        public IEnumerable<ISignalSink> Sinks { get { return outputMapper.Outputs; } }

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

        private void initialise()
        {
            Name = WaveOut.GetCapabilities(outDriver.DeviceNumber).ProductName;

            inputMapper.Initialise(this, format, inDriver);
            outputMapper.Initialise(this, format, outDriver);

            initialised = true;
        }

        public void Start()
        {
            if (!initialised)
            {
                initialise();
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
