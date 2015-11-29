using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ndaw.Filters.WindowFunctions
{
    public class BlackmanHarrisWindowFunction : IWindowFunction
    {
        public float[] CalculateCoefficients(int filterOrder /* Number of coefficients */)
        {
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
