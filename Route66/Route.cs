﻿using GMap.NET;
using MyLib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using static Route66.Adapters;

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
				switch (Path.GetExtension(fileName))
				{
					case ".xml":
						route = new XmlSerializer(typeof(Route)).Deserialize(new StreamReader(fileName)) as Route;
						break;
					case ".ar3":
						route = ReadAr3(fileName);
						break;
					default: route.IsNotSupported = true; break;
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
			return route;
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

		#endregion
	}
}
