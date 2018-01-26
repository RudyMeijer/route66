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
		#region CONSTRUCTOR
		public FormOptions(Settings settings)
		{
			InitializeComponent();
			settings.Save();
			//Settings = Settings.Load(propertyGrid1, set.fileName); // Set eventhandler.
			propertyGrid1.PropertyValueChanged += Settings.propertygrid_PropertyValueChanged;
			propertyGrid1.SelectedObject = settings;
			propertyGrid1.HelpVisible = settings.HelpVisible;
		}
		#endregion
		#region PROPERTIES
		public Settings Settings { get; set; }
		#endregion
		#region METHODES
		private void FormOptions_FormClosing(object sender, FormClosingEventArgs e) => propertyGrid1.Dispose();
		private void btnOk_Click(object sender, EventArgs e)
		{
			this.Close();
		}
		#endregion
	}
}
