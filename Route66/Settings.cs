﻿namespace Route66
{
    using System;
    using System.Windows.Forms;
    using System.Drawing;
    using System.Xml.Serialization;
    using System.IO;
    using System.ComponentModel;
    using System.Drawing.Design;
    using System.Windows.Forms.Design;
    using Route66;
    using MyLib;
    using GMap.NET.WindowsForms;

    [Serializable()]
    public class Settings
    {
        #region FIELDS
        private string fileName;
        private static Settings appSettings;
        private static PropertyGrid _propertyGrid;
        #endregion
        #region CONSTRUCTOR
        public Settings()
        {
            //
            // Set Defaults.
            //
            HelpVisible = true;
            RoutePath = @"C:\ProgramData\Aebi-Schmidt\AutologicRouteCreator\Routes";
            MachineType = MachineTypes.StandardSpreader;
            MapProvider = "BingHybridMap";
            FastDrawMode = true;
            SpeechSyntesizer = true;
            ArrowMarker = true;
            ToolTipMode = true;
        }
        #endregion
        #region PROPERTIES
        ////[Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        ////[Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMaxAttribute(0, 64255, 1)]

        [Category("Application Settings"), Description("Show description of applicatie parameters.")]
        public bool HelpVisible { get; set; }

        [Category("Application Settings"), DescriptionAttribute("Supervisors have special rights for modifications.")]
        public bool SupervisorMode { get; set; }

        public static bool IsChanged { get; private set; }

        public static Settings Global { get; private set; }

        [Category("Route Settings"), Description("Enter routes location. Leave empty to open last location.")]
        [Editor(typeof(FolderNameEditor), typeof(UITypeEditor))]
        public string RoutePath { get; set; }

        [Category("Route Settings"), Description("Turn speech syntesizer on for automate translating navigation messages.")]
        public bool SpeechSyntesizer { get; set; }

        [Category("Route Settings"), Description("Default machine type: standard spreader.")]
        public MachineTypes MachineType { get; set; }

        [Category("Route Settings"), Description("Default map provider: BingHybridMap, OpenStreetMap...")]
        public string MapProvider { get; set; }

        [Category("Route Settings"), Description("Route edit mode: Select current marker by mouse hover instead of left mouse click.")]
        public bool FastDrawMode { get; set; }

        [Category("Route Settings"), Description("Show red marker index in tooltip.")]
        public bool ToolTipMode { get; set; }

        [Category("Route Settings"), Description("Show current marker as blue arrow.")]
        public bool ArrowMarker { get; set; }

        [Category("Route Settings"), Description("Default file extension: 1=xml, 2=ar3, ...")]
        public int FileExtension { get; set; }
        #endregion
        #region METHODES

        public static Settings Load(PropertyGrid propertyGrid = null, string fileName = "Settings.xml", bool autoCreate = true)
        {
            appSettings = new Settings(); // Set defaults
            var backupFile = fileName.Replace(".xml", " backup.xml");
            try
            {
				if (File.Exists(fileName))
				{
					using (TextReader reader = new StreamReader(fileName)) appSettings = new XmlSerializer(typeof(Settings)).Deserialize(reader) as Settings;
				}
				else if (autoCreate)
				{
					appSettings.SaveAs(fileName);
				}
				else if (My.Show("Would you like to load default settings?", "", MessageBoxButtons.YesNo) == DialogResult.No)
				{
					Environment.Exit(1);
				}
			}
            catch (Exception ex)
            {
                My.Show(ex.Message, "Error during loading settings.");
                if (File.Exists(backupFile)) File.Copy(backupFile, fileName, true);
            }

            appSettings.fileName = fileName; // allow Save().
            _propertyGrid = propertyGrid;    // allow refresh().
            Global = appSettings;            // allow global access to settings.

            if (propertyGrid != null)
            {
                propertyGrid.SelectedObject = appSettings;
                propertyGrid.PropertyValueChanged += new PropertyValueChangedEventHandler(propertygrid_PropertyValueChanged);
                ////propertyGrid.CollapseAllGridItems();
            }
            return appSettings;
        }

        public static void propertygrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            var pg = s as PropertyGrid;
            var property = e.ChangedItem;
            if (property.Label == "SupervisorMode")
            {
                if (appSettings.SupervisorMode) appSettings.SupervisorMode = (Prompt.ShowDialog("Enter Supervisor password:", (s as PropertyGrid)?.ParentForm.Text) == "Rudy" + DateTime.Now.Minute.ToString("00"));
                else appSettings.SaveAs(appSettings.fileName); ////Save SuperVisor = false;
            }
            My.Log($"User {My.UserName} changed application setting {property.Label} from {e.OldValue} to {property.Value}.");
            if (property.Label.EndsWith("Path")) CheckPath(e);
            if (property.Label.StartsWith("Help")) pg.HelpVisible = (bool)property.Value;
            appSettings.Save();
            IsChanged = true;
        }

        private static void CheckPath(PropertyValueChangedEventArgs e)
        {
            string path = e.ChangedItem.Value as String;
            bool IsFile = Path.GetExtension(path) != "";
            string dir = (IsFile) ? Path.GetDirectoryName(path) : path;
            if (!Directory.Exists(dir))
            {
                if (My.Show("Create directory " + dir, e.ChangedItem.Label, MessageBoxButtons.YesNo) == DialogResult.No) return;
                Directory.CreateDirectory(dir);
            }
            if (IsFile && !File.Exists(path)) My.Show("File " + path + " doesn't exist.", e.ChangedItem.Label);
        }

        public void Save()
        {
            SaveAs(appSettings.fileName); // Save last loaded settings file.
        }

        public void SaveAs(string fileName)
        {
            using (var writer = new StreamWriter(fileName))
            {
                new XmlSerializer(typeof(Settings)).Serialize(writer, this);
            }
        }

        public void Copy(string path)
        {
            SaveAs(path + Path.PathSeparator + Path.GetFileName(this.fileName));
        }

        internal void Refresh()
        {
            _propertyGrid?.Refresh();
            Save();
        }
        #endregion
    }
}

