using ndaw.Core.Filters.FilterFunctions;
using System;

namespace ndaw.Core.Filters
{
    public interface IDigitalFilter
    {
        float LowerCutOffFrequency { get; set; }
        float UpperCutOffFrequency { get; set; }
        IFilterFunction FilterFunction { get; set; }
        float[] CalculateCoefficients(int filterOrder, int sampleRate);

        event EventHandler<EventArgs> Changed;
    }
}
