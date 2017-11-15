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
        #endregion
        #region CONSTRUCTOR
        public Form1()
        {
            InitializeComponent();
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
            gmap.MapProvider = GMap.NET.MapProviders.BingMapProvider.Instance;
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerOnly;
            gmap.Zoom = 13;
            gmap.SetPositionByKeywords("Paris, france");
            Console.WriteLine($"paris at {gmap.Position}");
            //gmap.ShowCenter = false;
        }
        #endregion
        private void button1_Click(object sender, EventArgs e)
        {
            mOverlay.Markers.Clear();
            UpdateRoute(mOverlay.Markers);
            mCurrentMarker = null;
            //mRoute.IsVisible = !mRoute.IsVisible;
            //mOverlay.IsVisibile = !mOverlay.IsVisibile;
        }

        private void xxxAddRoute()
        {
            GMapOverlay routes = new GMapOverlay("routes");
            List<PointLatLng> points = new List<PointLatLng>();
            points.Add(new PointLatLng(48.866383, 2.323575));
            points.Add(new PointLatLng(48.863868, 2.321554));
            points.Add(new PointLatLng(48.861017, 2.330030));
            GMapRoute route = new GMapRoute(points, "A walk in the park");
            route.Stroke = new Pen(Color.Red, 3);
            routes.Routes.Add(route);
            gmap.Overlays.Add(routes);
            Console.WriteLine($"Route {route.Name} distance = {route.Distance} km.");
        }

        private void AddMarker(int x = 0, int y = 0)
        {
            PointLatLng point = gmap.FromLocalToLatLng(x,y);
            mCurrentMarker = new GMarkerGoogle(point, GMarkerGoogleType.red_small);
            mOverlay.Markers.Add(mCurrentMarker);
            mOverlay.Routes[0].Points.Add(point);
            UpdateRoute(mOverlay.Markers);
            Console.WriteLine($"Marker added at {mCurrentMarker.LocalPosition}");
        }

        private void UpdateRoute(ObservableCollectionThreadSafe<GMapMarker> markers)
        {
            mRoute.Points.Clear();
            foreach (var item in markers) mRoute.Points.Add(item.Position);
            gmap.UpdateRouteLocalPosition(mRoute);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            gmap.MapProvider = comboBox1.SelectedItem as GMapProvider;
        }
        #region SEARCH
        private void textBox1_Validated(object sender, EventArgs e)
        {
            gmap.SetPositionByKeywords(textBox1.Text);
        }

        private void textBox1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) textBox1_Validated(null, null);
        }
        #endregion
        private void gmap_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && mCurrentMarker == null) AddMarker(e.X, e.Y);
            if (e.Button == MouseButtons.Right && mCurrentMarker != null) RemoveMarker(mCurrentMarker);
        }

        private void RemoveMarker(GMapMarker mCurrentMarker)
        {
            mOverlay.Markers.Remove(mCurrentMarker);
            UpdateRoute(mOverlay.Markers);
            mCurrentMarker = null;
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
    }
}
