using System;

namespace ndaw.Core.Filters.FilterFunctions
{
    public class HighPassFilterFunction : IFilterFunction
    {
        public float[] CalculateCoefficients(int filterOrder, float lowerCutoffFrequency, float upperCutoffFrequency, int sampleRate)
        {
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
