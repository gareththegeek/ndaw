using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ndaw.Core.Filters.FilterFunctions;

namespace ndaw.Core.Tests.FilterFunctions
{
    [TestClass]
    public class LowPassFilterFunctionTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Should_throw_if_lower_cutoff_frequency_is_negative()
        {
            var target = new LowPassFilterFunction();

            target.CalculateCoefficients(1, -1f, 1f, 1);
        }

        [TestMethod]
        public void Should_not_throw_if_upper_cutoff_frequency_is_negative()
        {
            var target = new LowPassFilterFunction();

            target.CalculateCoefficients(1, 1f, -1f, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Should_throw_if_filter_order_is_negative()
        {
            var target = new LowPassFilterFunction();

            target.CalculateCoefficients(-1, 1f, 1f, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Should_throw_if_sample_rate_is_negative()
        {
            var target = new LowPassFilterFunction();

            target.CalculateCoefficients(1, 1f, 1f, -1);
        }

        [TestMethod]
        public void Should_correctly_calculate_coefficients_for_even_numbered_order()
        {
            var target = new LowPassFilterFunction();

            var expected = new float[]
            {
                0.00453131273f,
                0.004532693f,
                0.004533767f,
                0.00453453371f,
                0.004534994f,
                0.00453514745f,
                0.004534994f,
                0.00453453371f,
                0.004533767f,
                0.004532693f,
                0.00453131273f
            };

            var actual = target.CalculateCoefficients(10, 100f, 0f, 44100);

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Should_correctly_calculate_coefficients_for_odd_numbered_order()
        {
            var target = new LowPassFilterFunction();

            var expected = new float[]
            {
                0.00453131273f,
                0.004532693f,
                0.004533767f,
                0.00453453371f,
                0.004534994f,
                0.00453514745f,
                0.004534994f,
                0.00453453371f,
                0.004533767f,
                0.004532693f,
                0.00453131273f,
                0.00452962564f
            };

            var actual = target.CalculateCoefficients(11, 100f, 0f, 44100);

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
