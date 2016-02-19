namespace Alert
{
	partial class frmBalloonTip
	{
		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 清理所有正在使用的资源。
		/// </summary>
		/// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows 窗体设计器生成的代码

		/// <summary>
		/// 设计器支持所需的方法 - 不要
		/// 使用代码编辑器修改此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.lblInfo = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.divAbout = new System.Windows.Forms.Panel();
            this.linkAbout = new System.Windows.Forms.LinkLabel();
            this.lblAboutTime = new System.Windows.Forms.Label();
            this.lblAppName = new System.Windows.Forms.Label();
            this.divTime = new System.Windows.Forms.Panel();
            this.lblDate = new System.Windows.Forms.Label();
            this.lblTime = new System.Windows.Forms.Label();
            this.divAlert = new System.Windows.Forms.Panel();
            this.lblAlertTime = new System.Windows.Forms.Label();
            this.timerClose = new System.Windows.Forms.Timer(this.components);
            this.panel1.SuspendLayout();
            this.divAbout.SuspendLayout();
            this.divTime.SuspendLayout();
            this.divAlert.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblInfo
            // 
            this.lblInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblInfo.Font = new System.Drawing.Font("SimSun", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblInfo.ForeColor = System.Drawing.Color.Blue;
            this.lblInfo.Location = new System.Drawing.Point(0, 20);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Padding = new System.Windows.Forms.Padding(5);
            this.lblInfo.Size = new System.Drawing.Size(249, 63);
            this.lblInfo.TabIndex = 0;
            this.lblInfo.Text = "提醒";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.divAbout);
            this.panel1.Controls.Add(this.divTime);
            this.panel1.Controls.Add(this.divAlert);
            this.panel1.Location = new System.Drawing.Point(5, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(341, 342);
            this.panel1.TabIndex = 2;
            // 
            // divAbout
            // 
            this.divAbout.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.divAbout.Controls.Add(this.linkAbout);
            this.divAbout.Controls.Add(this.lblAboutTime);
            this.divAbout.Controls.Add(this.lblAppName);
            this.divAbout.Location = new System.Drawing.Point(23, 212);
            this.divAbout.Name = "divAbout";
            this.divAbout.Size = new System.Drawing.Size(226, 97);
            this.divAbout.TabIndex = 3;
            this.divAbout.Visible = false;
            // 
            // linkAbout
            // 
            this.linkAbout.AutoSize = true;
            this.linkAbout.Location = new System.Drawing.Point(13, 70);
            this.linkAbout.Name = "linkAbout";
            this.linkAbout.Size = new System.Drawing.Size(0, 12);
            this.linkAbout.TabIndex = 2;
            this.linkAbout.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkAbout_LinkClicked);
            // 
            // lblAboutTime
            // 
            this.lblAboutTime.AutoSize = true;
            this.lblAboutTime.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblAboutTime.ForeColor = System.Drawing.Color.Blue;
            this.lblAboutTime.Location = new System.Drawing.Point(9, 34);
            this.lblAboutTime.Name = "lblAboutTime";
            this.lblAboutTime.Size = new System.Drawing.Size(200, 23);
            this.lblAboutTime.TabIndex = 1;
            this.lblAboutTime.Text = "2011年11月11日 12:22";
            // 
            // lblAppName
            // 
            this.lblAppName.AutoSize = true;
            this.lblAppName.Location = new System.Drawing.Point(11, 10);
            this.lblAppName.Name = "lblAppName";
            this.lblAppName.Size = new System.Drawing.Size(59, 12);
            this.lblAppName.TabIndex = 0;
            this.lblAppName.Text = "提醒 V1.0";
            // 
            // divTime
            // 
            this.divTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.divTime.Controls.Add(this.lblDate);
            this.divTime.Controls.Add(this.lblTime);
            this.divTime.Location = new System.Drawing.Point(23, 111);
            this.divTime.Name = "divTime";
            this.divTime.Size = new System.Drawing.Size(218, 95);
            this.divTime.TabIndex = 2;
            this.divTime.Visible = false;
            // 
            // lblDate
            // 
            this.lblDate.AutoSize = true;
            this.lblDate.Location = new System.Drawing.Point(11, 9);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(131, 12);
            this.lblDate.TabIndex = 1;
            this.lblDate.Text = "2011年12月20日 星期三";
            // 
            // lblTime
            // 
            this.lblTime.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTime.Font = new System.Drawing.Font("Tahoma", 40F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblTime.ForeColor = System.Drawing.Color.Blue;
            this.lblTime.Location = new System.Drawing.Point(-1, 12);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(218, 81);
            this.lblTime.TabIndex = 0;
            this.lblTime.Text = "12:30";
            this.lblTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // divAlert
            // 
            this.divAlert.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.divAlert.Controls.Add(this.lblAlertTime);
            this.divAlert.Controls.Add(this.lblInfo);
            this.divAlert.Location = new System.Drawing.Point(23, 20);
            this.divAlert.Name = "divAlert";
            this.divAlert.Size = new System.Drawing.Size(251, 85);
            this.divAlert.TabIndex = 1;
            this.divAlert.Visible = false;
            // 
            // lblAlertTime
            // 
            this.lblAlertTime.AutoSize = true;
            this.lblAlertTime.Location = new System.Drawing.Point(9, 8);
            this.lblAlertTime.Name = "lblAlertTime";
            this.lblAlertTime.Size = new System.Drawing.Size(125, 12);
            this.lblAlertTime.TabIndex = 2;
            this.lblAlertTime.Text = "2011年12月20日 11:11";
            // 
            // timerClose
            // 
            this.timerClose.Interval = 5000;
            this.timerClose.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // frmBalloonTip
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.ClientSize = new System.Drawing.Size(351, 352);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmBalloonTip";
            this.Opacity = 0.9D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "BalloonTip";
            this.TopMost = true;
            this.panel1.ResumeLayout(false);
            this.divAbout.ResumeLayout(false);
            this.divAbout.PerformLayout();
            this.divTime.ResumeLayout(false);
            this.divTime.PerformLayout();
            this.divAlert.ResumeLayout(false);
            this.divAlert.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label lblInfo;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Timer timerClose;
		private System.Windows.Forms.Panel divTime;
		private System.Windows.Forms.Label lblTime;
		private System.Windows.Forms.Panel divAlert;
		private System.Windows.Forms.Label lblDate;
		private System.Windows.Forms.Panel divAbout;
		private System.Windows.Forms.Label lblAboutTime;
		private System.Windows.Forms.Label lblAppName;
		private System.Windows.Forms.LinkLabel linkAbout;
		private System.Windows.Forms.Label lblAlertTime;
	}
}