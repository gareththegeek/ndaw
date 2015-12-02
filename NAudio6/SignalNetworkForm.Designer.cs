namespace ndaw
{
    partial class SignalNetworkForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.rightPanel = new System.Windows.Forms.Panel();
            this.verticalScrollBar = new System.Windows.Forms.VScrollBar();
            this.horizontalScrollBar = new System.Windows.Forms.HScrollBar();
            this.SignalNetworkControl = new ndaw.Graphics.Controls.SignalNetworkControl();
            this.rightPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // rightPanel
            // 
            this.rightPanel.Controls.Add(this.verticalScrollBar);
            this.rightPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.rightPanel.Location = new System.Drawing.Point(670, 0);
            this.rightPanel.Name = "rightPanel";
            this.rightPanel.Size = new System.Drawing.Size(20, 406);
            this.rightPanel.TabIndex = 1;
            // 
            // verticalScrollBar
            // 
            this.verticalScrollBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.verticalScrollBar.Location = new System.Drawing.Point(1, 0);
            this.verticalScrollBar.Name = "verticalScrollBar";
            this.verticalScrollBar.Size = new System.Drawing.Size(17, 388);
            this.verticalScrollBar.TabIndex = 0;
            // 
            // horizontalScrollBar
            // 
            this.horizontalScrollBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.horizontalScrollBar.Location = new System.Drawing.Point(0, 389);
            this.horizontalScrollBar.Name = "horizontalScrollBar";
            this.horizontalScrollBar.Size = new System.Drawing.Size(670, 17);
            this.horizontalScrollBar.TabIndex = 2;
            // 
            // SignalNetworkControl
            // 
            this.SignalNetworkControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SignalNetworkControl.Location = new System.Drawing.Point(0, 0);
            this.SignalNetworkControl.Name = "SignalNetworkControl";
            this.SignalNetworkControl.Nodes = null;
            this.SignalNetworkControl.Size = new System.Drawing.Size(670, 389);
            this.SignalNetworkControl.TabIndex = 3;
            // 
            // SignalNetworkForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(690, 406);
            this.Controls.Add(this.SignalNetworkControl);
            this.Controls.Add(this.horizontalScrollBar);
            this.Controls.Add(this.rightPanel);
            this.Name = "SignalNetworkForm";
            this.Text = "SignalNetworkForm";
            this.rightPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel rightPanel;
        private System.Windows.Forms.VScrollBar verticalScrollBar;
        private System.Windows.Forms.HScrollBar horizontalScrollBar;
        public Graphics.Controls.SignalNetworkControl SignalNetworkControl;


    }
}