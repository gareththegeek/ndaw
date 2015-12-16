using Microsoft.VisualStudio.TestTools.UnitTesting;
using NAudio.Wave;
using ndaw.Core.Filters.Implementations;
using System.Diagnostics;

namespace ndaw.Core.PerformanceTest
{
    [TestClass]
    public class FirFilterTest
    {
        [TestMethod]
        public void FirFilter_PerformanceTest_100_coefficients_1000_samples_and_50000_iterations()
        {
            var format = new WaveFormat(44100, 1);
            var target = new FirFilter();
            target.Format = format;

            target.Coefficients = new float[100];

            var time = Helpers.Time(() =>
            {
                var buffer = new float[1000];
                for (int i = 0; i < 50000; i++)
                {
                    target.Process(new float[][] { buffer }, 1000);
                }
            });
            Debug.WriteLine(string.Format("FirFilter performance test: {0}", time));
        }

        [TestMethod]
        public void FirFilter_PerformanceTest_20_coefficients_64_samples_and_1000000_iterations()
        {
            var format = new WaveFormat(44100, 1);
            var target = new FirFilter();
            target.Format = format;

            target.Coefficients = new float[20];

            var time = Helpers.Time(() =>
            {
                var buffer = new float[64];
                for (int i = 0; i < 1000000; i++)
                {
                    target.Process(new float[][] { buffer }, 64);
                }
            });
            Debug.WriteLine(string.Format("FirFilter performance test: {0}", time));
        }
    }
}
