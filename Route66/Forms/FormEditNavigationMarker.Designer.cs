namespace Route66
{
	partial class FormEditNavigationMarker
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.btnPlay = new System.Windows.Forms.Button();
			this.txtSoundFile = new System.Windows.Forms.TextBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.cmbMessage = new System.Windows.Forms.ComboBox();
			this.btnRemove = new System.Windows.Forms.Button();
			this.btnSave = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.txtSoundFile);
			this.groupBox1.Location = new System.Drawing.Point(12, 65);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(270, 47);
			this.groupBox1.TabIndex = 3;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Sound file";
			// 
			// btnPlay
			// 
			this.btnPlay.Location = new System.Drawing.Point(216, 16);
			this.btnPlay.Name = "btnPlay";
			this.btnPlay.Size = new System.Drawing.Size(44, 23);
			this.btnPlay.TabIndex = 1;
			this.btnPlay.Text = "Play";
			this.btnPlay.UseVisualStyleBackColor = true;
			this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
			// 
			// txtSoundFile
			// 
			this.txtSoundFile.Location = new System.Drawing.Point(6, 19);
			this.txtSoundFile.Name = "txtSoundFile";
			this.txtSoundFile.Size = new System.Drawing.Size(254, 20);
			this.txtSoundFile.TabIndex = 0;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.btnPlay);
			this.groupBox2.Controls.Add(this.cmbMessage);
			this.groupBox2.Location = new System.Drawing.Point(12, 12);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(270, 47);
			this.groupBox2.TabIndex = 5;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Message";
			// 
			// cmbMessage
			// 
			this.cmbMessage.FormattingEnabled = true;
			this.cmbMessage.Location = new System.Drawing.Point(3, 16);
			this.cmbMessage.Name = "cmbMessage";
			this.cmbMessage.Size = new System.Drawing.Size(207, 21);
			this.cmbMessage.TabIndex = 0;
			this.cmbMessage.SelectedIndexChanged += new System.EventHandler(this.cmbMessage_SelectedIndexChanged);
			this.cmbMessage.Validated += new System.EventHandler(this.cmbMessage_Validated);
			// 
			// btnRemove
			// 
			this.btnRemove.Location = new System.Drawing.Point(116, 127);
			this.btnRemove.Name = "btnRemove";
			this.btnRemove.Size = new System.Drawing.Size(75, 23);
			this.btnRemove.TabIndex = 6;
			this.btnRemove.Text = "Remove";
			this.btnRemove.UseVisualStyleBackColor = true;
			this.btnRemove.Click += new System.EventHandler(this.btnDelete_Click);
			// 
			// btnSave
			// 
			this.btnSave.Location = new System.Drawing.Point(197, 127);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(75, 23);
			this.btnSave.TabIndex = 7;
			this.btnSave.Text = "Save";
			this.btnSave.UseVisualStyleBackColor = true;
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// FormEditNavigationMarker
			// 
			this.AcceptButton = this.btnSave;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(294, 162);
			this.Controls.Add(this.btnSave);
			this.Controls.Add(this.btnRemove);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormEditNavigationMarker";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "Navigation marker";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormEditChangeMarker_FormClosing);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button btnRemove;
		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.ComboBox cmbMessage;
		private System.Windows.Forms.TextBox txtSoundFile;
		private System.Windows.Forms.Button btnPlay;
	}
}