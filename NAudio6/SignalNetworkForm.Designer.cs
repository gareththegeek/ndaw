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
            this.scrollableAreaControl1 = new ndaw.Graphics.Controls.ScrollableAreaControl();
            this.SuspendLayout();
            // 
            // scrollableAreaControl1
            // 
            this.scrollableAreaControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scrollableAreaControl1.Location = new System.Drawing.Point(0, 0);
            this.scrollableAreaControl1.Name = "scrollableAreaControl1";
            this.scrollableAreaControl1.ScrollableArea = null;
            this.scrollableAreaControl1.Size = new System.Drawing.Size(690, 406);
            this.scrollableAreaControl1.TabIndex = 0;
            // 
            // SignalNetworkForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(690, 406);
            this.Controls.Add(this.scrollableAreaControl1);
            this.Name = "SignalNetworkForm";
            this.Text = "SignalNetworkForm";
            this.ResumeLayout(false);

        }

        #endregion

        private Graphics.Controls.ScrollableAreaControl scrollableAreaControl1;



    }
}