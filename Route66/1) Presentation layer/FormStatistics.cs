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
            stat = overlay.ComputeStatistics();
            lblDrivingDistance.Text = $"{stat.DrivingDistance:f0} km.";
            lblSpreadingDistance.Text = $"{stat.SpreadingDistance:f0} km.";
            lblTotalDistance.Text = $"{stat.DrivingDistance+stat.SpreadingDistance:f0} km.";
            lblUptolastDistance.Text = $"{stat.UptoLastDistance:f0} kg.";
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void numSpeed_ValueChanged(object sender, EventArgs e)
        {
            var speed = (double)numSpeed.Value;
            lblDrivingTime.Text = $"{(stat.DrivingDistance/speed).ToString("hh:mm")} h.";
            lblSpreadingTime.Text = $"{stat.SpreadingDistance/speed:hh:mm} h.";
            lblTotalTime.Text = $"{stat.DrivingDistance + stat.SpreadingDistance/speed:hh:mm} h.";
            lblUptoLastTime.Text = $"{stat.UptoLastDistance/speed:hh:mm} h.";

            lblTotalAmount.Text = $"{stat.Dosage:f0} kg.";
            lblArea.Text = $"{stat.Area:f0} m2.";
        }

        private void numPersentage_ValueChanged(object sender, EventArgs e)
        {
            var persentage = (double)(numPersentage.Value/100);
            lblDry.Text = $"{stat.Dosage*(1-persentage):f0} kg.";
            lblWet.Text = $"{stat.Dosage*(persentage):f0} kg.";
        }
    }
}
