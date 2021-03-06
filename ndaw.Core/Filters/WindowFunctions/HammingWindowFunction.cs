﻿using System;

namespace ndaw.Core.Filters.WindowFunctions
{
    public class HammingWindowFunction : IWindowFunction
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
                w[n] = 0.54f - 0.46f * (float)Math.Cos(2 * Math.PI * n / N);
            }
            return w;
        }
    }
}
