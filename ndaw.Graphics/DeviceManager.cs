using D3D11 = SharpDX.Direct3D11;
using System;
using D3D = SharpDX.Direct3D;
using System.Windows.Forms;

namespace ndaw.Graphics
{
    public class DeviceManager: IDeviceManager
    {
        private bool disposed;

        public D3D11.DeviceContext DeviceContext { get; private set; }

        public DeviceManager()
        {
            var device = new D3D11.Device(
                D3D.DriverType.Hardware,
                /*DeviceCreationFlags.Debug |*/ D3D11.DeviceCreationFlags.BgraSupport);

            DeviceContext = device.ImmediateContext;
        }

        ~DeviceManager()
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
                    if (DeviceContext != null)
                    {
                        DeviceContext.Device.Dispose();
                        DeviceContext = null;
                    }
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
