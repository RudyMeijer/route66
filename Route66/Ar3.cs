using GMap.NET;
using MyLib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MyLib.My;

namespace Route66
{
	public static partial class Adapters
	{
		public static int[] errors;
		private static Dictionary<string, NavigationMessages> naviTypes;

		/// <summary>
		/// Read AR3 file.
		/// See http://confluence.ash.ads.org/display/EHP/Autologic+ar3+route+file+format.+V2
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		public static Route ReadAr3(String filename)
		{
			#region FIELDS
			var line = "";
			var version = "";
			var provider = CultureInfo.GetCultureInfo("en").NumberFormat;
			var distanceTable = new Dictionary<PointLatLng, int>();
			errors = new int[5];
			var sb = new StringBuilder();
			var lastDistance = -1;
			var route = new Route() { FileName = filename };
			var minimumDistanceBetweenMarkersInCm = 100;
			var random = new Random();
			#endregion
			using (TextReader reader = new StreamReader(filename))
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
							var distance = int.Parse(s[3]);
							if (distance < lastDistance) { ++errors[0]; sb.Append($"\n{line} has descending distance and will be ignored."); }
							else if (distance == lastDistance) { ++errors[1]; sb.Append($"\r\nDuplicated line {line}"); }
							else if (distance < (lastDistance + minimumDistanceBetweenMarkersInCm) && lastDistance > -1) { ++errors[1]; sb.Append($"\nMinimum distance {line} with respect to previous marker violated."); }
							else
							{
								var point = Unique(new PointLatLng(Double.Parse(s[2], provider), Double.Parse(s[1], provider)));
								route.GpsMarkers.Add(new GpsMarker(point));
								distanceTable.Add(point, distance);
							}
							lastDistance = distance;
						}
						#endregion
						//
						#region READ NAVIGATION MARKERS
						//
						else if (line.StartsWith("Instruction["))
						{
							var marker = new NavigationMarker(FindLatLng(s[1]));
							
							if (s[2] == "1007") // Get custom message.
								marker.Message = (s[6] != "") ? s[6] : Path.GetFileNameWithoutExtension(s[5]);
							else
								marker.Message = InstructionType(s[2]);

							if (marker.Message == "-") { ++errors[2]; sb.Append($"\n{line} Unkown navigation type {s[2]}"); }
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
								marker.PumpOnOff = s[2] == "1";
								marker.Hopper1OnOff = s[3] == "1";
								marker.SpreadingWidthLeft = Double.Parse(s[4]);
								marker.Hopper2OnOff = s[5] == "1";
								marker.SpreadingWidthRight = Double.Parse(s[6]);
								marker.Dosage = Double.Parse(s[7]);
							}
							else
							{
								// 1=DistanceFromStartInCm, SpreadSprayOnOff, SprayModeOnOff, Max, SecMat,Dosage, WidthLeft, WidthRight, SecDos, WidthLeftSpraying, WidthRightSpraying, CombiPercentage, HopperSelection, Marked, Message
								marker.SpreadingOnOff = s[2] == "1";
								marker.SprayingOnOff = s[3] == "1";
								marker.MaxOnOff = s[4] == "1";
								marker.SecMatOnOff = s[5] == "1";
								marker.Dosage = Double.Parse(s[6]) / 100;
								marker.SpreadingWidthLeft = Double.Parse(s[7]) / 100;
								marker.SpreadingWidthRight = Double.Parse(s[8]) / 100;
								marker.DosageLiquid = Double.Parse(s[9]) / 100;
								marker.SprayingWidthLeft = Double.Parse(s[10]) / 100;
								marker.SprayingWidthRight = Double.Parse(s[11]) / 100;
								marker.PersentageLiquid = Double.Parse(s[12]);

							}
							route.ChangeMarkers.Add(marker);
						}
						#endregion
					}
					catch (Exception ee) { ++errors[3]; sb.Append($"\nError in {line} {ee.Message} {ee.StackTrace}"); }
				}
			if (errors.Sum() > 0)
			{
				Log(sb.ToString());
				Log("End of requirement analyze.");
				Show($"Total {errors.Sum()} violations in route {Path.GetFileName(filename)} detected. \n" +
					$"{errors[1]} duplicated lines will be ignored.\n" +
					$"{errors[0]} points have descending distance and will be ignored. \n" +
					$"{errors[2]} unknown navigation types. \n" +
					$"{errors[4]} orphan markers found. They will be connected to Gps markers. \n" +
					$"{errors[3]} exceptions. \n" +
					$"See logfile for more information.", $"Requirements Conformation Report.");
			}
			return route;
			//
			// Make unique LatLng point. See Software Design Document.
			//
			PointLatLng Unique(PointLatLng point)
			{
				while (distanceTable.ContainsKey(point))
				{
					var r = random.NextDouble() / 100000;
					My.Log($"Make unique LatLng point {point} + {r}");
					point = new PointLatLng(point.Lat + r, point.Lng + r);
				}
				return point;
			}

			PointLatLng FindLatLng(string distance)
			{
				int d = int.Parse(distance);
				var lastKey = default(PointLatLng);
				foreach (var item in distanceTable)
				{
					if (item.Value < d)
					{
						lastKey = item.Key;
					}
					else if (item.Value == d)
					{
						lastKey = item.Key;
						break;
					}
					else if (item.Value > d)
					{
						++errors[4]; // No corresponding Gps marker (=orphan).
						lastDistance = distanceTable[lastKey];
						if (item.Value - d < d - lastDistance)
							lastKey = item.Key;
						break;
					}
				}
				distanceTable.Remove(lastKey); // Use distance only one's so that not both Navigation- and Change marker can be added to one gps marker.
				return lastKey;
			}
		}


		public static void WriteAr3(String fileName, Route route)
		{
			var provider = CultureInfo.GetCultureInfo("en").NumberFormat;
			using (TextWriter writer = new StreamWriter(fileName))
			{
				//
				#region WRITE HEADER.
				//
				writer.WriteLine($"Ar3Version: 2");
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
				var random = new Random();
				PointLatLng lastPoint = default(PointLatLng);
				foreach (var item in route.GpsMarkers)
				{
					var point = new PointLatLng(item.Lat, item.Lng);

					distance += Distance(lastPoint, point) * 100000;
					if (distanceTable.ContainsKey((int)distance))
						break;
					else
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
				foreach (var item in route.NavigationMarkers)
				{
					var point = new PointLatLng(item.Lat, item.Lng);
					var key = InstructionKey(item.Message);
					var soundfile = (key == "1007") ? item.SoundFile : "";
					writer.WriteLine($"Instruction[{idx++}]:{GetDistance(point)},{key},-1,-1,{soundfile},");
				}
				int GetDistance(PointLatLng point)
				{
					foreach (var kvp in distanceTable) if (kvp.Value == point) return kvp.Key;
					return 0;
				}
				#endregion
				//
				#region WRITE CHANGE MARKERS.
				//
				idx = 0;
				distance = 0d;
				lastPoint = default(PointLatLng);
				writer.WriteLine("ChangePoints: DistanceFromStartInCm, SpreadSprayOnOff, SprayModeOnOff, Max, SecMat,Dosage, WidthLeft, WidthRight, SecDos, WidthLeftSpraying, WidthRightSpraying, CombiPercentage, HopperSelection");
				foreach (var item in route.ChangeMarkers)
				{
					var point = new PointLatLng(item.Lat, item.Lng);
					writer.WriteLine($"ChangePoint[{idx++}]:{GetDistance(point)},{s(item.SpreadingOnOff)},{s(item.SprayingOnOff)},{s(item.MaxOnOff)},{s(item.SecMatOnOff)},{(int)(item.Dosage * 100)},{(int)(item.SpreadingWidthLeft * 100)},{(int)(item.SpreadingWidthRight * 100)},{(int)(item.DosageLiquid * 100)},{(int)(item.SprayingWidthLeft * 100)},{(int)(item.SprayingWidthRight * 100)},{item.PersentageLiquid},1");
				}
				#endregion
			}
		}
		#region HELPER METHODES
		private static string InstructionType(string key)
		{
			if (naviTypes == null) InitialyzeNavigationTypes();
			return naviTypes.ContainsKey(key) ? Translate.NavigationMessages[(int)naviTypes[key]] : "-";
		}
		private static string InstructionKey(string message)
		{
			if (naviTypes == null) InitialyzeNavigationTypes();
			var key = naviTypes.FirstOrDefault(x => Translate.NavigationMessages[(int)x.Value] == message).Key;
			return key ?? "1007";
		}
		private static void InitialyzeNavigationTypes()
		{
			naviTypes = new Dictionary<string, NavigationMessages>
			{
				{ "1", NavigationMessages.ENTER_ROUNDABOUT},
				{ "2", NavigationMessages.EXIT_ROUNDABOUT},
				{ "3", NavigationMessages.KEEP_LEFT},
				{ "4", NavigationMessages.TURN_LEFT},
				{ "5", NavigationMessages.KEEP_RIGHT},
				{ "6", NavigationMessages.TURN_RIGHT},
				{ "7", NavigationMessages.CONTINUE},
				{ "8", NavigationMessages.ARRIVE},
				{ "9", NavigationMessages.U_TURN},
				{ "10", NavigationMessages.BEAR_LEFT},
				{ "11", NavigationMessages.BEAR_RIGHT},
				{ "12", NavigationMessages.TURN_HARD_LEFT},
				{ "13", NavigationMessages.TURN_HARD_RIGHT},
				{ "14", NavigationMessages.TAKE_RAMP_LEFT},
				{ "15", NavigationMessages.TAKE_RAMP_RIGHT},
				{ "16", NavigationMessages.PROCEED},
				{ "21", NavigationMessages.MARKER},
				{ "22", NavigationMessages.BEGIN_BREAK},
				{ "23", NavigationMessages.END_BREAK},
				{ "24", NavigationMessages.ENTER_BIKE_LANE},
				{ "25", NavigationMessages.TURN_RIGHT_INTO_BIKE_LANE},
				{ "26", NavigationMessages.TURN_LEFT_INTO_BIKE_LANE},
				//{ "1007", NavigationMessages.CUSTOM_INSTRUCTION},
			};
		}
		private static object s(bool b) => (b) ? "1" : "0";
		private static double Distance(PointLatLng p1, PointLatLng p2)
		{
			if (p1.IsEmpty) return 0;
			var mr = new MapRoute(new List<PointLatLng>() { p1, p2 }, "compute distance");
			return mr.Distance;
		}
		#endregion
	}
}
