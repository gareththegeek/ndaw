namespace ndaw.Graphics.Controls
{
    partial class ScrollableAreaControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.rightPanel = new System.Windows.Forms.Panel();
            this.verticalScrollBar = new System.Windows.Forms.VScrollBar();
            this.horizontalScrollBar = new System.Windows.Forms.HScrollBar();
            this.pnlScrollableArea = new System.Windows.Forms.Panel();
            this.rightPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // rightPanel
            // 
            this.rightPanel.Controls.Add(this.verticalScrollBar);
            this.rightPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.rightPanel.Location = new System.Drawing.Point(738, 0);
            this.rightPanel.Name = "rightPanel";
            this.rightPanel.Size = new System.Drawing.Size(20, 459);
            this.rightPanel.TabIndex = 4;
            // 
            // verticalScrollBar
            // 
            this.verticalScrollBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.verticalScrollBar.LargeChange = 101;
            this.verticalScrollBar.Location = new System.Drawing.Point(1, 0);
            this.verticalScrollBar.Name = "verticalScrollBar";
            this.verticalScrollBar.Scroll += verticalScrollBar_Scroll;
            this.verticalScrollBar.Size = new System.Drawing.Size(17, 441);
            this.verticalScrollBar.SmallChange = 50;
            this.verticalScrollBar.TabIndex = 0;
            // 
            // horizontalScrollBar
            // 
            this.horizontalScrollBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.horizontalScrollBar.LargeChange = 100;
            this.horizontalScrollBar.Location = new System.Drawing.Point(0, 442);
            this.horizontalScrollBar.Name = "horizontalScrollBar";
            this.horizontalScrollBar.Scroll += horizontalScrollBar_Scroll;
            this.horizontalScrollBar.Size = new System.Drawing.Size(738, 17);
            this.horizontalScrollBar.SmallChange = 10;
            this.horizontalScrollBar.TabIndex = 5;
            // 
            // pnlScrollableArea
            // 
            this.pnlScrollableArea.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlScrollableArea.Location = new System.Drawing.Point(0, 0);
            this.pnlScrollableArea.Name = "pnlScrollableArea";
            this.pnlScrollableArea.Size = new System.Drawing.Size(738, 442);
            this.pnlScrollableArea.TabIndex = 6;
            // 
            // ScrollableAreaControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlScrollableArea);
            this.Controls.Add(this.horizontalScrollBar);
            this.Controls.Add(this.rightPanel);
            this.Name = "ScrollableAreaControl";
            this.Size = new System.Drawing.Size(758, 459);
            this.Resize += new System.EventHandler(this.ScrollableAreaControl_Resize);
            this.rightPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel rightPanel;
        private System.Windows.Forms.VScrollBar verticalScrollBar;
        private System.Windows.Forms.HScrollBar horizontalScrollBar;
        private System.Windows.Forms.Panel pnlScrollableArea;
    }
}
