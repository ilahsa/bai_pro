namespace Alert
{
	partial class frmNote
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
			this.txtNote = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// txtNote
			// 
			this.txtNote.AcceptsReturn = true;
			this.txtNote.AcceptsTab = true;
			this.txtNote.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtNote.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.txtNote.Location = new System.Drawing.Point(0, 0);
			this.txtNote.Multiline = true;
			this.txtNote.Name = "txtNote";
			this.txtNote.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtNote.Size = new System.Drawing.Size(477, 358);
			this.txtNote.TabIndex = 0;
			// 
			// frmNote
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(477, 358);
			this.Controls.Add(this.txtNote);
			this.MaximizeBox = false;
			this.Name = "frmNote";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "笔记本";
			this.TopMost = true;
			this.Load += new System.EventHandler(this.frmNote_Load);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmNote_FormClosing);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmNote_KeyDown);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox txtNote;
	}
}