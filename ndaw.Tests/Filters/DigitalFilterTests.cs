using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ndaw.Core.Filters;
using NSubstitute;
using ndaw.Core.Filters.FilterFunctions;

namespace ndaw.Core.Tests.Filters
{
    [TestClass]
    public class DigitalFilterTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Should_throw_if_lower_cutoff_frequency_is_negative()
        {
            var target = new DigitalFilter();

            target.LowerCutOffFrequency = -1f;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Should_throw_if_upper_cutoff_frequency_is_negative()
        {
            var target = new DigitalFilter();

            target.UpperCutOffFrequency = -1f;
        }

        [TestMethod]
        public void Should_correctly_store_state()
        {
            var target = new DigitalFilter();

            var expectedFilterFunction = Substitute.For<IFilterFunction>();

            target.LowerCutOffFrequency = 1f;
            target.UpperCutOffFrequency = 2f;
            target.FilterFunction = expectedFilterFunction;

            Assert.AreEqual(1f, target.LowerCutOffFrequency, FloatComparer.Epsilon);
            Assert.AreEqual(2f, target.UpperCutOffFrequency, FloatComparer.Epsilon);
            Assert.AreEqual(expectedFilterFunction, target.FilterFunction);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Should_throw_if_filter_function_is_null_when_calculating_coefficients()
        {
            var target = new DigitalFilter();

            target.FilterFunction = null;

            target.CalculateCoefficients(1, 2);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Should_throw_if_negative_filter_order_specified()
        {
            var target = new DigitalFilter();

            target.FilterFunction = Substitute.For<IFilterFunction>();

            target.CalculateCoefficients(-1, 2);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Should_throw_if_negative_sample_rate_specified()
        {
            var target = new DigitalFilter();

            target.FilterFunction = Substitute.For<IFilterFunction>();

            target.CalculateCoefficients(1, -2);
        }

        [TestMethod]
        public void Should_correctly_pass_parameters_to_filter_function()
        {
            var target = new DigitalFilter();

            target.FilterFunction = Substitute.For<IFilterFunction>();

            target.LowerCutOffFrequency = 2f;
            target.UpperCutOffFrequency = 5f;

            target.CalculateCoefficients(1, 3);

            target.FilterFunction.Received(1).CalculateCoefficients(1, 2f, 5f, 3);
        }

        [TestMethod]
        public void Should_return_the_result_of_filter_function()
        {
            var target = new DigitalFilter();

            var expected = new float[] { 1f, 2f, 3f, 4f, 5f };

            target.FilterFunction = Substitute.For<IFilterFunction>();
            target.FilterFunction.CalculateCoefficients(
                Arg.Any<int>(),
                Arg.Any<float>(),
                Arg.Any<float>(),
                Arg.Any<int>()).Returns(expected);

            target.LowerCutOffFrequency = 2f;
            target.UpperCutOffFrequency = 5f;

            var actual = target.CalculateCoefficients(1, 3);

            CollectionAssert.AreEqual(expected, actual, new FloatComparer());
        }

        [TestMethod]
        public void Should_raise_changed_event_when_lower_cutoff_frequency_changed()
        {
            var changed = false;

            var target = new DigitalFilter();
            target.Changed += (o, e) => { changed = true; };

            target.LowerCutOffFrequency = 7f;

            Assert.IsTrue(changed);
        }

        [TestMethod]
        public void Should_raise_changed_event_when_upper_cutoff_frequency_changed()
        {
            var changed = false;

            var target = new DigitalFilter();
            target.Changed += (o, e) => { changed = true; };

            target.UpperCutOffFrequency = 7f;

            Assert.IsTrue(changed);
        }
    }
}
