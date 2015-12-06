using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ndaw.Core.Effects;
using NAudio.Wave;
using ndaw.Core.Oscillators;
using NSubstitute;

namespace ndaw.Core.Tests
{
    [TestClass]
    public class FlangerTests
    {
        private Flanger target;
        private IOscillator lfo;

        [TestInitialize]
        public void Initialise()
        {
            lfo = Substitute.For<IOscillator>();
            lfo.Generate(Arg.Any<int>()).Returns(0f);

            target = new Flanger(lfo);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Should_throw_if_no_lfo_provider_to_constructor()
        {
            target = new Flanger(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Should_throw_if_depth_is_negative()
        {
            target.Depth = -0.1f;
        }

        [TestMethod]
        public void Should_set_lfo_amplitude_equal_to_depth()
        {
            target.Depth = 0.4f;

            Assert.AreEqual(0.4f, lfo.Amplitude);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Should_throw_if_wet_is_negative()
        {
            target.Wet = -0.2f;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Should_throw_if_wet_is_greater_than_one()
        {
            target.Wet = 1.2f;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Should_throw_if_frequency_is_negative()
        {
            target.Frequency = -0.1f;
        }

        [TestMethod]
        public void Should_apply_frequency_to_lfo()
        {
            target.Frequency = 123f;

            Assert.AreEqual(123f, lfo.Frequency);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Should_throw_if_maximum_delay_is_negative()
        {
            target.MaximumDelay = -1f;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Should_throw_if_buffer_is_null()
        {
            target.Format = new WaveFormat();

            target.Process(null, 5);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Should_throw_if_any_channel_buffer_is_null()
        {
            target.Format = new WaveFormat();

            var buffer = new float[][] 
            {
                new [] { 1f, 2f, 3f },
                null,
                new [] { 1f, 2f, 3f }
            };

            target.Process(buffer, 3);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Should_throw_if_format_is_null()
        {
            var buffer = new float[][] { new [] { 1f } };

            target.Process(buffer, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Should_throw_if_there_is_not_one_buffer_per_channel()
        {
            target.Format = new WaveFormat(44100, 2);

            var buffers = new float[][] { new[] { 1f } };

            target.Process(buffers, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Should_throw_if_buffer_length_is_less_than_count()
        {
            target.Format = new WaveFormat(44100, 1);

            var buffers = new float[][] { new[] { 1f } };

            target.Process(buffers, 2);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Should_throw_if_count_is_negative()
        {
            target.Format = new WaveFormat(44100, 1);

            var buffers = new float[][] { new[] { 1f } };

            target.Process(buffers, -1);
        }

        [TestMethod]
        public void Should_not_modify_signal_if_wet_is_zero()
        {
            target.Format = new WaveFormat(44100, 1);
            target.Wet = 0f;

            var signal = new [] { 1f, 2f, 3f, 4f, 5f };

            target.Process(new float[][] { signal }, signal.Length);
            target.Process(new float[][] { signal }, signal.Length);
            target.Process(new float[][] { signal }, signal.Length);
            target.Process(new float[][] { signal }, signal.Length);

            Assert.AreEqual(1f, signal[0]);
            Assert.AreEqual(2f, signal[1]);
            Assert.AreEqual(3f, signal[2]);
            Assert.AreEqual(4f, signal[3]);
            Assert.AreEqual(5f, signal[4]);
        }

        [TestMethod]
        public void Should_correctly_handle_multiple_channels()
        {
            target.Format = new WaveFormat(44100, 3);
            target.Wet = 0f;

            var signal0 = new[] { 1f, 2f, 3f, 4f, 5f };
            var signal1 = new[] { 6f, 7f, 8f, 9f, 10f };
            var signal2 = new[] { 11f, 12f, 13f, 14f, 15f };

            target.Process(new float[][] { signal0, signal1, signal2 }, 5);
            target.Process(new float[][] { signal0, signal1, signal2 }, 5);
            target.Process(new float[][] { signal0, signal1, signal2 }, 5);
            target.Process(new float[][] { signal0, signal1, signal2 }, 5);

            Assert.AreEqual(1f, signal0[0]);
            Assert.AreEqual(2f, signal0[1]);
            Assert.AreEqual(3f, signal0[2]);
            Assert.AreEqual(4f, signal0[3]);
            Assert.AreEqual(5f, signal0[4]);

            Assert.AreEqual(6f, signal1[0]);
            Assert.AreEqual(7f, signal1[1]);
            Assert.AreEqual(8f, signal1[2]);
            Assert.AreEqual(9f, signal1[3]);
            Assert.AreEqual(10f, signal1[4]);

            Assert.AreEqual(11f, signal2[0]);
            Assert.AreEqual(12f, signal2[1]);
            Assert.AreEqual(13f, signal2[2]);
            Assert.AreEqual(14f, signal2[3]);
            Assert.AreEqual(15f, signal2[4]);
        }

        [TestMethod]
        public void Should_pass_incremental_time_to_the_lfo_unit()
        {
            target.Format = new WaveFormat(44100, 1);
            var buffers = new float[][] { new[] { 1f, 2f } };

            target.Process(buffers, 2);

            lfo.Received(1).Generate(Arg.Is<int>(0));
            lfo.Received(1).Generate(Arg.Is<int>(1));

            target.Process(buffers, 2);

            lfo.Received(1).Generate(Arg.Is<int>(2));
            lfo.Received(1).Generate(Arg.Is<int>(3));

            target.Process(buffers, 2);

            lfo.Received(1).Generate(Arg.Is<int>(4));
            lfo.Received(1).Generate(Arg.Is<int>(5));
        }

        [TestMethod]
        public void Should_to_current_sample_the_past_sample_at_position_defined_by_lfo_plus_one_times_max_delay_over_two()
        {
            target.Format = new WaveFormat(1, 1);
            target.Wet = 0.5f;

            // Set max delay to 2 so that max delay over two equals one
            target.MaximumDelay = 2000;

            lfo.Generate(Arg.Any<int>()).Returns(-2f);

            var buffers = new float[][] { new[] { 1f, 2f, 3f } };

            target.Process(buffers, 3);

            buffers = new float[][] { new[] { 4f, 5f, 6f } };

            target.Process(buffers, 3);

            Assert.AreEqual((4f + 3f) / 2, buffers[0][0], 0.00001f);
            Assert.AreEqual((5f + 4f) / 2, buffers[0][1], 0.00001f);
            Assert.AreEqual((6f + 5f) / 2, buffers[0][2], 0.00001f);
        }

        [TestMethod]
        public void Should_return_the_name_Flanger()
        {
            Assert.AreEqual("Flanger", target.Name);
        }

        [TestMethod]
        public void Should_correctly_store_settings()
        {
            target.Depth = 1f;
            target.Wet = 0.5f;
            target.Frequency = 100f;
            target.MaximumDelay = 12f;
            var expectedFormat = new WaveFormat();
            target.Format = expectedFormat;

            Assert.AreEqual(1f, target.Depth);
            Assert.AreEqual(0.5f, target.Wet);
            Assert.AreEqual(100f, target.Frequency);
            Assert.AreEqual(12f, target.MaximumDelay);
            Assert.AreEqual(expectedFormat, target.Format);
        }
    }
}
