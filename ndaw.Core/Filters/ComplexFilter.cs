using NAudio.Wave;
using ndaw.Core.Filters.Implementations;
using ndaw.Core.Filters.WindowFunctions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace ndaw.Core.Filters
{
    public class ComplexFilter
    {
        private ObservableCollection<DigitalFilter> filters;
        public ICollection<DigitalFilter> Filters
        {
            get { return filters; }
        }

        private int filterOrder;
        public int FilterOrder
        {
            get { return filterOrder; }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException("value", "Filter order must be greater than zero");
                }

                if (value % 2 != 0)
                {
                    throw new ArgumentOutOfRangeException("value", "Filter order must be an even number");
                }

                filterOrder = value;
                updateCoefficients();
            }
        }

        public IFilterImplementation FilterImplementation { get; private set; }

        private IWindowFunction windowFunction;
        public IWindowFunction WindowFunction 
        {
            get { return windowFunction; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value", "Window function cannot be null");
                }

                windowFunction = value;
                updateCoefficients();
            }
        }

        private WaveFormat format;

        public ComplexFilter(
            WaveFormat format, 
            IWindowFunction windowFunction, 
            IFilterImplementation filterImplementation)
        {
            if (format == null)
            {
                throw new ArgumentNullException("format", "Format cannot be null");
            }

            if (windowFunction == null)
            {
                throw new ArgumentNullException("windowFunction", "Window function cannot be null");
            }

            if (filterImplementation == null)
            {
                throw new ArgumentNullException("filterImplementation", "Filter implementation cannot be null");
            }

            this.format = format;
            this.filterOrder = 20;
            this.windowFunction = windowFunction;
            this.FilterImplementation = filterImplementation;

            this.filters = new ObservableCollection<DigitalFilter>();
            this.filters.CollectionChanged += filters_CollectionChanged;

            updateCoefficients();
        }

        private void filters_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            updateCoefficients();
        }

        private void updateCoefficients()
        {
            var windowFunctionCoefficients = 
                windowFunction.CalculateCoefficients(
                    filterOrder);

            if (windowFunctionCoefficients.Length != filterOrder + 1)
            {
                throw new InvalidOperationException("Window function must return filter order plus one coefficients");
            }

            var filterFunctionCoefficients = Enumerable.Repeat(1f, filterOrder + 1).ToArray();
            foreach (var filter in filters)
            {
                var newCoefficients =
                    filter.CalculateCoefficients(
                        filterOrder,
                        format.SampleRate);

                for (int n = 0; n < filterOrder + 1; n++)
                {
                    filterFunctionCoefficients[n] *= newCoefficients[n];
                }
            }

            var coefficients = new float[filterOrder + 1];
            for (var n = 0; n < filterOrder + 1; n++)
            {
                coefficients[n] = windowFunctionCoefficients[n] * filterFunctionCoefficients[n];
            }

            FilterImplementation.Coefficients = coefficients;
        } 
    }
}
