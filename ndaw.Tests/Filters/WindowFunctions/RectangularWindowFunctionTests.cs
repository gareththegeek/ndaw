using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ndaw.Core.Filters.WindowFunctions;

namespace ndaw.Core.Tests.Filters.WindowFunctions
{
    [TestClass]
    public class RectangularWindowFunctionTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Should_throw_if_filter_order_is_negative()
        {
            var target = new RectangularWindowFunction();

            target.CalculateCoefficients(-1);
        }

        [TestMethod]
        public void Should_correctly_calculate_window_function_coefficients()
        {
            var target = new RectangularWindowFunction();

            var expected = new float[]
            {
                1.0f,
                1.0f,
                1.0f,
                1.0f,
                1.0f,
                1.0f,
                1.0f,
                1.0f,
                1.0f,
                1.0f,
                1.0f
            };

            var actual = target.CalculateCoefficients(10);

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
