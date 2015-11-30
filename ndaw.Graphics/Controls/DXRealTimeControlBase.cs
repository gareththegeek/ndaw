using System;
using System.Threading;

namespace ndaw.Graphics.Controls
{
    public partial class DXRealTimeControlBase : DXControlBase
    {
        protected bool abortRendering;

        public DXRealTimeControlBase()
        {
            InitializeComponent();
        }

        private void DXRealTimeControlBase_Load(object sender, EventArgs e)
        {
            if (DesignMode) return;

            new Thread(() =>
                {
                    while (!abortRendering)
                    {
                        if (abortRendering)
                        {
                            Thread.CurrentThread.Abort();
                            return;
                        }

                        Thread.Sleep(1000 / 120);
                        DXPaint();
                    }
                }).Start();
        }

        private void DXControlBase_Disposed(object sender, EventArgs e)
        {
            abortRendering = true;
        }
    }
}
