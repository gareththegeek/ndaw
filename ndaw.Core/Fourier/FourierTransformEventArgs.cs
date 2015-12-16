using NAudio.Dsp;
using System;

namespace ndaw.Core.Fourier
{
    public class FourierTransformEventArgs : EventArgs
    {
        public int TransformLength { get; set; }
        public int Channel { get; set; }
        public float[] Real { get; set; }
        public float[] Imaginary { get; set; }

        public FourierTransformEventArgs(
            int transformLength,
            int channel,
            Complex[] complex)
        {
            TransformLength = transformLength;
            Channel = channel;

            var count = complex.Length;

            Real = new float[count];
            Imaginary = new float[count];

            unsafe
            {
                fixed (float* fixedReal = Real, fixedImaginary = Imaginary)
                {
                    fixed(Complex* fixedComplex = complex)
                    {
                        var pReal = fixedReal;
                        var pImaginary = fixedImaginary;
                        // Treat complex as an array of floats where every other value is real
                        var pComplex = (float*)fixedComplex;

                        for (int i = 0; i < count; i++)
                        {
                            *pReal++ = *pComplex++;
                            *pImaginary++ = *pComplex++;
                        }
                    }
                }
            }

            //{
            //    TranformLength = transformLength,
            //    Channel = channel.Index,
            //    Real = channel.Complex.Select(c => (float)c.X).ToArray(),
            //    Imaginary = channel.Complex.Select(c => (float)c.Y).ToArray()
            //};
        }
    }
}
