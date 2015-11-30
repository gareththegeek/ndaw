using ndaw.Core.Fourier;
using SharpDX.Direct3D11;
using DXGI = SharpDX.DXGI;
using System;
using System.Windows.Forms;
using SharpDX.Direct2D1;
using SharpDX.Direct3D;
using SharpDX;
using System.Threading;
using NAudio.Utils;

namespace ndaw
{
    public partial class FourierForm : Form
    {
        private Device device;
        private DXGI.SwapChain swapChain;
        private DeviceContext context;
        private Texture2D backbuffer;
        private RenderTargetView backbufferView;

        private RenderTarget renderTarget;
        private SolidColorBrush solidBrush;

        private int fourierLength;
        private float[] fourierReal;
        private float[] fourierImaginary;
        private object fourierDataLock = new object();
        private object renderLock = new object();

        public FourierForm()
        {
            InitializeComponent();
        }

        private bool stopped = false;
        private float smoothing = 0.95f;
        private float[] history;
        public void fourier_DataReady(object sender, FourierTransformEventArgs e)
        {
            if (stopped) return;
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

        private void render()
        {
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

            lock (renderLock)
            {
                if (stopped) return;

                renderTarget.BeginDraw();
                renderTarget.Clear(Color.Black);
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

                            //var scaled = (int)(mag * ClientSize.Height);
                            history = BufferHelpers.Ensure(history, fourierLength * sizeof(float));

                            history[i] = smoothing * history[i] + ((1 - smoothing) * mag);
                            var scaled = (int)(((history[i] - minimumFourierValue) / (fourierScale)) * ClientSize.Height);

                            //var scaled = mag * panel1.Height;
                            //g.DrawLine(pen, i, panel1.Height, i, panel1.Height - scaled);

                            renderTarget.DrawLine(
                                new Vector2 { X = i, Y = ClientSize.Height },
                                new Vector2 { X = i, Y = ClientSize.Height - scaled },
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

                renderTarget.EndDraw();

                swapChain.Present(0, DXGI.PresentFlags.None);
            }
        }

        private void FourierForm_Load(object sender, EventArgs e)
        {
            DXGI.SwapChainDescription description = new DXGI.SwapChainDescription()
            {
                BufferCount = 1,
                ModeDescription = 
                    new DXGI.ModeDescription(ClientSize.Width, ClientSize.Height,
                                        new DXGI.Rational(60, 1), DXGI.Format.R8G8B8A8_UNorm),
                IsWindowed = true,
                SampleDescription = new DXGI.SampleDescription(1, 0),
                SwapEffect = DXGI.SwapEffect.Discard,
                Usage = DXGI.Usage.RenderTargetOutput,
                OutputHandle = this.Handle
            };

            Device.CreateWithSwapChain(
                DriverType.Hardware,
                /*DeviceCreationFlags.Debug |*/ DeviceCreationFlags.BgraSupport, 
                description,
                out device,
                out swapChain);

            context = device.ImmediateContext;

            backbuffer = Texture2D.FromSwapChain<Texture2D>(swapChain, 0);
            backbufferView = new RenderTargetView(device, backbuffer);

            var factory = new Factory();
            var surface = backbuffer.QueryInterface<DXGI.Surface>();
            renderTarget = new RenderTarget(
                factory, 
                surface,
                new RenderTargetProperties(new PixelFormat(DXGI.Format.Unknown, AlphaMode.Premultiplied)));
            solidBrush = new SolidColorBrush(renderTarget, Color.White);

            context.OutputMerger.SetTargets(backbufferView);

            var viewport = new Viewport(0, 0, this.ClientSize.Width, this.ClientSize.Height, 0f, 1f);
            context.Rasterizer.SetViewport(viewport);

            swapChain.Present(0, DXGI.PresentFlags.None);

            new Thread(() =>
                {
                    while (!stopped)
                    {
                        if (stopped)
                        {
                            Thread.CurrentThread.Abort();
                            return;
                        }

                        Thread.Sleep(1000 / 120);
                        render();
                    }
                }).Start();
        }

        private void FourierForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            stopped = true;

            lock (renderLock)
            {
                solidBrush.Dispose();
                renderTarget.Dispose();
                device.Dispose();
                swapChain.Dispose();
                context.Dispose();
                backbufferView.Dispose();
                backbuffer.Dispose();
            }
        }
    }
}

