using System;

namespace ndaw.Core.Filters.WindowFunctions
{
    public class BlackmanHarrisWindowFunction : IWindowFunction
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

                w[n] = (float)(
                        0.35875d
                        - 0.48829d * Math.Cos(1 * twoPi_nOverN)
                        + 0.14128d * Math.Cos(2 * twoPi_nOverN)
                        - 0.01168d * Math.Cos(3 * twoPi_nOverN)
                    );
            }
            return w;
        }
    }
}
