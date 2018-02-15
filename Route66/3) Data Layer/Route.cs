// <copyright file="Route.cs" company="Aebi Schmidt Nederland B.V.">
//   Aebi Schmidt Nederland B.V. All rights reserved.
// </copyright>
namespace Route66
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.IO;
	using System.Runtime.InteropServices;
	using System.Text;
	using System.Windows.Forms;
	using System.Xml.Serialization;
	using GMap.NET;
	using MyLib;
	using static Route66.Adapters;
	using static Route66.DataContracts;

	/// <summary>
	/// This class contains all data for one route.
	/// To comply to Single Responsible Principle it is equipped with a Load- and Save method to (de)serialize its properties to disk.
	/// </summary>
	public class Route
	{
		#region FIELDS
		// Use "internal" Access Modifiers to prohibit serialisation of variable [XmlIgnore]
		internal bool IsChanged;
		internal bool IsNotSupported;
		internal string FileName;
		internal int Distance = 0;
		private static Route route;
		#endregion
		#region CONSTRUCTOR
		/// <summary>
		/// Initializes a new instance of the <see cref="Route" /> class.
		/// </summary>
		public Route()
		{
			this.MachineType = MachineTypes.StandardSpreader;
			this.Version = "1.0";
			this.GpsMarkers = new List<GpsMarker>();
			this.ChangeMarkers = new List<ChangeMarker>();
			this.NavigationMarkers = new List<NavigationMarker>();
		}
		#endregion
		#region PROPERTIES
		public MachineTypes MachineType { get; set; }

		public string Version { get; set; }

		public List<GpsMarker> GpsMarkers { get; set; }

		public List<ChangeMarker> ChangeMarkers { get; set; }

		public List<NavigationMarker> NavigationMarkers { get; set; }
		public static bool IsDefaultFile { get; set; }
		#endregion
		#region METHODES
		public static Route Load(string fileName = "Route66.xml")
		{
			IsDefaultFile = fileName == "Route66.xml";
			route = new Route();
			try
			{
				switch (Path.GetExtension(fileName))
				{
					case ".xml":
						using (var reader = new StreamReader(fileName))
						{
							route = new XmlSerializer(typeof(Route)).Deserialize(reader) as Route;
						}
						break;
					case ".ar3":
						route = ReadAr3(fileName);
						break;
					default:
						route.IsNotSupported = true;
						break;
				}
			}
			catch (Exception ee)
			{
				if (!IsDefaultFile)
				{
					if (ee.InnerException != null)
					{
						My.Show($"{ee.InnerException.Message}", ee.Message);
					}
					else
					{
						My.Show($"{ee.Message}", "Loading route.");
					}
				}
			}

			route.FileName = fileName;
			return route;
		}

		public bool SaveAs(string fileName)
		{
			try
			{
				var dir = Path.GetDirectoryName(fileName);
				if (!Directory.Exists(dir) && dir.Length > 0)
				{
					Directory.CreateDirectory(dir);
				}

				switch (Path.GetExtension(fileName))
				{
					case ".xml":
						using (StreamWriter writer = new StreamWriter(fileName))
						{
							new XmlSerializer(typeof(Route)).Serialize(writer, this);
						}

						break;
					case ".ar3":
						WriteAr3(fileName, this);
						break;
					default:
						route.IsNotSupported = true;
						return false;
				}
				route.FileName = fileName;
				IsDefaultFile = fileName == "Route66.xml";
				this.IsChanged = false;
				return true;
			}
			catch (Exception e)
			{
				My.Log($"SaveAs error {e}");
			}
			return false;
		}

		public override string ToString()
		{
			return $"{FileName}";
		}
		#endregion
	}
}
