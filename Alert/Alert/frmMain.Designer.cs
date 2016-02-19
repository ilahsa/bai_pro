namespace Alert
{
	partial class frmMain
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
			this.notifyAlert = new System.Windows.Forms.NotifyIcon(this.components);
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.提醒管理ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.当前时间ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.记事本ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.显示时间ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.退出ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.timer = new System.Windows.Forms.Timer(this.components);
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.chkHalfHour = new System.Windows.Forms.CheckBox();
			this.chkSound = new System.Windows.Forms.CheckBox();
			this.chkHour = new System.Windows.Forms.CheckBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.divOne = new System.Windows.Forms.FlowLayoutPanel();
			this.lbl10 = new System.Windows.Forms.Label();
			this.lbl30 = new System.Windows.Forms.Label();
			this.lbl60 = new System.Windows.Forms.Label();
			this.radioButton4 = new System.Windows.Forms.RadioButton();
			this.radioButton3 = new System.Windows.Forms.RadioButton();
			this.radioButton2 = new System.Windows.Forms.RadioButton();
			this.radioButton1 = new System.Windows.Forms.RadioButton();
			this.btnAdd = new System.Windows.Forms.Button();
			this.txtMessage = new System.Windows.Forms.TextBox();
			this.dtTime = new System.Windows.Forms.DateTimePicker();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.listViewAlert = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.label4 = new System.Windows.Forms.Label();
			this.btnDel = new System.Windows.Forms.Button();
			this.btnDelOld = new System.Windows.Forms.Button();
			this.contextMenuStrip1.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.divOne.SuspendLayout();
			this.SuspendLayout();
			// 
			// notifyAlert
			// 
			this.notifyAlert.ContextMenuStrip = this.contextMenuStrip1;
			this.notifyAlert.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyAlert.Icon")));
			this.notifyAlert.Visible = true;
			this.notifyAlert.DoubleClick += new System.EventHandler(this.notifyAlert_DoubleClick);
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.提醒管理ToolStripMenuItem,
            this.当前时间ToolStripMenuItem,
            this.记事本ToolStripMenuItem,
            this.显示时间ToolStripMenuItem,
            this.toolStripMenuItem1,
            this.退出ToolStripMenuItem});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.ShowImageMargin = false;
			this.contextMenuStrip1.Size = new System.Drawing.Size(98, 120);
			// 
			// 提醒管理ToolStripMenuItem
			// 
			this.提醒管理ToolStripMenuItem.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold);
			this.提醒管理ToolStripMenuItem.Name = "提醒管理ToolStripMenuItem";
			this.提醒管理ToolStripMenuItem.Size = new System.Drawing.Size(97, 22);
			this.提醒管理ToolStripMenuItem.Text = "提醒管理";
			this.提醒管理ToolStripMenuItem.Click += new System.EventHandler(this.提醒管理ToolStripMenuItem_Click);
			// 
			// 当前时间ToolStripMenuItem
			// 
			this.当前时间ToolStripMenuItem.Name = "当前时间ToolStripMenuItem";
			this.当前时间ToolStripMenuItem.Size = new System.Drawing.Size(97, 22);
			this.当前时间ToolStripMenuItem.Text = "当前时间";
			this.当前时间ToolStripMenuItem.Click += new System.EventHandler(this.当前时间ToolStripMenuItem_Click);
			// 
			// 记事本ToolStripMenuItem
			// 
			this.记事本ToolStripMenuItem.Name = "记事本ToolStripMenuItem";
			this.记事本ToolStripMenuItem.Size = new System.Drawing.Size(97, 22);
			this.记事本ToolStripMenuItem.Text = "笔记本";
			this.记事本ToolStripMenuItem.Click += new System.EventHandler(this.记事本ToolStripMenuItem_Click);
			// 
			// 显示时间ToolStripMenuItem
			// 
			this.显示时间ToolStripMenuItem.Name = "显示时间ToolStripMenuItem";
			this.显示时间ToolStripMenuItem.Size = new System.Drawing.Size(97, 22);
			this.显示时间ToolStripMenuItem.Text = "关于...";
			this.显示时间ToolStripMenuItem.Click += new System.EventHandler(this.关于ToolStripMenuItem_Click);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(94, 6);
			// 
			// 退出ToolStripMenuItem
			// 
			this.退出ToolStripMenuItem.Name = "退出ToolStripMenuItem";
			this.退出ToolStripMenuItem.Size = new System.Drawing.Size(97, 22);
			this.退出ToolStripMenuItem.Text = "退出";
			this.退出ToolStripMenuItem.Click += new System.EventHandler(this.退出ToolStripMenuItem_Click);
			// 
			// timer
			// 
			this.timer.Interval = 1000;
			this.timer.Tick += new System.EventHandler(this.timer_Tick);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.chkHalfHour);
			this.groupBox1.Controls.Add(this.chkSound);
			this.groupBox1.Controls.Add(this.chkHour);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(432, 48);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "设置";
			// 
			// chkHalfHour
			// 
			this.chkHalfHour.AutoSize = true;
			this.chkHalfHour.Checked = true;
			this.chkHalfHour.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkHalfHour.Location = new System.Drawing.Point(94, 20);
			this.chkHalfHour.Name = "chkHalfHour";
			this.chkHalfHour.Size = new System.Drawing.Size(72, 16);
			this.chkHalfHour.TabIndex = 1;
			this.chkHalfHour.Text = "半点报时";
			this.chkHalfHour.UseVisualStyleBackColor = true;
			this.chkHalfHour.CheckedChanged += new System.EventHandler(this.chkSet_CheckedChanged);
			// 
			// chkSound
			// 
			this.chkSound.AutoSize = true;
			this.chkSound.Checked = true;
			this.chkSound.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkSound.Location = new System.Drawing.Point(172, 20);
			this.chkSound.Name = "chkSound";
			this.chkSound.Size = new System.Drawing.Size(72, 16);
			this.chkSound.TabIndex = 2;
			this.chkSound.Text = "声音提示";
			this.chkSound.UseVisualStyleBackColor = true;
			this.chkSound.CheckedChanged += new System.EventHandler(this.chkSet_CheckedChanged);
			// 
			// chkHour
			// 
			this.chkHour.AutoSize = true;
			this.chkHour.Checked = true;
			this.chkHour.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkHour.Location = new System.Drawing.Point(16, 20);
			this.chkHour.Name = "chkHour";
			this.chkHour.Size = new System.Drawing.Size(72, 16);
			this.chkHour.TabIndex = 0;
			this.chkHour.Text = "整点报时";
			this.chkHour.UseVisualStyleBackColor = true;
			this.chkHour.CheckedChanged += new System.EventHandler(this.chkSet_CheckedChanged);
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.divOne);
			this.groupBox2.Controls.Add(this.radioButton4);
			this.groupBox2.Controls.Add(this.radioButton3);
			this.groupBox2.Controls.Add(this.radioButton2);
			this.groupBox2.Controls.Add(this.radioButton1);
			this.groupBox2.Controls.Add(this.btnAdd);
			this.groupBox2.Controls.Add(this.txtMessage);
			this.groupBox2.Controls.Add(this.dtTime);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Location = new System.Drawing.Point(12, 76);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(432, 133);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "添加提醒";
			// 
			// divOne
			// 
			this.divOne.Controls.Add(this.lbl10);
			this.divOne.Controls.Add(this.lbl30);
			this.divOne.Controls.Add(this.lbl60);
			this.divOne.Location = new System.Drawing.Point(231, 46);
			this.divOne.Name = "divOne";
			this.divOne.Size = new System.Drawing.Size(188, 21);
			this.divOne.TabIndex = 7;
			this.divOne.Visible = false;
			// 
			// lbl10
			// 
			this.lbl10.AutoSize = true;
			this.lbl10.Cursor = System.Windows.Forms.Cursors.Hand;
			this.lbl10.ForeColor = System.Drawing.Color.Red;
			this.lbl10.Location = new System.Drawing.Point(3, 5);
			this.lbl10.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
			this.lbl10.Name = "lbl10";
			this.lbl10.Size = new System.Drawing.Size(53, 12);
			this.lbl10.TabIndex = 0;
			this.lbl10.Text = "10分钟后";
			this.lbl10.Click += new System.EventHandler(this.lbl10_Click);
			// 
			// lbl30
			// 
			this.lbl30.AutoSize = true;
			this.lbl30.Cursor = System.Windows.Forms.Cursors.Hand;
			this.lbl30.ForeColor = System.Drawing.Color.Blue;
			this.lbl30.Location = new System.Drawing.Point(62, 5);
			this.lbl30.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
			this.lbl30.Name = "lbl30";
			this.lbl30.Size = new System.Drawing.Size(53, 12);
			this.lbl30.TabIndex = 1;
			this.lbl30.Text = "30分钟后";
			this.lbl30.Click += new System.EventHandler(this.lbl30_Click);
			// 
			// lbl60
			// 
			this.lbl60.AutoSize = true;
			this.lbl60.Cursor = System.Windows.Forms.Cursors.Hand;
			this.lbl60.ForeColor = System.Drawing.Color.Blue;
			this.lbl60.Location = new System.Drawing.Point(121, 5);
			this.lbl60.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
			this.lbl60.Name = "lbl60";
			this.lbl60.Size = new System.Drawing.Size(47, 12);
			this.lbl60.TabIndex = 2;
			this.lbl60.Text = "1小时后";
			this.lbl60.Click += new System.EventHandler(this.lbl60_Click);
			// 
			// radioButton4
			// 
			this.radioButton4.AutoSize = true;
			this.radioButton4.Location = new System.Drawing.Point(214, 21);
			this.radioButton4.Name = "radioButton4";
			this.radioButton4.Size = new System.Drawing.Size(47, 16);
			this.radioButton4.TabIndex = 4;
			this.radioButton4.Text = "每年";
			this.radioButton4.UseVisualStyleBackColor = true;
			this.radioButton4.CheckedChanged += new System.EventHandler(this.radioButton4_CheckedChanged);
			// 
			// radioButton3
			// 
			this.radioButton3.AutoSize = true;
			this.radioButton3.Location = new System.Drawing.Point(161, 21);
			this.radioButton3.Name = "radioButton3";
			this.radioButton3.Size = new System.Drawing.Size(47, 16);
			this.radioButton3.TabIndex = 3;
			this.radioButton3.Text = "每月";
			this.radioButton3.UseVisualStyleBackColor = true;
			this.radioButton3.CheckedChanged += new System.EventHandler(this.radioButton3_CheckedChanged);
			// 
			// radioButton2
			// 
			this.radioButton2.AutoSize = true;
			this.radioButton2.Location = new System.Drawing.Point(108, 21);
			this.radioButton2.Name = "radioButton2";
			this.radioButton2.Size = new System.Drawing.Size(47, 16);
			this.radioButton2.TabIndex = 2;
			this.radioButton2.Text = "每天";
			this.radioButton2.UseVisualStyleBackColor = true;
			this.radioButton2.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
			// 
			// radioButton1
			// 
			this.radioButton1.AutoSize = true;
			this.radioButton1.Checked = true;
			this.radioButton1.Location = new System.Drawing.Point(55, 21);
			this.radioButton1.Name = "radioButton1";
			this.radioButton1.Size = new System.Drawing.Size(47, 16);
			this.radioButton1.TabIndex = 1;
			this.radioButton1.TabStop = true;
			this.radioButton1.Text = "一次";
			this.radioButton1.UseVisualStyleBackColor = true;
			this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
			// 
			// btnAdd
			// 
			this.btnAdd.Location = new System.Drawing.Point(55, 100);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new System.Drawing.Size(57, 23);
			this.btnAdd.TabIndex = 10;
			this.btnAdd.Text = "添 加";
			this.btnAdd.UseVisualStyleBackColor = true;
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			// 
			// txtMessage
			// 
			this.txtMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtMessage.Location = new System.Drawing.Point(55, 73);
			this.txtMessage.Name = "txtMessage";
			this.txtMessage.Size = new System.Drawing.Size(365, 21);
			this.txtMessage.TabIndex = 9;
			// 
			// dtTime
			// 
			this.dtTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dtTime.Location = new System.Drawing.Point(55, 46);
			this.dtTime.Name = "dtTime";
			this.dtTime.ShowUpDown = true;
			this.dtTime.Size = new System.Drawing.Size(170, 21);
			this.dtTime.TabIndex = 6;
			this.dtTime.ValueChanged += new System.EventHandler(this.dtTime_ValueChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(14, 79);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(41, 12);
			this.label3.TabIndex = 8;
			this.label3.Text = "内容：";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(14, 51);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(41, 12);
			this.label2.TabIndex = 5;
			this.label2.Text = "时间：";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(14, 23);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(41, 12);
			this.label1.TabIndex = 0;
			this.label1.Text = "类型：";
			// 
			// listViewAlert
			// 
			this.listViewAlert.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.listViewAlert.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
			this.listViewAlert.FullRowSelect = true;
			this.listViewAlert.GridLines = true;
			this.listViewAlert.Location = new System.Drawing.Point(12, 243);
			this.listViewAlert.MultiSelect = false;
			this.listViewAlert.Name = "listViewAlert";
			this.listViewAlert.Size = new System.Drawing.Size(432, 183);
			this.listViewAlert.TabIndex = 3;
			this.listViewAlert.UseCompatibleStateImageBehavior = false;
			this.listViewAlert.View = System.Windows.Forms.View.Details;
			this.listViewAlert.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listViewAlert_KeyDown);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "类型";
			this.columnHeader1.Width = 50;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "时间";
			this.columnHeader2.Width = 150;
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "内容";
			this.columnHeader3.Width = 220;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(12, 222);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(65, 12);
			this.label4.TabIndex = 2;
			this.label4.Text = "提醒列表：";
			// 
			// btnDel
			// 
			this.btnDel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnDel.Location = new System.Drawing.Point(384, 214);
			this.btnDel.Name = "btnDel";
			this.btnDel.Size = new System.Drawing.Size(57, 23);
			this.btnDel.TabIndex = 5;
			this.btnDel.Text = "删 除";
			this.btnDel.UseVisualStyleBackColor = true;
			this.btnDel.Click += new System.EventHandler(this.btnDel_Click);
			// 
			// btnDelOld
			// 
			this.btnDelOld.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnDelOld.Location = new System.Drawing.Point(307, 214);
			this.btnDelOld.Name = "btnDelOld";
			this.btnDelOld.Size = new System.Drawing.Size(71, 23);
			this.btnDelOld.TabIndex = 4;
			this.btnDelOld.Text = "过期删除";
			this.btnDelOld.UseVisualStyleBackColor = true;
			this.btnDelOld.Click += new System.EventHandler(this.btnDelOld_Click);
			// 
			// frmMain
			// 
			this.AcceptButton = this.btnAdd;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(457, 438);
			this.Controls.Add(this.btnDelOld);
			this.Controls.Add(this.btnDel);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.listViewAlert);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmMain";
			this.Opacity = 0;
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "提醒 V1.0";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.Shown += new System.EventHandler(this.Form1_Shown);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
			this.contextMenuStrip1.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.divOne.ResumeLayout(false);
			this.divOne.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.NotifyIcon notifyAlert;
		private System.Windows.Forms.Timer timer;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem 提醒管理ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem 退出ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem 显示时间ToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem 当前时间ToolStripMenuItem;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox chkHour;
		private System.Windows.Forms.CheckBox chkHalfHour;
		private System.Windows.Forms.CheckBox chkSound;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.DateTimePicker dtTime;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox txtMessage;
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.RadioButton radioButton4;
		private System.Windows.Forms.RadioButton radioButton3;
		private System.Windows.Forms.RadioButton radioButton2;
		private System.Windows.Forms.RadioButton radioButton1;
		private System.Windows.Forms.FlowLayoutPanel divOne;
		private System.Windows.Forms.ListView listViewAlert;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button btnDel;
		private System.Windows.Forms.Button btnDelOld;
		private System.Windows.Forms.Label lbl10;
		private System.Windows.Forms.Label lbl30;
		private System.Windows.Forms.Label lbl60;
		private System.Windows.Forms.ToolStripMenuItem 记事本ToolStripMenuItem;
	}
}

