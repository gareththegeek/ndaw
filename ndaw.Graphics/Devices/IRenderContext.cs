using SharpDX.Direct2D1;
using System;
using DW = SharpDX.DirectWrite;

namespace ndaw.Graphics.Devices
{
    public interface IRenderContext: IDisposable
    {
        object DeviceLock { get; }

        RenderTarget RenderTarget { get; }
        DW.Factory FontFactory { get; }
        void UpdateViewport();

        void Activate();
        void Present();
    }
}
