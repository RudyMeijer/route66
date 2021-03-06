﻿// <copyright file="Overlay.cs" company="Aebi Schmidt Nederland B.V.">
//   Aebi Schmidt Nederland B.V. All rights reserved.
// </copyright>

namespace Route66
{
	using System;
	using System.Drawing;
	using System.Linq;
	using System.Windows.Forms;
	using GMap.NET;
	using GMap.NET.MapProviders;
	using GMap.NET.WindowsForms;
	using GMap.NET.WindowsForms.Markers;
	using MyLib;
	using static Route66.DataContracts;

	/// <summary>This class exposes overlay related methods and properties. </summary>
	public class Overlay
	{
		#region FIELDS
		private readonly GMapControl gmap;
		private bool Initialize;
		private GpsMarker savedTag;
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
				Stroke = new Pen(Color.Red, 3)
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
		/// This class contains all route data and is serialyzed during Save command.
		/// </summary>
		public Route Route { get; set; }
		public MachineTypes MachineType
		{
			get => Route.MachineType;
			set { Route.MachineType = value; if (!Route.IsDefaultFile) Route.IsChanged = true; }
		}
		public bool IsChanged { get => Route.IsChanged; }
		public bool IsNotOnLastMarker { get => IsAutoRoute && RedRoute.Points.Count > 0 && CurrentMarker != Red.Markers.Last(); }
		/// <summary>
		/// This field contains Lat,Lng coordinates of the last clicked Red marker.
		/// </summary>
		public GMapMarker CurrentMarker { get; set; }
		#endregion
		#region METHODS
		internal void AddMarker(int x, int y)
		{
			PointLatLng point = gmap.FromLocalToLatLng(x, y);
			//
			// If autoroute is enabled then start autorouter.
			//
			if (IsAutoRoute && RedRoute.Points.Count > 0)
			{
				var end = new GMarkerGoogle(point, GMarkerGoogleType.red_small);
				My.Status($"Moment. Autoroute started at {CurrentMarker.ToolTipText} {CurrentMarker.Position}, stop {end.Position}");
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
			SetTooltip(CurrentMarker); // Not nessesarry but nice 4 debugging.
			ShowArrowMarker(CurrentMarker);
			Route.IsChanged = true;
			gmap.UpdateRouteLocalPosition(RedRoute);
			My.Log($"Add marker {CurrentMarker.Info()}.");
		}
		private void AddMarker(PointLatLng point)
		{
			var marker = new GMarkerGoogle(point, (Red.Markers.Count == 0) ? GMarkerGoogleType.green_big_go : GMarkerGoogleType.red_small);
			var idx = GetIndexRed(CurrentMarker) + 1;
			CurrentMarker = marker;
			Red.Markers.Insert(idx, marker);
			RedRoute.Points.Insert(idx, point);
		}
		internal void RemoveCurrentMarker()
		{
			Remove(CurrentMarker);
		}
		internal void Remove(GMapMarker marker)
		{
			if (marker == null) return;
			var idx = GetIndexRed(marker);
			if (idx >= 0)
			{
				My.Log($"Remove marker {marker.Info()}");
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
			// When Change- or Navigation marker is deleted then remove corresponding Gps marker tag.
			//
			if (!IsGpsMarker(marker)) GetRedMarker(marker.Tag as GpsMarker).Tag = null;
		}
		/// <summary>
		/// When Mouse is moved update position of:
		/// CurrentMarker, 
		/// Red, green, blue and arrow Marker,
		/// RedRoute points, 
		/// Change- and Navigation marker instances,
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
			if (!IsGpsMarker(CurrentMarker)) CurrentMarker = GetRedMarker(CurrentMarker.Tag as GpsMarker);
			//
			// Update green and blue marker position.
			//
			if (CurrentMarker.Tag is ChangeMarker) GetGreenMarker(CurrentMarker).Position = newPosition;
			if (CurrentMarker.Tag is NavigationMarker) GetBlueMarker(CurrentMarker).Position = newPosition;
			// 
			// Update Route point. Allways use currentmarker class to find index because we can have duplicated Positions (struct)!
			//
			var idx = GetIndexRed(CurrentMarker);
			RedRoute.Points[idx] = newPosition;
			gmap.UpdateRouteLocalPosition(RedRoute);
			//
			// Update current marker position. Red marker will implicit be updated.
			//
			////Red.Markers[idx].Position = newPosition;
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
		private MapRoute AutoRouter(GMapMarker start, GMapMarker end)
		{
			RoutingProvider rp = GMapProviders.OpenStreetMap; // use OpenStreetMap if provider does not implement routing
			return rp.GetRoute(start.Position, end.Position, false, false, (int)gmap.Zoom);
		}
		public void SetTooltip(GMapMarker item)
		{
			var idx = GetIndexRed(item);
			if (idx >= 0)
			{
				item.ToolTipMode = (Settings.ToolTipMode) ? MarkerTooltipMode.OnMouseOver : MarkerTooltipMode.Never;
				item.ToolTipText = $"{idx}";
			}
			//
			// Set Change- and Navigation tooltiptext.
			//
			if (IsChangeMarker(item) || IsNavigationMarker(item))
			{
				item.ToolTipText = $"{item.Tag}";
			}
		}

		public override string ToString() => CurrentMarker.Info();

		public void SetCurrentMarker(GMapMarker item)
		{
			Console.WriteLine($"Set CurrentMarker {item.Info()}");
			//
			// Always use red marker. unittest: Set NOGPS and drag green marker.
			//
			if (item == null || IsGpsMarker(item))
			{
				CurrentMarker = item; // On up/down key red pos is used. 
			}
			else
			{
				CurrentMarker = GetRedMarker(item.Tag as GpsMarker);
				if (GetIndexRed(CurrentMarker) > 0 && CurrentMarker.LocalPosition != item.LocalPosition)
					CurrentMarker.LocalPosition = item.LocalPosition; // gives jumping big marker. So when dragging copy Blue pos into red pos.
			}
			ShowArrowMarker(item);
			if (!Initialize) My.Status($" Info: {item.Info()}");
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
			var idx = GetIndexAny(currentMarker); // when noGPS currentMarker (= ChangeMarker) return -1 
			const double DEG = 180 / Math.PI;
			//
			// If this is last marker then use angele of previous marker.
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
				////Console.WriteLine($"Angle={angle} nextmarker={nextMarker.LocalPosition} currentmarker={currentMarker.LocalPosition}");
			}
			return (float)angle;
		}
		#region FIND & GET MARKERS
		internal int GetIndexAny(GMapMarker item)
		{
			if (item == null) return -1;
			if (IsGpsMarker(item)) return GetIndexRed(item);
			return GetIndexRed(GetRedMarker(item.Tag as GpsMarker));
		}
		internal int GetIndexRed(GMapMarker item) => Red.Markers.IndexOf(item);
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
		private GMapMarker GetRedMarker(GpsMarker m) // Class compare. save
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

		internal void CutMarker()
		{
			if (CurrentMarker?.Tag != null)
			{
				savedTag = CurrentMarker.Tag as GpsMarker;
				savedTag.Lat = 0;
				savedTag.Lng = 0;
				UpdateGreenAndBlueOverlay(Crud.Delete, CurrentMarker.Tag, null);
				CurrentMarker.Tag = null;
			}
		}

		internal void PastMarker()
		{
			if (CurrentMarker?.Tag != null) UpdateGreenAndBlueOverlay(Crud.Delete, CurrentMarker.Tag, null);
			{
				if (savedTag == null) { My.Show("Please first press X-key on Change- or Navigation marker."); return; }
				CurrentMarker.Tag = savedTag.DeepClone(); // Make multicopy possible.
				(CurrentMarker.Tag as GpsMarker).Lat = CurrentMarker.Position.Lat;
				(CurrentMarker.Tag as GpsMarker).Lng = CurrentMarker.Position.Lng;
				UpdateGreenAndBlueOverlay(Crud.Create, null, CurrentMarker.Tag);
			}
		}

		internal void Test()
		{
			ShowGreenRoute();
		}

		/// <summary>
		/// This function draw route tracks, where dosing occurs, as green route.
		/// </summary>
		public void ShowGreenRoute()
		{
			Console.WriteLine("ShowDosageRoute");
			var DosingState = false; // True when dosage/spaying is active.
			GMapRoute GreenRoute = null;
			Green.Routes.Clear();
			foreach (var rm in Red.Markers)
			{
				if (rm.Tag is ChangeMarker || IsLast(rm))
				{
					var cm = rm.Tag as ChangeMarker;
					//
					// If dosing state is off and change marker dosing is on then start green route.
					// Else if dosing state is on and change marker dosing is off then end green route.
					// Else add red markers to green route.
					//
					if (!DosingState && IsDosingOn(cm)) // Dosage turned on.
					{
						DosingState = true;
						GreenRoute = new GMapRoute("dosage") { Stroke = new Pen(Color.Green, 3) };
					}
					else if (DosingState && !IsDosingOn(cm)) // Dosage turned off.
					{
						DosingState = false;
						GreenRoute.Points.Add(rm.Position);
						Green.Routes.Add(GreenRoute);
					}
				}
				if (DosingState) GreenRoute.Points.Add(rm.Position);
			}
			if (DosingState) Green.Routes.Add(GreenRoute); // Last point dosing is on.
		}

		private bool IsDosingOn(ChangeMarker cm)
		{
			if (cm == null) return false;
			var IsSprayer = MachineType == MachineTypes.Sprayer || MachineType == MachineTypes.WspDosage || MachineType == MachineTypes.RspDosage;
			var IsDosing = MachineType != MachineTypes.Sprayer;

			return (IsDosing && cm.SpreadingOnOff) || (IsSprayer && cm.SprayingOnOff);
		}
		#endregion
		/// <summary>
		/// Show properties of current marker on windows form.
		/// Current marker type: Change marker or Navigation marker.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool EditMarker(bool IsNavigationMarker)
		{
			Form form = null;
			if (CurrentMarker == null) return false;
			var originalTag = this.CurrentMarker.Tag; // if DeepClone(); then marker is not removed from route on delete.
			var before = (CurrentMarker.Tag != null) ? 2 : 0;

			if (CurrentMarker.Tag == null)
			{
				if (IsNavigationMarker)
					form = new FormEditNavigationMarker(CurrentMarker);
				else
					form = new FormEditChangeMarker(CurrentMarker);
			}
			else if (CurrentMarker.Tag is ChangeMarker) form = new FormEditChangeMarker(CurrentMarker);
			else if (CurrentMarker.Tag is NavigationMarker) form = new FormEditNavigationMarker(CurrentMarker);

			form?.ShowDialog();
			//
			// Determine CRUD operation (None, Create, Update or Delete).
			// 0= No operation, 1 = Create, 2 = Delete, 3 = Update.
			//
			var after = (CurrentMarker.Tag != null) ? 1 : 0;
			var crud = (Crud)before + after;
			if (CurrentMarker.Tag != null) My.Log($"crud={crud} {CurrentMarker.Info()}");
			else if (originalTag != null) My.Log($"crud={crud} {CurrentMarker.Info()}   {originalTag.ToString().Replace('\n', ' ')}");
			UpdateGreenAndBlueOverlay(crud, originalTag, CurrentMarker.Tag);
			Route.IsChanged = crud != Crud.None;
			return true;
		}
		private enum Crud { None, Create, Delete, Update }
		private void UpdateGreenAndBlueOverlay(Crud crud, object origin, object tag)
		{
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
					break;
				default:
					My.Log($"Error crud operation {crud}");
					break;
			}
		}
		#region OPEN SAVE CONVERT ROUTE
		public bool OpenRoute(string fileName, bool IsSubroute = false)
		{
			Initialize = true;
			if (!IsSubroute) Clear();
			var originalFilename = Route.FileName;
			Route = Route.Load(fileName);
			if (Route.GpsMarkers.Count == 0) return false;
			ConvertRoute(Route);
			LoadOverlay(Route);
			//
			// If subroute is loaded then keep original filename.
			//
			if (IsSubroute)
			{
				Route.FileName = originalFilename;
				Route.IsChanged = true;
				gmap.UpdateRouteLocalPosition(RedRoute);
			}
			else gmap.ZoomAndCenterRoute(RedRoute);
			ShowGreenRoute();
			Initialize = false;
			return true;
		}

		private void ConvertRoute(Route route)
		{
			if (Route.MachineType != Settings.MachineType)
			{
				//
				// Todo convert route.
				//
				My.Status($"Route succesfull converted from {route.MachineType} to {Settings.MachineType}.", Color.LightGreen);
				route.MachineType = Settings.MachineType;
				route.IsChanged = true;
			}
		}

		public void Clear()
		{
			foreach (var overlay in gmap.Overlays)
			{
				if (overlay.Id.Contains("Arrow marker")) continue;
				overlay.Markers.Clear();
				if (overlay != Red) overlay.Routes.Clear();
				foreach (var route in overlay.Routes) route.Clear();
			}
			gmap.UpdateRouteLocalPosition(RedRoute);
			SetCurrentMarker(null);
		}

		internal GMapMarker SetArrowMarker(int offset)
		{
			var idx = GetIndexRed(CurrentMarker);
			if (idx >= 0)
			{
				idx += offset;
				idx = InRange(idx, 0, Red.Markers.Count - 1);
				SetCurrentMarker(Red.Markers[idx]);
				if (idx == Red.Markers.Count - 1)
				{
					My.Status($" End of route. Gps marker {idx}.");
					if (Settings.SpeechSyntesizer) My.PlaySound(" End of route.");
				}
				else if (CurrentMarker.Tag is NavigationMarker && Settings.SpeechSyntesizer)
				{
					My.PlaySound((CurrentMarker.Tag as NavigationMarker).Message);
				}
			}
			return CurrentMarker;
		}

		private int InRange(int val, int min, int max)
		{
			if (val < min) return min;
			if (val > max) return max;
			return val;
		}

		internal bool SaveAs(string fileName)
		{
			CopyOverlayTo(Route);
			return Route.SaveAs(fileName);
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
				else My.Status($"Green marker {new PointLatLng(item.Lat, item.Lng)} not found in red markers list.", Color.Red);
			}
			foreach (var item in route.NavigationMarkers)
			{
				var rm = FindRedMarker(item.Lat, item.Lng);
				if (rm != null)
				{
					if (rm.Tag == null)
					{
						rm.Tag = item;
						AddOverlayBlueMarker(rm);
					}
					else My.Status($"Can't add navigation instruction {item} to red marker tag {rm.Tag}.");
				}
				else My.Status($"Blue marker {new PointLatLng(item.Lat, item.Lng)} not found in red markers list.", Color.Red);
			}
			//
			// Set currentMarker to last point. (Autorouter enabled and leftmouse on empty map)
			//
			SetCurrentMarker(Red.Markers[Red.Markers.Count - 1]);
			My.Log($"{Red.Markers.Count} Gps markers, {Green.Markers.Count} Change markers, {Blue.Markers.Count} Navigation markers, {RedRoute.Distance} km.");
		}

		/// <summary>
		/// Add navigation markers, based on angle with previous marker.
		/// </summary>
		internal void AutoNavigate()
		{
			var cnt = 0;
			foreach (var item in Red.Markers) if (AddNavigation(item)) ++cnt;
			My.Status($"{cnt} navigation markers succesfull added.");
		}

		private bool AddNavigation(GMapMarker marker)
		{
			if (marker == null || marker.Tag != null || !IsGpsMarker(marker)) return false;
			var uturn = 30f;
			var marge = 45f;
			string message = null;
			var angle = GetAngeWithPreviousItem(marker);
			if (angle < uturn || angle > 360 - uturn) message = Translate.NavigationMessages[(int)NavigationTypes.U_TURN];
			if (angle > 90 - marge && angle < 90 + marge) message = Translate.NavigationMessages[(int)NavigationTypes.TURN_RIGHT];
			if (angle > 270 - marge && angle < 270 + marge) message = Translate.NavigationMessages[(int)NavigationTypes.TURN_LEFT];
			if (message != null)
			{
				var tag = new NavigationMarker(marker.Position);
				tag.Message = message;
				tag.SoundFile = My.ValidateFilename(message) + ".wav";
				marker.Tag = tag;
				AddOverlayBlueMarker(marker);
			}
			return message != null;
		}

		private float GetAngeWithPreviousItem(GMapMarker item)
		{
			var idx = GetIndexRed(item);
			if (idx <= 0) return 180;
			var prevItem = Red.Markers[idx - 1];
			var angle = 180 - Angle(item) + Angle(prevItem);
			if (angle < 0) angle += 360;
			if (angle > 360) angle -= 360;
			My.Status($" angle={angle}");
			return angle;
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
				route.GpsMarkers.Add(new GpsMarker(item.Position));
				if (item.Tag is ChangeMarker) route.ChangeMarkers.Add(item.Tag as ChangeMarker);
				if (item.Tag is NavigationMarker) route.NavigationMarkers.Add(item.Tag as NavigationMarker);
			}
		}
		#endregion
		private void AddOverlayGreenMarker(GMapMarker currentMarker)
		{
			var marker = new GMarkerGoogle(currentMarker.Position, GMarkerGoogleType.green_small);
			marker.Tag = currentMarker.Tag;
			Green.Markers.Add(marker);
		}
		private void AddOverlayBlueMarker(GMapMarker currentMarker)
		{
			var marker = new GMarkerGoogle(currentMarker.Position, GMarkerGoogleType.blue_small);
			marker.Tag = currentMarker.Tag;
			Blue.Markers.Add(marker);
		}
		/// <summary>
		/// Update all change markers with dosage <from> to dosage <to>.
		/// if from dosage = 0 then update all change markers.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		internal int UpdateDosageAllChangeMarkers(double from, double to)
		{
			var cnt = 0;
			foreach (var item in Green.Markers)
			{
				var x = item.Tag as ChangeMarker;
				if (x.Dosage == from || from == 0)
				{
					x.Dosage = to;
					++cnt;
				}
			}
			return cnt;
		}
		#region MARKER OVERLAY TYPES
		public bool IsChangeMarker(GMapMarker item) => item.Overlay.Id == "Change points";
		public bool IsNavigationMarker(GMapMarker item) => item.Overlay.Id == "Navigation points";
		public bool IsGpsMarker(GMapMarker item) => item.Overlay.Id == "Gps points";

		/// <summary>
		/// This function returns route statistics which are displayed on the route statistics form.
		/// </summary>
		/// <returns>Statistics</returns>
		public Statistics ComputeStatistics()
		{
			var statistics = new Statistics();
			var distance = 0d;   // Distance between current- and previous change markers in km.
			var prevDosage = 0d; // gr.
			var prevWidth = 0d;  // m.
			GMapMarker prevItem = null;
			foreach (var item in Red.Markers)
			{
				if (prevItem != null) distance += Adapters.Distance(prevItem.Position, item.Position);
				if (item.Tag is ChangeMarker || IsLast(item))
				{
					var cm = item.Tag as ChangeMarker;
					//
					// Compute total dosage.
					//
					var opp = prevWidth * distance;
					statistics.Dosage += prevDosage * opp; // in kg.
					if (prevDosage > 0)
					{
						statistics.SpreadingDistance += distance;
						statistics.Area += opp; // in 1000 m2.
					}
					else
					{
						statistics.DrivingDistance += distance;
					}
					if (cm == null) break; // Last marker.
					statistics.UptoLastDistance += distance;
					//
					// Get actual dosage and width for this change marker.
					//
					var (dosage, widthLeft, widthRight, active) = cm.GetDosageAndWith(MachineType);
					prevDosage = (active) ? dosage : 0;
					prevWidth = (active) ? widthLeft + widthRight : 0;
					distance = 0;
				}
				prevItem = item;
			}
			return statistics;
		}

		private bool IsLast(GMapMarker item) => item == Red.Markers.Last();
		#endregion
		#endregion
	}
}