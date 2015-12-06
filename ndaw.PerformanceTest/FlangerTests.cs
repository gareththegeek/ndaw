using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ndaw.Core.Effects;
using ndaw.Core.Oscillators;
using NSubstitute;
using NAudio.Wave;
using System.Diagnostics;

namespace ndaw.Core.PerformanceTest
{
    [TestClass]
    public class FlangerTests
    {
        [TestMethod]
        public void PerformanceTest_10000_iterations_1000_length_buffer()
        {
            var target = new Flanger(new SineWave());

            target.Format = new WaveFormat(44100, 1);

            var time = Helpers.Time(() =>
            {
                var buffer = new float[1000];
                for (int i = 0; i < 10000; i++)
                {
                    target.Process(new float[][] { buffer }, 1000);
                }
            });
            Debug.WriteLine(string.Format("FirFilter performance test: {0}", time));
        }
    }
}
