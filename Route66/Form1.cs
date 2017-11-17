using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET.ObjectModel;

namespace Route66
{
    public partial class Form1 : Form
    {
        #region FIELDS
        private Overlay Overlay;
        private bool IsDragging; // Moving with Left mouse button pressed.
        private bool IsOnMarker; // Mouse is on a Marker.
        public Settings Settings { get; set; }
        public Route Route { get; set; }
        #endregion
        #region CONSTRUCTOR
        public Form1()
        {
            InitializeComponent();

            My.Log($"Start {this.Text += My.Version}");
            Settings = Settings.Load();
            Route = Route.Load();
        }
        #endregion
        #region INITIALIZE
        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeGmap();
            InitializeOverlays();
            InitializeComboboxWithMapProviders();
            InitializeSettings();
        }

        private void InitializeSettings()
        {
            Route.MachineType = Settings.MachineType;
        }

        private void InitializeOverlays()
        {
            Overlay = new Overlay(gmap, "GpsPoints");
        }

        private void InitializeComboboxWithMapProviders()
        {
            //var x = GMap.NET.MapProviders.
            comboBox1.Items.Clear();
            comboBox1.Items.Add(BingMapProvider.Instance);
            comboBox1.Items.Add(GoogleMapProvider.Instance);
            comboBox1.Items.Add(GoogleSatelliteMapProvider.Instance);
            comboBox1.Items.Add(GoogleTerrainMapProvider.Instance);
            comboBox1.Items.Add(OpenCycleMapProvider.Instance);
            comboBox1.Items.Add(WikiMapiaMapProvider.Instance);
            comboBox1.Items.Add(BingHybridMapProvider.Instance);
            comboBox1.Items.Add(OpenStreetMapProvider.Instance);
            comboBox1.SelectedIndex = 0;
        }

        private void InitializeGmap()
        {
            //
            // See http://www.independent-software.com/gmap-net-beginners-tutorial-maps-markers-polygons-routes-updated-for-visual-studio-2015-and-gmap-net-1-7/
            //
            gmap.MapProvider = BingMapProvider.Instance;
            GMaps.Instance.Mode = AccessMode.ServerOnly;
            gmap.Zoom = 13;
            gmap.SetPositionByKeywords(textBox1.Text);
            Console.WriteLine($"{textBox1.Text} at {gmap.Position}");
            //gmap.ShowCenter = false;
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            gmap.MapProvider = comboBox1.SelectedItem as GMapProvider;
        }
        #endregion
        private void button1_Click(object sender, EventArgs e)
        {
            Overlay.Clear();
            IsOnMarker = IsDragging = false;
        }
        #region SEARCH PLACES
        private void textBox1_Validated(object sender, EventArgs e) => gmap.SetPositionByKeywords(textBox1.Text);

        private void textBox1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) textBox1_Validated(null, null);
        }
        #endregion
        #region EDIT ROUTE
        private void gmap_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && !IsOnMarker) { Overlay.AddMarker(e.X, e.Y); }
            if (e.Button == MouseButtons.Right && IsOnMarker) { Overlay.RemoveCurrentMarker(); }
        }
        private void gmap_OnMarkerLeave(GMapMarker item)
        {
            if (!IsDragging) IsOnMarker = false;
        }
        private void gmap_OnMarkerEnter(GMapMarker item)
        {
            if (!IsDragging)
            {
                IsOnMarker = true;
                Overlay.SetCurrentMarker(item);
            }
        }
        private void gmap_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && IsOnMarker)
            {
                IsDragging = true;
                Overlay.UpdateCurrentMarkerPosition(e.X, e.Y);
            }
        }
        private void gmap_MouseUp(object sender, MouseEventArgs e) => IsDragging = false;
        #endregion
        private void chkGpsPoints_CheckedChanged(object sender, EventArgs e)
        {
            gmap.MarkersEnabled = chkGpsPoints.Checked;
            gmap.Refresh();
        }

        private void chkShowTooltip_CheckedChanged(object sender, EventArgs e)
        {
            Overlay.SetTooltipOnOff(chkShowTooltip.Checked);
        }

        #region MENU ITEMS
        private void OptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = new FormOptions(Settings);
            f.ShowDialog();
            Settings = f.Settings;
            InitializeSettings();
        }
        #endregion


        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Settings.Save();
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //openFileDialog1.InitialDirectory = Settings.RoutePath;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Route = Route.Load(openFileDialog1.FileName);
                Overlay.Load(Route);
            }
        }
        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.InitialDirectory = openFileDialog1.InitialDirectory;
            saveFileDialog1.FileName = openFileDialog1.FileName;
            saveFileDialog1.Filter = openFileDialog1.Filter;
            saveFileDialog1.DefaultExt = openFileDialog1.DefaultExt;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Overlay.Save(Route);
                Route.SaveAs(saveFileDialog1.FileName);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Overlay.Save(Route);
            Route.Save();
        }
    }

}
