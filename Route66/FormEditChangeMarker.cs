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
        private GMapMarker marker;

        public FormEditChangeMarker(GMapMarker marker)
        {
            InitializeComponent();
            this.marker = marker;
            if (marker.Tag == null) marker.Tag = new ChangeMarker();
            ChangeMarker = marker.Tag as ChangeMarker;
            DisplayOnForm(ChangeMarker);
            ChangeMarker.Lat = marker.Position.Lat;
            ChangeMarker.Lng = marker.Position.Lng;
        }

        private void DisplayOnForm(ChangeMarker changeMarker)
        {
            numDosing.Value = (decimal)changeMarker.Dosing;
            numWidthLeft.Value = (decimal)changeMarker.WidthLeft;
            numWidthRight.Value = (decimal)changeMarker.WidthRight;
        }

        public ChangeMarker ChangeMarker { get; set; }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            marker.Tag = null;
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            ChangeMarker.Dosing = (double)numDosing.Value;
            ChangeMarker.WidthLeft = (double)numWidthLeft.Value;
            ChangeMarker.WidthRight = (double)numWidthRight.Value;
            marker.ToolTipText = ChangeMarker.ToString();
            this.Close();
        }
    }
}
