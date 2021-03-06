﻿using NAudio.Wave;
using ndaw.Core.Effects;
using ndaw.Core.Filters;
using ndaw.Core.Filters.FilterFunctions;
using ndaw.Core.Filters.Implementations;
using ndaw.Core.Filters.WindowFunctions;
using ndaw.Core.Fourier;
using ndaw.Core.Oscillators;
using ndaw.Core.Routing;
using ndaw.Core.Soundcard.Asio;
using ndaw.Core.Soundcard.Wave;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace ndaw
{
    public partial class Form1 : Form
    {
        private AsioOut asioDriver = null;
        private AsioCard asioCard;

        private WaveIn waveIn;
        private WaveOut waveOut;
        private WaveCard waveCard;

        private AsioInputMapper asioInput;
        private AsioOutputMapper asioOutput;

        private ComplexFilter filter;
        private MonoSignalNode filterNode;
        private StereoSignalNode stereoFilterNode;

        private FourierTransform fourier;
        private MonoSignalNode fourierNode;

        private Flanger flanger;
        private MonoSignalNode flangerNode;

        //private SquareWave squareWave;
        //private SineWave sineWave;
        //private ISignalNode oscillatorNode;

        private SineWave sineWave;
        private StereoSignalNode sineWaveNode;

        private SignalNetworkForm signalNetworkForm;
        
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cmbDevice.Items.AddRange(AsioOut.GetDriverNames());

            signalNetworkForm = new SignalNetworkForm();
            signalNetworkForm.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //if (recorder != null)
            //{
            //    recorder.StopRecording();
            //    recorder.Dispose();
            //}

            if (asioDriver != null)
            {
                asioDriver.Stop();
                asioDriver.Dispose();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var deviceIndex = cmbDevice.SelectedIndex;

            asioDriver = new AsioOut(deviceIndex);
            asioDriver.ChannelOffset = 0;

            var monoFormat = WaveFormat.CreateIeeeFloatWaveFormat(44100, 1);
            var stereoFormat = WaveFormat.CreateIeeeFloatWaveFormat(44100, 2);

            //waveIn = new WaveIn();
            //waveOut = new WaveOut();

            //waveIn.BufferMilliseconds = 10;
            //waveOut.DesiredLatency = 10;

            new Thread(() =>
            {
                asioInput = new AsioInputMapper();
                asioOutput = new AsioOutputMapper();
                asioCard = new AsioCard(monoFormat, asioDriver, asioInput, asioOutput);
                asioCard.Start();

                //waveCard = new WaveCard(stereoFormat, waveIn, waveOut, new WaveInputMapper(), new WaveOutputMapper());
                //waveCard.Start();

                BuildAudioInChain(monoFormat);

                signalNetworkForm.SignalNetworkControl.Nodes = new ObservableCollection<ISignalNode>();

                signalNetworkForm.SignalNetworkControl.Nodes.Add(asioInput);
                signalNetworkForm.SignalNetworkControl.Nodes.Add(flangerNode);
                signalNetworkForm.SignalNetworkControl.Nodes.Add(filterNode);
                signalNetworkForm.SignalNetworkControl.Nodes.Add(fourierNode);
                signalNetworkForm.SignalNetworkControl.Nodes.Add(asioOutput);
                


                //BuildSineWaveChain(stereoFormat);

            }).Start();

            for (int i = 0; i < asioDriver.DriverInputChannelCount; i++)
            {
                cboChannelIn.Items.Add(asioDriver.AsioInputChannelName(i));
            }

            for (int i = 0; i < asioDriver.DriverOutputChannelCount; i++)
            {
                cboChannelOut.Items.Add(asioDriver.AsioOutputChannelName(i));
            }
        }

        private void BuildAudioInChain(WaveFormat monoFormat)
        {
            filter = new ComplexFilter(
                monoFormat,
                new BlackmanHarrisWindowFunction(),
                new FirFilter());

            filter.Filters.Add(new DigitalFilter
            {
                FilterFunction = new LowPassFilterFunction(),
                LowerCutOffFrequency = 10000f
            });

            filterNode = new MonoSignalNode(monoFormat, filter.FilterImplementation);

            fourier = new FourierTransform(
                new FastFourierTransformProvider(), 
                new BlackmanHarrisWindowFunction(),
                2048);
            fourierNode = new MonoSignalNode(monoFormat, fourier);
            fourier.DataReady += fourierControl.fourier_DataReady;

            flanger = new Flanger(new SineWave());
            flangerNode = new MonoSignalNode(monoFormat, flanger);

            flangerNode.CentreIn.Source = asioInput.Sources.First();
            //flangerNode.CentreIn.Source = waveCard.Inputs.First();
            filterNode.CentreIn.Source = flangerNode.CentreOut;
            fourierNode.CentreIn.Source = filterNode.CentreOut;
            asioOutput.Sinks.ElementAt(0).Source = filterNode.CentreOut;
            asioOutput.Sinks.ElementAt(1).Source = filterNode.CentreOut;
            //waveCard.Outputs.ElementAt(0).Source = filterNode.CentreOut;
            //waveCard.Outputs.ElementAt(1).Source = filterNode.CentreOut;
        }

        private void BuildSineWaveChain(WaveFormat stereoFormat)
        {
            sineWave = new SineWave();
            sineWaveNode = new StereoSignalNode(stereoFormat, sineWave);

            sineWaveNode.LeftIn.Source = asioInput.Sources.ElementAt(0);
            sineWaveNode.RightIn.Source = asioInput.Sources.ElementAt(0);

            filter = new ComplexFilter(
                stereoFormat,
                new BlackmanHarrisWindowFunction(),
                new FirFilter());

            filter.Filters.Add(new DigitalFilter
            {
                FilterFunction = new BandStopFilterFunction(),
                LowerCutOffFrequency = 10000f,
                UpperCutOffFrequency = 12000f
            });

            stereoFilterNode = new StereoSignalNode(stereoFormat, filter.FilterImplementation);

            stereoFilterNode.LeftIn.Source = sineWaveNode.LeftOut;
            stereoFilterNode.RightIn.Source = sineWaveNode.RightOut;

            fourier = new FourierTransform(
                new FastFourierTransformProvider(),
                new BlackmanHarrisWindowFunction(),
                2048);
            fourierNode = new MonoSignalNode(stereoFormat, fourier);
            fourier.DataReady += fourierControl.fourier_DataReady;

            fourierNode.CentreIn.Source = stereoFilterNode.LeftOut;

            asioOutput.Sinks.ElementAt(0).Source = stereoFilterNode.LeftOut;
            asioOutput.Sinks.ElementAt(1).Source = stereoFilterNode.RightOut;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            button1_Click(sender, null);
        }

        private void vsbVolume_Scroll(object sender, ScrollEventArgs e)
        {
            
        }

        private void cmbDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void cboChannel_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void chkLowPass_CheckedChanged(object sender, EventArgs e)
        {
            if (filterNode != null)
            {
                filterNode.Bypass = !chkLowPass.Checked;
            }

            if (stereoFilterNode != null)
            {
                stereoFilterNode.Bypass = !chkLowPass.Checked;
            }
        }
    }
}
