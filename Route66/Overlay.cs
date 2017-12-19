using GMap.NET.WindowsForms;
using System.Drawing;
using System;
using GMap.NET;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.ObjectModel;
using System.Windows.Forms;
using MyLib;
using GMap.NET.MapProviders;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Route66
{
	internal class Overlay
	{
		#region FIELDS
		private GMapControl gmap;
		/// <summary>
		/// This field contains Lat,Lng coordinates of the last clicked Red marker.
		/// </summary>
		public GMapMarker CurrentMarker;
		private readonly GMapRoute RedRoute;
		private readonly GMapOverlay Red;
		private readonly GMapOverlay Green;
		private readonly GMapOverlay Blue;
		private readonly GmarkerRotate ArrowMarker;
		#endregion
		#region CONSTRUCTOR
		public Overlay(GMapControl gmap)
		{
			this.gmap = gmap;
			gmap.Overlays.Add(new GMapOverlay("Gps points"));
			gmap.Overlays.Add(new GMapOverlay("Change points"));
			gmap.Overlays.Add(new GMapOverlay("Navigation points"));
			gmap.Overlays.Add(new GMapOverlay("Arrow marker"));
			Red = gmap.Overlays[0];
			Green = gmap.Overlays[1];
			Blue = gmap.Overlays[2];

			ArrowMarker = new GmarkerRotate(new PointLatLng(1, 1), Properties.Resources.arrow3);
			ArrowMarker.IsHitTestVisible = false;
			gmap.Overlays[3].Markers.Add(ArrowMarker);

			RedRoute = new GMapRoute("routes")
			{
				Stroke = new Pen(Color.Red, 2)
			};
			Red.Routes.Add(RedRoute);
			Settings = Settings.Global;
			Route = Route.Load();
		}
		#endregion
		#region PROPERTIES
		/// <summary>
		/// True when intermediat markers will be placed on road.
		/// </summary>
		public bool IsAutoRoute { get; internal set; }
		public Settings Settings { get; }
		/// <summary>
		/// Route data of current route on form.
		/// Filled during Save. 
		/// </summary>
		public Route Route { get; set; }
		public MachineTypes MachineType
		{
			get => Route.MachineType;
			set { Route.MachineType = value; if (!Route.IsDefaultFile) Route.IsChanged = true; }
		}
		public bool IsChanged { get => Route.IsChanged; }
		#endregion
		#region METHODES
		internal void Remove(GMapMarker marker)
		{
			var idx = GetIndexRed(marker);
			if (idx >= 0)
			{
				Console.WriteLine($"Remove marker {marker.Info()}");
				Red.Markers.Remove(marker);
				RedRoute.Points.RemoveAt(idx);
				gmap.UpdateRouteLocalPosition(RedRoute);
				if (idx > 0)
					SetCurrentMarker(Red.Markers[idx - 1]);
				else if (Red.Markers.Count > 0)
					SetCurrentMarker(Red.Markers[idx]);
				else
					SetCurrentMarker(null);
				Route.IsChanged = true;
			}
			UpdateGreenAndBlueOverlay(Crud.Delete, marker.Tag, null);
			//
			// When Change- or Navigation marker is deleted then remove Gpspoint tag.
			//
			if (!IsGpsMarker(marker)) FindRedMarker(marker.Tag as GpsMarker).Tag = null;
		}
		/// <summary>
		/// When Mouse is moved update position of:
		/// CurrentMarker, 
		/// Red, green and blue Markers,
		/// RedRoute points, 
		/// ChangeMarker and NavigationMarker instances
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public bool UpdateCurrentMarkerPosition(int x, int y)
		{
			if (CurrentMarker == null) return false;
			var newPosition = gmap.FromLocalToLatLng(x, y);/// +11
			//
			// Use Gps marker.
			//
			if (!IsGpsMarker(CurrentMarker)) CurrentMarker = FindRedMarker(CurrentMarker.Tag as GpsMarker);

			if (CurrentMarker.Tag is ChangeMarker) GetGreenMarker(CurrentMarker).Position = newPosition;
			if (CurrentMarker.Tag is NavigationMarker) GetBlueMarker(CurrentMarker).Position = newPosition;
			// 
			// Update Route point.
			// Allways use currentmarker class to find index because we can have duplicated Positions (struct)!
			//
			var idx = GetIndexRed(CurrentMarker);
			RedRoute.Points[idx] = newPosition;
			gmap.UpdateRouteLocalPosition(RedRoute);
			//
			// Update current marker position.
			// Red marker will implicit be updated.
			//
			//Red.Markers[idx].Position = newPosition;
			CurrentMarker.Position = newPosition;
			if (CurrentMarker.Tag != null)
			{
				(CurrentMarker.Tag as GpsMarker).Lat = newPosition.Lat;
				(CurrentMarker.Tag as GpsMarker).Lng = newPosition.Lng;
			}
			Route.IsChanged = true;
			//
			// Update arrowmarker.
			//
			ArrowMarker.Position = newPosition;
			return true;
		}
		/// <summary>
		/// Create overlay route from red markers.
		/// </summary>
		private void CreateOverlayRoute()
		{
			RedRoute.Points.Clear();
			foreach (var item in Red.Markers)
			{
				RedRoute.Points.Add(item.Position);
			}
			gmap.UpdateRouteLocalPosition(RedRoute);
		}
		internal GMapMarker AddMarker(int x, int y)
		{
			PointLatLng point = gmap.FromLocalToLatLng(x, y);
			//
			// If autoroute is enabled then start autorouter.
			//
			if (IsAutoRoute && RedRoute.Points.Count > 0)
			{
				if (CurrentMarker != Red.Markers.Last() && MessageBox.Show($"Are you sure to insert route at current marker?", "Current marker in not at end of route.", MessageBoxButtons.YesNo) == DialogResult.No) return CurrentMarker;
				var end = new GMarkerGoogle(point, GMarkerGoogleType.red_small);
				My.Status($"Moment. Autoroute started at {CurrentMarker.ToolTipText}, stop {end.Position}");
				Application.DoEvents();

				var route = AutoRouter(CurrentMarker, end);

				My.Status((route == null) ? "Please check your internet connection." : "Ready");
				if (route?.Points.Count > 0)
				{
					route.Points.RemoveAt(0);
					foreach (var p in route.Points) AddMarker(p);
				}
			}
			else AddMarker(point);
			SetTooltipRed(CurrentMarker); // Not nessesarry but nice 4 debugging.
			ShowArrowMarker(CurrentMarker);
			Route.IsChanged = true;
			return CurrentMarker;
		}
		private bool AddMarker(PointLatLng point)
		{
			var marker = new GMarkerGoogle(point, (Red.Markers.Count == 0) ? GMarkerGoogleType.green_big_go : GMarkerGoogleType.red_small);
			var idx = GetIndexRed(CurrentMarker) + 1;
			CurrentMarker = marker;
			Red.Markers.Insert(idx, marker);
			RedRoute.Points.Insert(idx, point);
			gmap.UpdateRouteLocalPosition(RedRoute);
			Console.WriteLine($"Marker {idx} added at {marker.LocalPosition}");
			return true;
		}
		private MapRoute AutoRouter(GMapMarker start, GMapMarker end)
		{
			RoutingProvider rp = GMapProviders.OpenStreetMap; // use OpenStreetMap if provider does not implement routing
			return rp.GetRoute(start.Position, end.Position, false, false, 2);
		}

		public void Clear()
		{
			foreach (var overlay in gmap.Overlays)
			{
				if (overlay.Id.Contains("Arrow marker")) continue;
				overlay.Markers.Clear();
				foreach (var route in overlay.Routes) route.Clear();
			}
			gmap.UpdateRouteLocalPosition(RedRoute);
			SetCurrentMarker(null);
		}
		internal int GetIndexAny(GMapMarker item)
		{
			if (item == null) return -1;
			if (IsGpsMarker(item)) return GetIndexRed(item);
			return GetIndexRed(FindRedMarker(item.Tag as GpsMarker));
		}

		internal int GetIndexRed(GMapMarker item) => Red.Markers.IndexOf(item);

		public void SetTooltipRed(GMapMarker item)
		{
			var idx = GetIndexRed(item);
			if (idx >= 0)
			{
				item.ToolTipMode = (Settings.ToolTipMode) ? MarkerTooltipMode.OnMouseOver : MarkerTooltipMode.Never;
				item.ToolTipText = $"{idx}";
			}
		}

		public override string ToString() => $"Current marker={CurrentMarker?.Overlay.Id} {GetIndexRed(CurrentMarker)}, Tag={CurrentMarker?.Tag}, Total distance={RedRoute.Distance:f3} km.";

		public void SetCurrentMarker(GMapMarker item)
		{
			Console.WriteLine($"Set CurrentMarker {item.Info()}");
			//
			// Always use red marker. unittest: Set NOGPS and drag green marker.
			//
			if (item == null || IsGpsMarker(item))
				CurrentMarker = item; // On up/down key red pos is used. 
			else
			{
				CurrentMarker = FindRedMarker(item.Tag as GpsMarker);
				CurrentMarker.LocalPosition = item.LocalPosition; // So when dragging copy Blue pos into red pos.
			}
			ShowArrowMarker(item);
		}

		private void ShowArrowMarker(GMapMarker item)
		{
			ArrowMarker.IsVisible = item != null;
			if (item != null)
			{
				ArrowMarker.Position = item.Position;
				ArrowMarker.Angle = Angle(item);
			}
		}

		private float Angle(GMapMarker currentMarker)
		{
			var angle = 0d;
			var idx = GetIndexRed(CurrentMarker); // when noGPS currentMarker (= ChangeMarker) return -1 
			const double DEG = 180 / Math.PI;
			//
			// If last marker then use angele of previous marker.
			//
			if (idx == Red.Markers.Count - 1 && idx > 0)
			{
				idx -= 1;
				currentMarker = Red.Markers[idx];
			}
			//
			// Compute dy and dx.
			//
			if (idx < Red.Markers.Count - 1)
			{
				var nextMarker = Red.Markers[idx + 1];
				var dy = nextMarker.LocalPosition.Y - currentMarker.LocalPosition.Y;
				var dx = nextMarker.LocalPosition.X - currentMarker.LocalPosition.X;
				//
				// Compensate for Starting Marker size.
				//
				if (idx == 0) dy += currentMarker.Offset.Y - nextMarker.Offset.Y;
				if (idx == 0) dx += currentMarker.Offset.X - nextMarker.Offset.X;

				angle = Math.Atan2(dy, dx) * DEG;
				//Console.WriteLine($"Angle={angle} nextmarker={nextMarker.LocalPosition} currentmarker={currentMarker.LocalPosition}");
			}
			return (float)angle;
		}

		private GMapMarker FindRedMarker(GpsMarker m) // Class compare. save
		{
			foreach (var item in Red.Markers) if (item.Tag == m) return item;
			return null;
		}
		private GMapMarker FindRedMarker(double lat, double lng) // Struct compare.
		{
			var pos = new PointLatLng(lat, lng);
			foreach (var item in Red.Markers) if (item.Position == pos) return item;
			return null;
		}

		/// <summary>
		/// Determine current marker type: Change marker or Navigation marker.
		/// Show properties of current marker on windows form.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool EditMarker(bool IsNavigationMarker)
		{
			Form form = null;
			if (CurrentMarker == null) return false;
			var originalTag = CurrentMarker.Tag;// if DeepClone(); then marker is not removed from route on delete.
			var before = (CurrentMarker.Tag != null) ? 2 : 0;
			if (CurrentMarker.Tag is ChangeMarker) form = new FormEditChangeMarker(CurrentMarker);
			else if (CurrentMarker.Tag is NavigationMarker) form = new FormEditNavigationMarker(CurrentMarker);
			else if (CurrentMarker.Tag == null) if (IsNavigationMarker) form = new FormEditNavigationMarker(CurrentMarker);
				else form = new FormEditChangeMarker(CurrentMarker);
			else My.Log($"Error during edit Tag {CurrentMarker.Tag}");

			form.ShowDialog();
			//
			// Determine CRUD operation (Create, Update or Delete).
			// 0= No operation, 1 = Create, 2 = Delete, 3 = Update.
			//
			var after = (CurrentMarker.Tag != null) ? 1 : 0;
			var crud = (Crud)before + after;

			UpdateGreenAndBlueOverlay(crud, originalTag, CurrentMarker.Tag);
			Route.IsChanged = crud != Crud.None;
			return true;
		}
		private enum Crud { None, Create, Delete, Update }
		private void UpdateGreenAndBlueOverlay(Crud crud, object origin, object tag)
		{
			if (tag != null) Console.WriteLine($"{crud} {tag.ToString().Replace('\n', ' ')}");
			else if (origin != null) Console.WriteLine($"{crud} {origin.ToString().Replace('\n', ' ')}");

			switch (crud)
			{
				case Crud.None:
					break;
				case Crud.Create:
					if (tag is ChangeMarker) AddOverlayGreenMarker(CurrentMarker);
					if (tag is NavigationMarker) AddOverlayBlueMarker(CurrentMarker);
					break;
				case Crud.Delete:
					if (origin is ChangeMarker) foreach (var item in Green.Markers) if (item.Tag == origin)
							{
								Green.Markers.Remove(item); return;
							}
					if (origin is NavigationMarker) foreach (var item in Blue.Markers) if (item.Tag == origin)
							{
								Blue.Markers.Remove(item); return;
							}
					break;
				case Crud.Update:
					if (tag is ChangeMarker) GetGreenMarker(CurrentMarker).ToolTipText = $"{tag}";
					if (tag is NavigationMarker) GetBlueMarker(CurrentMarker).ToolTipText = $"{tag}";
					break;
				default:
					My.Log($"Error crud operation {crud}");
					break;
			}
		}

		#region OPEN SAVE CONVERT ROUTE
		internal bool OpenRoute(string fileName, bool IsSubroute = false)
		{
			if (!IsSubroute) Clear();
			Route = Route.Load(fileName);
			if (Route.GpsMarkers.Count == 0) return false;
			ConvertRoute(Route);
			LoadOverlay(Route);
			if (IsSubroute) Route.IsChanged = true;
			return true;
		}

		private void ConvertRoute(Route route)
		{
			if (Route.MachineType != Settings.MachineType)
			{
				//
				// Todo convert route.
				//
				My.Status($"Route converted from {route.MachineType} to {Settings.MachineType}.");
				route.MachineType = Settings.MachineType;
				route.IsChanged = true;
			}
		}

		internal void RemoveCurrentMarker()
		{
			Remove(CurrentMarker);
		}

		internal void SetArrowMarker(bool forward)
		{
			var idx = GetIndexRed(CurrentMarker);
			if (idx >= 0)
			{
				idx += (forward) ? 1 : -1;
				idx = InRange(idx, 0, Red.Markers.Count - 1);
				SetCurrentMarker(Red.Markers[idx]);
				if (idx == Red.Markers.Count - 1) My.Status($"End of route. Gps marker {idx}."); else My.Status(" Ready");
			}
		}

		private int InRange(int val, int min, int max)
		{
			if (val < min) return min;
			if (val > max) return max;
			return val;
		}

		internal string Save()
		{
			var fileName = (Route.IsDefaultFile) ? Path.Combine(Settings.RoutePath, "Route66.xml") : Route.FileName;
			SaveAs(fileName);
			return fileName;
		}

		internal void SaveAs(string fileName)
		{
			CopyOverlayTo(Route);
			Route.SaveAs(fileName);
		}

		/// <summary>
		/// Copy Route class into overlays.
		/// </summary>
		/// <param name="route"></param>
		private void LoadOverlay(Route route)
		{
			foreach (var red in route.GpsMarkers) AddMarker(new PointLatLng(red.Lat, red.Lng));
			//
			// Copy green and blue into red markers tags.
			//
			foreach (var item in route.ChangeMarkers)
			{
				var rm = FindRedMarker(item.Lat, item.Lng);
				if (rm != null)
				{
					rm.Tag = item;
					AddOverlayGreenMarker(rm);
				}
				else My.Status($"Green marker {item} not found in red markers list.");
			}
			foreach (var item in route.NavigationMarkers)
			{
				var rm = FindRedMarker(item.Lat, item.Lng);
				if (rm != null)
				{
					rm.Tag = item;
					AddOverlayBlueMarker(rm);
				}
				else My.Status($"Blue marker {item} not found in red markers list.");
			}
			//
			// Set currentMarker to last point. (Autorouter enabled and leftmouse on empty map)
			//
			SetCurrentMarker(Red.Markers[Red.Markers.Count - 1]);
			CreateOverlayRoute();
			gmap.ZoomAndCenterRoute(RedRoute);
		}

		/// <summary>
		/// Copy overlays into Route class.
		/// </summary>
		/// <param name="route"></param>
		private void CopyOverlayTo(Route route)
		{
			route.GpsMarkers.Clear();
			route.ChangeMarkers.Clear();
			route.NavigationMarkers.Clear();
			foreach (var item in Red.Markers)
			{
				route.GpsMarkers.Add(new GpsMarker(item.Position.Lng, item.Position.Lat));
				if (item.Tag is ChangeMarker) route.ChangeMarkers.Add(item.Tag as ChangeMarker);
				if (item.Tag is NavigationMarker) route.NavigationMarkers.Add(item.Tag as NavigationMarker);
			}
		}
		#endregion
		private GMapMarker GetGreenMarker(GMapMarker currentMarker)
		{
			foreach (var item in Green.Markers) if (item.Tag == currentMarker.Tag) return item;
			return null;
		}
		private GMapMarker GetBlueMarker(GMapMarker currentMarker)
		{
			foreach (var item in Blue.Markers) if (item.Tag == currentMarker.Tag) return item;
			return null;
		}
		private void AddOverlayGreenMarker(GMapMarker currentMarker)
		{
			var marker = new GMarkerGoogle(currentMarker.Position, GMarkerGoogleType.green_small);
			marker.Tag = currentMarker.Tag;
			marker.ToolTipText = currentMarker.Tag.ToString();
			Green.Markers.Add(marker);
		}
		private void AddOverlayBlueMarker(GMapMarker currentMarker)
		{
			var marker = new GMarkerGoogle(currentMarker.Position, GMarkerGoogleType.blue_small);
			marker.Tag = currentMarker.Tag;
			marker.ToolTipText = currentMarker.Tag.ToString();
			Blue.Markers.Add(marker);
		}
		/// <summary>
		/// Update all change markers with dosage <from> to dosage <to>.
		/// if from dosage = 0 then update all change markers.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		internal int UpdateAllChangeMarkers(double from, double to)
		{
			var cnt = 0;
			foreach (var item in Green.Markers)
			{
				var x = item.Tag as ChangeMarker;
				if (x.Dosage == from || from == 0)
				{
					x.Dosage = to;
					item.ToolTipText = x.ToString();
					++cnt;
				}
			}
			return cnt;
		}
		#region MARKER OVERLAY TYPES
		public bool IsChangeMarker(GMapMarker item) => item.Overlay.Id == "Change points";
		public bool IsNavigationMarker(GMapMarker item) => item.Overlay.Id == "Navigation points";
		public bool IsGpsMarker(GMapMarker item) => item.Overlay.Id == "Gps points";
		#endregion
		#endregion
	}
}