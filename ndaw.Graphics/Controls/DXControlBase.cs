using System;
using System.Windows.Forms;
using ndaw.Graphics.Devices;
using SharpDX;

namespace ndaw.Graphics.Controls
{
    public partial class DXControlBase : UserControl
    {
        protected IRenderContext context;

        public DXControlBase()
        {
            InitializeComponent();

            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint
                | ControlStyles.UserPaint
                /*| ControlStyles.OptimizedDoubleBuffer*/, true);
        }

        private void DXControlBase_Load(object sender, EventArgs e)
        {
            if (DesignMode) return;

            this.Disposed += DXControlBase_Disposed;
            context = new RenderContext(DeviceManager.Instance, this);
        }

        private void DXControlBase_Resize(object sender, System.EventArgs e)
        {
            if (DesignMode) return;
            if (context == null) return;

            context.UpdateViewport();
            Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Refresh();
        }

        public virtual void Refresh()
        {
            if (DesignMode) return;

            lock (context.DeviceLock)
            {
                context.Activate();

                paint();
                
                context.Present();
            }
        }

        protected virtual void paint()
        {
            context.RenderTarget.BeginDraw();

            context.RenderTarget.Clear(Color4.Black);

            context.RenderTarget.EndDraw();
        }

        private void DXControlBase_Disposed(object sender, EventArgs e)
        {
            if (context != null)
            {
                lock (context.DeviceLock)
                {
                    context.Dispose();
                    context = null;
                }
            }
        }
    }
}
