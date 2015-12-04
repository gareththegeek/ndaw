using System;
using D3D11 = SharpDX.Direct3D11;
using D2D = SharpDX.Direct2D1;
using System.Windows.Forms;
using DW = SharpDX.DirectWrite;

namespace ndaw.Graphics.Devices
{
    public interface IDeviceManager: IDisposable
    {
        object DeviceLock { get; }
        D3D11.DeviceContext DeviceContext { get; }
        D2D.Factory Direct2dFactory { get; }
        DW.Factory DirectWriteFactory { get; }
    }
}
