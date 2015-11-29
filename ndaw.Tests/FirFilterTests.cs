using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ndaw.Filters.Implementations;
using NAudio.Wave;

namespace ndaw.Tests
{
    [TestClass]
    public class FirFilterTests
    {
        private FirFilter target;

        [TestInitialize]
        public void Initialise()
        {
            var format = new WaveFormat(44100, 1);
            target = new FirFilter();
            target.Format = format;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Should_throw_if_buffer_is_null()
        {
            target.Coefficients = new[] { 1f };
            target.Process(null, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Should_throw_if_count_exceeds_buffer_length()
        {
            target.Coefficients = new[] { 1f };
            var buffer = new float[][] { new[] { 1f } };
            target.Process(buffer, 2);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Should_throw_if_count_is_negative()
        {
            target.Coefficients = new[] { 1f };
            var buffer = new float[][] { new[] { 1f } };
            target.Process(buffer, -1);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Should_throw_if_coefficients_is_null()
        {
            var buffer = new float[][] { new[] { 1f } };
            target.Process(buffer, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Should_throw_if_format_is_null()
        {
            target.Coefficients = new[] { 1f };
            var buffer = new float[][] { new[] { 1f } };

            target.Format = null;

            target.Process(buffer, 1);
        }

        [TestMethod]
        public void Should_record_product_of_sample_and_coefficient_to_buffer()
        {
            target.Coefficients = new[] { 0.3f };
            var input = new float[][] { new [] { 10f, 20f, 30f, 40f } };
            target.Process(input, 4);

            Assert.AreEqual(1, input.Length);
            Assert.AreEqual(4, input[0].Length);
            Assert.AreEqual(3f, input[0][0]);
            Assert.AreEqual(6f, input[0][1]);
            Assert.AreEqual(9f, input[0][2]);
            Assert.AreEqual(12f, input[0][3]);
        }

        [TestMethod]
        public void Should_sum_the_product_of_weight_and_sample()
        {
            target.Coefficients = new[] { 0.3f, 0.2f, 0.1f };
            var buffer = new float[][] { new[] { 5f, 7f, 11f, 13f } };

            target.Process(buffer, 4);

            Assert.AreEqual(1, buffer.Length);
            Assert.AreEqual(4, buffer[0].Length);
            Assert.AreEqual(0.3f * 5f, buffer[0][0], 0.00001);
            Assert.AreEqual(0.3f * 7f + 0.2f * 5f, buffer[0][1], 0.00001);
            Assert.AreEqual(0.3f * 11f + 0.2f * 7f + 0.1f * 5f, buffer[0][2], 0.00001);
            Assert.AreEqual(0.3f * 13f + 0.2f * 11f + 0.1f * 7f, buffer[0][3], 0.00001);
        }

        [TestMethod]
        public void Should_preserve_historic_values()
        {
            target.Coefficients = new[] { 1f, 0.5f };

            var buffer = new float[][] { new[] { 1f } };
            target.Process(buffer, 1);

            buffer = new float[][] { new[] { 2f } };
            target.Process(buffer, 1);

            Assert.AreEqual(1, buffer.Length);
            Assert.AreEqual(1, buffer[0].Length);
            Assert.AreEqual(2f + 0.5f * 1f, buffer[0][0], 0.0001);
        }

        [TestMethod]
        public void Should_maintain_separation_of_channels()
        {
            var format = new WaveFormat(44100, 2);
            target = new FirFilter();
            target.Format = format;

            target.Coefficients = new[] { 0.3f, 0.2f, 0.1f };
            var buffers = new float[][]
            {
                new[] { 5f, 11f },
                new [] { 7f, 13f }
            };

            target.Process(buffers, 2);

            Assert.AreEqual(2, buffers.Length);
            Assert.AreEqual(2, buffers[0].Length);
            Assert.AreEqual(2, buffers[1].Length);

            Assert.AreEqual(0.3f * 5f, buffers[0][0], 0.00001);
            Assert.AreEqual(0.3f * 11f + 0.2f * 5f, buffers[0][1], 0.00001);

            Assert.AreEqual(0.3f * 7f, buffers[1][0], 0.00001);
            Assert.AreEqual(0.3f * 13f + 0.2f * 7f, buffers[1][1], 0.00001);
        }

        [TestMethod]
        public void Should_preserve_historic_values_with_multiple_channels()
        {
            var format = new WaveFormat(44100, 2);
            target = new FirFilter();
            target.Format = format;

            target.Coefficients = new[] { 1f, 0.5f };

            var buffers = new float[][]
            {
                new[] { 1f, 3f },
                new[] { 2f, 4f }
            };
            target.Process(buffers, 2);

            buffers = new float[][]
            {
                new[] { 5f, 7f },
                new[] { 6f, 8f }
            };
            target.Process(buffers, 2);

            Assert.AreEqual(5f + 0.5f * 3f, buffers[0][0], 0.0001);
            Assert.AreEqual(7f + 0.5f * 5f, buffers[0][1], 0.0001);
            Assert.AreEqual(6f + 0.5f * 4f, buffers[1][0], 0.0001);
            Assert.AreEqual(8f + 0.5f * 6f, buffers[1][1], 0.0001);
        }
    }
}
