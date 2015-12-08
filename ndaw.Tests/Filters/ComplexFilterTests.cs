using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ndaw.Core.Filters;
using NSubstitute;
using ndaw.Core.Filters.WindowFunctions;
using ndaw.Core.Filters.Implementations;
using NAudio.Wave;

namespace ndaw.Core.Tests.Filters
{
    [TestClass]
    public class ComplexFilterTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Should_throw_if_no_format_specified()
        {
            var target = new ComplexFilter(
                null,
                Substitute.For<IWindowFunction>(),
                Substitute.For<IFilterImplementation>());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Should_throw_if_no_window_function_specified()
        {
            var target = new ComplexFilter(
                new WaveFormat(),
                null,
                Substitute.For<IFilterImplementation>());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Should_throw_if_no_implementation_specified()
        {
            var target = new ComplexFilter(
                new WaveFormat(),
                Substitute.For<IWindowFunction>(),
                null);
        }

        [TestMethod]
        public void Should_apply_window_function_to_coefficients_on_instantiation()
        {
            var windowFunction = Substitute.For<IWindowFunction>();
            var implementation = Substitute.For<IFilterImplementation>();

            var expected = new[] 
            { 
                1f, 2f, 3f, 4f, 5f,
                6f, 7f, 8f, 9f, 10f,
                11f,
                12f, 13f, 14f, 15f, 16f,
                17f, 18f, 19f, 20f, 21f
            };

            windowFunction.CalculateCoefficients(Arg.Is<int>(20)).Returns(expected);

            var target = new ComplexFilter(new WaveFormat(), windowFunction, implementation);

            CollectionAssert.AreEqual(expected, implementation.Coefficients);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Should_throw_if_window_function_returns_the_wrong_number_of_coefficients()
        {
            var windowFunction = Substitute.For<IWindowFunction>();
            windowFunction.CalculateCoefficients(Arg.Any<int>())
                .Returns(new float[] { });

            var target = new ComplexFilter(
                new WaveFormat(),
                windowFunction,
                Substitute.For<IFilterImplementation>());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Should_throw_if_negative_filter_order_specified()
        {
            var windowFunction = Substitute.For<IWindowFunction>();
            windowFunction.CalculateCoefficients(Arg.Any<int>())
                .Returns(new float[21]);

            var target = new ComplexFilter(
                new WaveFormat(),
                windowFunction,
                Substitute.For<IFilterImplementation>());

            target.FilterOrder = -1;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Should_throw_if_odd_filter_order_specified()
        {
            var windowFunction = Substitute.For<IWindowFunction>();
            windowFunction.CalculateCoefficients(Arg.Any<int>())
                .Returns(new float[21]);

            var target = new ComplexFilter(
                new WaveFormat(),
                windowFunction,
                Substitute.For<IFilterImplementation>());

            target.FilterOrder = 3;
        }

        [TestMethod]
        public void Should_update_implementation_coefficients_when_filter_order_modified()
        {
            var windowFunction = Substitute.For<IWindowFunction>();
            windowFunction.CalculateCoefficients(Arg.Any<int>())
                .Returns(new float[21]);

            var implementation = Substitute.For<IFilterImplementation>();

            var target = new ComplexFilter(
                new WaveFormat(),
                windowFunction,
                implementation);

            var expected = new float[] { 1f, 2f, 3f, 4f, 5f };

            windowFunction.CalculateCoefficients(Arg.Any<int>())
                .Returns(expected);

            target.FilterOrder = 4;

            CollectionAssert.AreEqual(expected, implementation.Coefficients);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Should_throw_if_window_function_set_to_null()
        {
            var windowFunction = Substitute.For<IWindowFunction>();
            windowFunction.CalculateCoefficients(Arg.Any<int>())
                .Returns(new float[21]);

            var target = new ComplexFilter(
                new WaveFormat(),
                windowFunction,
                Substitute.For<IFilterImplementation>());

            target.WindowFunction = null;
        }

        [TestMethod]
        public void Should_update_implementation_coefficients_when_window_function_modified()
        {
            var windowFunction = Substitute.For<IWindowFunction>();
            windowFunction.CalculateCoefficients(Arg.Any<int>())
                .Returns(new float[21]);

            var implementation = Substitute.For<IFilterImplementation>();

            var target = new ComplexFilter(
                new WaveFormat(),
                windowFunction,
                implementation);

            var expected = new float[]
            {
                1f, 1f, 1f, 1f, 1f,
                2f, 2f, 2f, 2f, 2f,
                3f, 3f, 3f, 3f, 3f,
                4f, 4f, 4f, 4f, 4f,
                5f
            };

            var newWindowFunction = Substitute.For<IWindowFunction>();
            newWindowFunction.CalculateCoefficients(Arg.Any<int>())
                .Returns(expected);

            target.WindowFunction = newWindowFunction;

            CollectionAssert.AreEqual(expected, implementation.Coefficients);
        }


    }
}
