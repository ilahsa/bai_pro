using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Alert
{
	public partial class frmNote : Form
	{
		public frmNote()
		{
			InitializeComponent();
		}

		private string FilePath = "";
		private string oldString = "";
		private void frmNote_Load(object sender, EventArgs e)
		{
			FilePath = Application.StartupPath + "\\data.txt";
			if (File.Exists(FilePath))
			{
				this.txtNote.Text = File.ReadAllText(FilePath);
				oldString = this.txtNote.Text;
			}
		}

		private void SaveFile()
		{
			if (oldString == this.txtNote.Text)
			{
				return;
			}
			File.Delete(FilePath);
			File.AppendAllText(FilePath, this.txtNote.Text);
			oldString = this.txtNote.Text;
		}

		private void frmNote_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (e.CloseReason == CloseReason.UserClosing)
			{
				this.Visible = false;
				e.Cancel = true;
			}
			SaveFile();
		}

		private void frmNote_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Control && e.KeyCode == Keys.S)
			{
				SaveFile();
			}
		}
	}
}