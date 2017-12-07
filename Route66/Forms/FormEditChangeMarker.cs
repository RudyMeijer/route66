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
			Settings = Settings.Global;
			this.Text += $" {Settings.MachineType}";
		}

		private void FormEditChangeMarker_Load(object sender, EventArgs e)
		{
			InitializeFormLayout(Settings.MachineType);
			DisplayOnForm(ChangeMarker);
		}
		#endregion
		/// <summary>
		/// Determine which fields must be shown according machineType.
		/// 
		/// See http://confluence.ash.ads.org/pages/resumedraft.action?draftId=12451841&draftShareId=4fe65ae2-5191-4570-9e32-ad841719fd5c
		/// </summary>
		/// <param name="machineType"></param>
		private void InitializeFormLayout(MachineTypes machineType)
		{
			bool IsSprayer = machineType == MachineTypes.Sprayer;
			// Row 1
			chkSpreading.Visible = !IsSprayer;
			chkDualWidth.Visible = machineType == MachineTypes.WspDosage;
			chkSpraying.Visible = machineType == MachineTypes.WspDosage || machineType == MachineTypes.RspDosage || IsSprayer;
			chkPump.Visible = false;
			// Row 2
			grpDosage.Visible = !IsSprayer;
			grpMax.Visible = true;
			grpSecMat.Visible = !IsSprayer;
			grpSecLiquid.Visible = machineType == MachineTypes.WspPercentage || machineType == MachineTypes.RspPercentage || machineType == MachineTypes.WspDosage || machineType == MachineTypes.RspDosage;
			grpDosageLiquid.Visible = chkSpraying.Visible;
			grpHopper.Visible = machineType == MachineTypes.Dst;
			// Row 3
			grpSpreadingWidth.Visible = !IsSprayer;
			grpSprayingWidth.Visible = machineType == MachineTypes.WspDosage || IsSprayer;
			// Center visible controls.
			FormEditChangeMarker_Resize(null, null);
		}
		private int SetLeftMargin(FlowLayoutPanel flowLayoutPanel)
		{
			var width = 0;
			var marge = 8;
			foreach (var item in flowLayoutPanel.Controls)
			{
				//if (item is GroupBox && (item as GroupBox).Visible )
					if ((item as Control).Visible && !(item is Label))
					width += (item as Control).Width + marge;
			}
			return (flowLayoutPanel.Width - marge - width) / 2;
		}
		/// <summary>
		/// This methode displays Changemarker (xml) onto the form.
		/// If this methode changes then also update methode GetFromForm!
		/// </summary>
		/// <param name="cm"></param>
		private void DisplayOnForm(ChangeMarker cm)
		{
			// Row 1
			chkSpreading.Checked = cm.SpreadingOnOff;
			chkDualWidth.Checked = cm.DualWidthOnOff;
			chkSpraying.Checked = cm.SprayingOnOff;
			chkPump.Checked = cm.PumpOnOff;
			// Row 2
			numDosage.Value = (decimal)cm.Dosage;
			chkMaxOnOff.Checked = cm.MaxOnOff;
			chkSecMatOnOff.Checked = cm.SecMatOnOff;
			numSecLiquid.Value = (decimal)cm.SecLiquid;
			numDosageLiquid.Value = (decimal)cm.DosageLiquid;
			chkHopper1OnOff.Checked = cm.Hopper1OnOff;
			chkHopper2OnOff.Checked = cm.Hopper2OnOff;
			// Row 3
			numSpreadingWidthLeft.Value = (decimal)cm.SpreadingWidthLeft;
			numSpreadingWidthRight.Value = (decimal)cm.SpreadingWidthRight;

			numSprayingWidthLeft.Value = (decimal)cm.SprayingWidthLeft;
			numSprayingWidthRight.Value = (decimal)cm.SprayingWidthRight;
		}
		/// <summary>
		/// This methode is the complement of previous methode DisplayOnForm.
		/// </summary>
		/// <param name="cm"></param>
		private void GetFromForm(ChangeMarker cm)
		{
			// Row 1
			cm.SpreadingOnOff = chkSpreading.Checked;
			cm.DualWidthOnOff = chkDualWidth.Checked;
			cm.SprayingOnOff = chkSpraying.Checked;
			cm.PumpOnOff = chkPump.Checked;
			// Row 2
			cm.Dosage = (double)numDosage.Value;
			cm.MaxOnOff = chkMaxOnOff.Checked;
			cm.SecMatOnOff = chkSecMatOnOff.Checked;
			cm.SecLiquid = (double)numSecLiquid.Value;
			cm.DosageLiquid = (double)numDosageLiquid.Value;
			cm.Hopper1OnOff = chkHopper1OnOff.Checked;
			cm.Hopper2OnOff = chkHopper2OnOff.Checked;
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
		private void btnRemove_Click(object sender, EventArgs e)
		{
			marker.Tag = null;
			IsButton = true;
			this.Close();
		}
		private void FormEditChangeMarker_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (!IsButton) marker.Tag = originalTag;
		}

		private void numSpreadingWidthLeftRight_ValueChanged(object sender, EventArgs e)
		{
			lblSpreadingTotalWidth.Text = $"{numSpreadingWidthLeft.Value + numSpreadingWidthRight.Value} m";
			if (chkDualWidth.Enabled && !chkDualWidth.Checked)
			{
				numSprayingWidthLeft.Value = numSpreadingWidthLeft.Value;
				numSprayingWidthRight.Value = numSpreadingWidthRight.Value;
			}
		}

		private void numSprayingWidthLeftRight_ValueChanged(object sender, EventArgs e)
		{
			lblSprayingTotalWidth.Text = $"{numSprayingWidthLeft.Value + numSprayingWidthRight.Value} m";
		}

		private void chkMaxOnOff_CheckedChanged(object sender, EventArgs e)
		{
			var c = (sender as CheckBox);
			var n = c.Name;
			c.Text = (c.Checked) ? "ON" : "OFF";
		}

		private void FormEditChangeMarker_Resize(object sender, EventArgs e)
		{
			lblMargeRow1.Width = SetLeftMargin(flowLayoutPanel1);
			lblMargeRow2.Width = SetLeftMargin(flowLayoutPanel2);
			lblMargeRow3.Width = SetLeftMargin(flowLayoutPanel3);
		}
		#region ENABLE DISABLE FIELDS
		private void chkSpreading_CheckedChanged(object sender, EventArgs e)
		{
			var on = (sender as CheckBox).Checked;
			grpDosage.Enabled = on;
			grpSecMat.Enabled = on;
			grpSecLiquid.Enabled = on && chkSecMatOnOff.Checked;
			grpHopper.Enabled = on;
			grpSpreadingWidth.Enabled = on;
			chkDualWidth.Enabled = on && chkSpraying.Checked == true;
			// Make Spreading and Spraying mutual exclusieve on RspDosage.
			if (on && Settings.MachineType == MachineTypes.RspDosage) chkSpraying.Checked = false;
		}
		private void chkSpraying_CheckedChanged(object sender, EventArgs e)
		{
			var on = (sender as CheckBox).Checked;
			grpDosageLiquid.Enabled = on;
			grpSprayingWidth.Enabled = on;
			chkDualWidth.Enabled = on && chkSpreading.Checked == true;
			// Make Spreading and Spraying mutual exclusieve on RspDosage.
			if (on && Settings.MachineType == MachineTypes.RspDosage) chkSpreading.Checked = false;
		}

		private void chkSecMatOnOff_CheckedChanged(object sender, EventArgs e)
		{
			chkMaxOnOff_CheckedChanged(sender, e);
			var on = (sender as CheckBox).Checked;
			grpSecLiquid.Enabled = on;
		}

		private void chkDualWidth_CheckedChanged(object sender, EventArgs e)
		{
			grpSprayingWidth.Enabled = chkDualWidth.Checked;
		}
		#endregion
	}
}