using GMap.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Route66
{
	#region DATA CONTRACTS
	public enum MachineTypes
	{
		StandardSpreader,
		WspPercentage,
		WspDosage,
		RspPercentage,
		RspDosage,
		Sprayer,
		Dst,
		StreetWasher
	}
	public enum NavigationMessages
	{
		TURN_LEFT,
		TURN_RIGHT,
		ENTER_ROUNDABOUT,
		EXIT_ROUNDABOUT,
		KEEP_LEFT,
		KEEP_RIGHT,
		U_TURN,
		TURN_HARD_LEFT,
		TURN_HARD_RIGHT,
		TAKE_RAMP_LEFT,
		TAKE_RAMP_RIGHT,
		ENTER_BIKE_LANE,
		BEGIN_BREAK,
		END_BREAK,
		TURN_RIGHT_INTO_BIKE_LANE,
		TURN_LEFT_INTO_BIKE_LANE,
		ARRIVE,
		MARKER,
		PROCEED
	}

	/// <summary>
	/// This class contains all properties for a blue navigationmarker: soundfile, navigation message...
	/// </summary>
	[Serializable]
	public class NavigationMarker : GpsMarker
	{
		public NavigationMarker() { }
		public NavigationMarker(PointLatLng position) : base(position)
		{
			Message = "Turn right";
			SoundFile = "Turn right.wav";
		}

		public override string ToString()
		{
			return $"{Message}";
		}
		#region PROPERTIES
		[XmlAttribute()]
		public string SoundFile { get; set; }
		[XmlAttribute()]
		public string Message { get; set; }
		#endregion
	}

	/// <summary>
	/// This class contains all properties for a green changemarker: dosage, leftWidth, rightWidth...
	/// </summary>
	[Serializable]
	public class ChangeMarker : GpsMarker
	{
		public ChangeMarker() { }
		public ChangeMarker(PointLatLng position) : base(position)
		{
			Dosage = 20.0;
			SpreadingWidthLeft = 1.0;
			SpreadingWidthRight = 1.0;
			SpreadingOnOff = true;
		}
		public override string ToString()
		{
			return $"Dosage {Dosage} g\nLeft {SpreadingWidthLeft} m\nRight {SpreadingWidthRight} m";
		}
		#region ROW1
		[XmlAttribute("Spreading")]public bool SpreadingOnOff { get; set; }
		[XmlAttribute("DualWidth")]public bool DualWidthOnOff { get; set; }
		[XmlAttribute("Spraying")] public bool SprayingOnOff { get; set; }
		[XmlAttribute("Pump")]public bool PumpOnOff { get; set; }
		#endregion
		#region ROW2
		//
		// Remove get;set to promote fields in xmlfile.
		//
		[XmlAttribute("Dosage")] public double Dosage;
		[XmlAttribute("Max")]public bool MaxOnOff { get; set; }
		[XmlAttribute("SecMat")]public bool SecMatOnOff { get; set; }
		[XmlAttribute("Liquid")]public double SecLiquid { get; set; }
		[XmlAttribute("DosageLiquid")]public double DosageLiquid { get; set; }
		[XmlAttribute("Hopper1")]public bool Hopper1OnOff { get; set; }
		[XmlAttribute("Hopper2")]public bool Hopper2OnOff { get; set; }
		#endregion
		#region ROW3
		[XmlAttribute("WidthLeft")] public double SpreadingWidthLeft;
		[XmlAttribute("WidthRight")] public double SpreadingWidthRight;
		[XmlAttribute("SprayLeft")] public double SprayingWidthLeft { get; set; }
		[XmlAttribute("SprayRight")] public double SprayingWidthRight { get; set; }
		#endregion
	}

	/// <summary>
	/// This class contains all properties for a red GPS marker: Lng, Lat 
	/// </summary>
	[Serializable]
	public class GpsMarker
	{
		//public GpsMarker(double lat, double lng)
		//{
		//	Lng = lng;
		//	Lat = lat;
		//}
		public GpsMarker()
		{
			Lng = 4.0;
			Lat = 52.0;
		}

		public GpsMarker(PointLatLng point)
		{
			Lat = point.Lat;
			Lng = point.Lng;
		}

		[XmlAttribute()]
		public double Lat { get; set; }
		[XmlAttribute()]
		public double Lng { get; set; }
		public override string ToString()
		{
			return $"Lat {Lat} Lng {Lng}";
		}
	}
	#endregion
}
