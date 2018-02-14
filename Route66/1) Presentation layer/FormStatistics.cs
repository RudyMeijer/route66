using GMap.NET;
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
    public partial class FormStatistics : Form
    {
        private Statistics stat;

        public FormStatistics(Overlay overlay)
        {
            InitializeComponent();
            this.Text += $" {overlay.Settings.MachineType}";
            stat = overlay.ComputeStatistics();
            lblDrivingDistance.Text = $"{stat.DrivingDistance:f1} km";
            lblSpreadingDistance.Text = $"{stat.SpreadingDistance:f1} km";
            lblTotalDistance.Text = $"{stat.DrivingDistance + stat.SpreadingDistance:f1} km";
            lblUptolastDistance.Text = $"{stat.UptoLastDistance:f1} km";

            lblTotalAmount.Text = $"{stat.Dosage:f0} kg";
            lblArea.Text = $"{stat.Area:f3} m2";
            numSpeed_ValueChanged(null,null);
            numPersentage_ValueChanged(null, null);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void numSpeed_ValueChanged(object sender, EventArgs e)
        {
            var speed = (double)numSpeed.Value;
            lblDrivingTime.Text = $"{Hour(stat.DrivingDistance / speed)} h";
            lblSpreadingTime.Text = $"{Hour(stat.SpreadingDistance / speed)} h";
            lblTotalTime.Text = $"{Hour((stat.DrivingDistance + stat.SpreadingDistance) / speed)} h";
            lblUptoLastTime.Text = $"{Hour(stat.UptoLastDistance / speed)} h";
        }

        private string Hour(double hour) => new DateTime().AddHours(hour).ToString("HH:mm");

        private void numPersentage_ValueChanged(object sender, EventArgs e)
        {
            var persentage = (double)(numPersentage.Value / 100);
            lblDry.Text = $"{stat.Dosage * (1 - persentage):f0} kg";
            lblWet.Text = $"{stat.Dosage * (persentage):f0} kg";
        }
    }
}
