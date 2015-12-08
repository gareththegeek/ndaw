using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ndaw.Core.Filters.FilterFunctions;

namespace ndaw.Core.Tests.FilterFunctions
{
    [TestClass]
    public class BandStopFilterFunctionTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Should_throw_if_lower_cutoff_frequency_is_negative()
        {
            var target = new BandStopFilterFunction();

            target.CalculateCoefficients(1, -1f, 1f, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Should_throw_if_upper_cutoff_frequency_is_negative()
        {
            var target = new BandStopFilterFunction();

            target.CalculateCoefficients(1, 1f, -1f, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Should_throw_if_filter_order_is_negative()
        {
            var target = new BandStopFilterFunction();

            target.CalculateCoefficients(-1, 1f, 1f, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Should_throw_if_sample_rate_is_negative()
        {
            var target = new BandStopFilterFunction();

            target.CalculateCoefficients(1, 1f, 1f, -1);
        }

        [TestMethod]
        public void Should_correctly_calculate_coefficients_for_even_numbered_order()
        {
            var target = new BandStopFilterFunction();

            var expected = new float[]
            {
                -0.0370804667f,
                -0.03840339f,
                -0.0394493565f,
                -0.04020569f,
                -0.0406632f,
                0.9591837f,
                -0.0406632f,
                -0.04020569f,
                -0.0394493565f,
                -0.03840339f,
                -0.0370804667f
            };

            var actual = target.CalculateCoefficients(10, 100f, 1000f, 44100);

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Should_correctly_calculate_coefficients_for_odd_numbered_order()
        {
            var target = new BandStopFilterFunction();

            var expected = new float[]
            {
                -0.0370804667f,
                -0.03840339f,
                -0.0394493565f,
                -0.04020569f,
                -0.0406632f,
                0.9591837f,
                -0.0406632f,
                -0.04020569f,
                -0.0394493565f,
                -0.03840339f,
                -0.0370804667f,
                -0.0354965627f
            };

            var actual = target.CalculateCoefficients(11, 100f, 1000f, 44100);

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
