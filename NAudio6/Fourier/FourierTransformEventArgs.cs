﻿using System;

namespace ndaw.Fourier
{
    public class FourierTransformEventArgs: EventArgs
    {
        public int TranformLength { get; set; }
        public int Channel { get; set; }
        public float[] Real { get; set; }
        public float[] Imaginary { get; set; }
    }
}
