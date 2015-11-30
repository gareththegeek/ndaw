namespace ndaw
{
    partial class Form1
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
            this.cmbDevice = new System.Windows.Forms.ComboBox();
            this.vsbSamples = new System.Windows.Forms.VScrollBar();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.cboChannelOut = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.cboChannelIn = new System.Windows.Forms.ComboBox();
            this.chkLowPass = new System.Windows.Forms.CheckBox();
            this.fourierControl = new ndaw.Graphics.Controls.FourierControl();
            this.SuspendLayout();
            // 
            // cmbDevice
            // 
            this.cmbDevice.FormattingEnabled = true;
            this.cmbDevice.Location = new System.Drawing.Point(215, 61);
            this.cmbDevice.Name = "cmbDevice";
            this.cmbDevice.Size = new System.Drawing.Size(385, 21);
            this.cmbDevice.TabIndex = 0;
            this.cmbDevice.SelectedIndexChanged += new System.EventHandler(this.cmbDevice_SelectedIndexChanged);
            // 
            // vsbSamples
            // 
            this.vsbSamples.Location = new System.Drawing.Point(64, 29);
            this.vsbSamples.Maximum = 128;
            this.vsbSamples.Minimum = 1;
            this.vsbSamples.Name = "vsbSamples";
            this.vsbSamples.Size = new System.Drawing.Size(20, 270);
            this.vsbSamples.TabIndex = 3;
            this.vsbSamples.Value = 32;
            this.vsbSamples.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vsbVolume_Scroll);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(182, 151);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(95, 20);
            this.textBox1.TabIndex = 4;
            // 
            // cboChannelOut
            // 
            this.cboChannelOut.FormattingEnabled = true;
            this.cboChannelOut.Location = new System.Drawing.Point(215, 88);
            this.cboChannelOut.Name = "cboChannelOut";
            this.cboChannelOut.Size = new System.Drawing.Size(385, 21);
            this.cboChannelOut.TabIndex = 5;
            this.cboChannelOut.SelectedIndexChanged += new System.EventHandler(this.cboChannel_SelectedIndexChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(622, 78);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(622, 129);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // cboChannelIn
            // 
            this.cboChannelIn.FormattingEnabled = true;
            this.cboChannelIn.Location = new System.Drawing.Point(215, 115);
            this.cboChannelIn.Name = "cboChannelIn";
            this.cboChannelIn.Size = new System.Drawing.Size(385, 21);
            this.cboChannelIn.TabIndex = 6;
            // 
            // chkLowPass
            // 
            this.chkLowPass.AutoSize = true;
            this.chkLowPass.Checked = true;
            this.chkLowPass.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkLowPass.Location = new System.Drawing.Point(315, 153);
            this.chkLowPass.Name = "chkLowPass";
            this.chkLowPass.Size = new System.Drawing.Size(72, 17);
            this.chkLowPass.TabIndex = 7;
            this.chkLowPass.Text = "Low Pass";
            this.chkLowPass.UseVisualStyleBackColor = true;
            this.chkLowPass.CheckedChanged += new System.EventHandler(this.chkLowPass_CheckedChanged);
            // 
            // fourierControl
            // 
            this.fourierControl.Location = new System.Drawing.Point(182, 196);
            this.fourierControl.Name = "fourierControl";
            this.fourierControl.Size = new System.Drawing.Size(506, 298);
            this.fourierControl.TabIndex = 8;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(733, 516);
            this.Controls.Add(this.fourierControl);
            this.Controls.Add(this.chkLowPass);
            this.Controls.Add(this.cboChannelIn);
            this.Controls.Add(this.cboChannelOut);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.vsbSamples);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.cmbDevice);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbDevice;
        private System.Windows.Forms.VScrollBar vsbSamples;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ComboBox cboChannelOut;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ComboBox cboChannelIn;
        private System.Windows.Forms.CheckBox chkLowPass;
        private Graphics.Controls.FourierControl fourierControl;
    }
}

