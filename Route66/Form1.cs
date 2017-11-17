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
        private Overlay Overlay;
        #region FIELDS
        //private GMapOverlay mOverlay;
        //private GMapRoute mRoute;
        //private GMapMarker mCurrentMarker;
        private bool IsDragging;
        /// <summary>
        /// LastMarker is used to insert a marker in the route.
        /// </summary>
        //private GMapMarker mLastMarker;
        private bool IsOnMarker;

        public Settings Settings { get; set; }
        #endregion
        #region CONSTRUCTOR
        public Form1()
        {
            InitializeComponent();
            Settings = Settings.Load();
            My.Log($"Start {this.Text += My.Version}");
        }
        #endregion
        #region INITIALIZE
        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeGmap();
            InitializeOverlays();
            InitializeComboboxWithMapProviders();
        }

        private void InitializeOverlays()
        {
            Overlay = new Overlay(gmap, "rawpoints");
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
        //private void xxxAddRoute()
        //{
        //    GMapOverlay routes = new GMapOverlay("routes");
        //    List<PointLatLng> points = new List<PointLatLng>();
        //    points.Add(new PointLatLng(48.866383, 2.323575));
        //    points.Add(new PointLatLng(48.863868, 2.321554));
        //    points.Add(new PointLatLng(48.861017, 2.330030));
        //    GMapRoute route = new GMapRoute(points, "A walk in the park");
        //    route.Stroke = new Pen(Color.Red, 3);
        //    routes.Routes.Add(route);
        //    gmap.Overlays.Add(routes);
        //    Console.WriteLine($"Route {route.Name} distance = {route.Distance} km.");
        //}
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
            //Console.WriteLine($"{DateTime.Now} OnMarkerLeave");
            if (!IsDragging) IsOnMarker = false;
        }
        private void gmap_OnMarkerEnter(GMapMarker item)
        {
            //Console.WriteLine($"{DateTime.Now} OnMarkerEnter");
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
                Overlay.UpdateCurrentMarker(e.X, e.Y);
            }
        }
        private void gmap_MouseUp(object sender, MouseEventArgs e) => IsDragging = false;
        #endregion
        private void chkRawPoints_CheckedChanged(object sender, EventArgs e)
        {
            gmap.MarkersEnabled = chkRawPoints.Checked;
            gmap.Refresh();
        }

        private void chkShowTooltip_CheckedChanged(object sender, EventArgs e)
        {
            Overlay.SetTooltipOnOff(chkShowTooltip.Checked);
        }

        #region MENU ITEMS
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // Route.SaveAs(saveFileDialog1.FileName);
            }
        }
        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = new FormOptions(Settings);
            f.ShowDialog();
            Settings = f.Settings;
        }
        #endregion


        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Settings.Save();
        }
    }
}
