
namespace ndaw.Core.Filters.WindowFunctions
{
    public interface IWindowFunction
    {
        float[] CalculateCoefficients(int filterOrder);
    }
}
