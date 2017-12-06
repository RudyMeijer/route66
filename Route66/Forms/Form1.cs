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
using System.Collections;

namespace Route66
{
	public  partial class Form1 : Form
	{
		#region FIELDS
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
		/// <summary>
		/// Allow remove of overlaying markers via push/pop.
		/// </summary>
		private HashSet<GMapMarker> MarkerHash;

		#endregion
		#region CONSTRUCTOR
		public Form1()
		{
			InitializeComponent();
			Title = this.Text += My.Version + " ";
			Settings = Settings.Load();
		}
		#endregion
		#region PROPERTIES
		/// <summary>
		/// Application configuration loaded on startup from Settings.xml. 
		/// </summary>
		public Settings Settings { get; } // Never create new instance of settings.
										  /// <summary>
										  /// Route data of current route on form.
										  /// Filled during Save. 
										  /// </summary>
		public Route Route { get; set; }
		#endregion
		#region INITIALIZE
		private  void Form1_Load(object sender, EventArgs e)
		{
			InitializeLogfile();
			My.Log($"Start {Title} User {My.UserName} {My.WindowsVersion}");
			My.SetStatus(toolStripStatusLabel1);
			Route = Route.Load();
			InitializeGmapProvider();
			InitializeOverlays();
			InitializeComboboxWithMapProviders();
			InitializeSettings();
			MarkerHash = new HashSet<GMapMarker>();
			if (Settings.SupervisorMode) OpenToolStripMenuItem_Click(null, null);
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
			//gmap.Refresh();
			chkShowTooltip.Checked = Settings.ToolTipMode;
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

		private void InitializeGmapProvider()
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
			// Select marker 
			//
			if (e.Button == MouseButtons.Left && IsOnMarker && !Settings.FastDrawMode) { Overlay.SetCurrentMarker(LastMarker); }
			//
			// Edit marker
			//
			else if (e.Button == MouseButtons.Left && IsOnMarker && e.Clicks == 2) { Route.IsChanged = Overlay.EditMarker(Key); }
			//
			// Add marker
			//
			else if (e.Button == MouseButtons.Left && !IsOnMarker && IsEditRoute()) { Overlay.AddMarker(e.X, e.Y); Route.IsChanged = true; }
			//
			// Remove marker
			//
			else if (e.Button == MouseButtons.Right && IsOnMarker && IsEditRoute()) { Overlay.Remove(LastMarker); Pop(true); Route.IsChanged = true; }
		}

		private bool IsEditRoute()
		{
			if (chkEditRoute.Checked) return true;
			MessageBox.Show($"Please enable edit route on the right side.", $"Dear mr {My.UserName}");
			return false;
		}

		private void Pop(bool force = false)
		{
			Console.Write($"Pop {LastMarker.ToolTipText} Leave marker");
			IsOnMarker = false;
			if (MarkerHash.Count == 0) return; // Marker is dropped onto other marker.
			MarkerHash.Remove(LastMarker);
			if (MarkerHash.Count > 0)
			{
				LastMarker = MarkerHash.ElementAt(MarkerHash.Count-1);
				IsOnMarker = true;
				Console.WriteLine($" Peek {LastMarker.ToolTipText} count {MarkerHash.Count}");
			}
			else Console.WriteLine();
		}
		private void gmap_OnMarkerLeave(GMapMarker item)
		{
			//if (!IsDragging && item.Overlay == gmap.Overlays[0])
			{
				Pop();
			}
		}
		/// <summary>
		/// When hover over marker and fast draw mode is enabled set current marker.
		/// </summary>
		/// <param name="item"></param>
		private void gmap_OnMarkerEnter(GMapMarker item)
		{
			Console.Write("Push ");
			MarkerHash.Add(item);
			if (!IsDragging)
			{
				if (item.Overlay == gmap.Overlays[0]) // Allow red markers only!!
				{
					IsOnMarker = true;
					LastMarker = item;
					if (Settings.FastDrawMode) Overlay.SetCurrentMarker(item);
				}
				else if (item.Tag is NavigationMarker)
				{
					if (Settings.SpeechSyntesizer && item.Overlay.IsVisibile)
					{
						var tag = item.Tag as NavigationMarker;
						My.PlaySound(tag.Message);
					}
				}
			}
		}
		private void gmap_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left && IsOnMarker && IsEditRoute())
			{
				if (!IsDragging) Console.WriteLine("start dragging " + LastMarker.ToolTipText);
				Route.IsChanged = IsDragging = true;
				Overlay.UpdateCurrentMarkerPosition(e.X, e.Y);
			}
		}
		private void gmap_MouseUp(object sender, MouseEventArgs e)
		{
			if (IsDragging) Console.WriteLine("stop dragging " + LastMarker?.ToolTipText);
			IsDragging = false;
		}
		#endregion
		#region MAP ZOOM KEYS
		/// <summary>
		/// Clear status bar when mouse leaves the map.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void gmap_MouseLeave(object sender, EventArgs e) => My.Status(" Ready");
		private void gmap_KeyDown(object sender, KeyEventArgs e)
		{
			Console.WriteLine($"KeyDown = {e.KeyCode}");
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
			My.Status($" Zoom factor = {gmap.Zoom}");
		}
		#endregion
		#region MENU ITEMS
		private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
		{
			AskToSaveModifiedRoute();
			openFileDialog1.InitialDirectory = Settings.RoutePath;
			if (openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				if (Path.GetExtension(openFileDialog1.FileName) != ".xml")
				{
					MessageBox.Show("Sorry, this function is not implemented yet.",$"Deer mr {My.UserName}");
					return;
				}
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
					MessageBox.Show("Sorry, this function is not implemented yet.", $"Deer mr {My.UserName}");
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
			//Settings = f.Settings; // Form.Settings point to new instance. Overlay.Settings still points to old instance!
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
		private void btnClear_Click(object sender, EventArgs e)
		{
			Overlay.Clear();
			IsOnMarker = IsDragging = false;
			MarkerHash.Clear();
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
		private void chkShowTooltip_CheckedChanged(object sender, EventArgs e) => Settings.ToolTipMode = chkShowTooltip.Checked;
		private void numDosing_ValueChanged(object sender, EventArgs e)
		{
		}

		private void btnChangeGlobalDosing_Click(object sender, EventArgs e)
		{
			var from = (double)numDosingFrom.Value;
			var to = (double)numDosingTo.Value;
			Overlay.UpdateAllChangeMarkers(from, to);

		}

		private void chkAutoRoute_CheckedChanged(object sender, EventArgs e)
		{
			Overlay.AutoRoute = (sender as CheckBox).Checked;
		}
		#endregion
	}
}
