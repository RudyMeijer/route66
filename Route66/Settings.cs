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

[Serializable()]
public class Settings
{
	#region FIELDS
	public string fileName;
	private static Settings appSettings;
	private static PropertyGrid _propertyGrid;
	public static Settings Global { get; private set; }
	private static string backupFile;
	#endregion
	#region PROPERTIES
	//[Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
	//[Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMaxAttribute(0, 64255, 1)]
	[Category("Application Settings"), Description("Show description of applicatie parameters.")]
	public bool HelpVisible { get; set; }

	[Category("Application Settings"), DescriptionAttribute("Supervisors have special rights for modifications.")]
	public bool SupervisorMode { get; set; }
	public static bool IsChanged { get; private set; }

	[Category("Route Settings"), Description("Enter routes location. Leave empty to open last location.")]
	[Editor(typeof(FolderNameEditor), typeof(UITypeEditor))]
	public string RoutePath { get; set; }

	[Category("Route Settings"), Description("Default machine type: standard spreader.")]
	public MachineTypes MachineType { get; set; }
	[Category("Route Settings"), Description("Map provider: BingHybridMap, OpenStreetMap...")]
	public string MapProvider { get; set; }
	[Category("Route Settings"), Description("Route edit mode: Select current marker by mouse hover instead of left mouse click.")]
	public bool FastDrawMode { get; set; }
	[Category("Route Settings"), Description("Set speech recognition for navigation markers on/off.")]
	public bool SpeechRecognition { get; set; }
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
		SpeechRecognition = true;
	}
	#endregion
	#region METHODES
	public static Settings Load(PropertyGrid propertyGrid = null, string fileName = "Settings.xml", bool autoCreate = true)
	{
		appSettings = new Settings(); // Set defaults
		backupFile = fileName.Replace(".xml", " backup.xml");
		try
		{
			if (File.Exists(fileName))
			{
				using (TextReader reader = new StreamReader(fileName))
					appSettings = new XmlSerializer(typeof(Settings)).Deserialize(reader) as Settings;
			}
			else if (autoCreate) appSettings.SaveAs(fileName);
			else if (MessageBox.Show("Would you like to load default settings?", "", MessageBoxButtons.YesNo) == DialogResult.No)
				Environment.Exit(1);
		}
		catch (Exception ex)
		{
			My.Log($"Error loading appsettings {ex} trying restore backup.");
			if (File.Exists(backupFile)) File.Copy(backupFile, fileName, true);
			Environment.Exit(1);
		}

		appSettings.fileName = fileName; // allow Save().
		_propertyGrid = propertyGrid;    // allow refresh().
		Global = appSettings;

		if (propertyGrid != null)
		{
			propertyGrid.SelectedObject = appSettings;
			propertyGrid.PropertyValueChanged += new PropertyValueChangedEventHandler(propertygrid_PropertyValueChanged);
			//propertyGrid.CollapseAllGridItems();
		}
		return appSettings;
	}

	static void propertygrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
	{
		var pg = s as PropertyGrid;
		var property = e.ChangedItem;
		if (property.Label == "SupervisorMode")
		{
			if (appSettings.SupervisorMode) appSettings.SupervisorMode = (Prompt.ShowDialog("Enter Supervisor password:", _propertyGrid.ParentForm.Text) == "Rudy" + DateTime.Now.Minute.ToString("00"));
			else appSettings.SaveAs(appSettings.fileName); //Save SuperVisor = false;
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
			if (MessageBox.Show("Create directory " + dir, e.ChangedItem.Label, MessageBoxButtons.YesNo) == DialogResult.No) return;
			Directory.CreateDirectory(dir);
			//
			// Copy emu files.
			//
			//CopyEmuFile("_AllImages.emu", dir);
		}
		if (IsFile && !File.Exists(path)) MessageBox.Show("File " + path + " doesn't exist.", e.ChangedItem.Label);
	}
	//private static void CopyEmuFile(string file, string dir)
	//{
	//    try
	//    {
	//        if (dir.ToUpper().Contains("IMAGE") && File.Exists(file)) File.Copy(file, dir + "\\" + file);
	//    }
	//    catch (Exception ex) { My.Log("Error in copy emu file {0}\\{1} {2}", dir, file, ex); }
	//}
	public void Save()
	{
		SaveAs(appSettings.fileName); // Save last loaded settings file.
	}
	public void SaveAs(string fileName)
	{
		XmlSerializer serializer = new XmlSerializer(typeof(Settings));
		using (var writer = new StreamWriter(fileName)) { serializer.Serialize(writer, this); }
	}
	public void Copy(string path)
	{
		SaveAs(path + "\\" + Path.GetFileName(this.fileName));
	}
	internal void Refresh()
	{
		if (_propertyGrid != null) _propertyGrid.Refresh();
		Save();
	}
	#endregion
}

