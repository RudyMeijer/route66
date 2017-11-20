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
        private readonly object originalTag;
        private bool IsButton;

        public ChangeMarker ChangeMarker { get; set; }

        public FormEditChangeMarker(GMapMarker marker)
        {
            InitializeComponent();
            this.marker = marker;
            originalTag = marker.Tag;
            if (marker.Tag == null) marker.Tag = new ChangeMarker(marker.Position);
            ChangeMarker = marker.Tag as ChangeMarker;
            DisplayOnForm(ChangeMarker);
        }

        private void DisplayOnForm(ChangeMarker changeMarker)
        {
            numDosing.Value = (decimal)changeMarker.Dosing;
            numWidthLeft.Value = (decimal)changeMarker.WidthLeft;
            numWidthRight.Value = (decimal)changeMarker.WidthRight;
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            ChangeMarker.Dosing = (double)numDosing.Value;
            ChangeMarker.WidthLeft = (double)numWidthLeft.Value;
            ChangeMarker.WidthRight = (double)numWidthRight.Value;
            IsButton = true;
            // marker.ToolTipText = ChangeMarker.ToString();
            this.Close();
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            marker.Tag = null;
            IsButton = true;
            //marker.ToolTipText = "";
            this.Close();
        }
        private void FormEditChangeMarker_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!IsButton) marker.Tag = originalTag;
        }
    }
}
