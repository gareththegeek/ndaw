using System;

namespace ndaw.Core.Filters.FilterFunctions
{
    public class BandPassFilterFunction : IFilterFunction
    {
        public float[] CalculateCoefficients(
            int filterOrder, 
            float lowerCutoffFrequency, 
            float upperCutoffFrequency, 
            int sampleRate)
        {
            if (filterOrder <= 0)
            {
                throw new ArgumentOutOfRangeException("filterOrder", "Order must be a positive value");
            }
            if (lowerCutoffFrequency <= 0f)
            {
                throw new ArgumentOutOfRangeException("lowerCutoffFrequency", "Lower cut-off frequency must be a positive value");
            }
            if (upperCutoffFrequency <= 0f)
            {
                throw new ArgumentOutOfRangeException("upperCutoffFrequency", "Upper cut-off frequency must be a positive value");
            }
            if (sampleRate <= 0f)
            {
                throw new ArgumentOutOfRangeException("sampleRate", "Sample rate must be a positive value");
            }

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

                    h[n] = (float)((Math.Sin(omega2 * nM) / PInM) - (Math.Sin(omega1 * nM) / PInM));
                }
                else
                {
                    h[n] = (float)((omega2 - omega1) / Math.PI);
                }
            }

            return h;
        }
    }
}
