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
		/// <summary>
		/// Last mouse down position. Set on mousedown event.
		/// Used to detemine if dragging mode must be entered during mousemove event.
		/// </summary>
		private MouseEventArgs eLast;
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
			My.Log($"=========== Start {Title} User {My.UserName} {My.WindowsVersion} =============\n", InitializeLogfile());
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
		private string InitializeLogfile()
		{
			try
			{
				var IsRemote = My.Drive.ToUpper() != "C:\\";
				var fileName = (IsRemote) ? My.LogFile : My.CheckPath(Settings.RoutePath, "log", My.LogFile);
				if (File.Exists(fileName))
				{
					var fi = new FileInfo(fileName);
					if (fi.Length > 1000000) File.Delete(fileName);
				}
				return fileName;
			}
			catch (Exception ee) { My.Log($"InitializeLogfile {ee.Message + ee.InnerException}"); }
			return "";
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
			GMaps.Instance.Mode = AccessMode.ServerAndCache;
			gmap.Zoom = 13;
			gmap.SetPositionByKeywords(txtSearchPlaces.Text);
			gmap.DragButton = MouseButtons.Left;
			gmap.IgnoreMarkerOnMouseWheel = true;
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
		/// Add, Edit marker
		/// </summary>
		private void gmap_MouseUp(object sender, MouseEventArgs e)
		{
			Console.WriteLine($" gmap_MouseUp focus={gmap.Focused}");
			if (IsDragging) Console.WriteLine("stop dragging");
			if (e.Button == MouseButtons.Left && !IsOnMarker && !IsDragging && IsEditMode()) { Overlay.AddMarker(e.X, e.Y); }
			if (e.Button == MouseButtons.Left && IsOnMarker && !IsDragging) { Overlay.EditMarker(CtrlKeyIsPressed); chkChangePoints.Checked = chkNavPoints.Checked = true; }
			IsDragging = false;
		}
		/// <summary>
		/// Select, Remove marker
		/// </summary>
		private void gmap_MouseDown(object sender, MouseEventArgs e)
		{
			try
			{
				Console.WriteLine($"MouseDown {e.Button} IsOnMarker={IsOnMarker}, IsDragging={IsDragging}");
				this.eLast = e;
				if (!IsOnMarker) return;
				if (e.Button == MouseButtons.Left) { Overlay.SetCurrentMarker(LastEnteredMarker); }
				if (e.Button == MouseButtons.Right && IsEditMode()) { Overlay.Remove(LastEnteredMarker); IsOnMarker = false; }
			}
			catch (Exception ee) { My.Status($"Error {ee}"); }
		}
		private bool IsEditMode()
		{
			if (chkEditRoute.Checked) return true;
			My.Show($"Please enable edit route on the right side.");
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
				Console.WriteLine($"Enter {item.Info()}");
				if (Settings.FastDrawMode || !chkGpsPoints.Checked) Overlay.SetCurrentMarker(item);
				if (Overlay.IsNavigationMarker(item) && Settings.SpeechSyntesizer && chkNavPoints.Checked)
				{
					My.PlaySound((item.Tag as NavigationMarker).Message);
				}
			}
		}
		private void gmap_OnMarkerLeave(GMapMarker item)
		{
			//Console.WriteLine($"Leave IsDragging={IsDragging}");
			if (!IsDragging) IsOnMarker = false;
		}
		/// <summary>
		/// Move marker.
		/// </summary>
		private void gmap_MouseMove(object sender, MouseEventArgs e)
		{
			//Console.WriteLine($"gmap_MouseMove x,y=({e.X},{e.Y})");
			if (e.Button == MouseButtons.Left && (IsOnMarker || IsMouseOutsideRegion(e, 30)))
			{
				if (!IsDragging) Console.WriteLine("start dragging ");
				IsDragging = true;
				if (IsOnMarker && chkEditRoute.Checked)
				{
					Overlay.SetCurrentMarker(LastEnteredMarker);
					Overlay.UpdateCurrentMarkerPosition(e.X, e.Y);
				}
			}
		}
		/// <summary>
		/// True when mouse is outside last mousedown position.
		/// </summary>
		/// <param name="e"></param>
		/// <param name="region"></param>
		/// <returns></returns>
		private bool IsMouseOutsideRegion(MouseEventArgs e, int region)
		{
			if (eLast == null) return true; // Doubleclick in fileopenmenu -> adds marker.
											
			//Console.WriteLine($"IsMouseOutsideRegion delta x,y=({Math.Abs(e.X - eLast.X)},{Math.Abs(e.Y - eLast.Y)})");
			return (Math.Abs(e.X - eLast.X) > region || Math.Abs(e.Y - eLast.Y) > region);
		}
		/// <summary>
		/// When mouse leaves the map: clear statusbar and reset currentmarker.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void gmap_MouseLeave(object sender, EventArgs e)
		{
			//Console.WriteLine("gmap_MouseLeave");
			IsOnMarker = false;
			My.Status(" Ready", SystemColors.Control);
		}
		private void gmap_OnMapZoomChanged()
		{
			if (toolStripStatusLabel1.Text.StartsWith(" ")) My.Status($" Zoom factor = {gmap.Zoom}");
			var save = chkGpsPoints.Checked;
			chkGpsPoints.Checked = true; // Bug update angle.
			gmap.Refresh();
			chkGpsPoints.Checked = save;
		}
		#endregion
		#region PROCESS KEYS
		protected override bool ProcessCmdKey(ref Message msg, Keys keyCode)
		{
			Console.WriteLine($"ProcessCmdKey={keyCode} focused={gmap.Focused}");
			if (gmap.Focused && (Overlay.CurrentMarker != null || keyCode == Keys.I))
			{
				CtrlKeyIsPressed = keyCode == (Keys.ControlKey | Keys.Control);
				var key = keyCode & ~Keys.Control;
				switch (key)
				{
					case Keys.Up: LastEnteredMarker = Overlay.SetArrowMarker(true); break;
					case Keys.Down: LastEnteredMarker = Overlay.SetArrowMarker(false); break;
					case Keys.Delete: if (IsEditMode()) { Overlay.RemoveCurrentMarker(); IsOnMarker = false; } break;
					case Keys.C: Overlay.EditMarker(false); break;
					case Keys.N: Overlay.EditMarker(true); break;
					case Keys.I: My.Status($"currentmarker={Overlay.CurrentMarker.Info()}, LastEnteredMarker={((Overlay?.CurrentMarker == LastEnteredMarker) ? "+" : LastEnteredMarker.Info())}, IsOnMarker={IsOnMarker}, IsDragging={IsDragging}, gmap.Focused={gmap.Focused}"); break;
					case Keys.X: Overlay.CutMarker(); break;
					case Keys.V: Overlay.PastMarker(); break;
					default: return base.ProcessCmdKey(ref msg, keyCode);
				}
				return true;
			}
			return base.ProcessCmdKey(ref msg, keyCode); // handle Ctrl-O, Ctrl-S, Holten
		}
		#endregion
		#region MENU ITEMS
		private void NewToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!AskToSaveModifiedRoute())
			{
				btnClear_Click(sender, e);
				Overlay.Route.FileName = "Untitled.xml";
				this.Text = Title + Overlay.Route;
			}
		}
		private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var IsSubroute = sender is string;
			AskToSaveModifiedRoute();
			openFileDialog1.InitialDirectory = Settings.RoutePath;
			openFileDialog1.Title = (IsSubroute) ? "Add subroute" : "Open route";
			if (openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				My.Log($"{openFileDialog1.Title} {openFileDialog1.FileName } {((IsSubroute) ? " at " + Overlay : "")}");
				if (!Overlay.OpenRoute(openFileDialog1.FileName, IsSubroute))
				{
					if (Overlay.Route.IsNotSupported) { My.Show($"Sorry, filetype {Path.GetExtension(openFileDialog1.FileName)} is not supported yet."); return; }
					My.Status($"Error This file contains no Gps markers.");
				}
				//
				// If subroute is loaded then keep original filename.
				//
				if (IsSubroute) My.Status($"Subroute {openFileDialog1.FileName} Succesfully added.");
				this.Text = Title + Overlay.Route;
				//IsDragging = true;// Doubleclick in fileopenmenu -> adds marker.
				LastEnteredMarker = null;
			}
		}
		private void AddtoolStripMenuItem_Click(object sender, EventArgs e)
		{
			var idx = Overlay.GetIndexRed(Overlay.CurrentMarker);
			if (idx == -1) { My.Show("Please open a mainroute first."); return; }
			if (My.Show($"Do you want to insert subroute at current Gps marker {idx}?", null, MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
			{
				OpenToolStripMenuItem_Click("SubRoute", null);
			}
		}
		private bool AskToSaveModifiedRoute()
		{
			if (Overlay.IsChanged)
			{
				var res = My.Show($"Do you want to save changes to {Overlay.Route.FileName}?", "Route is changed.", MessageBoxButtons.YesNoCancel);
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
					My.Show("Sorry, this function is not implemented yet.");
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
			My.Log($"Search {txtSearchPlaces.Text}");
			if (gmap.MapProvider == BingHybridMapProvider.Instance) comboBox1.SelectedItem = OpenStreetMapProvider.Instance;
			gmap.Zoom = 14;
			gmap.SetPositionByKeywords(txtSearchPlaces.Text);
		}

		private void txtSearch_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			if (e.KeyCode == Keys.Enter) txtSearch_Validated(null, null);
		}
		#endregion
		#region RIGHT PANEL
		private void btnClear_Click(object sender, EventArgs e)
		{
			My.Status($"Clear Route {Overlay.Route}.");
			Overlay.Clear();
			IsOnMarker = false;
		}
		private void chkGpsPoints_CheckedChanged(object sender, EventArgs e)
		{
			foreach (var item in gmap.Overlays[0].Markers) item.IsVisible = chkGpsPoints.Checked;
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
		private void chkArrowMarker_CheckedChanged(object sender, EventArgs e)
		{
			Settings.ArrowMarker = (sender as CheckBox).Checked;
			gmap.Overlays[3].IsVisibile = Settings.ArrowMarker;
		}
		private void chkShowTooltip_CheckedChanged(object sender, EventArgs e) => Settings.ToolTipMode = chkShowTooltip.Checked;
		private void chkAutoRoute_CheckedChanged(object sender, EventArgs e)
		{
			Overlay.IsAutoRoute = (sender as CheckBox).Checked;
		}
		private void btnAutoNavigate_Click(object sender, EventArgs e)
		{
			chkNavPoints.Checked = true;
			Overlay.AutoNavigate();
		}
		private void btnChangeGlobalDosage_Click(object sender, EventArgs e)
		{
			var from = (double)numDosageFrom.Value;
			var to = (double)numDosageTo.Value;
			var res = Overlay.UpdateDosageAllChangeMarkers(from, to);
			if (res > 0)
				My.Status($"Dosage of {res} Changemarkers successfull updated from {from} to {to} gr/m2.");
			else
				My.Status($"No Changemarkers found with dosage {from}.", Color.Red);
			numDosageFrom.Value = numDosageTo.Value;
		}
		#endregion
	}
}
