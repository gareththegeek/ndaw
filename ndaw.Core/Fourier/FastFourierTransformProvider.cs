﻿using NAudio.Dsp;
using System;
using System.Diagnostics.CodeAnalysis;

namespace ndaw.Core.Fourier
{
    [ExcludeFromCodeCoverage]
    public class FastFourierTransformProvider: IFastFourierTransformProvider
    {
        public void FFT(bool forward, int m, Complex[] data)
        {
            FastFourierTransform.FFT(forward, m, data);
        }
    }
}
