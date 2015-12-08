using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ndaw.Core.Filters.WindowFunctions;

namespace ndaw.Core.Tests.Filters.WindowFunctions
{
    [TestClass]
    public class BlackmanWindowFunctionTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Should_throw_if_filter_order_is_negative()
        {
            var target = new BlackmanWindowFunction();

            target.CalculateCoefficients(-1);
        }

        [TestMethod]
        public void Should_correctly_calculate_window_function_coefficients()
        {
            var target = new BlackmanWindowFunction();

            var expected = new float[]
            {
                -0.0000000149011612f,
                0.0402128436f,
                0.200770125f,
                0.509787142f,
                0.8492299f,
                1.0f,
                0.8492299f,
                0.509787142f,
                0.200770125f,
                0.0402128436f,
                -0.0000000149011612f
            };

            var actual = target.CalculateCoefficients(10);

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
