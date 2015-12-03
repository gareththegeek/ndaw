using D3D11 = SharpDX.Direct3D11;
using System;
using D3D = SharpDX.Direct3D;
using D2D = SharpDX.Direct2D1;
using System.Windows.Forms;

namespace ndaw.Graphics.Devices
{
    public class DeviceManager: IDeviceManager
    {
        private bool disposed;

        private object deviceLock = new object();
        public object DeviceLock { get { return deviceLock; } }

        public D3D11.DeviceContext DeviceContext { get; private set; }

        public D2D.Factory Factory { get; private set; }

        private static DeviceManager instance;
        public static DeviceManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DeviceManager();
                }
                return instance;
            }
        }

        public DeviceManager()
        {
            var device = new D3D11.Device(
                D3D.DriverType.Hardware,
                /*DeviceCreationFlags.Debug |*/ D3D11.DeviceCreationFlags.BgraSupport);

            DeviceContext = device.ImmediateContext;
            Factory = new D2D.Factory();
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
