using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ndaw.Core.Filters;
using NSubstitute;
using ndaw.Core.Filters.WindowFunctions;
using ndaw.Core.Filters.Implementations;
using NAudio.Wave;
using ndaw.Core.Filters.FilterFunctions;

namespace ndaw.Core.Tests.Filters
{
    [TestClass]
    public class ComplexFilterTests
    {
        private ComplexFilter target;

        private WaveFormat format;
        private IWindowFunction windowFunction;
        private IFilterImplementation implementation;

        [TestInitialize]
        public void TestInitialise()
        {
            format = new WaveFormat(44100, 2);
            windowFunction = Substitute.For<IWindowFunction>();
            implementation = Substitute.For<IFilterImplementation>();

            windowFunction
                .CalculateCoefficients(Arg.Any<int>())
                .Returns(new float[21]);

            target = new ComplexFilter(format, windowFunction, implementation);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Should_throw_if_no_format_specified()
        {
            target = new ComplexFilter(
                null,
                Substitute.For<IWindowFunction>(),
                Substitute.For<IFilterImplementation>());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Should_throw_if_no_window_function_specified()
        {
            target = new ComplexFilter(
                new WaveFormat(),
                null,
                Substitute.For<IFilterImplementation>());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Should_throw_if_no_implementation_specified()
        {
            target = new ComplexFilter(
                new WaveFormat(),
                Substitute.For<IWindowFunction>(),
                null);
        }

        [TestMethod]
        public void Should_apply_window_function_to_coefficients_on_instantiation()
        {
            var expected = new[] 
            { 
                1f, 2f, 3f, 4f, 5f,
                6f, 7f, 8f, 9f, 10f,
                11f,
                12f, 13f, 14f, 15f, 16f,
                17f, 18f, 19f, 20f, 21f
            };

            windowFunction.CalculateCoefficients(Arg.Is<int>(20)).Returns(expected);

            target = new ComplexFilter(new WaveFormat(), windowFunction, implementation);

            CollectionAssert.AreEqual(expected, implementation.Coefficients, new FloatComparer());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Should_throw_if_window_function_returns_the_wrong_number_of_coefficients()
        {
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
            target.FilterOrder = -1;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Should_throw_if_odd_filter_order_specified()
        {
            target.FilterOrder = 3;
        }

        [TestMethod]
        public void Should_update_implementation_coefficients_when_filter_order_modified()
        {
            var expected = new float[] { 1f, 2f, 3f, 4f, 5f };

            windowFunction.CalculateCoefficients(Arg.Any<int>())
                .Returns(expected);

            target.FilterOrder = 4;

            CollectionAssert.AreEqual(expected, implementation.Coefficients, new FloatComparer());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Should_throw_if_window_function_set_to_null()
        {
            target.WindowFunction = null;
        }

        [TestMethod]
        public void Should_update_implementation_coefficients_when_window_function_modified()
        {
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

            CollectionAssert.AreEqual(expected, implementation.Coefficients, new FloatComparer());
        }

        [TestMethod]
        public void Should_correctly_store_state()
        {
            windowFunction.CalculateCoefficients(Arg.Any<int>())
                .Returns(new float[5]);

            var expectedFunction = Substitute.For<IWindowFunction>();
            expectedFunction.CalculateCoefficients(Arg.Any<int>())
                .Returns(new float[5]);

            target.FilterOrder = 4;
            target.WindowFunction = expectedFunction;

            Assert.AreEqual(4, target.FilterOrder);
            Assert.AreEqual(expectedFunction, target.WindowFunction);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Should_throw_if_filter_returns_incorrect_number_of_coefficients()
        {
            var filter = Substitute.For<IDigitalFilter>();
            filter
                .CalculateCoefficients(Arg.Any<int>(), Arg.Any<int>())
                .Returns(new float[7]);

            target.Filters.Add(filter);
        }

        [TestMethod]
        public void Should_update_implementation_coefficients_when_filter_collection_changes()
        {
            var windowFunctionCoefficients = new[] 
            {
                1f, 2f, 3f, 4f, 5f,
                1f, 2f, 3f, 4f, 5f,
                1f, 2f, 3f, 4f, 5f,
                1f, 2f, 3f, 4f, 5f,
                1f
            };

            var filterFunctionCoefficients = new[]
            {
                7f, 5f, 2f, 7f, 5f,
                2f, 7f, 5f, 2f, 7f,
                5f, 2f, 7f, 5f, 2f,
                7f, 5f, 2f, 7f, 5f,
                2f
            };

            var expected = new[]
            {
                // Product of filter and window functions
                7f, 10f, 6f, 28f, 25f,
                2f, 14f, 15f, 8f, 35f,
                5f, 4f, 21f, 20f, 10f,
                7f, 10f, 6f, 28f, 25f,
                2f
            };

            windowFunction.CalculateCoefficients(Arg.Any<int>())
                .Returns(windowFunctionCoefficients);

            var filter = Substitute.For<IDigitalFilter>();
            filter
                .CalculateCoefficients(Arg.Is<int>(20), Arg.Is<int>(44100))
                .Returns(filterFunctionCoefficients);

            target = new ComplexFilter(
                new WaveFormat(44100, 2),
                windowFunction,
                implementation);

            target.Filters.Add(filter);

            CollectionAssert.AreEqual(expected, implementation.Coefficients, new FloatComparer());
        }

        [TestMethod]
        public void Should_multiply_coefficients_from_all_filters_together()
        {
            windowFunction.CalculateCoefficients(Arg.Any<int>())
                .Returns(new[] { 1f, 1f, 1f });

            var filter1 = Substitute.For<IDigitalFilter>();
            var filter2 = Substitute.For<IDigitalFilter>();
            var filter3 = Substitute.For<IDigitalFilter>();

            filter1.CalculateCoefficients(Arg.Any<int>(), Arg.Any<int>())
                .Returns(new[] { 2f, 3f, 4f });

            filter2.CalculateCoefficients(Arg.Any<int>(), Arg.Any<int>())
                .Returns(new[] { 3f, 5f, 7f });

            filter3.CalculateCoefficients(Arg.Any<int>(), Arg.Any<int>())
                .Returns(new[] { 3f, 4f, 5f });

            var expected = new[]
            {
                18f, 60f, 140f
            };

            target.FilterOrder = 2;

            target.Filters.Add(filter1);
            target.Filters.Add(filter2);
            target.Filters.Add(filter3);

            CollectionAssert.AreEqual(expected, implementation.Coefficients, new FloatComparer());
        }

        [TestMethod]
        public void Should_update_implementation_coefficients_when_filter_changed_event_fires()
        {
            windowFunction.CalculateCoefficients(Arg.Any<int>())
                .Returns(new []
                {
                    1f, 1f, 1f, 1f, 1f
                });

            var expected = new []
            {
                1f, 2f, 3f, 4f, 5f
            };

            var filter = Substitute.For<IDigitalFilter>();
            filter.CalculateCoefficients(Arg.Any<int>(), Arg.Any<int>())
                .Returns(new float[5]);

            target.FilterOrder = 4;

            target.Filters.Add(filter);

            filter.CalculateCoefficients(Arg.Any<int>(), Arg.Any<int>())
                .Returns(expected);

            filter.Changed += Raise.EventWith(filter, new EventArgs());

            CollectionAssert.AreEqual(expected, implementation.Coefficients, new FloatComparer());
        }
    }
}
