using GMap.NET.WindowsForms;
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
	public partial class FormEditChangeMarker : Form
	{
		#region FIELDS
		private GMapMarker marker;
		private readonly object originalTag;
		private bool IsButton;

		public ChangeMarker ChangeMarker { get; set; }
		public Settings Settings { get; private set; }
		#endregion
		#region CONSTRUCTOR
		public FormEditChangeMarker(GMapMarker marker)
		{
			InitializeComponent();
			this.marker = marker;
			originalTag = marker.Tag;
			if (marker.Tag == null) marker.Tag = new ChangeMarker(marker.Position);
			ChangeMarker = marker.Tag as ChangeMarker;
			DisplayOnForm(ChangeMarker);
			this.Settings = Settings.Global; 
		}
		#endregion
		private void DisplayOnForm(ChangeMarker changeMarker)
		{
			numDosing.Value = (decimal)changeMarker.Dosing;
			numWidthLeft.Value = (decimal)changeMarker.WidthLeft;
			numWidthRight.Value = (decimal)changeMarker.WidthRight;
		}
		private void GetFromForm()
		{
			ChangeMarker.Dosing = (double)numDosing.Value;
			ChangeMarker.WidthLeft = (double)numWidthLeft.Value;
			ChangeMarker.WidthRight = (double)numWidthRight.Value;
		}
		private void btnSave_Click(object sender, EventArgs e)
		{
			GetFromForm();
			IsButton = true;
			this.Close();
		}

		private void btnDelete_Click(object sender, EventArgs e)
		{
			marker.Tag = null;
			IsButton = true;
			this.Close();
		}
		private void FormEditChangeMarker_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (!IsButton) marker.Tag = originalTag;
		}

		private void button1_Click(object sender, EventArgs e)
		{
		}

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			grpAction.Visible = (sender as CheckBox).Checked;
		}

		private void checkBox2_CheckedChanged(object sender, EventArgs e)
		{
			var c= sender as CheckBox;
			c.Text = (c.Checked) ? "ON" : "OFF";
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{

		}
	}
}
