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
using MyLib;
using System.Diagnostics;

namespace Route66
{
	public partial class Form1 : Form
	{
		#region FIELDS
		/// <summary>
		/// Application configuration loaded on startup from Settings.xml. 
		/// </summary>
		public Settings Settings { get; set; }
		/// <summary>
		/// Route data of current route on form.
		/// Filled during Save. 
		/// </summary>
		public Route Route { get; set; }
		/// <summary>
		/// Contains all overlays like routes and markers.
		/// </summary>
		private Overlay Overlay;
		/// <summary>
		/// Moving while Leftmouse button pressed.
		/// </summary>
		private bool IsDragging;
		/// <summary>
		/// Mouse is on a Marker.
		/// </summary>
		private bool IsOnMarker;
		/// <summary>
		/// Last marker entered with mouse.
		/// </summary>
		private GMapMarker LastMarker;
		/// <summary>
		/// Title shown on top of form.
		/// </summary>
		private readonly string Title;
		private bool CtrlKeyIsPressed;
		private KeyEventArgs Key;

		#endregion
		#region CONSTRUCTOR
		public Form1()
		{
			InitializeComponent();
			Title = this.Text += My.Version + " ";
		}
		#endregion
		#region INITIALIZE
		private void Form1_Load(object sender, EventArgs e)
		{
			InitializeLogfile();
			My.Log($"Start {Title} User {My.UserName} {My.WindowsVersion}");
			My.SetStatus(toolStripStatusLabel1);
			Settings = Settings.Load();
			Route = Route.Load();
			InitializeGmap();
			InitializeOverlays();
			InitializeComboboxWithMapProviders();
			InitializeSettings();
		}
		/// <summary>
		/// Limit maximum logfile size to 1 Mb.
		/// </summary>
		private void InitializeLogfile()
		{
			try
			{
				var fileName = My.ExeFile + ".log";
				if (File.Exists(fileName))
				{
					var fi = new FileInfo(fileName);
					if (fi.Length > 1000000) File.Delete(fileName);
				}
			}
			catch (Exception ee) { My.Log($"InitializeLogfile {ee.Message + ee.InnerException}"); }
		}
		private void InitializeSettings()
		{
			Route.MachineType = Settings.MachineType;
			var idx = GetIndex(Settings.MapProvider);
			if (idx == 6) gmap.Zoom = 9;
			comboBox1.SelectedIndex = idx;
			gmap.Refresh();
			Overlay.AutoRoute = Settings.AutoRoute;
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
			//comboBox1.SelectedIndex = 0;
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
			if (comboBox1.SelectedItem != null)
			{
				gmap.MapProvider = comboBox1.SelectedItem as GMapProvider;
				Settings.MapProvider = gmap.MapProvider.Name;
			}
		}
		private void Form1_FormClosed(object sender, FormClosedEventArgs e)
		{
			Settings.Save();
			AskToSaveModifiedRoute();
		}

		#endregion
		#region EDIT ROUTE
		private void gmap_MouseDown(object sender, MouseEventArgs e)
		{
			//
			// Add marker
			//
			if (e.Button == MouseButtons.Left && !IsOnMarker) { Overlay.AddMarkers(e.X, e.Y); Route.IsChanged = true; }
			//
			// Remove marker
			//
			if (e.Button == MouseButtons.Right && IsOnMarker) { Overlay.Remove(LastMarker); IsOnMarker = false; Route.IsChanged = true; }
			//
			// Select current marker 
			//
			if (e.Button == MouseButtons.Left && IsOnMarker && !Settings.FastDrawMode) { Overlay.SetCurrentMarker(LastMarker); }
			//
			// Edit marker
			//
			if (IsOnMarker && e.Clicks == 2) { Route.IsChanged = Overlay.EditMarker(Key); }
		}
		private void gmap_OnMarkerLeave(GMapMarker item)
		{
			if (!IsDragging) IsOnMarker = false;
		}
		/// <summary>
		/// When hover over marker and fast draw mode is enabled set current marker.
		/// </summary>
		/// <param name="item"></param>
		private void gmap_OnMarkerEnter(GMapMarker item)
		{
			if (!IsDragging)
			{
				IsOnMarker = true;
				LastMarker = item;
				if (Settings.FastDrawMode) Overlay.SetCurrentMarker(item);
			}
		}
		private void gmap_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left && IsOnMarker)
			{
				Route.IsChanged = IsDragging = true;
				Overlay.UpdateCurrentMarkerPosition(e.X, e.Y);
			}
		}
		private void gmap_MouseUp(object sender, MouseEventArgs e) => IsDragging = false;
		/// <summary>
		/// Clear status bar when mouse leaves the map.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void gmap_MouseLeave(object sender, EventArgs e) => My.Status(" Ready");
		private void gmap_KeyDown(object sender, KeyEventArgs e)
		{
			Console.WriteLine($"KeyDown Ctrl = {e.Control}");
			Key = e;
			CtrlKeyIsPressed = e.Control;
		}

		private void gmap_KeyUp(object sender, KeyEventArgs e)
		{
			Key = e;
			Console.WriteLine($"KeyUp Ctrl = {e.Control}");
		}
		private void gmap_OnMapZoomChanged()
		{
			My.Status($"Zoom factor = {gmap.Zoom}");
		}
		#endregion
		#region MENU ITEMS
		private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
		{
			AskToSaveModifiedRoute();
			openFileDialog1.InitialDirectory = Settings.RoutePath;
			if (openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				Route = Route.Load(openFileDialog1.FileName);
				Overlay.Load(Route);
				this.Text = Title + openFileDialog1.FileName;
			}
		}

		private void AskToSaveModifiedRoute()
		{
			if (Route.IsChanged && MessageBox.Show("Save current route?", "Route is changed.", MessageBoxButtons.YesNo) == DialogResult.Yes)
				SaveToolStripMenuItem_Click(null, null);
		}
		private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Overlay.CopyTo(Route);
			if (Route.IsDefaultFile) Route.SaveAs(Path.Combine(Settings.RoutePath, "Route66.xml"));
			else Route.Save();
			this.Text = Title + Route.FileName;
		}

		/// <summary>
		/// This function is called on Save, SaveAs and Form_Closed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			saveFileDialog1.InitialDirectory = openFileDialog1.InitialDirectory;
			saveFileDialog1.FileName = openFileDialog1.FileName;
			saveFileDialog1.Filter = openFileDialog1.Filter;
			saveFileDialog1.DefaultExt = openFileDialog1.DefaultExt;
			saveFileDialog1.Title = (sender == null) ? "Route is changed. Save changes?" : "Save As";
			if (saveFileDialog1.ShowDialog() == DialogResult.OK)
			{
				if (Path.GetExtension(saveFileDialog1.FileName) != ".xml")
				{
					MessageBox.Show("Sorry, this function is not implemented yet.");
					return;
				}
				Overlay.CopyTo(Route);
				Route.SaveAs(saveFileDialog1.FileName);
				this.Text = Title + Route.FileName;
			}
		}
		/// <summary>
		/// Show configuration menu.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OptionsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var f = new FormOptions(Settings);
			f.ShowDialog();
			Settings = f.Settings;
			InitializeSettings();
		}
		/// <summary>
		/// Show help page in default browser.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		//
		private void HelpToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process.Start("Explorer", "documents\\help.html");
		}
		#endregion
		#region SEARCH PLACES
		private void textBox1_Validated(object sender, EventArgs e)
		{
			if (gmap.MapProvider == BingHybridMapProvider.Instance) comboBox1.SelectedItem = OpenStreetMapProvider.Instance;
			gmap.Zoom = 14;
			gmap.SetPositionByKeywords(txtSearchPlaces.Text);
		}

		private void textBox1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			if (e.KeyCode == Keys.Enter) textBox1_Validated(null, null);
		}
		#endregion
		#region RIGHT PANE
		private void button1_Click(object sender, EventArgs e)
		{
			Overlay.Clear();
			IsOnMarker = IsDragging = false;
			//this.Text = Title + "Create new route by click left mouse on map.";
			//var x = Route.FileName;
		}
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
