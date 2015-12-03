using System;
using DXGI = SharpDX.DXGI;
using D2D = SharpDX.Direct2D1;
using D3D11 = SharpDX.Direct3D11;
using System.Windows.Forms;
using SharpDX;
using System.Drawing;

namespace ndaw.Graphics.Devices
{
    public class RenderContext: IRenderContext
    {
        public object DeviceLock { get { return deviceManager.DeviceLock; } }

        private Control control;

        private IDeviceManager deviceManager;
        private D3D11.DeviceContext deviceContext;

        private DXGI.SwapChain swapChain;
        private D3D11.Texture2D backbuffer;
        private D3D11.RenderTargetView backbufferView;
        private Viewport viewport;

        private D2D.RenderTarget renderTarget;

        public D2D.RenderTarget RenderTarget
        {
            get { return renderTarget; }
        }

        public void Activate()
        {
            lock (deviceManager.DeviceLock)
            {
                deviceContext.OutputMerger.SetTargets(backbufferView);
                deviceContext.Rasterizer.SetViewport(viewport);
            }
        }

        public void Present()
        {
            lock (deviceManager.DeviceLock)
            {
                swapChain.Present(0, DXGI.PresentFlags.None);
            }
        }

        public RenderContext(IDeviceManager deviceManager, Control control)
        {
            if (deviceManager == null)
            {
                throw new ArgumentNullException("deviceManager", "Device Manager cannot be null");
            }

            if (control == null)
            {
                throw new ArgumentNullException("control", "Control cannot be null");
            }

            this.deviceManager = deviceManager;
            this.deviceContext = deviceManager.DeviceContext;
            this.control = control;

            createSwapChain();
        }

        private void createSwapChain()
        {
            lock (deviceManager.DeviceLock)
            {
                var factory = new DXGI.Factory();

                var description = new DXGI.SwapChainDescription()
                {
                    BufferCount = 1,
                    ModeDescription =
                        new DXGI.ModeDescription(
                            control.ClientSize.Width,
                            control.ClientSize.Height,
                            new DXGI.Rational(60, 1),
                            DXGI.Format.R8G8B8A8_UNorm),
                    IsWindowed = true,
                    SampleDescription = new DXGI.SampleDescription(1, 0),
                    SwapEffect = DXGI.SwapEffect.Discard,
                    Usage = DXGI.Usage.RenderTargetOutput,
                    OutputHandle = control.Handle
                };

                swapChain = new DXGI.SwapChain(factory, deviceContext.Device, description);

                backbuffer = D3D11.Texture2D.FromSwapChain<D3D11.Texture2D>(swapChain, 0);
                backbufferView = new D3D11.RenderTargetView(deviceContext.Device, backbuffer);
            }

            var d2dFactory = deviceManager.Factory;
            var surface = backbuffer.QueryInterface<DXGI.Surface>();
            renderTarget = new D2D.RenderTarget(
                d2dFactory,
                surface,
                new D2D.RenderTargetProperties(
                    new D2D.PixelFormat(
                        DXGI.Format.Unknown,
                        D2D.AlphaMode.Premultiplied)));

            renderTarget.AntialiasMode = D2D.AntialiasMode.Aliased;

            createViewport();
        }

        private void createViewport()
        {
            viewport = new Viewport(
                0, 0,
                control.ClientSize.Width,
                control.ClientSize.Height,
                0f, 1f);
        }

        public void UpdateViewport()
        {
            disposeResources();
            createSwapChain();
        }

        private void disposeResources()
        {
            if (renderTarget != null) renderTarget.Dispose();
            if (swapChain != null) swapChain.Dispose();
            if (backbufferView != null) backbufferView.Dispose();
            if (backbuffer != null) backbuffer.Dispose();
        }

        private bool disposed;

        ~RenderContext()
        {
            dispose(false);
        }

        protected virtual void dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;

                if (disposing)
                {
                    disposeResources();
                }
            }
        }

        public void Dispose()
        {
            dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
