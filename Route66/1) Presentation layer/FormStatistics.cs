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

        public FormStatistics(Overlay overlay)
        {
            InitializeComponent();
            var statistics = overlay.ComputeStatistics();
            lblTotalDistance.Text = $"Total distance = {statistics.TotalDistance:f0} km.";
            lblTotalDosage.Text = $"Total dosage = {statistics.TotalDosage:f0} kg.";
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
