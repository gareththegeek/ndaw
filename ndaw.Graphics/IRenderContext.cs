using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ndaw.Graphics
{
    public interface IRenderContext: IDisposable
    {
        RenderTarget RenderTarget { get; }

        void Activate();
        void Present();
    }
}
