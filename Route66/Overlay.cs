﻿using GMap.NET.WindowsForms;
using System.Drawing;
using System;
using GMap.NET;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.ObjectModel;
using System.Windows.Forms;
using MyLib;
using GMap.NET.MapProviders;

namespace Route66
{
	internal class Overlay
	{
		#region FIELDS
		private GMapControl Map;
		/// <summary>
		/// This field contains Lat,Lng coordinates of the last clicked marker.
		/// </summary>
		private GMapMarker CurrentMarker;
		private readonly GMapRoute RedRoute;
		private readonly GMapOverlay Red;
		private readonly GMapOverlay Green;
		private readonly GMapOverlay Blue;
		#endregion
		#region CONSTRUCTOR
		public Overlay(GMapControl gmap)
		{
			this.Map = gmap;
			gmap.Overlays.Add(new GMapOverlay("Gps points"));
			gmap.Overlays.Add(new GMapOverlay("Change points"));
			gmap.Overlays.Add(new GMapOverlay("Navigation points"));
			Red = gmap.Overlays[0];
			Green = gmap.Overlays[1];
			Blue = gmap.Overlays[2];
			RedRoute = new GMapRoute("routes")
			{
				Stroke = new Pen(Color.Red, 2)
			};
			Red.Routes.Add(RedRoute);
			Settings = Settings.Global;
		}
		#endregion
		#region PROPERTIES
		/// <summary>
		/// True when intermediat markers will be placed on road.
		/// </summary>
		public bool AutoRoute { get; internal set; }
		public Settings Settings { get; }
		#endregion
		#region METHODES
		internal GMapMarker Remove(GMapMarker marker)
		{
			//if (marker == null) return false;
			var idx = Red.Markers.IndexOf(marker);
			Console.WriteLine($"Remove marker {idx}");
			if (idx >=0)
			{
				Red.Markers.Remove(marker);
				UpdateGreenAndBlueOverlay(Crud.Delete, marker.Tag, null);
				RedRoute.Points.RemoveAt(idx);
				Map.UpdateRouteLocalPosition(RedRoute);
				if (idx > 0)
				{
					CurrentMarker = Red.Markers[idx - 1];
				}
			}
			else My.Status($"Error in Remove marker {marker?.ToolTipText}");
			return CurrentMarker;
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
			var newPosition = Map.FromLocalToLatLng(x, y);
			if (CurrentMarker.Tag is ChangeMarker) GetGreenMarker(CurrentMarker).Position = newPosition;
			if (CurrentMarker.Tag is NavigationMarker) GetBlueMarker(CurrentMarker).Position = newPosition;
			// 
			// Update Route point.
			// Allways use currentmarker class to find index because we can have duplicated Positions (struct)!
			//
			var idx = Red.Markers.IndexOf(CurrentMarker);
			RedRoute.Points[idx] = newPosition;

			Map.UpdateRouteLocalPosition(RedRoute);
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
			Map.UpdateRouteLocalPosition(RedRoute);
		}
		internal void AddMarker(int x, int y)
		{
			PointLatLng point = Map.FromLocalToLatLng(x, y);
			if (AutoRoute && RedRoute.Points.Count > 0)
			{
				var end = new GMarkerGoogle(point, GMarkerGoogleType.red_small);

				var route = AutoRouter(CurrentMarker, end);
				if (route != null && route.Points.Count > 0)
				{
					route.Points.RemoveAt(0);
					foreach (var p in route.Points) AddMarker(p);
				}
			}
			else AddMarker(point);
		}
		public void Clear()
		{
			Red.Markers.Clear();
			Green.Markers.Clear();
			Blue.Markers.Clear();
			RedRoute.Clear();
			Map.UpdateRouteLocalPosition(RedRoute);
			CurrentMarker = null;
		}
		public void SetTooltip(GMapMarker item)
		{
			var idx = Red.Markers.IndexOf(item);
			item.ToolTipMode = (Settings.ToolTipMode) ? MarkerTooltipMode.OnMouseOver : MarkerTooltipMode.Never;
			item.ToolTipText = $"{idx}";
		}

		public void SetCurrentMarker(GMapMarker item)
		{
			var icon = item.GetType().GetField("Type").GetValue(item);
			var idx = Red.Markers.IndexOf(item);
			Console.WriteLine($"current marker {idx} {item.Position} {icon}");
			if (idx >= 0)
			{
				CurrentMarker = item;
			}
			else My.Status($"Error in SetCurrentMarker {item.ToolTipText}");
		}
		/// <summary>
		/// Copy Route class into overlays.
		/// </summary>
		/// <param name="route"></param>
		public void Load(Route route)
		{
			Red.Markers.Clear();
			Green.Markers.Clear();
			Blue.Markers.Clear();
			foreach (var red in route.GpsMarkers)
			{
				Red.Markers.Add(new GMarkerGoogle(new PointLatLng(red.Lat, red.Lng), (Red.Markers.Count == 0) ? GMarkerGoogleType.green_big_go : GMarkerGoogleType.red_small));
			}
			//
			// Copy green and blue tags into red markers.
			//
			foreach (var item in route.ChangeMarkers)
			{
				var rm = FindRedMarker(item);
				if (rm != null)
				{
					rm.Tag = item;
					AddGreenMarker(rm);
				}
				else My.Status($"Green marker {item} not found in red markers list.");
			}
			foreach (var item in route.NavigationMarkers)
			{
				var rm = FindRedMarker(item);
				if (rm != null)
				{
					rm.Tag = item;
					AddBlueMarker(rm);
				}
				else My.Status($"Blue marker {item} not found in red markers list.");
			}
			//foreach (var blue in route.NavigationMarkers) if (blue.Lat == red.Lat && blue.Lng == red.Lng)
			//	{
			//		cm.Tag = blue;
			//		AddBlueMarker(cm);
			//	}
			CreateOverlayRoute();
			Map.ZoomAndCenterRoute(RedRoute);
		}

		private GMapMarker FindRedMarker(GpsMarker m)// todo performance test.
		{
			var pos = new PointLatLng(m.Lat, m.Lng);
			foreach (var item in Red.Markers) if (item.Position == pos) return item;
			return null;
		}

		/// <summary>
		/// Copy overlays into Route class.
		/// </summary>
		/// <param name="route"></param>
		public void CopyTo(Route route)
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
		/// <summary>
		/// Determine current marker type: Change marker or Navigation marker.
		/// Show properties of current marker on windows form.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool EditMarker(KeyEventArgs key)
		{
			var originalTag = CurrentMarker.Tag;// if DeepClone(); then marker is not removed from route on delete.
			var before = (CurrentMarker.Tag != null) ? 2 : 0;
			Form form = null;
			if (CurrentMarker.Tag is ChangeMarker) form = new FormEditChangeMarker(CurrentMarker);
			else if (CurrentMarker.Tag is NavigationMarker) form = new FormEditNavigationMarker(CurrentMarker);
			//
			// If user clicked on red marker.
			// then pressing Ctrl key will edit Navigation marker.
			//
			else if (CurrentMarker.Tag == null)
				if (key != null && key.Control) form = new FormEditNavigationMarker(CurrentMarker);
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
			return crud != Crud.None;
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
					if (tag is ChangeMarker) AddGreenMarker(CurrentMarker);
					if (tag is NavigationMarker) AddBlueMarker(CurrentMarker);
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
		private GMapMarker GetGreenMarker(GMapMarker currentMarker)
		{
			foreach (var item in Green.Markers) if (item.Position == currentMarker.Position) return item;
			return null;
		}
		private GMapMarker GetBlueMarker(GMapMarker currentMarker)
		{
			foreach (var item in Blue.Markers) if (item.Position == currentMarker.Position) return item;
			return null;
		}
		private void AddGreenMarker(GMapMarker currentMarker)
		{
			Green.Markers.Add(new GMarkerGoogle(currentMarker.Position, GMarkerGoogleType.green_small));
			Green.Markers[Green.Markers.Count - 1].Tag = currentMarker.Tag;
			Green.Markers[Green.Markers.Count - 1].ToolTipText = currentMarker.Tag.ToString();
		}
		private void AddBlueMarker(GMapMarker currentMarker)
		{
			Blue.Markers.Add(new GMarkerGoogle(currentMarker.Position, GMarkerGoogleType.blue_small));
			Blue.Markers[Blue.Markers.Count - 1].Tag = currentMarker.Tag;
			Blue.Markers[Blue.Markers.Count - 1].ToolTipText = currentMarker.Tag.ToString();
		}
		/// <summary>
		/// Update all change markers with dosage <from> to dosage <to>.
		/// if from dosage = 0 then update all change markers.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		private bool AddMarker(PointLatLng point)
		{
			var marker = new GMarkerGoogle(point, GMarkerGoogleType.red_small);
			var idx = Red.Markers.IndexOf(CurrentMarker) + 1;
			CurrentMarker = marker;
			Red.Markers.Insert(idx, marker);
			RedRoute.Points.Insert(idx, point);
			Map.UpdateRouteLocalPosition(RedRoute);
			Console.WriteLine($"Marker {idx} added at {marker.Position}");
			return true;
		}
		private MapRoute AutoRouter(GMapMarker start, GMapMarker end)
		{
			Console.WriteLine($"Autoroute start {start.Position}, stop {end.Position}");
			RoutingProvider rp = Map.MapProvider as RoutingProvider;
			if (rp == null) rp = GMapProviders.OpenStreetMap; // use OpenStreetMap if provider does not implement routing
			return rp.GetRoute(start.Position, end.Position, false, false, 2);
		}
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
		#endregion
	}
}