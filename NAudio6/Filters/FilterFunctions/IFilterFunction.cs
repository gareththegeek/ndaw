using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ndaw.Filters.FilterFunctions
{
    public interface IFilterFunction
    {
        float[] CalculateCoefficients(int filterOrder, float lowerCutoffFrequency, float upperCutoffFrequency, int sampleRate);
    }
}
