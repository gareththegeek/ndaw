using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ndaw.Core.Oscillators;
using NAudio.Wave;

namespace ndaw.Core.Tests.Oscillators
{
    [TestClass]
    public class SineWaveTests
    {
        private SineWave target;

        [TestInitialize]
        public void TestInitialise()
        {
            target = new SineWave();
            target.Format = new WaveFormat(44100, 2);
        }

        [TestMethod]
        public void Should_return_correct_name()
        {
            Assert.AreEqual("Sine Wave", target.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Should_throw_if_negative_frequency_used()
        {
            target.Frequency = -1f;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Should_throw_if_negative_amplitude_used()
        {
            target.Amplitude = -1f;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Should_throw_if_amplitude_greater_than_one_used()
        {
            target.Amplitude = 1.1f;
        }

        [TestMethod]
        public void Should_correctly_store_state()
        {
            var expectedFormat = new WaveFormat();

            target.Frequency = 3f;
            target.Amplitude = 0.7f;
            target.Format = expectedFormat;

            Assert.AreEqual(3f, target.Frequency, 0.00001f);
            Assert.AreEqual(0.7f, target.Amplitude, 0.00001f);
            Assert.AreEqual(expectedFormat, target.Format);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Should_throw_if_buffers_are_null()
        {
            target.Process(null, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Should_throw_if_any_buffer_is_null()
        {
            var buffers = new float[][] { new float[3], null };

            target.Process(buffers, 3);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Should_throw_if_format_is_null()
        {
            target.Format = null;

            var buffers = new float[][] { new float[1], new float[1] };

            target.Process(buffers, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Should_throw_if_number_of_channels_and_buffers_differ()
        {
            var buffers = new float[][] { new float[2], new float[2] };

            target.Format = new WaveFormat(44100, 1);

            target.Process(buffers, 2);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Should_throw_if_buffer_length_and_count_differ()
        {
            var buffers = new float[][] { new float[2], new float[1] };

            target.Format = new WaveFormat(44100, 2);

            target.Process(buffers, 2);
        }

        [TestMethod]
        public void Should_generate_correct_samples_with_respect_to_time()
        {
            var expected = new[]
            {
                // Sine wave at 0, 0.25, 0.5, 0.75, 0
                0f, 1f, 0f, -1f, 0f
            };

            var buffers = new float[][] { new float[5] };

            target.Format = new WaveFormat(400, 1);
            target.Frequency = 100f;
            target.Amplitude = 1f;

            target.Process(buffers, 5);

            CollectionAssert.AreEqual(expected, buffers[0], new FloatComparer());
        }

        [TestMethod]
        public void Should_multiply_output_by_amplitude()
        {
            var expected = new[]
            {
                // Sine wave at 0, 0.25, 0.5, 0.75, 0
                0f, 0.25f, 0f, -0.25f, 0f
            };

            var buffers = new float[][] { new float[5] };

            target.Format = new WaveFormat(400, 1);
            target.Frequency = 100f;
            target.Amplitude = 0.25f;

            target.Process(buffers, 5);

            CollectionAssert.AreEqual(expected, buffers[0], new FloatComparer());
        }

        [TestMethod]
        public void Should_output_on_all_channels()
        {
            var expected = new[]
            {
                // Sine wave at 0, 0.25, 0.5, 0.75, 0
                0f, 1f, 0f, -1f, 0f
            };

            var buffers = new float[][] { new float[5], new float[5] };

            target.Format = new WaveFormat(400, 2);
            target.Frequency = 100f;
            target.Amplitude = 1f;

            target.Process(buffers, 5);

            CollectionAssert.AreEqual(expected, buffers[0], new FloatComparer());
            CollectionAssert.AreEqual(expected, buffers[1], new FloatComparer());
        }
    }
}
