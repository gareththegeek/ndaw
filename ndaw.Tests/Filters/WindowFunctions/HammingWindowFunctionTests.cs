using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ndaw.Core.Filters.WindowFunctions;

namespace ndaw.Core.Tests.Filters.WindowFunctions
{
    [TestClass]
    public class HammingWindowFunctionTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Should_throw_if_filter_order_is_negative()
        {
            var target = new HammingWindowFunction();

            target.CalculateCoefficients(-1);
        }

        [TestMethod]
        public void Should_correctly_calculate_window_function_coefficients()
        {
            var target = new HammingWindowFunction();

            var expected = new float[]
            {
                0.08000001f,
                0.1678522f,
                0.3978522f,
                0.682147861f,
                0.9121478f,
                1.0f,
                0.9121478f,
                0.682147861f,
                0.3978522f,
                0.1678522f,
                0.08000001f
            };

            var actual = target.CalculateCoefficients(10);

            Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }
    }
}
