using System;
using D3D11 = SharpDX.Direct3D11;
using D2D = SharpDX.Direct2D1;
using System.Windows.Forms;

namespace ndaw.Graphics.Devices
{
    public interface IDeviceManager: IDisposable
    {
        object DeviceLock { get; }
        D3D11.DeviceContext DeviceContext { get; }
        D2D.Factory Factory { get; }
    }
}
