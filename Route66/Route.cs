using GMap.NET;
using MyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Route66
{
	public class Route
	{
		#region FIELDS
		//[XmlIgnore]
		internal string FileName;
		public static bool IsDefaultFile;
		internal bool IsNotSupported;
		//[XmlIgnore]
		internal bool IsChanged;
		private static Route route;
		#endregion
		#region CONSTRUCTOR
		public Route()
		{
			MachineType = MachineTypes.StandardSpreader;
			Version = "1.0";
			GpsMarkers = new List<GpsMarker>();
			ChangeMarkers = new List<ChangeMarker>();
			NavigationMarkers = new List<NavigationMarker>();
		}
		#endregion
		#region METHODES
		public static Route Load(string fileName = "Route66.xml")
		{
			IsDefaultFile = (fileName == "Route66.xml");
			route = new Route();
			try
			{
				using (TextReader reader = new StreamReader(fileName))
				{
					switch (Path.GetExtension(fileName))
					{
						case ".xml":
							route = new XmlSerializer(typeof(Route)).Deserialize(reader) as Route;
							break;
						case ".ar3":
							ReadAr3(reader, route);
							break;
						default: route.IsNotSupported = true; break;
					}
				}
			}
			catch (Exception ee)
			{
				if (!IsDefaultFile)
				{
					if (ee.InnerException != null)
						My.Show($"{ee.InnerException.Message}", ee.Message);
					else
						My.Show($"{ee.Message}", "Loading route.");
				}
			}
			route.FileName = fileName;
			route.IsChanged = false;
			return route;
		}

		private static void ReadAr3(TextReader reader, Route route)
		{
			var line="";
			var version = "";
			while ((line = reader.ReadLine())!=null)
			{
				var s = line.Split(':', ',');
				if (line.StartsWith("Ar3")) version = line.Split(':')[1];
				else if (line.StartsWith("MachineType")) route.MachineType = My.GetEnum<MachineTypes>(s[1]);
				else if (line.StartsWith("WayPoint[")) route.GpsMarkers.Add(new GpsMarker(s[1],s[2],s[3]));
				else if (line.StartsWith("Instruction[")) route.NavigationMarkers.Add(new NavigationMarker(s[1]));
				else if (line.StartsWith("ChangePoint[")) route.ChangeMarkers.Add(new ChangeMarker(s));
			}
		}

		public void Save() => SaveAs(FileName);
		public void SaveAs(string fileName)
		{
			try
			{
				var dir = Path.GetDirectoryName(fileName);
				if (!Directory.Exists(dir) && dir.Length > 0) Directory.CreateDirectory(dir);

				XmlSerializer serializer = new XmlSerializer(typeof(Route));
				using (StreamWriter writer = new StreamWriter(fileName)) serializer.Serialize(writer, this);
				route.FileName = fileName;
				IsDefaultFile = (fileName == "Route66.xml");
				My.Status($"Saved route succesfull to {this}");
				IsChanged = false;
			}
			catch (Exception e)
			{
				throw new Exception(e.ToString());
			}
		}
		public override string ToString()
		{
			return $"{FileName} V{Version}, Type {MachineType}";
		}
		#endregion
		#region PROPERTIES
		public MachineTypes MachineType { get; set; }
		public String Version { get; set; }
		public List<GpsMarker> GpsMarkers { get; set; }
		public List<ChangeMarker> ChangeMarkers { get; set; }
		public List<NavigationMarker> NavigationMarkers { get; set; }

		internal void Clear()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
