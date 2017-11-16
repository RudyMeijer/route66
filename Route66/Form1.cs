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
        private GMapOverlay mOverlay;
        private GMapRoute mRoute;
        private GMapMarker mCurrentMarker;
        private bool IsDragging;
        /// <summary>
        /// LastMarker is used to insert a marker in the route.
        /// </summary>
        private GMapMarker mLastMarker;

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
            mOverlay = new GMapOverlay("markers");
            mRoute = new GMapRoute("routes");
            mRoute.Stroke = new Pen(Color.Red, 2);
            mOverlay.Routes.Add(mRoute);
            gmap.Overlays.Add(mOverlay);
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
            mOverlay.Markers.Clear();
            UpdateRoute(mOverlay.Markers);
            mCurrentMarker = mLastMarker = null;
            //gmap.MarkersEnabled = !gmap.MarkersEnabled;

            //mRoute.IsVisible = !mRoute.IsVisible;
            //mOverlay.IsVisibile = !mOverlay.IsVisibile;
            //xxxAddRoute();
            gmap.Refresh();
        }


        private void UpdateRoute(ObservableCollectionThreadSafe<GMapMarker> markers)
        {
            var on = chkShowTooltip.Checked;
            mRoute.Points.Clear();
            foreach (var item in markers)
            {

                mRoute.Points.Add(item.Position);
            }
            gmap.UpdateRouteLocalPosition(mRoute);
        }

        #region SEARCH PLACES
        private void textBox1_Validated(object sender, EventArgs e) => gmap.SetPositionByKeywords(textBox1.Text);

        private void textBox1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) textBox1_Validated(null, null);
        }
        #endregion
        #region EDIT ROUTE
        private void AddMarker(int x, int y)
        {
            PointLatLng point = gmap.FromLocalToLatLng(x,y);
            mCurrentMarker = new GMarkerGoogle(point, GMarkerGoogleType.red_small);
            if (mLastMarker == null) mLastMarker = mCurrentMarker;
            if (mLastMarker == mCurrentMarker)
            {
                mOverlay.Markers.Add(mCurrentMarker);
            }
            else // Insert marker.
            {
                var idx = mOverlay.Markers.IndexOf(mLastMarker);
                mOverlay.Markers.Insert(idx, mCurrentMarker);
                mLastMarker = mCurrentMarker;
            }
            mOverlay.Routes[0].Points.Add(point);
            UpdateRoute(mOverlay.Markers);
            Console.WriteLine($"Marker added at {mCurrentMarker.LocalPosition}");
        }
        private void RemoveMarker(GMapMarker mCurrentMarker)
        {
            mOverlay.Markers.Remove(mCurrentMarker);
            UpdateRoute(mOverlay.Markers);
            mCurrentMarker = mLastMarker = null;
        }
        private void gmap_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (mCurrentMarker == null)
                    AddMarker(e.X, e.Y);
                else
                    mLastMarker = mCurrentMarker;
            }
            if (e.Button == MouseButtons.Right && mCurrentMarker != null) RemoveMarker(mCurrentMarker);
        }
        private void gmap_OnMarkerLeave(GMapMarker item)
        {
            if (!IsDragging) mCurrentMarker = null;
        }
        private void gmap_OnMarkerEnter(GMapMarker item)
        {
            if (!IsDragging) mCurrentMarker = item;
        }
        private void gmap_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && mCurrentMarker != null)
            {
                IsDragging = true;
                mCurrentMarker.Position = gmap.FromLocalToLatLng(e.X, e.Y);
                UpdateRoute(mOverlay.Markers);
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
            SetTooltipOnOff(chkShowTooltip.Checked);
        }

        private void SetTooltipOnOff(bool on)
        {
            foreach (var item in mOverlay.Markers)
            {
                item.ToolTipMode = (on) ? MarkerTooltipMode.OnMouseOver : MarkerTooltipMode.Never;
                item.ToolTipText = $"Dosing {item.LocalPosition.X} gr.\nLat={item.Position.Lat:f3} Lng={item.Position.Lng:f3}";
            }
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
