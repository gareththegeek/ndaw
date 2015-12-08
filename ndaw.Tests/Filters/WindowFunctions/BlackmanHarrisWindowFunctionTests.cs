using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ndaw.Core.Filters.WindowFunctions;

namespace ndaw.Core.Tests.Filters.WindowFunctions
{
    [TestClass]
    public class BlackmanHarrisWindowFunctionTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Should_throw_if_filter_order_is_negative()
        {
            var target = new BlackmanHarrisWindowFunction();

            target.CalculateCoefficients(-1);
        }

        [TestMethod]
        public void Should_correctly_calculate_window_function_coefficients()
        {
            var target = new BlackmanHarrisWindowFunction();

            var expected = new float[]
            {
                0.00006f,
                0.0109823309f,
                0.103011489f,
                0.385892659f,
                0.7938335f,
                1.0f,
                0.7938335f,
                0.385892659f,
                0.103011489f,
                0.0109823309f,
                0.00006f
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
