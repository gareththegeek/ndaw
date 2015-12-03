using NAudio.Utils;
using ndaw.Core.Fourier;
using SharpDX;
using SharpDX.Direct2D1;
using System;
using System.Windows.Forms;

namespace ndaw.Graphics.Controls
{
    public partial class FourierControl : DXRealTimeControlBase
    {
        private SolidColorBrush solidBrush;

        private int fourierLength;
        private float[] fourierReal;
        private float[] fourierImaginary;
        private object fourierDataLock = new object();

        private float smoothing = 0.95f;
        private float[] history;

        public void fourier_DataReady(object sender, FourierTransformEventArgs e)
        {
            if (abortRendering) return;
            if (e.Channel != 0) return;

            lock (fourierDataLock)
            {
                fourierLength = e.TranformLength;
                fourierReal = e.Real;
                fourierImaginary = e.Imaginary;
            }

            //if (InvokeRequired)
            //{
            //    if (stopped) return;
            //    this.Invoke(new Action(() => fourier_DataReady(sender, e)));
            //    return;
            //}
        }

        private float minimumFourierValue = float.MaxValue;
        private float maximumFourierValue = float.MinValue;
        private float fourierScale = 1f;

        public FourierControl()
        {
            InitializeComponent();
        }

        public void FourierControl_Load(object sender, EventArgs e)
        {
            if (DesignMode) return;

            solidBrush = new SolidColorBrush(context.RenderTarget, Color.White);
        }

        protected override void paint()
        {
            if (DesignMode) return;

            float[] real = null;
            float[] imaginary = null;

            lock (fourierDataLock)
            {
                if (fourierReal != null)
                {
                    real = new float[fourierReal.Length];
                    System.Buffer.BlockCopy(fourierReal, 0, real, 0, fourierReal.Length * sizeof(float));
                }

                if (fourierImaginary != null)
                {
                    imaginary = new float[fourierImaginary.Length];
                    System.Buffer.BlockCopy(fourierImaginary, 0, imaginary, 0, fourierImaginary.Length * sizeof(float));
                }
            }

            context.RenderTarget.BeginDraw();
            context.RenderTarget.Clear(Color.Black);
            solidBrush.Color = new Color4(1, 1, 1, 1);

            if (real != null && imaginary != null)
            {
                if (real.Length != imaginary.Length)
                {
                    throw new NotSupportedException("Real and imaginary fourier results must be the same length");
                }

                var max = float.MinValue;
                var min = float.MaxValue;
                var maxI = -1;
                var minI = -1;

                for (var i = 0; i < real.Length; i++)
                {
                    var x = real[i];
                    var y = imaginary[i];
                    var mag = x * x + y * y;

                    if (mag != 0f)
                    {
                        mag = (float)Math.Log10(mag);

                        if (mag > max)
                        {
                            max = mag;
                            maxI = i;
                        }
                        if (mag < min)
                        {
                            min = mag;
                            minI = i;
                        }

                        var height = ClientSize.Height;

                        //var scaled = (int)(mag * ClientSize.Height);
                        history = BufferHelpers.Ensure(history, fourierLength * sizeof(float));

                        history[i] = smoothing * history[i] + ((1 - smoothing) * mag);
                        var scaled = (int)(((history[i] - minimumFourierValue) / (fourierScale)) * height);

                        //var scaled = mag * panel1.Height;
                        //g.DrawLine(pen, i, panel1.Height, i, panel1.Height - scaled);

                        context.RenderTarget.DrawLine(
                            new Vector2 { X = i, Y = height },
                            new Vector2 { X = i, Y = height - scaled },
                            solidBrush);
                    }
                }

                //TODO remove magic number for SampleRate
                var sampleRateOverN = 44100f/*SampleRate*/ / real.Length;
                var maxF = maxI * sampleRateOverN;
                var minF = minI * sampleRateOverN;

                if (max > maximumFourierValue) maximumFourierValue = max;
                if (min < minimumFourierValue) minimumFourierValue = min;
                fourierScale = maximumFourierValue - minimumFourierValue;
            }

            context.RenderTarget.EndDraw();
        }
    }
}
