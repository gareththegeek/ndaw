using Microsoft.VisualStudio.TestTools.UnitTesting;
using NAudio.Dsp;
using NAudio.Wave;
using ndaw.Core.Filters.WindowFunctions;
using ndaw.Core.Fourier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ndaw.Core.PerformanceTest
{
    [TestClass]
    public class FourierTransformTests
    {
        private class ProviderFake : IFastFourierTransformProvider
        {
            public void FFT(bool forward, int m, NAudio.Dsp.Complex[] data)
            {
                
            }
        }

        [TestMethod]
        public void FourierTransform_PerformanceTest()
        {
            var transformProvider = new ProviderFake();
            var windowFunction = new BlackmanHarrisWindowFunction();

            var target = new FourierTransform(
                transformProvider,
                windowFunction,
                4096 * 4);

            target.Format = new WaveFormat(44100, 1);

            var buffers = new float[][] { new float[64] };

            for (int i = 0; i < 500000; i++)
            {
                target.Process(buffers, 64);
            }
        }
    }
}
