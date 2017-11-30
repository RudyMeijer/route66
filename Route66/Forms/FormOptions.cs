using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Route66
{
	public partial class FormOptions : Form
	{
		public Settings Settings { get; set; }

		public FormOptions(Settings set)
		{
			InitializeComponent();
			set.Save();
			this.Settings = Settings.Global;//.Load(propertyGrid1, set.fileName); // Set eventhandler.
			this.propertyGrid1.SelectedObject = Settings;
			this.propertyGrid1.HelpVisible = Settings.HelpVisible;
		}

		private void FormOptions_FormClosing(object sender, FormClosingEventArgs e)
		{
			propertyGrid1.Dispose();
		}
	}
}
