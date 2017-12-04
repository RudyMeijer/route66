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
		RspDstPercentage,
		WspDstPercentage,
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
		TURN_RIGHT_INTO_BIKE_LANE,
		TURN_LEFT_INTO_BIKE_LANE,
	}

	/// <summary>
	/// This class contains all properties for a blue navigationmarker: soundfile, navigation message...
	/// </summary>
	[Serializable]
	public class NavigationMarker : GpsMarker
	{
		public NavigationMarker() { }
		public NavigationMarker(PointLatLng position) : base(position.Lng, position.Lat)
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
	/// This class contains all properties for a green changemarker: dosing, leftWidth, rightWidth...
	/// </summary>
	[Serializable]
	public class ChangeMarker : GpsMarker
	{
		public ChangeMarker() { }
		public ChangeMarker(PointLatLng position) : base(position.Lng, position.Lat)
		{
			Dosing = 20.0;
			WidthLeft = 1.0;
			WidthRight = 1.0;
		}
		public override string ToString()
		{
			return $"Dosing {Dosing}\nWidthLeft {WidthLeft}\nWidthRight {WidthRight}";
		}
		[XmlAttribute()]
		public double Dosing { get; set; }
		[XmlAttribute()]
		public double WidthLeft { get; set; }
		[XmlAttribute()]
		public double WidthRight { get; set; }
	}

	/// <summary>
	/// This class contains all properties for a red GPS marker: Lng, Lat 
	/// </summary>
	[Serializable]
	public class GpsMarker
	{
		public GpsMarker(double lng, double lat)
		{
			Lng = lng;
			Lat = lat;
		}
		public GpsMarker()
		{
			Lng = 4.0;
			Lat = 52.0;
		}
		[XmlAttribute()]
		public double Lng { get; set; }
		[XmlAttribute()]
		public double Lat { get; set; }
	}
	#endregion
}
