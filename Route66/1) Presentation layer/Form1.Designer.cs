namespace Route66
{
	partial class Form1
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.btnClear = new System.Windows.Forms.Button();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.gmap = new GMap.NET.WindowsForms.GMapControl();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.btnChangeGlobalDosage = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.numDosageTo = new System.Windows.Forms.NumericUpDown();
			this.numDosageFrom = new System.Windows.Forms.NumericUpDown();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.btnAutoNavigate = new System.Windows.Forms.Button();
			this.chkAutoRoute = new System.Windows.Forms.CheckBox();
			this.chkEditRoute = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.chkArrowMarker = new System.Windows.Forms.CheckBox();
			this.chkShowTooltip = new System.Windows.Forms.CheckBox();
			this.chkNavPoints = new System.Windows.Forms.CheckBox();
			this.chkChangePoints = new System.Windows.Forms.CheckBox();
			this.chkGpsPoints = new System.Windows.Forms.CheckBox();
			this.txtSearchPlaces = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.AddtoolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.groupBox3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numDosageTo)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numDosageFrom)).BeginInit();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.menuStrip1.SuspendLayout();
			this.statusStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnClear
			// 
			this.btnClear.Location = new System.Drawing.Point(1, 11);
			this.btnClear.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.btnClear.Name = "btnClear";
			this.btnClear.Size = new System.Drawing.Size(143, 35);
			this.btnClear.TabIndex = 0;
			this.btnClear.Text = "Clear";
			this.btnClear.UseVisualStyleBackColor = true;
			this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainer1.Location = new System.Drawing.Point(0, 33);
			this.splitContainer1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.gmap);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.groupBox3);
			this.splitContainer1.Panel2.Controls.Add(this.groupBox2);
			this.splitContainer1.Panel2.Controls.Add(this.groupBox1);
			this.splitContainer1.Panel2.Controls.Add(this.txtSearchPlaces);
			this.splitContainer1.Panel2.Controls.Add(this.label1);
			this.splitContainer1.Panel2.Controls.Add(this.comboBox1);
			this.splitContainer1.Panel2.Controls.Add(this.btnClear);
			this.splitContainer1.Size = new System.Drawing.Size(1261, 735);
			this.splitContainer1.SplitterDistance = 1123;
			this.splitContainer1.SplitterWidth = 6;
			this.splitContainer1.TabIndex = 1;
			// 
			// gmap
			// 
			this.gmap.Bearing = 0F;
			this.gmap.CanDragMap = true;
			this.gmap.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gmap.EmptyTileColor = System.Drawing.Color.Navy;
			this.gmap.GrayScaleMode = false;
			this.gmap.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
			this.gmap.LevelsKeepInMemmory = 5;
			this.gmap.Location = new System.Drawing.Point(0, 0);
			this.gmap.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.gmap.MarkersEnabled = true;
			this.gmap.MaxZoom = 20;
			this.gmap.MinZoom = 2;
			this.gmap.MouseWheelZoomEnabled = true;
			this.gmap.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionWithoutCenter;
			this.gmap.Name = "gmap";
			this.gmap.NegativeMode = false;
			this.gmap.PolygonsEnabled = true;
			this.gmap.RetryLoadTile = 0;
			this.gmap.RoutesEnabled = true;
			this.gmap.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Integer;
			this.gmap.SelectedAreaFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(65)))), ((int)(((byte)(105)))), ((int)(((byte)(225)))));
			this.gmap.ShowTileGridLines = false;
			this.gmap.Size = new System.Drawing.Size(1123, 735);
			this.gmap.TabIndex = 0;
			this.gmap.Zoom = 13D;
			this.gmap.OnMarkerEnter += new GMap.NET.WindowsForms.MarkerEnter(this.gmap_OnMarkerEnter);
			this.gmap.OnMarkerLeave += new GMap.NET.WindowsForms.MarkerLeave(this.gmap_OnMarkerLeave);
			this.gmap.OnMapZoomChanged += new GMap.NET.MapZoomChanged(this.gmap_OnMapZoomChanged);
			this.gmap.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gmap_MouseDown);
			this.gmap.MouseLeave += new System.EventHandler(this.gmap_MouseLeave);
			this.gmap.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gmap_MouseMove);
			this.gmap.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gmap_MouseUp);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.btnChangeGlobalDosage);
			this.groupBox3.Controls.Add(this.label3);
			this.groupBox3.Controls.Add(this.label2);
			this.groupBox3.Controls.Add(this.numDosageTo);
			this.groupBox3.Controls.Add(this.numDosageFrom);
			this.groupBox3.Location = new System.Drawing.Point(3, 540);
			this.groupBox3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.groupBox3.Size = new System.Drawing.Size(144, 159);
			this.groupBox3.TabIndex = 6;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Dosage";
			// 
			// btnChangeGlobalDosage
			// 
			this.btnChangeGlobalDosage.Location = new System.Drawing.Point(9, 109);
			this.btnChangeGlobalDosage.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.btnChangeGlobalDosage.Name = "btnChangeGlobalDosage";
			this.btnChangeGlobalDosage.Size = new System.Drawing.Size(116, 35);
			this.btnChangeGlobalDosage.TabIndex = 7;
			this.btnChangeGlobalDosage.Text = "Change";
			this.btnChangeGlobalDosage.UseVisualStyleBackColor = true;
			this.btnChangeGlobalDosage.Click += new System.EventHandler(this.btnChangeGlobalDosage_Click);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(10, 72);
			this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(27, 20);
			this.label3.TabIndex = 8;
			this.label3.Text = "To";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(10, 32);
			this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(46, 20);
			this.label2.TabIndex = 7;
			this.label2.Text = "From";
			// 
			// numDosageTo
			// 
			this.numDosageTo.Location = new System.Drawing.Point(66, 69);
			this.numDosageTo.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.numDosageTo.Name = "numDosageTo";
			this.numDosageTo.Size = new System.Drawing.Size(58, 26);
			this.numDosageTo.TabIndex = 1;
			this.toolTip1.SetToolTip(this.numDosageTo, "Update dosage globally");
			this.numDosageTo.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
			// 
			// numDosageFrom
			// 
			this.numDosageFrom.Location = new System.Drawing.Point(66, 29);
			this.numDosageFrom.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.numDosageFrom.Name = "numDosageFrom";
			this.numDosageFrom.Size = new System.Drawing.Size(58, 26);
			this.numDosageFrom.TabIndex = 0;
			this.toolTip1.SetToolTip(this.numDosageFrom, "Enter zero to update all dosages.");
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.btnAutoNavigate);
			this.groupBox2.Controls.Add(this.chkAutoRoute);
			this.groupBox2.Controls.Add(this.chkEditRoute);
			this.groupBox2.Location = new System.Drawing.Point(1, 365);
			this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.groupBox2.Size = new System.Drawing.Size(144, 165);
			this.groupBox2.TabIndex = 5;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Route";
			// 
			// btnAutoNavigate
			// 
			this.btnAutoNavigate.Location = new System.Drawing.Point(9, 104);
			this.btnAutoNavigate.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.btnAutoNavigate.Name = "btnAutoNavigate";
			this.btnAutoNavigate.Size = new System.Drawing.Size(132, 35);
			this.btnAutoNavigate.TabIndex = 9;
			this.btnAutoNavigate.Text = "Auto navigate";
			this.toolTip1.SetToolTip(this.btnAutoNavigate, "Add navigation markers based on road angle.");
			this.btnAutoNavigate.UseVisualStyleBackColor = true;
			this.btnAutoNavigate.Click += new System.EventHandler(this.btnAutoNavigate_Click);
			// 
			// chkAutoRoute
			// 
			this.chkAutoRoute.AutoSize = true;
			this.chkAutoRoute.Location = new System.Drawing.Point(9, 68);
			this.chkAutoRoute.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.chkAutoRoute.Name = "chkAutoRoute";
			this.chkAutoRoute.Size = new System.Drawing.Size(110, 24);
			this.chkAutoRoute.TabIndex = 2;
			this.chkAutoRoute.Text = "Auto route";
			this.toolTip1.SetToolTip(this.chkAutoRoute, "Add intermediate markers automatic onto road.");
			this.chkAutoRoute.UseVisualStyleBackColor = true;
			this.chkAutoRoute.CheckedChanged += new System.EventHandler(this.chkAutoRoute_CheckedChanged);
			// 
			// chkEditRoute
			// 
			this.chkEditRoute.AutoSize = true;
			this.chkEditRoute.Location = new System.Drawing.Point(9, 29);
			this.chkEditRoute.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.chkEditRoute.Name = "chkEditRoute";
			this.chkEditRoute.Size = new System.Drawing.Size(104, 24);
			this.chkEditRoute.TabIndex = 1;
			this.chkEditRoute.Text = "Edit route";
			this.chkEditRoute.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.chkArrowMarker);
			this.groupBox1.Controls.Add(this.chkShowTooltip);
			this.groupBox1.Controls.Add(this.chkNavPoints);
			this.groupBox1.Controls.Add(this.chkChangePoints);
			this.groupBox1.Controls.Add(this.chkGpsPoints);
			this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.groupBox1.Location = new System.Drawing.Point(1, 135);
			this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.groupBox1.Size = new System.Drawing.Size(144, 220);
			this.groupBox1.TabIndex = 3;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Markers";
			// 
			// chkArrowMarker
			// 
			this.chkArrowMarker.AutoSize = true;
			this.chkArrowMarker.Location = new System.Drawing.Point(9, 136);
			this.chkArrowMarker.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.chkArrowMarker.Name = "chkArrowMarker";
			this.chkArrowMarker.Size = new System.Drawing.Size(76, 24);
			this.chkArrowMarker.TabIndex = 5;
			this.chkArrowMarker.Text = "Arrow";
			this.chkArrowMarker.UseVisualStyleBackColor = true;
			this.chkArrowMarker.CheckedChanged += new System.EventHandler(this.chkArrowMarker_CheckedChanged);
			// 
			// chkShowTooltip
			// 
			this.chkShowTooltip.AutoSize = true;
			this.chkShowTooltip.Location = new System.Drawing.Point(9, 172);
			this.chkShowTooltip.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.chkShowTooltip.Name = "chkShowTooltip";
			this.chkShowTooltip.Size = new System.Drawing.Size(82, 24);
			this.chkShowTooltip.TabIndex = 4;
			this.chkShowTooltip.Text = "Tooltip";
			this.chkShowTooltip.UseVisualStyleBackColor = true;
			this.chkShowTooltip.CheckedChanged += new System.EventHandler(this.chkShowTooltip_CheckedChanged);
			// 
			// chkNavPoints
			// 
			this.chkNavPoints.AutoSize = true;
			this.chkNavPoints.Checked = true;
			this.chkNavPoints.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkNavPoints.Location = new System.Drawing.Point(9, 100);
			this.chkNavPoints.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.chkNavPoints.Name = "chkNavPoints";
			this.chkNavPoints.Size = new System.Drawing.Size(109, 24);
			this.chkNavPoints.TabIndex = 3;
			this.chkNavPoints.Text = "Navigation";
			this.chkNavPoints.UseVisualStyleBackColor = true;
			this.chkNavPoints.CheckedChanged += new System.EventHandler(this.chkNavPoints_CheckedChanged);
			// 
			// chkChangePoints
			// 
			this.chkChangePoints.AutoSize = true;
			this.chkChangePoints.Checked = true;
			this.chkChangePoints.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkChangePoints.Location = new System.Drawing.Point(9, 65);
			this.chkChangePoints.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.chkChangePoints.Name = "chkChangePoints";
			this.chkChangePoints.Size = new System.Drawing.Size(91, 24);
			this.chkChangePoints.TabIndex = 2;
			this.chkChangePoints.Text = "Change";
			this.chkChangePoints.UseVisualStyleBackColor = true;
			this.chkChangePoints.CheckedChanged += new System.EventHandler(this.chkChangePoints_CheckedChanged);
			// 
			// chkGpsPoints
			// 
			this.chkGpsPoints.AutoSize = true;
			this.chkGpsPoints.Checked = true;
			this.chkGpsPoints.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkGpsPoints.Location = new System.Drawing.Point(9, 29);
			this.chkGpsPoints.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.chkGpsPoints.Name = "chkGpsPoints";
			this.chkGpsPoints.Size = new System.Drawing.Size(69, 24);
			this.chkGpsPoints.TabIndex = 1;
			this.chkGpsPoints.Text = "GPS";
			this.chkGpsPoints.UseVisualStyleBackColor = true;
			this.chkGpsPoints.CheckedChanged += new System.EventHandler(this.chkGpsPoints_CheckedChanged);
			// 
			// txtSearchPlaces
			// 
			this.txtSearchPlaces.Location = new System.Drawing.Point(68, 54);
			this.txtSearchPlaces.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.txtSearchPlaces.Name = "txtSearchPlaces";
			this.txtSearchPlaces.Size = new System.Drawing.Size(75, 26);
			this.txtSearchPlaces.TabIndex = 2;
			this.txtSearchPlaces.Text = "Nassau";
			this.txtSearchPlaces.WordWrap = false;
			this.txtSearchPlaces.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtSearch_PreviewKeyDown);
			this.txtSearchPlaces.Validated += new System.EventHandler(this.txtSearch_Validated);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 58);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(60, 20);
			this.label1.TabIndex = 1;
			this.label1.Text = "Search";
			// 
			// comboBox1
			// 
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Location = new System.Drawing.Point(1, 94);
			this.comboBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(140, 28);
			this.comboBox1.TabIndex = 1;
			this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
			// 
			// menuStrip1
			// 
			this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.optionsToolStripMenuItem,
            this.helpToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Padding = new System.Windows.Forms.Padding(9, 2, 0, 2);
			this.menuStrip1.Size = new System.Drawing.Size(1261, 33);
			this.menuStrip1.TabIndex = 2;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.AddtoolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.exitToolStripMenuItem});
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(50, 29);
			this.toolStripMenuItem1.Text = "File";
			// 
			// newToolStripMenuItem
			// 
			this.newToolStripMenuItem.Name = "newToolStripMenuItem";
			this.newToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
			this.newToolStripMenuItem.Size = new System.Drawing.Size(206, 30);
			this.newToolStripMenuItem.Text = "New";
			this.newToolStripMenuItem.Click += new System.EventHandler(this.NewToolStripMenuItem_Click);
			// 
			// openToolStripMenuItem
			// 
			this.openToolStripMenuItem.Name = "openToolStripMenuItem";
			this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
			this.openToolStripMenuItem.Size = new System.Drawing.Size(206, 30);
			this.openToolStripMenuItem.Text = "Open";
			this.openToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItem_Click);
			// 
			// AddtoolStripMenuItem
			// 
			this.AddtoolStripMenuItem.Name = "AddtoolStripMenuItem";
			this.AddtoolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
			this.AddtoolStripMenuItem.Size = new System.Drawing.Size(206, 30);
			this.AddtoolStripMenuItem.Text = "Add";
			this.AddtoolStripMenuItem.Click += new System.EventHandler(this.AddtoolStripMenuItem_Click);
			// 
			// saveToolStripMenuItem
			// 
			this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
			this.saveToolStripMenuItem.Size = new System.Drawing.Size(206, 30);
			this.saveToolStripMenuItem.Text = "Save";
			this.saveToolStripMenuItem.Click += new System.EventHandler(this.SaveToolStripMenuItem_Click);
			// 
			// saveAsToolStripMenuItem
			// 
			this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
			this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(206, 30);
			this.saveAsToolStripMenuItem.Text = "Save As...";
			this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.SaveAsToolStripMenuItem_Click);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(206, 30);
			this.exitToolStripMenuItem.Text = "Exit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// optionsToolStripMenuItem
			// 
			this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
			this.optionsToolStripMenuItem.Size = new System.Drawing.Size(88, 29);
			this.optionsToolStripMenuItem.Text = "Options";
			this.optionsToolStripMenuItem.Click += new System.EventHandler(this.OptionsToolStripMenuItem_Click);
			// 
			// helpToolStripMenuItem
			// 
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			this.helpToolStripMenuItem.Size = new System.Drawing.Size(61, 29);
			this.helpToolStripMenuItem.Text = "Help";
			this.helpToolStripMenuItem.Click += new System.EventHandler(this.HelpToolStripMenuItem_Click);
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.DefaultExt = "xml";
			this.openFileDialog1.FileName = "Route66";
			this.openFileDialog1.Filter = "xml files|*.xml|ar3 files|*.ar3|gpx files|*.gpx|All files|*.*";
			// 
			// statusStrip1
			// 
			this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
			this.statusStrip1.Location = new System.Drawing.Point(0, 738);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 21, 0);
			this.statusStrip1.Size = new System.Drawing.Size(1261, 30);
			this.statusStrip1.TabIndex = 3;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// toolStripStatusLabel1
			// 
			this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
			this.toolStripStatusLabel1.Size = new System.Drawing.Size(1239, 25);
			this.toolStripStatusLabel1.Spring = true;
			this.toolStripStatusLabel1.Text = "Ready";
			this.toolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1261, 768);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.Name = "Form1";
			this.Text = "Route66";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
			this.Load += new System.EventHandler(this.Form1_Load);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numDosageTo)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numDosageFrom)).EndInit();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnClear;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private GMap.NET.WindowsForms.GMapControl gmap;
		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.TextBox txtSearchPlaces;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox chkNavPoints;
		private System.Windows.Forms.CheckBox chkChangePoints;
		private System.Windows.Forms.CheckBox chkGpsPoints;
		private System.Windows.Forms.CheckBox chkShowTooltip;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;
		private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.CheckBox chkEditRoute;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.NumericUpDown numDosageFrom;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown numDosageTo;
		private System.Windows.Forms.Button btnChangeGlobalDosage;
		private System.Windows.Forms.CheckBox chkAutoRoute;
		private System.Windows.Forms.CheckBox chkArrowMarker;
		private System.Windows.Forms.ToolStripMenuItem AddtoolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
		private System.Windows.Forms.Button btnAutoNavigate;
	}
}

