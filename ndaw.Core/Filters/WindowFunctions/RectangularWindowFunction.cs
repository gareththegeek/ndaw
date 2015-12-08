using System;

namespace ndaw.Core.Filters.WindowFunctions
{
    public class RectangularWindowFunction: IWindowFunction
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
                w[n] = 1f;
            }
            return w;
        }
    }
}
