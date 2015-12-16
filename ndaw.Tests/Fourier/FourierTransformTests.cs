using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ndaw.Core.Fourier;
using NSubstitute;
using NAudio.Wave;
using NAudio.Dsp;
using ndaw.Core.Filters.WindowFunctions;

namespace ndaw.Core.Tests.Fourier
{
    [TestClass]
    public class FourierTransformTests
    {
        private IFastFourierTransformProvider transformProvider;
        private IWindowFunction windowFunction;

        private FourierTransform target;

        [TestInitialize]
        public void TestInitialise()
        {
            var ones = new float[100];
            for (int i = 0; i < 100; i++)
            {
                ones[i] = 1f;
            }

            transformProvider = Substitute.For<IFastFourierTransformProvider>();
            windowFunction = Substitute.For<IWindowFunction>();
            windowFunction.CalculateCoefficients(Arg.Any<int>()).Returns(ones);

            target = new FourierTransform(transformProvider, windowFunction, 1);

            target.Format = new WaveFormat(44100, 1);
        }

        [TestMethod]
        public void Should_return_correct_name()
        {
            Assert.AreEqual("Fourier Transform", target.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Should_throw_if_transform_length_is_not_a_power_of_two()
        {
            target = new FourierTransform(transformProvider, windowFunction, 3);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Should_throw_if_transform_provider_is_null()
        {
            target = new FourierTransform(null, windowFunction, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Should_throw_if_window_function_is_null()
        {
            target = new FourierTransform(transformProvider, null, 1);
        }

        [TestMethod]
        public void Should_correctly_store_state()
        {
            var expected = new WaveFormat(44100, 1);

            target.Format = expected;

            Assert.AreEqual(expected, target.Format);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Should_throw_if_no_buffer_provided()
        {
            target.Process(null, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Should_throw_if_a_channel_buffer_is_null()
        {
            var buffers = new float[][] { new float[1], null };

            target.Process(buffers, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Should_throw_if_format_is_null()
        {
            target.Format = null;

            target.Process(new float[][] { new float[1] }, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Should_throw_if_number_of_buffers_and_channels_differs()
        {
            var buffers = new float[][] { new float[1], new float[1] };

            target.Format = new WaveFormat(44100, 1);

            target.Process(buffers, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Should_throw_if_count_and_buffer_length_differ()
        {
            var buffers = new float[][] { new float[4] };

            target.Format = new WaveFormat(44100, 1);

            target.Process(buffers, 5);
        }

        [TestMethod]
        public void Should_pass_samples_to_FFT_implementation_as_real_component()
        {
            var buffers = new float[][]{ new []
            {
                1f, 2f, 3f, 4f
            }};

            target = new FourierTransform(transformProvider, windowFunction, 4);
            target.Format = new WaveFormat(44100, 1);

            target.Process(buffers, 4);

            transformProvider.Received(1).FFT(
                Arg.Any<bool>(),
                Arg.Any<int>(),
                Arg.Is<Complex[]>(x => x.Length == 4
                    && x[0].X == 1f
                    && x[1].X == 2f
                    && x[2].X == 3f
                    && x[3].X == 4f));
        }

        [TestMethod]
        public void Should_pass_zeros_to_FFT_implementation_as_imaginary_component()
        {
            var buffers = new float[][]{ new []
            {
                1f, 2f, 3f, 4f
            }};

            target = new FourierTransform(transformProvider, windowFunction, 4);
            target.Format = new WaveFormat(44100, 1);

            target.Process(buffers, 4);

            transformProvider.Received(1).FFT(
                Arg.Any<bool>(),
                Arg.Any<int>(),
                Arg.Is<Complex[]>(x => x.Length == 4
                    && x[0].Y == 0f
                    && x[1].Y == 0f
                    && x[2].Y == 0f
                    && x[3].Y == 0f));
        }

        [TestMethod]
        public void Should_perform_forwards_FFT()
        {
            var buffers = new float[][] { new float[4] };

            target = new FourierTransform(transformProvider, windowFunction, 4);
            target.Format = new WaveFormat(44100, 1);

            target.Process(buffers, 4);

            transformProvider.Received(1).FFT(
                Arg.Is<bool>(true),
                Arg.Any<int>(),
                Arg.Any<Complex[]>());
        }

        [TestMethod]
        public void Should_provide_the_power_of_two_for_the_transform_length()
        {
            var buffers = new float[][] { new float[16] };

            target = new FourierTransform(transformProvider, windowFunction, 16);
            target.Format = new WaveFormat(44100, 1);

            target.Process(buffers, 16);

            transformProvider.Received(1).FFT(
                Arg.Any<bool>(),
                Arg.Is<int>(4),
                Arg.Any<Complex[]>());
        }

        [TestMethod]
        public void Should_provide_the_FFT_provider_result_via_the_DataReady_event()
        {
            var buffers = new float[][] { new float[4] };

            var expected = new float[]
            {
                5f, 6f, 7f, 8f
            };

            transformProvider.FFT(Arg.Any<bool>(), Arg.Any<int>(), Arg.Do<Complex[]>(x =>
            {
                x[0] = new Complex { X = expected[0], Y = 0f };
                x[1] = new Complex { X = expected[1], Y = 0f };
                x[2] = new Complex { X = expected[2], Y = 0f };
                x[3] = new Complex { X = expected[3], Y = 0f };
            }));

            target = new FourierTransform(transformProvider, windowFunction, 4);

            float[] actual = null;
            target.DataReady += (s, e) => actual = e.Real;

            target.Format = new WaveFormat(44100, 1);

            target.Process(buffers, 4);

            CollectionAssert.AreEqual(expected, actual, new FloatComparer());
        }

        [TestMethod]
        public void Should_apply_window_function_to_FFT_inputs()
        {
            var buffers = new float[][]{ new []
            {
                1f, 2f, 3f, 4f
            }};

            windowFunction.CalculateCoefficients(Arg.Is(4))
                .Returns(new[] { 5f, 6f, 7f, 8f });

            target = new FourierTransform(transformProvider, windowFunction, 4);
            target.Format = new WaveFormat(44100, 1);

            target.Process(buffers, 4);

            transformProvider.Received(1).FFT(
                Arg.Any<bool>(),
                Arg.Any<int>(),
                Arg.Is<Complex[]>(x => x.Length == 4
                    && x[0].X == 5f
                    && x[1].X == 12f
                    && x[2].X == 21f
                    && x[3].X == 32f));
        }

        [TestMethod]
        public void Should_correctly_process_multiple_channels()
        {
            var buffers = new float[][] { new float[4], new float[4] };

            var expected = new float[]
            {
                5f, 6f, 7f, 8f
            };

            transformProvider.FFT(Arg.Any<bool>(), Arg.Any<int>(), Arg.Do<Complex[]>(x =>
            {
                x[0] = new Complex { X = expected[0], Y = 0f };
                x[1] = new Complex { X = expected[1], Y = 0f };
                x[2] = new Complex { X = expected[2], Y = 0f };
                x[3] = new Complex { X = expected[3], Y = 0f };
            }));

            target = new FourierTransform(transformProvider, windowFunction, 4);

            float[][] actual = new float[2][];
            target.DataReady += (s, e) => actual[e.Channel] = e.Real;

            target.Format = new WaveFormat(44100, 2);

            target.Process(buffers, 4);

            CollectionAssert.AreEqual(expected, actual[0], new FloatComparer());
            CollectionAssert.AreEqual(expected, actual[1], new FloatComparer());
        }
    }
}
