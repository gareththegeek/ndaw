using ndaw.Graphics.Controls;
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
        public SignalNetworkControl SignalNetworkControl { get; set; }

        public SignalNetworkForm()
        {
            InitializeComponent();

            SignalNetworkControl = new SignalNetworkControl();
            scrollableAreaControl1.ScrollableArea = SignalNetworkControl;
        }
    }
}
