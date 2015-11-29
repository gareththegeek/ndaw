using ndaw.Filters.FilterFunctions;
using System;

namespace ndaw.Filters
{
    public class DigitalFilter
    {
        private float lowerCutoffFrequency;
        public float LowerCutOffFrequency
        {
            get { return lowerCutoffFrequency; }
            set
            {
                if (value <= 0f)
                {
                    throw new ArgumentOutOfRangeException("value", "Lower cut off frequency must be greater than zero");
                }

                lowerCutoffFrequency = value;
            }
        }

        private float upperCutoffFrequency;
        public float UpperCutOffFrequency
        {
            get { return upperCutoffFrequency; }
            set
            {
                if (value <= 0f)
                {
                    throw new ArgumentOutOfRangeException("value", "Upper cut off frequency must be greater than zero");
                }

                upperCutoffFrequency = value;
            }
        }

        private IFilterFunction filterFunction;
        public IFilterFunction FilterFunction
        {
            get { return filterFunction; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value", "Filter function cannot be null");
                }

                filterFunction = value;
            }
        }

        public float[] CalculateCoefficients(int filterOrder, int sampleRate)
        {
            if (filterFunction == null)
            {
                throw new NotSupportedException("Must have a filter function in order to calculate coefficients");
            }

            return filterFunction.CalculateCoefficients(
                filterOrder,
                lowerCutoffFrequency,
                upperCutoffFrequency,
                sampleRate);
        }
    }
}
