using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ndaw
{
    public partial class SignalNetworkForm : Form
    {
        public SignalNetworkForm()
        {
            InitializeComponent();
        }

        private void verticalScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            SignalNetworkControl.ViewY = verticalScrollBar.Value;
        }

        private void horizontalScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            SignalNetworkControl.ViewX = horizontalScrollBar.Value;
        }
    }
}
