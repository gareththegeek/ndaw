﻿using NAudio.Wave;
using ndaw.Core.Routing;
using System;
using System.Collections.Generic;

namespace ndaw.Core.Soundcard.Asio
{
    public class AsioCard : ISoundcard
    {
        public string Name { get; set; }

        public IEnumerable<ISignalSource> Sources { get { return inputMapper.Inputs; } }
        public IEnumerable<ISignalSink> Sinks { get { return outputMapper.Outputs; } }

        private AsioOut driver;
        private AsioInputMapper inputMapper;
        private AsioOutputMapper outputMapper;
        private WaveFormat format;

        private bool initialised = false;
        
        public AsioCard(
            WaveFormat format,
            AsioOut driver, 
            AsioInputMapper inputMapper, 
            AsioOutputMapper outputMapper)
        {
            if (format == null)
            {
                throw new ArgumentNullException("format", "Must specify an audio format");
            }

            if (driver == null)
            {
                throw new ArgumentNullException("driver", "Asio driver cannot be null");
            }

            if (inputMapper == null)
            {
                throw new ArgumentNullException("inputMapper", "Asio input mapper cannot be null");
            }

            if (outputMapper == null)
            {
                throw new ArgumentNullException("outputMapper", "Asio output mapper cannot be null");
            }

            this.format = format;
            this.driver = driver;
            this.inputMapper = inputMapper;
            this.outputMapper = outputMapper;
        }

        private void initialise()
        {
            Name = driver.DriverName;

            inputMapper.Initialise(this, format, driver);
            outputMapper.Initialise(this, format, driver);

            driver.InitRecordAndPlayback(
                outputMapper.OutputBuffer, 
                inputMapper.Format.Channels,
                inputMapper.Format.SampleRate);
#if DEBUG
            driver.ShowControlPanel();
#endif
            initialised = true;
        }

        public void Start()
        {
            if (!initialised)
            {
                initialise();
            }

            driver.Play();
        }

        public void Stop()
        {
            driver.Stop();
        }
    }
}
