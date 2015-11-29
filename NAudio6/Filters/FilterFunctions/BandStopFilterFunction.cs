using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ndaw.Filters.FilterFunctions
{
    public class BandStopFilterFunction : IFilterFunction
    {
        public float[] CalculateCoefficients(int filterOrder, float lowerCutoffFrequency, float upperCutoffFrequency, int sampleRate)
        {
            var N = filterOrder;
            var M = N / 2;

            var h = new float[N + 1];
            var omega1 = (2 * Math.PI * lowerCutoffFrequency) / sampleRate;
            var omega2 = (2 * Math.PI * upperCutoffFrequency) / sampleRate;

            for (int n = 0; n < N + 1; n++)
            {
                if (n != M)
                {
                    var nM = n - M;
                    var PInM = Math.PI * nM;

                    h[n] = (float)((Math.Sin(omega1 * nM) / PInM) - (Math.Sin(omega2 * nM) / PInM));
                }
                else
                {
                    h[n] = (float)(1f - ((omega2 - omega1) / Math.PI));
                }
            }

            return h;
        }
    }
}
