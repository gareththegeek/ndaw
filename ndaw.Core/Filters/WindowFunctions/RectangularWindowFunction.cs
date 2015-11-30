
namespace ndaw.Core.Filters.WindowFunctions
{
    public class RectangularWindowFunction: IWindowFunction
    {
        public float[] CalculateCoefficients(int filterOrder /* Number of coefficients */)
        {
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
