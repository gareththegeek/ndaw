using SharpDX.Direct2D1;
using System;

namespace ndaw.Graphics.Devices
{
    public interface IRenderContext: IDisposable
    {
        object DeviceLock { get; }

        RenderTarget RenderTarget { get; }
        void UpdateViewport();

        void Activate();
        void Present();
    }
}
