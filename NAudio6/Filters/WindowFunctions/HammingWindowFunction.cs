using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ndaw.Filters.WindowFunctions
{
    public class HammingWindowFunction : IWindowFunction
    {
        public float[] CalculateCoefficients(int filterOrder /* Number of coefficients */)
        {
            var N = filterOrder;
            var w = new float[N + 1];
            for (int n = 0; n <= N; n++)
            {
                w[n] = 0.54f - 0.46f * (float)Math.Cos(2 * Math.PI * n / N);
            }
            return w;
        }
    }
}
