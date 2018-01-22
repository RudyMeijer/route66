﻿// <copyright file="DataContracts.cs" company="Aebi Schmidt Nederland B.V.">
//   Aebi Schmidt Nederland B.V. All rights reserved.
// </copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GMap.NET;

[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder", Justification = "Reviewed. Suppression is OK here.")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK here.")]

namespace Route66
{
    /// <summary>
    /// This enum contains MachineTypes.
    /// </summary>
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

    /// <summary>
    /// This enum contains NavigationMessages.
    /// </summary>
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
        CONTINUE,
        PROCEED,
        BEAR_LEFT,
        BEAR_RIGHT
    }

    #region DATA CONTRACTS
    /// <summary>
    /// This class contains data contracts.
    /// </summary>
    public class DataContracts
    {
        /// <summary>
        /// This class contains all properties for a blue navigation marker: sound file, navigation message...
        /// </summary>
        [Serializable]
        public class NavigationMarker : GpsMarker
        {
            /// <summary>
            /// Default constructor needed for serialization.
            /// Initializes a new instance of the <see cref="NavigationMarker" /> class.
            /// </summary>
            public NavigationMarker()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="NavigationMarker" /> class.
            /// </summary>
            /// <param name="position">Latitude Longitude coordinate</param>
            public NavigationMarker(PointLatLng position) : base(position)
            {
                this.Message = "Turn right";
                this.SoundFile = "Turn right.wav";
            }
            #region PROPERTIES
            /// <summary>
            /// Gets or sets sound file.
            /// </summary>
            [XmlAttribute]
            public string SoundFile { get; set; }
            
            /// <summary>
            /// Gets or sets message.
            /// </summary>
            [XmlAttribute]
            public string Message { get; set; }
            #endregion

            /// <summary>
            /// override string
            /// </summary>
            /// <returns>return message</returns>
            public override string ToString()
            {
                return $"{Message}";
            }
        }

        /// <summary>
        /// This class contains all properties for a green change marker: dosage, leftWidth, rightWidth...
        /// </summary>
        [Serializable]
        public class ChangeMarker : GpsMarker
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ChangeMarker" /> class.
            /// </summary>
            public ChangeMarker()
            {
            }

            /// <summary>
            /// Initializes a new instance of this class.
            /// </summary>
            /// <param name="position">Latitude Longitude coordinate</param>
            public ChangeMarker(PointLatLng position) : base(position)
            {
                this.Dosage = 20.0;
                this.SpreadingWidthLeft = 1.0;
                this.SpreadingWidthRight = 1.0;
                this.SpreadingOnOff = false;
            }

            #region ROW1

            [XmlAttribute("Spreading")] public bool SpreadingOnOff { get; set; }

            [XmlAttribute("DualWidth")] public bool DualWidthOnOff { get; set; }

            [XmlAttribute("Spraying")] public bool SprayingOnOff { get; set; }

            [XmlAttribute("Pump")] public bool PumpOnOff { get; set; }
            #endregion
            #region ROW2
            /// <summary>
            /// Remove get;set to promote fields in xmlfile.
            /// </summary>
            [XmlAttribute("Dosage")] public double Dosage;

            [XmlAttribute("Max")] public bool MaxOnOff { get; set; }

            [XmlAttribute("SecMat")] public bool SecMatOnOff { get; set; }

            [XmlAttribute("PersentageLiquid")] public double PersentageLiquid { get; set; }

            [XmlAttribute("DosageLiquid")] public double DosageLiquid { get; set; }

            [XmlAttribute("Hopper1")] public bool Hopper1OnOff { get; set; }

            [XmlAttribute("Hopper2")] public bool Hopper2OnOff { get; set; }
            #endregion
            #region ROW3
            [XmlAttribute("WidthLeft")] public double SpreadingWidthLeft;

            [XmlAttribute("WidthRight")] public double SpreadingWidthRight;

            [XmlAttribute("SprayLeft")] public double SprayingWidthLeft { get; set; }

            [XmlAttribute("SprayRight")] public double SprayingWidthRight { get; set; }
            #endregion

            /// <summary>
            /// override string
            /// </summary>
            /// <returns>return message</returns>
            public override string ToString()
            {
                return (Settings.Global.MachineType == MachineTypes.StreetWasher) ? 
                    $"Pressure {Dosage} bar \nLeft {SpreadingWidthLeft} m \nRight {SpreadingWidthRight} m"
                    :
                    $"Dosage {Dosage} g \nLeft {SpreadingWidthLeft} m \nRight {SpreadingWidthRight} m";
            }
        }

        /// <summary>
        /// This class contains all properties for a red GPS marker: Lng, Lat 
        /// </summary>
        [Serializable]
        public class GpsMarker
        {
            /// <summary>
            /// Initializes a new instance of this class.
            /// </summary>
            public GpsMarker()
            {
                this.Lng = 4.0;
                this.Lat = 52.0;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="GpsMarker" /> class.
            /// </summary>
            /// <param name="point">Latlng point</param>
            public GpsMarker(PointLatLng point)
            {
                this.Lat = point.Lat;
                this.Lng = point.Lng;
            }

            /// <summary>
            /// Gets or sets Lat
            /// </summary>
            [XmlAttribute]
            public double Lat { get; set; }

            [XmlAttribute]
            public double Lng { get; set; }
            
            /// <summary>
            /// override string
            /// </summary>
            /// <returns>return message</returns>
            public override string ToString()
            {
                return $"Lat {Lat} Lng {Lng}";
            }
        }
    }
    #endregion
}
