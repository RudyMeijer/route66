﻿// <copyright file="Ar3.cs" company="Aebi Schmidt Nederland B.V.">
//   Aebi Schmidt Nederland B.V. All rights reserved.
// </copyright>
namespace Route66
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using GMap.NET;
	using MyLib;
	using static MyLib.My;
	using static Route66.DataContracts;

	/// <summary>
	/// Helper class for reading / writing AR3 files.
	/// Todo statistics dosage.
	/// </summary>
	public static class Adapters
	{
		private static Dictionary<string, NavigationTypes> naviTypes;
		private static PointLatLng latLngFirstNav;

		/// <summary>
		/// Read AR3 file. See http://confluence.ash.ads.org/display/EHP/Autologic+ar3+route+file+format.+V2
		/// </summary>
		/// <param name="filename">ar3 filename</param>
		/// <returns>ar3 route</returns>
		public static Route ReadAr3(string filename)
		{
			#region FIELDS
			//
			// The distance table contains the relation between LatLng and distance.
			// Filled during Read Gps markers. Filled with unique latlng and sorted distance.
			//
			var distanceTable = new Dictionary<PointLatLng, int>();
			var line = string.Empty;
			var version = string.Empty;
			var startPoint = new PointLatLng();
			var lastDistance = -1;
			var route = new Route() { FileName = filename };
			var random = new Random();
			var previousChangeMarker = new ChangeMarker();
			errors = new int[5]; // used in unittesting.
			#endregion
			using (TextReader reader = new StreamReader(filename))
			{
				while ((line = reader.ReadLine()) != null)
				{
					try
					{
						var s = line.Split(':', ',');
						//
						#region READ HEADER
						//
						if (line.StartsWith("Ar3")) version = s[1];
						else if (line.StartsWith("MachineType")) route.MachineType = My.GetEnum<MachineTypes>(s[1]);
						#endregion
						//
						#region READ GPS MARKERS
						//
						else if (line.StartsWith("WayPoint["))
						{
							var distance = (int)My.Val(s[3]);
							if (distance < lastDistance) { My.Log($"{++errors[0]} {line} has descending distance and will be ignored."); }
							else if (distance == lastDistance) { My.Log($"{++errors[1]} Duplicated line {line}"); }
							else
							{
								var point = Unique(new PointLatLng(My.Val(s[2]), My.Val(s[1])), distance);
								route.GpsMarkers.Add(new GpsMarker(point));
								if (startPoint.IsEmpty)
								{
									startPoint = point;
									latLngFirstNav = Unique(startPoint, 20); // Sort dictionary 4 holten_cityjet.ar3
								}
							}
							lastDistance = distance;
						}
						#endregion
						//
						#region READ NAVIGATION MARKERS
						//
						else if (line.StartsWith("Instruction["))
						{
							PointLatLng latlng;

							if (s[1] == "0") // Set first Navigation marker not at distance zero. This is reserved for Change marker.
							{
								latlng = latLngFirstNav;
								route.GpsMarkers.Insert(1, new GpsMarker(latlng));
							}
							else
							{
								latlng = FindLatLng(s[1]);
							}
							var marker = new NavigationMarker(latlng);

							if (s[2] == "1007") // Get custom message.
								marker.Message = (s[6] != "") ? s[6] : Path.GetFileNameWithoutExtension(s[5]);
							else
								marker.Message = NavigationMessage(s[2]);
							//
							// Get roundabout index.
							//
							marker.RoundAboutIndex = Math.Max((int)My.Val(s[3]), 0);
							if (marker.Message == "-") { My.Log($"{++errors[2]} {line} Unkown navigation type {s[2]}"); }
							marker.SoundFile = My.ValidateFilename((s[5] != "") ? s[5] : (marker.Message + ".wav")); // Todo create soundfile.
							route.NavigationMarkers.Add(marker);
						}
						#endregion
						//
						#region READ CHANGE MARKERS
						//
						else if (line.StartsWith("ChangePoint["))
						{
							var marker = new ChangeMarker(FindLatLng(s[1]));
							if (route.MachineType == MachineTypes.StreetWasher)
							{
								// 1=DistanceFromStart, ActivityState, LeftNozzleIsActive, LeftNozzlePosition, RightNozzleIsActive, RightNozzlePosition, waterpressure,Marked, Message
								// If this code is changed then modify also methode DisplayOnForm.
								//
								// Handle empty strings; use previous marker value.
								marker.SpreadingOnOff = My.Bool(s[2], previousChangeMarker.SpreadingOnOff);
								marker.Hopper1OnOff = My.Bool(s[3], previousChangeMarker.Hopper1OnOff);
								marker.SpreadingWidthLeft = My.Val(s[4], previousChangeMarker.SpreadingWidthLeft);
								marker.Hopper2OnOff = My.Bool(s[5], previousChangeMarker.Hopper2OnOff);
								marker.SpreadingWidthRight = My.Val(s[6], previousChangeMarker.SpreadingWidthRight);
								marker.Dosage = My.Val(s[7], previousChangeMarker.Dosage);
								//
								// Set default pressure according spec.
								//
								if (marker.Dosage < 5) marker.Dosage = 5;
							}
							else
							{
								// 1=DistanceFromStartInCm, SpreadSprayOnOff, SprayModeOnOff, Max, SecMat,Dosage, WidthLeft, WidthRight, SecDos, WidthLeftSpraying, WidthRightSpraying, CombiPercentage, HopperSelection, Marked, Message
								marker.SpreadingOnOff = My.Bool(s[2], previousChangeMarker.SpreadingOnOff);
								marker.SprayingOnOff = My.Bool(s[3], previousChangeMarker.SprayingOnOff);
								marker.MaxOnOff = My.Bool(s[4], previousChangeMarker.MaxOnOff);
								marker.SecMatOnOff = My.Bool(s[5], previousChangeMarker.SecMatOnOff);
								marker.Dosage = My.Val(s[6], previousChangeMarker.Dosage * 100) / 100;
								marker.SpreadingWidthLeft = My.Val(s[7], previousChangeMarker.SpreadingWidthLeft * 100) / 100;
								marker.SpreadingWidthRight = My.Val(s[8], previousChangeMarker.SpreadingWidthRight * 100) / 100;
								marker.DosageLiquid = My.Val(s[9], previousChangeMarker.DosageLiquid * 100) / 100;
								marker.SprayingWidthLeft = My.Val(s[10], previousChangeMarker.SprayingWidthLeft * 100) / 100;
								marker.SprayingWidthRight = My.Val(s[11], previousChangeMarker.SprayingWidthRight * 100) / 100;
								marker.PersentageLiquid = My.Val(s[12], previousChangeMarker.PersentageLiquid);
							}
							//
							// First change marker should have distance 0.
							//
							if (route.ChangeMarkers.Count == 0 && s[1] != "0")
							{
								route.ChangeMarkers.Add(new ChangeMarker(startPoint));
							}
							route.ChangeMarkers.Add(marker);
							previousChangeMarker = marker;
						}
						#endregion
					}
					catch (Exception ee) { Log($"{++errors[3]} Error in {line} {ee.Message} {ee.StackTrace}"); }
				}
				//
				// First change marker should have distance 0.
				//
				if (route.ChangeMarkers.Count == 0)
				{
					route.ChangeMarkers.Add(new ChangeMarker(startPoint));
				}

			}
			My.Log("End of requirement analyze.");
			return route;
			//
			// Make unique LatLng point. See Software Design Document.
			//
			PointLatLng Unique(PointLatLng point, int distance)
			{
				while (distanceTable.ContainsKey(point))
				{
					var r = random.NextDouble() / 100000;
					point = new PointLatLng(point.Lat + r, point.Lng + r);
				}
				distanceTable.Add(point, distance);
				return point;
			}

			PointLatLng FindLatLng(string sdistance)
			{
				var distance = (int)My.Val(sdistance);
				var last = default(KeyValuePair<PointLatLng, int>);
				var idx = 0;
				foreach (var item in distanceTable)
				{
					++idx;
					if (item.Value < distance)
					{
						last = item;
					}
					else if (item.Value == distance)
					{
						last = item;
						break;
					}
					else if (item.Value > distance) // No corresponding Gps marker (orphan).
					{
						if (last.Key.IsEmpty) last = item;
						if (item.Value - distance < distance - last.Value) last = item;
						if (distance - last.Value < 200) break; // Don't mark as orphan when marker is within 200 cm of current or previous marker.
						if (item.Value - distance < 200) break;
						++errors[4];
						break;
					}
				}
				distanceTable.Remove(last.Key); // Use distance only one's so that both Navigation- and Change marker can't be added to one gps marker.

				return last.Key;
			}
		}

		public static void WriteAr3(String fileName, Route route)
		{
			var provider = CultureInfo.GetCultureInfo("en").NumberFormat;
			var isWasher = route.MachineType == MachineTypes.StreetWasher;
			using (TextWriter writer = new StreamWriter(fileName))
			{
				//
				#region WRITE HEADER.
				//
				writer.WriteLine($"Ar3Version:2");
				writer.WriteLine($"MachineType: {route.MachineType}");
				writer.WriteLine($"ImageFiles:");
				writer.WriteLine($"SoundFiles:");
				writer.WriteLine($"RouteID: {Path.GetFileNameWithoutExtension(fileName)}");
				writer.WriteLine($"RouteTimestamp: {DateTime.Now.ToString("yyyyMMdd HH.mm.ss")}");
				writer.WriteLine($"RouteResult:"); // : only written by route66.
				writer.WriteLine($"Duration: 0");
				writer.WriteLine($"Length: {route.Distance}");
				writer.WriteLine($"CalcTime: 0");
				writer.WriteLine($"WayPoints: Longitude, Latitude, DistanceFromStartInCm");
				#endregion
				//
				#region WRITE GPS MARKERS.
				//
				var idx = 0;
				var distance = 0d;
				var distanceTable = new Dictionary<int, PointLatLng>();
				PointLatLng lastPoint = default(PointLatLng);
				foreach (var item in route.GpsMarkers)
				{
					var point = new PointLatLng(item.Lat, item.Lng);

					distance += Distance(lastPoint, point) * 100000;
					//
					// According SDD design decision 2b: LatLng are unique. 
					// Make distance unique when sequential Latlng's are within 1 cm.
					//
					while (distanceTable.ContainsKey((int)distance))
					{
						++distance;
						My.Log($"WriteAr3: Waypoint[{idx}] create unique distance {(int)distance}.");
					}
					distanceTable.Add((int)distance, point);
					writer.WriteLine($"WayPoint[{idx++}]:{point.Lng.ToString(provider)},{point.Lat.ToString(provider)},{(int)distance}");
					lastPoint = point;
				}
				#endregion
				//
				#region WRITE NAVIGATION MARKERS.
				//
				idx = 0;
				distance = 0d;
				lastPoint = default(PointLatLng);
				writer.WriteLine("Instructions: DistanceFromStartInCm, InstructionType, RoundaboutIndex, RoundaboutCount, SoundFile, Message");
				writer.WriteLine($"Instruction[{idx++}]:0,16,-1,-1,,");
				foreach (var item in route.NavigationMarkers)
				{
					var point = new PointLatLng(item.Lat, item.Lng);
					var key = NavigationKey(item.Message);
					var soundfile = (key == "1007") ? item.SoundFile : "";
					var roundAboutIndex = (key == "1") ? item.RoundAboutIndex : -1;
					if (idx != 1 || key != "16")
						writer.WriteLine($"Instruction[{idx++}]:{GetDistance(point)},{key},{roundAboutIndex},-1,{soundfile},");
				}
				int GetDistance(PointLatLng point)
				{
					foreach (var kvp in distanceTable) if (kvp.Value == point) return (int)kvp.Key;
					return -1;
				}
				#endregion
				//
				#region WRITE CHANGE MARKERS.
				//
				idx = 0;
				distance = 0d;
				lastPoint = default(PointLatLng);
				writer.WriteLine((isWasher) ? "ChangePoints: DistanceFromStartInCm, ActivityState, LeftNozzleIsActive, LeftNozzlePosition, RightNozzleIsActive, RightNozzlePosition, WaterPressure, Marked, Message"
					: "ChangePoints: DistanceFromStartInCm, SpreadSprayOnOff, SprayModeOnOff, Max, SecMat,Dosage, WidthLeft, WidthRight, SecDos, WidthLeftSpraying, WidthRightSpraying, CombiPercentage, HopperSelection");
				foreach (var item in route.ChangeMarkers)
				{
					var point = new PointLatLng(item.Lat, item.Lng);
					writer.WriteLine((isWasher) ? $"ChangePoint[{idx++}]:{GetDistance(point)},{s(item.SpreadingOnOff)},{s(item.Hopper1OnOff)},{item.SpreadingWidthLeft},{s(item.Hopper2OnOff)},{item.SpreadingWidthRight},{item.Dosage}"
						: $"ChangePoint[{idx++}]:{GetDistance(point)},{s(item.SpreadingOnOff)},{s(item.SprayingOnOff)},{s(item.MaxOnOff)},{s(item.SecMatOnOff)},{(int)(item.Dosage * 100)},{(int)(item.SpreadingWidthLeft * 100)},{(int)(item.SpreadingWidthRight * 100)},{(int)(item.DosageLiquid * 100)},{(int)(item.SprayingWidthLeft * 100)},{(int)(item.SprayingWidthRight * 100)},{item.PersentageLiquid},1");
				}
				#endregion
			}
		}
		#region PROPERTIES
		public static int[] errors { get; set; }
		#endregion
		#region HELPER METHODES
		/// <summary>
		/// This function translates an navigation type in ar3 file to corresponding navigation message.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		private static string NavigationMessage(string key)
		{
			if (naviTypes == null) InitialyzeNavigationTypes();
			return naviTypes.ContainsKey(key) ? Translate.NavigationMessages[(int)naviTypes[key]] : "-";
		}
		/// <summary>
		/// This function translates an navigation message to corresponding navigation type.
		/// This function is complementary to previous function NavigationMessage.
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		private static string NavigationKey(string message)
		{
			if (naviTypes == null) InitialyzeNavigationTypes();
			var key = naviTypes.FirstOrDefault(x => Translate.NavigationMessages[(int)x.Value] == message).Key;
			return key ?? "1007";
		}
		private static void InitialyzeNavigationTypes()
		{
			naviTypes = new Dictionary<string, NavigationTypes>
			{
				{ "1", NavigationTypes.ENTER_ROUNDABOUT},
				{ "2", NavigationTypes.EXIT_ROUNDABOUT},
				{ "3", NavigationTypes.KEEP_LEFT},
				{ "4", NavigationTypes.TURN_LEFT},
				{ "5", NavigationTypes.KEEP_RIGHT},
				{ "6", NavigationTypes.TURN_RIGHT},
				{ "7", NavigationTypes.CONTINUE},
				{ "8", NavigationTypes.ARRIVE},
				{ "9", NavigationTypes.U_TURN},
				{ "10", NavigationTypes.BEAR_LEFT},
				{ "11", NavigationTypes.BEAR_RIGHT},
				{ "12", NavigationTypes.TURN_HARD_LEFT},
				{ "13", NavigationTypes.TURN_HARD_RIGHT},
				{ "14", NavigationTypes.TAKE_RAMP_LEFT},
				{ "15", NavigationTypes.TAKE_RAMP_RIGHT},
				{ "16", NavigationTypes.PROCEED},
				{ "21", NavigationTypes.MARKER},
				{ "22", NavigationTypes.BEGIN_BREAK},
				{ "23", NavigationTypes.END_BREAK},
				{ "24", NavigationTypes.ENTER_BIKE_LANE},
				{ "25", NavigationTypes.TURN_RIGHT_INTO_BIKE_LANE},
				{ "26", NavigationTypes.TURN_LEFT_INTO_BIKE_LANE},
				//
				// Translate old navigation messages to new ar3 format v2.
				//
				//{ "1005", NavigationMessages.BEGIN_PAUZE},
				//{ "1006", NavigationMessages.END_PAUZE},
				//{ "1007", NavigationMessages.CUSTOM_INSTRUCTION},
				{ "1008", NavigationTypes.MARKER},
				{ "1009", NavigationTypes.BEGIN_BREAK},
				{ "1010", NavigationTypes.END_BREAK},
			};
		}
		private static object s(bool b) => (b) ? "1" : "0";
		public static double Distance(PointLatLng p1, PointLatLng p2)
		{
			if (p1.IsEmpty) return 0;
			var mr = new MapRoute(new List<PointLatLng>() { p1, p2 }, "compute distance");
			return mr.Distance;
		}

		////private static PointLatLng Interpolate(int distance, KeyValuePair<PointLatLng, int> last, KeyValuePair<PointLatLng, int> item)
		////{
		////    var t = My.InverseLerp(distance, last.Value, item.Value);
		////    var lat = My.Lerp(t, (float)last.Key.Lat, (float)item.Key.Lat);
		////    var lng = My.Lerp(t, (float)last.Key.Lng, (float)item.Key.Lng);
		////    var point = new PointLatLng(lat, lng);
		////    return point;
		////}
		#endregion
	}
}
