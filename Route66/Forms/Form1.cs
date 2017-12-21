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
		/// Application configuration loaded on startup from Settings.xml.
		/// Never create new instance of settings.
		/// </summary>
		public Settings Settings { get; }
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
		/// <summary>
		/// Last key pressed was a Ctrl key.
		/// </summary>
		private bool CtrlKeyIsPressed;
		/// <summary>
		/// Mouse is on a Marker.
		/// True when any marker is entered.
		/// False when any marker is leaved or when map control is leaved.
		/// </summary>
		private bool IsOnMarker;
		/// <summary>
		/// Last marker that is entered with the mouse. 
		/// Promoted to current marker on mouse move. 
		/// </summary>
		private GMapMarker LastEnteredMarker;
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
			var idx = GetComboIndex(Settings.MapProvider);
			if (idx == 6) gmap.Zoom = 9;
			comboBox1.SelectedIndex = idx;
			chkShowTooltip.Checked = Settings.ToolTipMode;
			if (Settings.SupervisorMode) chkEditRoute.Checked = true;
			chkArrowMarker.Checked = Settings.ArrowMarker;
		}

		private int GetComboIndex(string mapProvider)
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
				Console.WriteLine($"MouseDown {e.Button} IsOnMarker={IsOnMarker}, IsDragging={IsDragging}");
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
				//Console.WriteLine($"gmap_OnMarkerClick {e.Button} {item.Info()}");
				if (e.Button == MouseButtons.Left) {Overlay.SetCurrentMarker(item); }
				if (e.Button == MouseButtons.Right && IsEditMode()) { Overlay.Remove(item); IsOnMarker = false; }
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
			if (e.Button == MouseButtons.Right) My.Status($"Info: {Overlay}, IsOnMarker = {IsOnMarker}");
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
			if (!IsDragging)
			{
				IsOnMarker = true;
				LastEnteredMarker = item;
				Overlay.SetTooltipRed(item);
				Console.WriteLine($"{DateTime.Now} Enter {item.Info()}");
				if (Settings.FastDrawMode || !chkGpsPoints.Checked) Overlay.SetCurrentMarker(item);
				if (Overlay.IsNavigationMarker(item) && Settings.SpeechSyntesizer)
				{
					My.PlaySound((item.Tag as NavigationMarker).Message);
				}
			}
		}
		private void gmap_OnMarkerLeave(GMapMarker item)
		{
			if (!IsDragging) IsOnMarker = false;
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
				Overlay.SetCurrentMarker(LastEnteredMarker);
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
		/// When mouse leaves the map: clear statusbar and reset currentmarker.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void gmap_MouseLeave(object sender, EventArgs e)
		{
			//Console.WriteLine("gmap_MouseLeave");
			IsOnMarker = false;
			My.Status(" Ready");
		}
		private void gmap_KeyDown(object sender, KeyEventArgs e)
		{
			Console.WriteLine($"KeyDown = {e.KeyCode}");
			CtrlKeyIsPressed = e.Control;
			if (Overlay.CurrentMarker == null) return;
			if (e.KeyCode == Keys.Up) Overlay.SetArrowMarker(true);
			if (e.KeyCode == Keys.Down) Overlay.SetArrowMarker(false);
			if (e.KeyCode == Keys.Delete && IsEditMode()) { Overlay.RemoveCurrentMarker(); IsOnMarker = false; }
			if (e.KeyCode == Keys.C) Overlay.EditMarker(false);
			if (e.KeyCode == Keys.N) Overlay.EditMarker(true);
		}
		private void gmap_KeyUp(object sender, KeyEventArgs e)
		{
			Console.WriteLine($"KeyUp = {e.KeyCode}");
			CtrlKeyIsPressed = false;
			if (e.KeyCode == Keys.Up) Overlay.SetArrowMarker(true);
			if (e.KeyCode == Keys.Down) Overlay.SetArrowMarker(false);
		}
		private void gmap_OnMapZoomChanged()
		{
			if (toolStripStatusLabel1.Text.StartsWith(" "))
				My.Status($" Zoom factor = {gmap.Zoom}");
		}
		#endregion
		#region MENU ITEMS
		private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var IsSubroute = sender is string;
			AskToSaveModifiedRoute();
			openFileDialog1.InitialDirectory = Settings.RoutePath;
			openFileDialog1.Title = (IsSubroute) ? "Add subroute" : "Open route";
			if (openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				var originalFilename = Overlay.Route.FileName;
				My.Log($"{openFileDialog1.Title} {openFileDialog1.FileName } {((IsSubroute) ? " at " + Overlay : "")}");
				if (Path.GetExtension(openFileDialog1.FileName) != ".xml")
				{
					MessageBox.Show("Sorry, this function is not implemented yet.", $"Deer mr {My.UserName}");
					return;
				}
				if (!Overlay.OpenRoute(openFileDialog1.FileName, IsSubroute)) My.Status($"Error This file contains no Gps markers.");
				//
				// If subroute is loaded then keep original filename.
				//
				if (IsSubroute)
				{
					My.Status($"Subroute {openFileDialog1.FileName} Succesfully added.");
					Overlay.Route.FileName = originalFilename;
				}
				this.Text = Title + Overlay.Route;
			}
		}
		private void AddtoolStripMenuItem_Click(object sender, EventArgs e)
		{
			var idx = Overlay.GetIndexRed(Overlay.CurrentMarker);
			if (idx == -1) { MessageBox.Show("Please open a mainroute first.", $"Dear mr {My.UserName}:"); return; }
			if (MessageBox.Show($"Do you want to insert subroute at current Gps marker {idx}?", "Route66", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
			{
				OpenToolStripMenuItem_Click("SubRoute", null);
			}
		}
		private bool AskToSaveModifiedRoute()
		{
			if (Overlay.IsChanged)
			{
				var res = MessageBox.Show("Save current route?", "Route is changed.", MessageBoxButtons.YesNoCancel);
				if (res == DialogResult.Yes) SaveToolStripMenuItem_Click(null, null);
				return res == DialogResult.Cancel;
			}
			return false;
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

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			My.Log($"{Title} closed by user {My.UserName}.");
			Settings.Save();
			e.Cancel = AskToSaveModifiedRoute();
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
			My.Status($"Clear Route {Overlay.Route}.");
			Overlay.Clear();
			IsOnMarker = false;
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
		private void chkArrowMarker_CheckedChanged(object sender, EventArgs e)
		{
			Settings.ArrowMarker = (sender as CheckBox).Checked;
			gmap.Overlays[3].IsVisibile = Settings.ArrowMarker;
		}
		#endregion
	}
}
