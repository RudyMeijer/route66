using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Route66
{
    internal class Route
    {
        private static Route route;
        private string fileName;

        public static Route Load( [Optional, DefaultParameterValue("Route66.xml")] string fileName)
        {
            route = new Route();
            try
            {
                using (TextReader reader = new StreamReader(fileName))
                {
                    route = new XmlSerializer(typeof(Route)).Deserialize(reader) as Route;
                }
            }
            catch (Exception exception)
            {
                if (MessageBox.Show(exception.Message + "\nWould you like to load default settings?", "Loading application settings.", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    Environment.Exit(1);
                }
            }
            route.fileName = fileName;
            return route;
        }




        public void SaveAs(string fileName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Route));
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                serializer.Serialize((TextWriter)writer, this);
            }
        }
    }
}