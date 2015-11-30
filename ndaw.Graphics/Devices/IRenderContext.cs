using SharpDX.Direct2D1;
using System;

namespace ndaw.Graphics.Devices
{
    public interface IRenderContext: IDisposable
    {
        RenderTarget RenderTarget { get; }

        void Activate();
        void Present();
    }
}
