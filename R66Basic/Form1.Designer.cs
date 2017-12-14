namespace R66Basic
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
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.gmap = new GMap.NET.WindowsForms.GMapControl();
			this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.btnClear = new System.Windows.Forms.Button();
			this.statusStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
			this.SuspendLayout();
			// 
			// menuStrip1
			// 
			this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(867, 24);
			this.menuStrip1.TabIndex = 0;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// statusStrip1
			// 
			this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
			this.statusStrip1.Location = new System.Drawing.Point(0, 500);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(867, 25);
			this.statusStrip1.TabIndex = 1;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// toolStripStatusLabel1
			// 
			this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
			this.toolStripStatusLabel1.Size = new System.Drawing.Size(151, 20);
			this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 24);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.gmap);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.numericUpDown1);
			this.splitContainer1.Panel2.Controls.Add(this.textBox1);
			this.splitContainer1.Panel2.Controls.Add(this.btnClear);
			this.splitContainer1.Size = new System.Drawing.Size(867, 476);
			this.splitContainer1.SplitterDistance = 729;
			this.splitContainer1.TabIndex = 2;
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
			this.gmap.Size = new System.Drawing.Size(729, 476);
			this.gmap.TabIndex = 0;
			this.gmap.Zoom = 0D;
			this.gmap.OnMarkerClick += new GMap.NET.WindowsForms.MarkerClick(this.gmap_OnMarkerClick);
			this.gmap.OnMarkerEnter += new GMap.NET.WindowsForms.MarkerEnter(this.gmap_OnMarkerEnter);
			this.gmap.OnMarkerLeave += new GMap.NET.WindowsForms.MarkerLeave(this.gmap_OnMarkerLeave);
			this.gmap.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gmap_KeyDown);
			this.gmap.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.gmap_MouseDoubleClick);
			this.gmap.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gmap_MouseDown);
			// 
			// numericUpDown1
			// 
			this.numericUpDown1.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.numericUpDown1.Location = new System.Drawing.Point(26, 144);
			this.numericUpDown1.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
			this.numericUpDown1.Name = "numericUpDown1";
			this.numericUpDown1.Size = new System.Drawing.Size(48, 22);
			this.numericUpDown1.TabIndex = 3;
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(26, 116);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(100, 22);
			this.textBox1.TabIndex = 1;
			this.textBox1.Text = "Vletgaarsmaten";
			this.textBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
			this.textBox1.Validated += new System.EventHandler(this.textBox1_Validated);
			// 
			// btnClear
			// 
			this.btnClear.Location = new System.Drawing.Point(22, 32);
			this.btnClear.Name = "btnClear";
			this.btnClear.Size = new System.Drawing.Size(75, 23);
			this.btnClear.TabIndex = 0;
			this.btnClear.Text = "Clear";
			this.btnClear.UseVisualStyleBackColor = true;
			this.btnClear.Click += new System.EventHandler(this.button1_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(867, 525);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "Form1";
			this.Text = "Form1";
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private GMap.NET.WindowsForms.GMapControl gmap;
		private System.Windows.Forms.Button btnClear;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
		private System.Windows.Forms.NumericUpDown numericUpDown1;
	}
}

