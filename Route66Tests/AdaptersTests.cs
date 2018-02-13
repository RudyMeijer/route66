using GMap.NET;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyLib;
using Route66;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Route66.Tests
{
    [TestClass()]
    public class AdaptersTests
    {
        private PointLatLng startpoint;
        private PointLatLng FirstNavpoint;

        [TestMethod, DeploymentItem(@"..\..\Test data\")]
        public void ReadAr3Test()
        {
            var filename = "KleineRoute.ar3";
            var route = Adapters.ReadAr3(filename);
            Assert.IsFalse(Route.IsDefaultFile, "ReadAr3 error 0");
            Assert.IsTrue(route.GpsMarkers.Count == 595, "ReadAr3 error 1");
            Assert.IsTrue(route.ChangeMarkers.Count == 66, "ReadAr3 error 2");
            Assert.IsTrue(route.NavigationMarkers.Count == 56, "ReadAr3 error 3");
            //
            // Test last navigation marker.
            //
            var np = route.NavigationMarkers[55];
            Assert.IsTrue(np.Lat == 52.2769583333333, "ReadAr3 error 4");
            Assert.IsTrue(np.SoundFile == "Arrive.wav", "ReadAr3 error 5");
            //
            // Test internal Filename.
            //
            Assert.IsTrue((string)My.GetField(route, "FileName") == filename, "ReadAr3 error 6");
            //
            // Test requirements.
            //
            Assert.IsTrue(Adapters.errors[0] == 1, "Requirement violation 7");
            Assert.IsTrue(Adapters.errors[1] == 4, "Requirement violation 8");
            Assert.IsTrue(Adapters.errors[2] == 0, "Requirement violation 9");
            Assert.IsTrue(Adapters.errors[3] == 0, "Requirement violation 10");
            Assert.IsTrue(Adapters.errors[4] == 112, "Requirement violation 11");
        }
        [TestMethod, DeploymentItem(@"..\..\Test data\")]
        public void RequirementsAr3Test()
        {
            var p = My.CurrentDirectory;
            var filename = "Requirements test.ar3";
            var route = Adapters.ReadAr3(filename);
            Assert.IsTrue(route.GpsMarkers.Count == 5, "Requirement violation 12");
            Assert.IsTrue(route.NavigationMarkers.Count == 1, "Requirement violation 13");
            Assert.IsTrue(route.ChangeMarkers.Count == 4, "Requirement violation 14");
            //
            // Check errors.
            //
            var e = Adapters.errors;
            Assert.IsTrue(e[0] == 1, "Points have descending distance and will be ignored.");
            Assert.IsTrue(e[1] == 2, "Duplicated line.");
            Assert.IsTrue(e[2] == 1, "Unkown navigation type.");
            Assert.IsTrue(e[3] == 0, "Exception.");
            Assert.IsTrue(e[4] == 2, "Orphan.");
            //
            // Write ar3 and read back.
            //
            filename = "Requirements test wr.ar3";
            Adapters.WriteAr3(filename, route);
            var route2 = Adapters.ReadAr3(filename);
            Assert.IsTrue(route2.GpsMarkers.Count == 5, "Requirement violation 22");
            Assert.IsTrue(route2.NavigationMarkers.Count == 1, "Requirement violation 23");
            Assert.IsTrue(route2.ChangeMarkers.Count == 4, "Requirement violation 24");
        }

        [TestMethod, DeploymentItem(@"..\..\Test data\")]
        public void UniqueLatLngTest()
        {
            UniqueTest("Holten_cityjet - Copy.ar3");
            UniqueTest("Holten_cityjet - Copy wr.ar3");
            UniqueTest("KleineRoute.ar3");
            UniqueTest("Requirements test.ar3");
        }
        public void UniqueTest(string filename)
        {
            Console.WriteLine($"Unique test {filename}");
            var route = Adapters.ReadAr3(filename);
            //
            // Check if gps markers are unique.
            //
            var idx = 0;
            var uniqueGps = new Dictionary<PointLatLng, int>();
            foreach (var item in route.GpsMarkers)
            {
                var point = new PointLatLng(item.Lat, item.Lng);
                if (uniqueGps.ContainsKey(point))
                    Assert.Fail($"Gps[{idx}] marker {point} not unique.");
                else uniqueGps.Add(point, 0);

                if (idx == 0) startpoint = point;
                if (idx == 1) FirstNavpoint = point;
                ++idx;
            }
            //
            // Check if navigation markers are unique and in Gps table.
            //
            idx = 0;
            var uniqueNav = new Dictionary<PointLatLng, int>();
            foreach (var item in route.NavigationMarkers)
            {
                var point = new PointLatLng(item.Lat, item.Lng);
                if(idx==0 && point != FirstNavpoint)
                {
                    Assert.Fail($"First Navigation marker[{idx}] {point} is not equal to gps marker[1] {FirstNavpoint}.");
                }
                if (uniqueNav.ContainsKey(point))
                {
                    var i = route.NavigationMarkers.FindIndex(n => n.Lat == point.Lat);
                    Assert.Fail($"Navigation marker[{idx}] is equal to marker[{i}] {point}.");
                }
                else uniqueNav.Add(point, 0);

                Assert.IsTrue(uniqueGps.ContainsKey(point), $"Navigation[{idx}] orphan marker. Not in Gps table.");
                ++idx;
            }
            //
            // Check if change markers are unique and in Gps table.
            //
            idx = 0;
            var uniqueChange = new Dictionary<PointLatLng, int>();
            foreach (var item in route.ChangeMarkers)
            {
                var point = new PointLatLng(item.Lat, item.Lng);
                if (idx == 0 && point != startpoint)
                {
                    Assert.Fail($"First Change marker[{idx}] {point} is not equal to gps marker[0] {startpoint}.");
                }
                if (uniqueChange.ContainsKey(point))
                {
                    var i = route.ChangeMarkers.FindIndex(n => n.Lat == point.Lat);
                    Assert.Fail($"Change marker[{idx}] is equal to marker[{i}] {point}.");
                }
                else uniqueChange.Add(point, 0);

                Assert.IsTrue(uniqueGps.ContainsKey(point), $"Change marker[{idx}] is orphan. Not in Gps table.");
                ++idx;
            }
        }
    }
}