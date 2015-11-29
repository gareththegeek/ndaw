using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ndaw.Filters.WindowFunctions
{
    public interface IWindowFunction
    {
        float[] CalculateCoefficients(int filterOrder);
    }
}
