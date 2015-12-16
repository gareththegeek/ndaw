using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ndaw.Core.Oscillators;
using NAudio.Wave;

namespace ndaw.Core.PerformanceTest
{
    [TestClass]
    public class SineWaveTests
    {
        [TestMethod]
        public void SineWave_1000_samples_500000_iterations_4_channels()
        {
            var buffers = new[]
            {
                new float[1000],
                new float[1000],
                new float[1000],
                new float[1000]
            };

            var target = new SineWave();
            target.Format = new WaveFormat(44100, 4);

            for (int i = 0; i < 500000; i++)
            {
                target.Process(buffers, 1000);
            }
        }
    }
}
