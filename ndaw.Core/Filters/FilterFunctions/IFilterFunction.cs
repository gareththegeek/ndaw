using System;

namespace ndaw.Core.Filters.FilterFunctions
{
    public interface IFilterFunction
    {
        float[] CalculateCoefficients(int filterOrder, float lowerCutoffFrequency, float upperCutoffFrequency, int sampleRate);
    }
}
