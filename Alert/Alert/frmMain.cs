using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Media;

namespace Alert
{
	public partial class frmMain : Form
	{
		public frmMain()
		{
			InitializeComponent();
		}

		#region 初始化
		private string FileSetPath = "";
		private string oldString = "";
		private bool isInitFinish = false;
		private void Form1_Load(object sender, EventArgs e)
		{
			FileSetPath = Application.StartupPath + "\\set.txt";
			if (File.Exists(FileSetPath))
			{
				//加载设置
				oldString = File.ReadAllText(FileSetPath);
				string[] strs = oldString.Split('\r');
				this.chkHour.Checked = strs[0].IndexOf("不整点报时") == -1;
				this.chkHalfHour.Checked = strs[0].IndexOf("不半点报时") == -1;
				this.chkSound.Checked = strs[0].IndexOf("不声音提示") == -1;
				Constants.IsHourAlert = this.chkHour.Checked;
				Constants.IsHalfHourAlert = this.chkHalfHour.Checked;
				Constants.IsSound = this.chkSound.Checked;
				Constants.AlertList = new List<TimeAlert>();
				for (int i = 1; i < strs.Length; i++)
				{
					if(strs[i].Trim() == "")
					{
						continue;
					}
					string[] strAlert = strs[i].Trim().Split('|');
					if (strAlert.Length == 3)
					{
						TimeAlert item = new TimeAlert();
						item.Message = strAlert[2];
						item.SplitType = TimeAlert.GetSplitType(strAlert[0]);
						switch (item.SplitType)
						{
							case TimeAlert.SplitTypeEnum.OnlyOne:
								item.AlertTime = DateTime.Parse(strAlert[1]);
								break;
							case TimeAlert.SplitTypeEnum.Day:
								item.AlertTime = DateTime.Parse("2000-01-01 " + strAlert[1]);
								break;
							case TimeAlert.SplitTypeEnum.Month:
								item.AlertTime = DateTime.Parse("2000-01-" + strAlert[1]);
								break;
							case TimeAlert.SplitTypeEnum.Year:
								item.AlertTime = DateTime.Parse("2000-" + strAlert[1]);
								break;
							default:
								break;
						}
						Constants.AlertList.Add(item);
					}
				}
				ResetList();
			}

			ImageList l = new ImageList();
			l.ImageSize = new Size(1, 23);
			this.listViewAlert.SmallImageList = l;

			SelectTime(TimeAlert.SplitTypeEnum.OnlyOne);
			//Constants.AlertList.Add(new TimeAlert("测试提醒！", TimeAlert.SplitTypeEnum.OnlyOne, DateTime.Now.AddSeconds(10)));
			this.timer.Start();

			isInitFinish = true;
		}
		#endregion

		#region 提醒检测
		//每秒检测一次
		private void timer_Tick(object sender, EventArgs e)
		{
			DateTime dtNow = DateTime.Now;
			IList<string> strAlert = new List<string>();
			foreach (TimeAlert r in Constants.AlertList)
			{
				bool isSame = dtNow.Hour == r.AlertTime.Hour && dtNow.Minute == r.AlertTime.Minute && dtNow.Second == r.AlertTime.Second;
				if (!isSame)
				{
					continue;
				}
				switch (r.SplitType)
				{
					case TimeAlert.SplitTypeEnum.OnlyOne:
						if (dtNow.Date == r.AlertTime.Date)
						{
							strAlert.Add(r.Message);
						}
						break;
					case TimeAlert.SplitTypeEnum.Day:
						strAlert.Add(r.Message);
						break;
					case TimeAlert.SplitTypeEnum.Month:
						if (dtNow.Day == r.AlertTime.Day)
						{
							strAlert.Add(r.Message);
						}
						break;
					case TimeAlert.SplitTypeEnum.Year:
						if (dtNow.Month == r.AlertTime.Month && dtNow.Day == r.AlertTime.Day)
						{
							strAlert.Add(r.Message);
						}
						break;
					default:
						break;
				}
			}

			if (strAlert.Count > 0)
			{
				string str = "";
				if (strAlert.Count == 1)
				{
					str = strAlert[0];
				}
				else
				{
					foreach (string r in strAlert)
					{
						if (r != "")
						{
							str += r + "; ";
						}
					}
				}
				if (str == "")
				{
					str = "无";
				}
				ShowMessage(frmBalloonTip.AlertType.Alert, str);
				return;
			}

			//整点提示
			if (Constants.IsHourAlert && dtNow.Minute == 0 && dtNow.Second == 0)
			{
				ShowMessage(frmBalloonTip.AlertType.Time, "");
			}

			//半点提示
			if (Constants.IsHalfHourAlert && dtNow.Minute == 30 && dtNow.Second == 0)
			{
				ShowMessage(frmBalloonTip.AlertType.Time, "");
			}
			
			//测试
			//if (dtNow.Second % 10 == 0)
			//{
			//    ShowMessage(frmBalloonTip.AlertType.Time, "");
			//}
		}

		//显示消息
		frmBalloonTip frmMsg = null;
		SoundPlayer play = new SoundPlayer(Alert.Properties.Resources.msg);
		private void ShowMessage(frmBalloonTip.AlertType alertType, string str)
		{
			if (Constants.IsSound)
			{
				play.Play();
			}

			if (frmMsg == null)
			{
				frmMsg = new frmBalloonTip();
				frmMsg.Init(alertType, str);
				frmMsg.Show();
			}
			else
			{
				frmMsg.Init(alertType, str);
			}
		}
		#endregion

		#region 菜单
		private bool isClose = false;
		private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.notifyAlert.Visible = false;
			this.isClose = true;
			this.Close();
		}

		private void 提醒管理ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ResetList();

			this.Visible = true;
			this.WindowState = FormWindowState.Normal;
			this.Activate();
			if (this.radioButton1.Checked)
			{
				this.dtTime.Value = AfterTime(10);
				this.lbl10.ForeColor = Color.Red;
			}
			this.txtMessage.Focus();
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (!this.isClose && e.CloseReason == CloseReason.UserClosing)
			{
				e.Cancel = true;
				this.Visible = false;
			}
		}

		private void Form1_Shown(object sender, EventArgs e)
		{
			ShowMessage(frmBalloonTip.AlertType.About, "");
			this.Visible = false;
			this.Opacity = 100;
		}

		private void 当前时间ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ShowMessage(frmBalloonTip.AlertType.Time, "");
		}

		private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ShowMessage(frmBalloonTip.AlertType.About, "");
		}

		private void 记事本ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			notifyAlert_DoubleClick(null, null);
		}

		frmNote frmNote = null;
		private void notifyAlert_DoubleClick(object sender, EventArgs e)
		{
			if (frmNote == null)
			{
				frmNote = new frmNote();
				frmNote.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - frmNote.Width, 0);
				frmNote.Show();
			}
			else
			{
				frmNote.Visible = true;
				frmNote.WindowState = FormWindowState.Normal;
				frmNote.Activate();
			}
		}
		#endregion

		#region 添加提醒
		private void radioButton1_CheckedChanged(object sender, EventArgs e)
		{
			if (radioButton1.Checked)
			{
				SelectTime(TimeAlert.SplitTypeEnum.OnlyOne);
			}
		}

		private void radioButton2_CheckedChanged(object sender, EventArgs e)
		{
			if (radioButton2.Checked)
			{
				SelectTime(TimeAlert.SplitTypeEnum.Day);
			}
		}

		private void radioButton3_CheckedChanged(object sender, EventArgs e)
		{
			if (radioButton3.Checked)
			{
				SelectTime(TimeAlert.SplitTypeEnum.Month);
			}
		}

		private void radioButton4_CheckedChanged(object sender, EventArgs e)
		{
			if (radioButton4.Checked)
			{
				SelectTime(TimeAlert.SplitTypeEnum.Year);
			}
		}

		private void SelectTime(Alert.TimeAlert.SplitTypeEnum type)
		{
			DateTime dtNow = DateTime.Now;
			switch (type)
			{
				case TimeAlert.SplitTypeEnum.OnlyOne:
					this.dtTime.MinDate = dtNow;
					this.dtTime.Value = AfterTime(10);
					this.lbl10.ForeColor = Color.Red;
					this.dtTime.CustomFormat = "yyyy年MM月dd日 HH时mm分";
					this.dtTime.Width = 170;
					this.divOne.Visible = true;
					break;
				case TimeAlert.SplitTypeEnum.Day:
					this.dtTime.MinDate = new DateTime(2000, 1, 1);
					this.dtTime.Value = dtNow.Date.AddHours(9).AddMinutes(10);
					this.dtTime.CustomFormat = "HH时mm分";
					this.dtTime.Width = 85;
					this.divOne.Visible = false;
					break;
				case TimeAlert.SplitTypeEnum.Month:
					this.dtTime.MinDate = new DateTime(2000, 1, 1);
					this.dtTime.Value = dtNow.Date.AddHours(9).AddMinutes(10);
					this.dtTime.CustomFormat = "dd日 HH时mm分";
					this.dtTime.Width = 120;
					this.divOne.Visible = false;
					break;
				case TimeAlert.SplitTypeEnum.Year:
					this.dtTime.MinDate = new DateTime(2000, 1, 1);
					this.dtTime.Value = dtNow.Date.AddHours(9).AddMinutes(10);
					this.dtTime.CustomFormat = "MM月dd日 HH时mm分";
					this.dtTime.Width = 140;
					this.divOne.Visible = false;
					break;
				default:
					break;
			}
		}

		private DateTime AfterTime(int minute)
		{
			DateTime dtNow = DateTime.Now;
			return dtNow.AddMinutes(minute).AddSeconds(-dtNow.Second).AddMilliseconds(-dtNow.Millisecond);
		}

		private void lbl10_Click(object sender, EventArgs e)
		{
			this.dtTime.Value = AfterTime(10);
			this.lbl10.ForeColor = Color.Red;
			this.lbl30.ForeColor = Color.Blue;
			this.lbl60.ForeColor = Color.Blue;
		}

		private void lbl30_Click(object sender, EventArgs e)
		{
			this.dtTime.Value = AfterTime(30);
			this.lbl10.ForeColor = Color.Blue;
			this.lbl30.ForeColor = Color.Red;
			this.lbl60.ForeColor = Color.Blue;
		}

		private void lbl60_Click(object sender, EventArgs e)
		{
			this.dtTime.Value = AfterTime(60);
			this.lbl10.ForeColor = Color.Blue;
			this.lbl30.ForeColor = Color.Blue;
			this.lbl60.ForeColor = Color.Red;
		}

		private void dtTime_ValueChanged(object sender, EventArgs e)
		{
			this.lbl10.ForeColor = Color.Blue;
			this.lbl30.ForeColor = Color.Blue;
			this.lbl60.ForeColor = Color.Blue;
		}

		private void btnAdd_Click(object sender, EventArgs e)
		{
			TimeAlert item = new TimeAlert();
			item.Message = this.txtMessage.Text;
			item.AlertTime = this.dtTime.Value;
			if (radioButton1.Checked)
			{
				item.SplitType = TimeAlert.SplitTypeEnum.OnlyOne;
			}
			if (radioButton2.Checked)
			{
				item.SplitType = TimeAlert.SplitTypeEnum.Day;
			}
			if (radioButton3.Checked)
			{
				item.SplitType = TimeAlert.SplitTypeEnum.Month;
			}
			if (radioButton4.Checked)
			{
				item.SplitType = TimeAlert.SplitTypeEnum.Year;
			}
			bool isExist = false;
			foreach (TimeAlert r in Constants.AlertList)
			{
				if (r.SplitType == item.SplitType && r.AlertTime == item.AlertTime)
				{
					if (item.Message == "")
					{
						//如果已经有提醒了，新添提醒内容为空，则不处理
						return;
					}
					if (r.Message == "")
					{
						r.Message = item.Message;
					}
					else
					{
						r.Message += "; " + item.Message;
					}
					isExist = true;
					break;
				}
			}
			if (!isExist)
			{
				Constants.AlertList.Add(item);
			}

			this.txtMessage.Text = "";

			SaveSet();
			ResetList();
		}

		private void ResetList()
		{
			this.listViewAlert.BeginUpdate();
			this.listViewAlert.Items.Clear();
			foreach (TimeAlert r in Constants.AlertList)
			{
				string strTime = "";
				switch (r.SplitType)
				{
					case TimeAlert.SplitTypeEnum.OnlyOne:
						strTime = r.AlertTime.ToString("yyyy年MM月dd日 HH时mm分");
						break;
					case TimeAlert.SplitTypeEnum.Day:
						strTime = r.AlertTime.ToString("HH时mm分");
						break;
					case TimeAlert.SplitTypeEnum.Month:
						strTime = r.AlertTime.ToString("dd日 HH时mm分");
						break;
					case TimeAlert.SplitTypeEnum.Year:
						strTime = r.AlertTime.ToString("MM月dd日 HH时mm分");
						break;
					default:
						break;
				}
				string[] str = { TimeAlert.GetSplitTypeName(r.SplitType), strTime, r.Message };
				ListViewItem item = new ListViewItem(str);
				if (r.SplitType == TimeAlert.SplitTypeEnum.OnlyOne && r.AlertTime < DateTime.Now)
				{
					item.ForeColor = Color.Red;
				}
				this.listViewAlert.Items.Add(item);
			}
			this.listViewAlert.EndUpdate();
		}

		private void btnDel_Click(object sender, EventArgs e)
		{
			if (this.listViewAlert.SelectedItems.Count == 0)
			{
				return;
			}
			Constants.AlertList.RemoveAt(this.listViewAlert.SelectedIndices[0]);
			this.listViewAlert.Items.RemoveAt(this.listViewAlert.SelectedIndices[0]);

			SaveSet();
		}

		private void listViewAlert_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete)
			{
				this.btnDel.PerformClick();
			}
		}

		private void btnDelOld_Click(object sender, EventArgs e)
		{
			int i = Constants.AlertList.RemoveAll(IsOld);
			if (i > 0)
			{
				SaveSet();
				ResetList();
			}
		}

		private static bool IsOld(TimeAlert item)
		{
			return item.SplitType == TimeAlert.SplitTypeEnum.OnlyOne && item.AlertTime < DateTime.Now;
		}

		#endregion

		#region 保存设置
		private void chkSet_CheckedChanged(object sender, EventArgs e)
		{
			if (!isInitFinish)
			{
				return;
			}
			Constants.IsHourAlert = chkHour.Checked;
			Constants.IsHalfHourAlert = chkHalfHour.Checked;
			Constants.IsSound = chkSound.Checked;
			SaveSet();
		}

		private void SaveSet()
		{
			string str = "";
			str += (Constants.IsHourAlert ? "整点报时" : "不整点报时") + ";";
			str += (Constants.IsHalfHourAlert ? "半点报时" : "不半点报时") + ";";
			str += (Constants.IsSound ? "声音提示" : "不声音提示") + ";";
			str += "\r\n";
			foreach (TimeAlert r in Constants.AlertList)
			{
				str += TimeAlert.GetSplitTypeName(r.SplitType) + "|";
				switch (r.SplitType)
				{
					case TimeAlert.SplitTypeEnum.OnlyOne:
						str += r.AlertTime.ToString("yyyy-MM-dd HH:mm");
						break;
					case TimeAlert.SplitTypeEnum.Day:
						str += r.AlertTime.ToString("HH:mm");
						break;
					case TimeAlert.SplitTypeEnum.Month:
						str += r.AlertTime.ToString("dd HH:mm");
						break;
					case TimeAlert.SplitTypeEnum.Year:
						str += r.AlertTime.ToString("MM-dd HH:mm");
						break;
					default:
						break;
				}
				str += "|" + r.Message + "\r\n";
			}
			if (oldString == str)
			{
				return;
			}
			oldString = str;

			File.Delete(FileSetPath);
			File.AppendAllText(FileSetPath, str);
		}
		#endregion
	}
}