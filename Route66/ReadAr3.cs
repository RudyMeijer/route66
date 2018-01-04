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

		public static Route ReadAr3(String filename)
		{
			var line = "";
			var version = "";
			var provider = CultureInfo.GetCultureInfo("en").NumberFormat;
			var distanceTable = new Dictionary<int, PointLatLng>();
			errors = new int[5];
			var sb = new StringBuilder();
			var lastKey = 0;
			var lastDistance = -1;
			var route = new Route() { FileName = filename };
			using (TextReader reader = new StreamReader(filename))
			while ((line = reader.ReadLine()) != null)
			{
				try
				{
					var s = line.Split(':', ',');

					if (line.StartsWith("Ar3")) version = s[1];
					else if (line.StartsWith("MachineType")) route.MachineType = My.GetEnum<MachineTypes>(s[1]);
					else if (line.StartsWith("WayPoint["))
					{
						var point = new PointLatLng(Double.Parse(s[2], provider), Double.Parse(s[1], provider));
						var distance = int.Parse(s[3]);
						if (distance < lastDistance) { ++errors[0]; sb.Append($"\n{line} has descending distance and will be ignored."); }
						else if (distance == lastDistance) { ++errors[1]; sb.Append($"\nDuplicated line {line}"); }
						else
						{
							route.GpsMarkers.Add(new GpsMarker(point));
							distanceTable.Add(distance, point);
						}
						lastDistance = distance;
					}
					else if (line.StartsWith("Instruction["))
					{
						var marker = new NavigationMarker(FindLatLng(s[1]));
						marker.Message = InstructionType(s[2]);
						marker.SoundFile = My.ValidateFilename(marker.Message) + ".wav"; // Todo create soundfile.
						if (marker.Message == "-") { ++errors[2]; sb.Append($"\n{line} Unkown navigation type {s[2]}"); }
						route.NavigationMarkers.Add(marker);
					}
					else if (line.StartsWith("ChangePoint["))
					{
						var marker = new ChangeMarker(FindLatLng(s[1]));
						if (route.MachineType == MachineTypes.StreetWasher)
						{
							// Todo
						}
						else
						{
							marker.Dosage = Double.Parse(s[6]) / 100; // Todo fill all other properties.
							marker.SpreadingWidthLeft = Double.Parse(s[7]) / 100;
							marker.SpreadingWidthRight = Double.Parse(s[8]) / 100;
						}
						route.ChangeMarkers.Add(marker);
					}
				}
				catch (Exception ee) { ++errors[3]; sb.Append($"\nError in {line} {ee.Message} {ee.StackTrace}"); }
			}
			if (errors.Sum() > 0)
			{
				Log(sb.ToString());
				Show($"Total {errors.Sum()} errors in route {Path.GetFileName(filename)} detected. \n" +
					$"{errors[1]} duplicated lines will be ignored.\n" +
					$"{errors[0]} points have descending distance and will be ignored. \n" +
					$"{errors[2]} unknown navigation types. \n" +
					$"{errors[4]} orphan markers found. They will be connected to Gps markers. \n" +
					$"{errors[3]} other errors. \n" +
					$"All errors are succesfully resolved. See logfile for more information.", $"Requirements conformation report.");
			}
			return route;

			PointLatLng FindLatLng(string distance)
			{
				int d = int.Parse(distance);
				lastKey = 0;
				foreach (var item in distanceTable)
				{
					if (item.Key >= d)
					{
						if (item.Key > d) ++errors[4]; // No corresponding Gps marker (=orphan).
						distanceTable.Remove(item.Key);// Use distance only one's so that not both Nav and Change marker can be added to one gps marker.
						return item.Value;
					}
					else if (item.Key < lastKey) sb.Append($"\nDistance {line} not accending.");

					lastKey = item.Key;
				}
				return distanceTable[lastKey];
			}
		}

		private static string InstructionType(string v)
		{
			switch (v)
			{
				case "1": return Translate.NavigationMessages[(int)NavigationMessages.ENTER_ROUNDABOUT];
				case "4": return Translate.NavigationMessages[(int)NavigationMessages.TURN_LEFT];
				case "6": return Translate.NavigationMessages[(int)NavigationMessages.TURN_RIGHT];
				case "8": return Translate.NavigationMessages[(int)NavigationMessages.ARRIVE];
				case "9": return Translate.NavigationMessages[(int)NavigationMessages.U_TURN];
				case "15": return Translate.NavigationMessages[(int)NavigationMessages.TAKE_RAMP_RIGHT];
				case "16": return Translate.NavigationMessages[(int)NavigationMessages.PROCEED];
				case "1008": return Translate.NavigationMessages[(int)NavigationMessages.MARKER];
				default: return "-";
			}
		}

	}
}
