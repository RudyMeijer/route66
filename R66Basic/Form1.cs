using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using MyLib;
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
using R66Basic.Properties;

namespace R66Basic
{
	public partial class Form1 : Form
	{
		private List<GMapMarker> PointCloud;
		private readonly GMapOverlay Red;
		private readonly GMapRoute RedRoute;

		public bool IsOnMarker { get { return PointCloud.Count > 0; } }

		public Form1()
		{
			InitializeComponent();
			PointCloud = new List<GMapMarker>();
			My.SetStatus(toolStripStatusLabel1);

			gmap.MapProvider = GMap.NET.MapProviders.OpenStreetMapProvider.Instance;
			GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerOnly;
			gmap.Overlays.Add(new GMapOverlay("Red"));
			gmap.Overlays.Add(new GMapOverlay("Green"));
			gmap.Overlays.Add(new GMapOverlay("Blue"));

			Red = gmap.Overlays[0];
			RedRoute = new GMapRoute("RedRoute");
			RedRoute.Stroke = new Pen(Color.Red,2);
			Red.Routes.Add(RedRoute);
			textBox1_Validated(null, null);
			this.Width = 600 + splitContainer1.Panel2.Width + 29;
			this.Height = 400 + menuStrip1.Height + statusStrip1.Height + 39;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			foreach (var overlay in gmap.Overlays)
			{
				overlay.Markers.Clear();
				foreach (var route in overlay.Routes)
				{
					route.Clear();
					gmap.UpdateRouteLocalPosition(route);
				}
			}
			PointCloud.Clear();
		}

		private void gmap_MouseDown(object sender, MouseEventArgs e)
		{
			Console.WriteLine($"{DateTime.Now} gmap_MouseDown {e.Button} {e.Clicks} ({e.X},{e.Y})");
			if (e.Button == MouseButtons.Left && !IsOnMarker) AddMarker(e);
		}

		private void gmap_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			Console.WriteLine($"{DateTime.Now} gmap_MouseDoubleClick {e.Button} {e.Clicks} ({e.X},{e.Y})");
		}

		private void gmap_OnMarkerClick(GMap.NET.WindowsForms.GMapMarker item, MouseEventArgs e)
		{
			Console.WriteLine($"{DateTime.Now} gmap_OnMarkerClick {item.ToolTipText} {e.Button} {e.Clicks}");
			if (e.Button == MouseButtons.Right) RemoveMarker(item);
		}


		private void gmap_OnMarkerEnter(GMapMarker item)
		{
			//item.ToolTipText = $"{item.Overlay.Markers.IndexOf(item)}";
			Console.WriteLine($"{DateTime.Now} gmap_OnMarkerEnter {item.ToolTipText} pointcloud {PointCloud.Count+1}");
			PointCloud.Add(item);

		}

		private void gmap_OnMarkerLeave(GMapMarker item)
		{
			//Console.WriteLine($"{DateTime.Now} gmap_OnMarkerLeave {item.ToolTipText}");
			PointCloud.Remove(item);
		}

		private void AddMarker(MouseEventArgs e)
		{
			Console.WriteLine($"{DateTime.Now} AddMarker ({e.X},{e.Y})");
			PointLatLng point = gmap.FromLocalToLatLng(e.X, e.Y);
			//var marker = new GMarkerGoogle(point, GMarkerGoogleType.red_small);
			var marker1 = new GmarkerRotate(point, Resources.arrow3);
			marker1.Rotation = (float)numericUpDown1.Value;
			Red.Markers.Add(marker1);
			//Red.Markers.Add(marker);
			//RedRoute.Points.Add(point);
			//marker.ToolTipText = $"{marker.Overlay.Markers.Count-1}";
			//gmap.UpdateRouteLocalPosition(RedRoute);
		}
		private void RemoveMarker(GMapMarker item)
		{
			Red.Markers.Remove(item);
			RedRoute.Points.Remove(item.Position);
			gmap.UpdateRouteLocalPosition(RedRoute);
			PointCloud.Remove(item);
		}

		private void textBox1_Validated(object sender, EventArgs e)
		{
			gmap.Zoom = 15;
			gmap.SetPositionByKeywords(textBox1.Text);
		}

		private void textBox1_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter) this.Validate();
		}
	}
}
