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
		#region FIELDS
		#endregion
		#region CONSTRUCTOR
		public FormOptions(Settings set)
		{
			InitializeComponent();
			set.Save();
			Settings = Settings.Load(propertyGrid1, set.fileName); // Set eventhandler.
			propertyGrid1.SelectedObject = Settings;
			propertyGrid1.HelpVisible = Settings.HelpVisible;
		}
		#endregion
		#region PROPERTIES
		public Settings Settings { get; set; }
		#endregion
		#region METHODES
		private void FormOptions_FormClosing(object sender, FormClosingEventArgs e) => propertyGrid1.Dispose();
		#endregion
	}
}
