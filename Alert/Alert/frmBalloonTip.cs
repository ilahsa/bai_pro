using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Media;

namespace Alert
{
	public partial class frmBalloonTip : Form
	{
		public frmBalloonTip()
		{
			InitializeComponent();
		}

		public enum AlertType
		{ 
			/// <summary>
			/// 关于
			/// </summary>
			About,

			/// <summary>
			/// 报时
			/// </summary>
			Time,

			/// <summary>
			/// 提醒
			/// </summary>
			Alert
		}

		private string[] Weeks = { "日", "一", "二", "三", "四", "五", "六" };
		public void Init(AlertType alertType, string message)
		{
			this.divAbout.Visible = false;
			this.divTime.Visible = false;
			this.divAlert.Visible = false;

			Panel div = null;
			switch (alertType)
			{
				case AlertType.About:
                    var str1= DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                    byte[] u1 = Encoding.UTF8.GetBytes(str1);
                    this.lblAboutTime.Text = UTF8Encoding.UTF8.GetString(u1);
					div = this.divAbout;
					break;
				case AlertType.Time:
                    var str2 = DateTime.Now.ToString("yyyy-MM-dd ") ;

                    this.lblDate.Text = str2;
                    this.lblTime.Text = DateTime.Now.ToString("HH:mm");
					div = this.divTime;
					break;
				case AlertType.Alert:
                    var str3 = DateTime.Now.ToString("yyyy-MM-dd- HH:mm");
                    byte[] u3 = Encoding.UTF8.GetBytes(str3);
                    this.lblAlertTime.Text = UTF8Encoding.UTF8.GetString(u3);
                    this.lblInfo.Text = message;
					div = this.divAlert;
					break;
				default:
					break;
			}

			//显示屏幕右下角
			this.Width = div.Width + 10;
			this.Height = div.Height + 10;
			this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - this.Width - 10, Screen.PrimaryScreen.WorkingArea.Height - this.Height - 10);
			div.Visible = true;
			div.Dock = DockStyle.Fill;
			div.BorderStyle = BorderStyle.None;

			this.Opacity = 0.9;
			this.isClose = false;
			this.timerClose.Interval = 5000;
			this.timerClose.Start();
			//this.Visible = true;
		}

		private bool isClose = false;
		private void timer1_Tick(object sender, EventArgs e)
		{
			//当鼠标在提示窗口上时，不关闭窗口
			Rectangle rect = new Rectangle(this.Location, new Size(this.Width, this.Height));
			if (rect.Contains(Control.MousePosition))
			{
				return;
			}
			
			if (!isClose)
			{
				//准备关闭窗口
				this.timerClose.Interval = 100;
				this.timerClose.Start();
				this.isClose = true;
			}
			else
			{
				//渐隐窗口
				this.Opacity -= 0.1;
				if (this.Opacity <= 0)
				{
					//this.Close();
					//this.Dispose();
					this.timerClose.Stop();
					this.Location = new Point(this.Size.Width * -1, this.Location.Y);
				}
			}
		}

		private void linkAbout_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			//System.Diagnostics.Process.Start("http://zjfree.cnblogs.com");
		}
	}
}