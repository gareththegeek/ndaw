using System;
using D3D11 = SharpDX.Direct3D11;
using System.Windows.Forms;

namespace ndaw.Graphics
{
    public interface IDeviceManager: IDisposable
    {
        D3D11.DeviceContext DeviceContext { get; }
    }
}
