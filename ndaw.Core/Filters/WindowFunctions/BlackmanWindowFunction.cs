using System;

namespace ndaw.Core.Filters.WindowFunctions
{
    public class BlackmanWindowFunction : IWindowFunction
    {
        public float[] CalculateCoefficients(int filterOrder /* Number of coefficients */)
        {
            if (filterOrder <= 0)
            {
                throw new ArgumentOutOfRangeException("filterOrder", "Filter order must be a positive value");
            }

            var N = filterOrder;
            var w = new float[N + 1];
            for (int n = 0; n <= N; n++)
            {
                var twoPi_nOverN = 2 * Math.PI * n / N;

                w[n] = 0.42f - 0.5f * (float)Math.Cos(twoPi_nOverN) + 0.08f * (float)Math.Cos(2 * twoPi_nOverN);
            }
            return w;
        }
    }
}
