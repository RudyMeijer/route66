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
			Settings = Settings.Global;
			InitializeFormLayout(Settings.MachineType);
		}

		private void InitializeFormLayout(MachineTypes machineType)
		{
			bool IsSprayer = machineType == MachineTypes.Sprayer;
			// Row 1
			chkSpreading.Visible = !IsSprayer;
			chkDualWidth.Visible = false;
			chkSpraying.Visible = IsSprayer;
			chkMode.Visible = false;
			chkPump.Visible = false;
			// Row 2
			grpDosage.Visible = !IsSprayer;
			grpMax.Visible = true;
			grpSecMat.Visible = !IsSprayer;
			grpSecLiquid.Visible = machineType == MachineTypes.WspDosage || machineType == MachineTypes.RspDosage || IsSprayer;
			grpSecDosage.Visible = grpSecLiquid.Visible;
			grpHopper.Visible = machineType == MachineTypes.Dst;
			// Row 3
			grpSpreadingWidth.Visible = !IsSprayer;
			grpSprayingWidth.Visible = IsSprayer;
		}
		#endregion
		private void DisplayOnForm(ChangeMarker cm)
		{
			// Row 1
			chkSpreading.Checked = cm.SpreadingOnOff;
			chkDualWidth.Checked = cm.DualWidthOnOff;
			chkSpraying.Checked = cm.SprayingOnOff;
			chkMode.Checked = cm.ModeOnOff;
			chkPump.Checked = cm.PumpOnOff;
			// Row 2
			numDosage.Value = (decimal)cm.Dosage;
			chkMaxOnOff.Checked = cm.MaxOnOff;
			chkSecMatOnOff.Checked = cm.SecMatOnOff;
			numSecLiquid.Value = (decimal)cm.SecLiquid;
			numSecDosage.Value = (decimal)cm.SecDosage;
			chkHopperOnOff.Checked = cm.HopperOnOff;
			// Row 3
			numSpreadingWidthLeft.Value = (decimal)cm.SpreadingWidthLeft;
			numSpreadingWidthRight.Value = (decimal)cm.SpreadingWidthRight;

			numSprayingWidthLeft.Value = (decimal)cm.SprayingWidthLeft;
			numSprayingWidthRight.Value = (decimal)cm.SprayingWidthRight;
		}
		private void GetFromForm(ChangeMarker cm)
		{
			// Row 1
			cm.SpreadingOnOff = chkSpreading.Checked;
			cm.DualWidthOnOff = chkDualWidth.Checked;
			cm.SprayingOnOff = chkSpraying.Checked;
			cm.ModeOnOff = chkMode.Checked;
			cm.PumpOnOff = chkPump.Checked;
			// Row 2
			cm.Dosage = (double)numDosage.Value;
			cm.MaxOnOff = chkMaxOnOff.Checked;
			cm.SecMatOnOff = chkSecMatOnOff.Checked;
			cm.SecLiquid = (double)numSecLiquid.Value;
			cm.SecDosage = (double)numSecDosage.Value;
			cm.HopperOnOff = chkHopperOnOff.Checked;
			// Row 3
			cm.SpreadingWidthLeft = (double)numSpreadingWidthLeft.Value;
			cm.SpreadingWidthRight = (double)numSpreadingWidthRight.Value;

			cm.SprayingWidthLeft = (double)numSprayingWidthLeft.Value;
			cm.SprayingWidthRight = (double)numSprayingWidthRight.Value;
		}
		private void btnSave_Click(object sender, EventArgs e)
		{
			GetFromForm(ChangeMarker);
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

		//private void button1_Click(object sender, EventArgs e)
		//{
		//}

		//private void checkBox1_CheckedChanged(object sender, EventArgs e)
		//{
		//	grpAction.Visible = (sender as CheckBox).Checked;
		//}

		//private void checkBox2_CheckedChanged(object sender, EventArgs e)
		//{
		//	var c = sender as CheckBox;
		//	c.Text = (c.Checked) ? "ON" : "OFF";
		//}

		private void numSpreadingWidthLeft_ValueChanged(object sender, EventArgs e)
		{
			lblSpreadingTotalWidth.Text = $"{numSpreadingWidthLeft.Value + numSpreadingWidthRight.Value} m";
		}

		private void numSprayingWidthLeft_ValueChanged(object sender, EventArgs e)
		{
			lblSprayingTotalWidth.Text = $"{numSprayingWidthLeft.Value + numSprayingWidthRight.Value} m";
		}

		private void chkMaxOnOff_CheckedChanged(object sender, EventArgs e)
		{
			var c = (sender as CheckBox);
			var n = c.Name;
			c.Text = (c.Checked) ? "ON" : "OFF";
		}
	}
}