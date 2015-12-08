using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ndaw.Core.Filters.FilterFunctions;

namespace ndaw.Core.Tests.FilterFunctions
{
    [TestClass]
    public class HighPassFilterFunctionTests
    {
        [TestMethod]
        public void Should_not_throw_if_lower_cutoff_frequency_is_negative()
        {
            var target = new HighPassFilterFunction();

            target.CalculateCoefficients(1, -1f, 1f, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Should_throw_if_upper_cutoff_frequency_is_negative()
        {
            var target = new HighPassFilterFunction();

            target.CalculateCoefficients(1, 1f, -1f, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Should_throw_if_filter_order_is_negative()
        {
            var target = new HighPassFilterFunction();

            target.CalculateCoefficients(-1, 1f, 1f, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Should_throw_if_sample_rate_is_negative()
        {
            var target = new HighPassFilterFunction();

            target.CalculateCoefficients(1, 1f, 1f, -1);
        }

        [TestMethod]
        public void Should_correctly_calculate_coefficients_for_even_numbered_order()
        {
            var target = new HighPassFilterFunction();

            var expected = new float[]
            {
                -0.04161178f,
                -0.0429360829f,
                -0.0439831242f,
                -0.0447402224f,
                -0.0451981947f,
                0.954648554f,
                -0.0451981947f,
                -0.0447402224f,
                -0.0439831242f,
                -0.0429360829f,
                -0.04161178f
            };

            var actual = target.CalculateCoefficients(10, 0f, 1000f, 44100);

            Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i], 0.00001f);
            }
        }

        [TestMethod]
        public void Should_correctly_calculate_coefficients_for_odd_numbered_order()
        {
            var target = new HighPassFilterFunction();

            var expected = new float[]
            {
                -0.04161178f,
                -0.0429360829f,
                -0.0439831242f,
                -0.0447402224f,
                -0.0451981947f,
                0.954648554f,
                -0.0451981947f,
                -0.0447402224f,
                -0.0439831242f,
                -0.0429360829f,
                -0.04161178f,
                -0.0400261879f
            };

            var actual = target.CalculateCoefficients(11, 0f, 1000f, 44100);

            Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i], 0.00001f);
            }
        }
    }
}
