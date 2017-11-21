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
using System.IO;

namespace Route66
{
    public partial class Form1 : Form
    {
        #region FIELDS
        private Overlay Overlay;
        private bool IsDragging; // Moving with Left mouse button pressed.
        private bool IsOnMarker; // Mouse is on a Marker.
        private readonly string Title;

        public Settings Settings { get; set; }
        public Route Route { get; set; }
        #endregion
        #region CONSTRUCTOR
        public Form1()
        {
            InitializeComponent();
            Title = this.Text += My.Version + " ";
            My.Log($"Start {Title}");
            My.SetStatus(toolStripStatusLabel1);
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
            var idx = GetIndex(Settings.MapProvider);
            if (idx == 6) gmap.Zoom = 10;// PositionByKeywords("lng:-74.696044921875 lat:22.857194700969629");
            comboBox1.SelectedIndex = idx;
            gmap.Refresh();
        }

        private int GetIndex(string mapProvider)
        {
            for (int i = 0; i < comboBox1.Items.Count; i++)
                if (comboBox1.Items[i].GetType().Name.Contains(Settings.MapProvider))
                    return i;
            return 0;
        }

        private void InitializeOverlays()
        {
            Overlay = new Overlay(gmap);
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
            gmap.SetPositionByKeywords(txtSearchPlaces.Text);
            Console.WriteLine($"{txtSearchPlaces.Text} at {gmap.Position}");
            //gmap.ShowCenter = false;
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            gmap.MapProvider = comboBox1.SelectedItem as GMapProvider;
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Settings.Save();
        }

        #endregion
        #region EDIT ROUTE
        private void gmap_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && !IsOnMarker) { Overlay.AddMarker(e.X, e.Y); }
            if (e.Button == MouseButtons.Right && IsOnMarker) { Overlay.RemoveCurrentMarker(); IsOnMarker = false; }
            if (IsOnMarker && e.Clicks == 2) { Overlay.EditMarker(e); }
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
        /// <summary>
        /// Clear status bar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gmap_MouseLeave(object sender, EventArgs e) => My.Status("Ready");
        #endregion
        #region MENU ITEMS
        private void OptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = new FormOptions(Settings);
            f.ShowDialog();
            Settings = f.Settings;
            InitializeSettings();
        }
        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = Settings.RoutePath;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Route = Route.Load(openFileDialog1.FileName);
                Overlay.Load(Route);
                this.Text = Title + openFileDialog1.FileName;
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
                Overlay.CopyTo(Route);
                Route.SaveAs(saveFileDialog1.FileName);
            }
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Overlay.CopyTo(Route);
            if (Route.IsDefaultFile) Route.SaveAs(Path.Combine(Settings.RoutePath, "Route66.xml"));
            else Route.Save();
        }
        #endregion
        #region RIGHT PANE
        private void button1_Click(object sender, EventArgs e)
        {
            Overlay.Clear();
            IsOnMarker = IsDragging = false;
        }
        #region SEARCH PLACES
        private void textBox1_Validated(object sender, EventArgs e) => gmap.SetPositionByKeywords(txtSearchPlaces.Text);

        private void textBox1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) textBox1_Validated(null, null);
        }
        #endregion
        private void chkGpsPoints_CheckedChanged(object sender, EventArgs e)
        {
            foreach (var item in gmap.Overlays[0].Markers)
            {
                item.IsVisible = chkGpsPoints.Checked;
            }
            gmap.Refresh();
        }
        private void chkChangePoints_CheckedChanged(object sender, EventArgs e)
        {
            gmap.Overlays[1].IsVisibile = chkChangePoints.Checked;
            gmap.Refresh();
        }
        private void chkNavPoints_CheckedChanged(object sender, EventArgs e)
        {
            gmap.Overlays[2].IsVisibile = chkNavPoints.Checked;
            gmap.Refresh();

        }
        private void chkShowTooltip_CheckedChanged(object sender, EventArgs e)
        {
            Overlay.SetTooltipOnOff(chkShowTooltip.Checked);
        }
        #endregion
    }
}
