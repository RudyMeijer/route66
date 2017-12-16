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
	public partial class Form1 : Form
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
		/// Title shown on top of form.
		/// </summary>
		private readonly string Title;
		private bool CtrlKeyIsPressed;
		private KeyEventArgs Key;
		/// <summary>
		/// This list contains markers which are overlapping eachother.
		/// When zoomed out all markers are in the pointcloud.
		/// Allow remove of overlaying markers via push/pop.
		/// </summary>
		private List<GMapMarker> PointCloud;

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
		/// Never create new instance of settings.
		/// </summary>
		public Settings Settings { get; }
		/// <summary>
		/// Mouse is on a Marker.
		/// Increment when Red marker is entered.
		/// Decrement when Red marker is leaved.
		/// </summary>
		private bool IsOnMarker { get => PointCloud.Count > 0; }
		#endregion
		#region INITIALIZE
		private void Form1_Load(object sender, EventArgs e)
		{
			InitializeLogfile();
			My.Log($"Start {Title} User {My.UserName} {My.WindowsVersion}");
			My.SetStatus(toolStripStatusLabel1);
			InitializeGmapProvider();
			InitializeOverlays();
			InitializeComboboxWithMapProviders();
			InitializeSettings();
			PointCloud = new List<GMapMarker>();
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
			Overlay.MachineType = Settings.MachineType;
			var idx = GetIndex(Settings.MapProvider);
			if (idx == 6) gmap.Zoom = 9;
			comboBox1.SelectedIndex = idx;
			chkShowTooltip.Checked = Settings.ToolTipMode;
			if (Settings.SupervisorMode) chkEditRoute.Checked = true;
			chkCurrentMarker.Checked = Settings.CurrentMarker;
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

		#endregion
		#region EDIT ROUTE
		/// <summary>
		/// Add marker
		/// </summary>
		private void gmap_MouseDown(object sender, MouseEventArgs e)
		{
			try
			{
				Console.WriteLine($"MouseDown {e.Button} pointCloud={PointCloud.Count} IsOnMarker={IsOnMarker}, IsDragging={IsDragging}");
				if (e.Button == MouseButtons.Left && !IsOnMarker && IsEditMode()) { Overlay.AddMarker(e.X, e.Y); }
			}
			catch (Exception ee) { My.Status($"Error {ee}"); }
		}
		/// <summary>
		/// Select or Remove marker 
		/// </summary>
		private void gmap_OnMarkerClick(GMapMarker item, MouseEventArgs e)
		{
			try
			{
				Console.WriteLine($"gmap_OnMarkerClick {e.Button} {item.Overlay.Id} {Overlay.GetIndex(item)}={item.ToolTipText.Replace('\n', ' ')}, IsMouseOver={item.IsMouseOver}");
				if (e.Button == MouseButtons.Left) { Overlay.SetCurrentMarker(item); }
				//if (e.Button == MouseButtons.Right && Overlay.IsGpsMarker(item) && IsEditMode()) Overlay.Remove(Pop(PointCloud));
				if (e.Button == MouseButtons.Right)
				{
					if (Overlay.IsGpsMarker(item))
					{
						if (PointCloud.Count > 0 && IsEditMode()) Overlay.Remove(Pop(PointCloud));
					}
					else
						Overlay.Remove(item);
				}
			}
			catch (Exception ee) { My.Status($"Error {ee}"); }
		}
		/// <summary>
		/// Edit marker
		/// </summary>
		private void gmap_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			chkChangePoints.Checked = chkNavPoints.Checked = true;
			if (e.Button == MouseButtons.Left && IsOnMarker) { Overlay.EditMarker(CtrlKeyIsPressed); }
			if (e.Button == MouseButtons.Right) My.Status($"Info: CurrentMarker={Overlay.CurrentMarker?.ToolTipText} IsOnMarker={IsOnMarker}, pointCount={PointCloud.Count}, ");
		}
		private GMapMarker Pop(List<GMapMarker> pointCloud)
		{
			int max = -2;
			GMapMarker marker = null;
			foreach (var x in pointCloud)
			{
				var n = Overlay.GetIndex(x);// int.Parse(x.ToolTipText);
				if (n > max)
				{
					max = n;
					marker = x;
				}
			}
			pointCloud.Remove(marker);
			return marker;
		}

		private bool IsEditMode()
		{
			if (chkEditRoute.Checked) return true;
			MessageBox.Show($"Please enable edit route on the right side.", $"Dear mr {My.UserName}");
			return false;
		}
		/// <summary>
		/// When hover over marker and fast draw mode is enabled set current marker.
		/// </summary>
		/// <param name="item"></param>
		private void gmap_OnMarkerEnter(GMapMarker item)
		{
			Overlay.SetRedTooltip(item);
			Console.WriteLine($"{PointCloud.Count} Enter {item.Overlay.Id} {item.ToolTipText?.Replace('\n', ' ')}");
			if (Overlay.IsGpsMarker(item) || !chkGpsPoints.Checked)
			{
				PointCloud.Add(item);
				if (!IsDragging && Settings.FastDrawMode) Overlay.SetCurrentMarker(item);
			}
			else if (Overlay.IsNavigationMarker(item) && Settings.SpeechSyntesizer)
			{
				My.PlaySound((item.Tag as NavigationMarker).Message);
			}
		}
		private void gmap_OnMarkerLeave(GMapMarker item)
		{
			if (Overlay.IsGpsMarker(item) || !chkGpsPoints.Checked)
			{
				PointCloud.Remove(item);
				//Console.WriteLine($"{PointCloud.Count} Leave {item.ToolTipText}");
			}
		}
		/// <summary>
		/// Drag marker
		/// </summary>
		private void gmap_MouseMove(object sender, MouseEventArgs e)
		{
			//Console.WriteLine($"gmap_MouseMove ({e.X},{e.Y})");
			if (e.Button == MouseButtons.Left && IsOnMarker && chkEditRoute.Checked)
			{
				if (!IsDragging) Console.WriteLine("start dragging ");
				IsDragging = true;
				Overlay.SetCurrentMarker(PointCloud[0]);
				Overlay.UpdateCurrentMarkerPosition(e.X, e.Y);
			}
		}
		private void gmap_MouseUp(object sender, MouseEventArgs e)
		{
			if (IsDragging) Console.WriteLine("stop dragging ");
			IsDragging = false;
		}
		#endregion
		#region MAP ZOOM KEYS
		/// <summary>
		/// Clear status bar when mouse leaves the map.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void gmap_MouseLeave(object sender, EventArgs e)
		{
			//Console.WriteLine("gmap_MouseLeave");
			PointCloud.Clear();
			My.Status(" Ready");
		}
		private void gmap_KeyDown(object sender, KeyEventArgs e)
		{
			Console.WriteLine($"KeyDown = {e.KeyCode}");
			Key = e;
			CtrlKeyIsPressed = e.Control;
			if (Overlay.CurrentMarker == null) return;
			if (Key.KeyCode == Keys.Up) Overlay.SetArrowMarker(true);
			if (Key.KeyCode == Keys.Down) Overlay.SetArrowMarker(false);
			if (Key.KeyCode == Keys.Delete && IsEditMode()) { PointCloud.Remove(Overlay.CurrentMarker); Overlay.RemoveCurrentMarker(); }
			if (Key.KeyCode == Keys.C) Overlay.EditMarker(false);
			if (Key.KeyCode == Keys.N) Overlay.EditMarker(true);
		}
		private void gmap_KeyUp(object sender, KeyEventArgs e)
		{
			Console.WriteLine($"KeyUp = {e.KeyCode}");
			Key = e; CtrlKeyIsPressed = false;
			if (Key.KeyCode == Keys.Up) Overlay.SetArrowMarker(true);
			if (Key.KeyCode == Keys.Down) Overlay.SetArrowMarker(false);
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
					MessageBox.Show("Sorry, this function is not implemented yet.", $"Deer mr {My.UserName}");
					return;
				}
				if (!Overlay.OpenRoute(openFileDialog1.FileName)) My.Status($"Error This file contains no Gps markers.");
				this.Text = Title + openFileDialog1.FileName;
			}
		}
		private void AskToSaveModifiedRoute()
		{
			if (Overlay.IsChanged && MessageBox.Show("Save current route?", "Route is changed.", MessageBoxButtons.YesNo) == DialogResult.Yes)
				SaveToolStripMenuItem_Click(null, null);
		}
		private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.Text = Title + Overlay.Save();
		}
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
				Overlay.SaveAs(saveFileDialog1.FileName);
				this.Text = Title + saveFileDialog1.FileName;
			}
		}
		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}
		private void Form1_FormClosed(object sender, FormClosedEventArgs e)
		{
			My.Log($"{Title} stopped by user {My.UserName}.");
			Settings.Save();
			AskToSaveModifiedRoute();
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
		private void HelpToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process.Start("Explorer", "documents\\help.html");
		}
		#endregion
		#region SEARCH PLACES
		private void txtSearch_Validated(object sender, EventArgs e)
		{
			if (gmap.MapProvider == BingHybridMapProvider.Instance) comboBox1.SelectedItem = OpenStreetMapProvider.Instance;
			gmap.Zoom = 14;
			gmap.SetPositionByKeywords(txtSearchPlaces.Text);
		}

		private void txtSearch_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			if (e.KeyCode == Keys.Enter) txtSearch_Validated(null, null);
		}
		#endregion
		#region RIGHT PANE
		private void btnClear_Click(object sender, EventArgs e)
		{
			Console.WriteLine("btnClear_Click");
			Overlay.Clear();
			PointCloud.Clear();
		}
		private void chkGpsPoints_CheckedChanged(object sender, EventArgs e)
		{
			foreach (var item in gmap.Overlays[0].Markers)
			{
				item.IsVisible = chkGpsPoints.Checked;
			}
			gmap.Refresh();
			//
			// Disable edit route when Gps marker are not shown.
			//
			//chkEditRoute.Enabled = chkGpsPoints.Checked;
			//if (!chkGpsPoints.Checked) chkEditRoute.Checked = false;
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
		private void btnChangeGlobalDosing_Click(object sender, EventArgs e)
		{
			var from = (double)numDosingFrom.Value;
			var to = (double)numDosingTo.Value;
			var res = Overlay.UpdateAllChangeMarkers(from, to);
			if (res > 0)
				My.Status($"Dosage of {res} Changemarkers successfull updated from {from} to {to} gr/m2.");
			else
				My.Status($"No Changemarkers found with dosage {from}.", Color.Red);
			numDosingFrom.Value = numDosingTo.Value;
		}
		private void chkAutoRoute_CheckedChanged(object sender, EventArgs e)
		{
			Overlay.IsAutoRoute = (sender as CheckBox).Checked;
		}
		private void chkCurrent_CheckedChanged(object sender, EventArgs e)
		{
			Settings.CurrentMarker = (sender as CheckBox).Checked;
			gmap.Overlays[3].IsVisibile = Settings.CurrentMarker;
		}
		#endregion
	}
}
