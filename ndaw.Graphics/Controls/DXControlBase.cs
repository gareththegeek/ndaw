using System;
using System.Windows.Forms;
using ndaw.Graphics.Devices;
using SharpDX;

namespace ndaw.Graphics.Controls
{
    public partial class DXControlBase : UserControl
    {
        protected object renderLock = new object();
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
            DXPaint();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            DXPaint();
        }

        protected virtual void DXPaint()
        {
            if (DesignMode) return;

            context.Activate();
            context.RenderTarget.Clear(Color4.Black);
            context.Present();
        }

        private void DXControlBase_Disposed(object sender, EventArgs e)
        {
            if (context != null)
            {
                lock (renderLock)
                {
                    context.Dispose();
                    context = null;
                }
            }
        }
    }
}
