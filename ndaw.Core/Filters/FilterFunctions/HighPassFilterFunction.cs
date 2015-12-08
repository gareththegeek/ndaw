using System;

namespace ndaw.Core.Filters.FilterFunctions
{
    public class HighPassFilterFunction : IFilterFunction
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
            var omega = (2 * Math.PI * upperCutoffFrequency) / sampleRate;

            for (int n = 0; n < N + 1; n++)
            {
                if (n != M)
                {
                    var nM = n - M;
                    h[n] = (float)(-Math.Sin(omega * nM) / (Math.PI * nM));
                }
                else
                {
                    h[n] = (float)(1d - omega / Math.PI);
                }
            }

            return h;
        }
    }
}
