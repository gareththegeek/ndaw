using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ndaw.Core
{
    public static class Utility
    {
        public static float MillisecondsToSamples(float sampleRate, float ms)
        {
            return (ms / 1000f) * sampleRate;
        }
    }
}
