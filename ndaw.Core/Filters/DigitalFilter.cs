using ndaw.Core.Filters.FilterFunctions;
using System;

namespace ndaw.Core.Filters
{
    public class DigitalFilter: IDigitalFilter
    {
        public event EventHandler<EventArgs> Changed;

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
                if (Changed != null)
                {
                    Changed.Invoke(this, new EventArgs());
                }
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
                if (Changed != null)
                {
                    Changed.Invoke(this, new EventArgs());
                }
            }
        }

        private IFilterFunction filterFunction;
        public IFilterFunction FilterFunction
        {
            get { return filterFunction; }
            set
            {
                filterFunction = value;
            }
        }

        public float[] CalculateCoefficients(int filterOrder, int sampleRate)
        {
            if (filterFunction == null)
            {
                throw new InvalidOperationException("Must have a filter function in order to calculate coefficients");
            }
            if (filterOrder <= 0)
            {
                throw new ArgumentOutOfRangeException("filterOrder", "Filter order must be a positive value");
            }
            if (sampleRate <= 0)
            {
                throw new ArgumentOutOfRangeException("sampleRate", "Sample rate must be a positive value");
            }

            return filterFunction.CalculateCoefficients(
                filterOrder,
                lowerCutoffFrequency,
                upperCutoffFrequency,
                sampleRate);
        }
    }
}
