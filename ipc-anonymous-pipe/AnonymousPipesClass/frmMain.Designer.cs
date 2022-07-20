namespace AnonymousPipesClass
{
    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.textOut = new System.Windows.Forms.TextBox();
            this.btSendText = new System.Windows.Forms.Button();
            this.lbTextIn = new System.Windows.Forms.ListBox();
            this.tsStatusBar = new System.Windows.Forms.ToolStrip();
            this.tsbStatus = new System.Windows.Forms.ToolStripLabel();
            this.tsbStartServer = new System.Windows.Forms.ToolStripButton();
            this.tsbStopServer = new System.Windows.Forms.ToolStripButton();
            this.tsbConnectToPipe = new System.Windows.Forms.ToolStripButton();
            this.tsbStartClient = new System.Windows.Forms.ToolStripButton();
            this.tsStatusBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // textOut
            // 
            this.textOut.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textOut.Location = new System.Drawing.Point(13, 206);
            this.textOut.Name = "textOut";
            this.textOut.Size = new System.Drawing.Size(296, 20);
            this.textOut.TabIndex = 2;
            // 
            // btSendText
            // 
            this.btSendText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btSendText.Location = new System.Drawing.Point(315, 204);
            this.btSendText.Name = "btSendText";
            this.btSendText.Size = new System.Drawing.Size(75, 23);
            this.btSendText.TabIndex = 3;
            this.btSendText.Text = "Send Text";
            this.btSendText.UseVisualStyleBackColor = true;
            this.btSendText.Click += new System.EventHandler(this.btSendText_Click);
            // 
            // lbTextIn
            // 
            this.lbTextIn.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbTextIn.FormattingEnabled = true;
            this.lbTextIn.Location = new System.Drawing.Point(12, 12);
            this.lbTextIn.Name = "lbTextIn";
            this.lbTextIn.Size = new System.Drawing.Size(378, 186);
            this.lbTextIn.TabIndex = 4;
            // 
            // tsStatusBar
            // 
            this.tsStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tsStatusBar.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsStatusBar.ImageScalingSize = new System.Drawing.Size(0, 0);
            this.tsStatusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbStatus,
            this.tsbStartServer,
            this.tsbStopServer,
            this.tsbConnectToPipe,
            this.tsbStartClient});
            this.tsStatusBar.Location = new System.Drawing.Point(0, 232);
            this.tsStatusBar.Name = "tsStatusBar";
            this.tsStatusBar.Padding = new System.Windows.Forms.Padding(0, 0, 4, 0);
            this.tsStatusBar.Size = new System.Drawing.Size(403, 39);
            this.tsStatusBar.TabIndex = 6;
            this.tsStatusBar.Text = "toolStrip1";
            // 
            // tsbStatus
            // 
            this.tsbStatus.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbStatus.Name = "tsbStatus";
            this.tsbStatus.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.tsbStatus.Size = new System.Drawing.Size(34, 36);
            this.tsbStatus.Text = "Idle.";
            // 
            // tsbStartServer
            // 
            this.tsbStartServer.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsbStartServer.AutoSize = false;
            this.tsbStartServer.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbStartServer.Image = ((System.Drawing.Image)(resources.GetObject("tsbStartServer.Image")));
            this.tsbStartServer.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbStartServer.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbStartServer.Name = "tsbStartServer";
            this.tsbStartServer.Size = new System.Drawing.Size(36, 36);
            this.tsbStartServer.Text = "Start Pipe Server";
            this.tsbStartServer.Click += new System.EventHandler(this.tsbStartServer_Click);
            // 
            // tsbStopServer
            // 
            this.tsbStopServer.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsbStopServer.AutoSize = false;
            this.tsbStopServer.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbStopServer.Image = ((System.Drawing.Image)(resources.GetObject("tsbStopServer.Image")));
            this.tsbStopServer.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbStopServer.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbStopServer.Name = "tsbStopServer";
            this.tsbStopServer.Padding = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.tsbStopServer.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.tsbStopServer.Size = new System.Drawing.Size(36, 36);
            this.tsbStopServer.Text = "Stop Pipe Server";
            this.tsbStopServer.Click += new System.EventHandler(this.tsbStopServer_Click);
            // 
            // tsbConnectToPipe
            // 
            this.tsbConnectToPipe.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsbConnectToPipe.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbConnectToPipe.Image = ((System.Drawing.Image)(resources.GetObject("tsbConnectToPipe.Image")));
            this.tsbConnectToPipe.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbConnectToPipe.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbConnectToPipe.Name = "tsbConnectToPipe";
            this.tsbConnectToPipe.Size = new System.Drawing.Size(36, 36);
            this.tsbConnectToPipe.Text = "Connect to Pipe Server";
            this.tsbConnectToPipe.Click += new System.EventHandler(this.tsbConnectToPipe_Click);
            // 
            // tsbStartClient
            // 
            this.tsbStartClient.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsbStartClient.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbStartClient.Image = ((System.Drawing.Image)(resources.GetObject("tsbStartClient.Image")));
            this.tsbStartClient.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbStartClient.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbStartClient.Name = "tsbStartClient";
            this.tsbStartClient.Size = new System.Drawing.Size(36, 36);
            this.tsbStartClient.Text = "Start Client Application";
            this.tsbStartClient.Click += new System.EventHandler(this.tsbStartClient_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(403, 271);
            this.Controls.Add(this.tsStatusBar);
            this.Controls.Add(this.lbTextIn);
            this.Controls.Add(this.btSendText);
            this.Controls.Add(this.textOut);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Anonymous Pipes Tester";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.tsStatusBar.ResumeLayout(false);
            this.tsStatusBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textOut;
        private System.Windows.Forms.Button btSendText;
        private System.Windows.Forms.ListBox lbTextIn;
        private System.Windows.Forms.ToolStrip tsStatusBar;
        private System.Windows.Forms.ToolStripButton tsbStartServer;
        private System.Windows.Forms.ToolStripButton tsbStopServer;
        private System.Windows.Forms.ToolStripLabel tsbStatus;
        private System.Windows.Forms.ToolStripButton tsbConnectToPipe;
        private System.Windows.Forms.ToolStripButton tsbStartClient;
    }
}

