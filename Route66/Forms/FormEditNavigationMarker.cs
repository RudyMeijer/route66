namespace Route66
{
	using GMap.NET.WindowsForms;
	using MyLib;
	using System;
	using System.IO;
	using System.Windows.Forms;
	public partial class FormEditNavigationMarker : Form
	{
		#region FIELDS
		private GMapMarker marker;
		private readonly object originalTag;
		private bool IsButton;
		#endregion
		#region CONSTRUCTOR
		public FormEditNavigationMarker(GMapMarker marker)
		{
			InitializeComponent();
			InitializeComboBox();
			this.marker = marker;
			originalTag = marker.Tag;
			if (marker.Tag == null) marker.Tag = new NavigationMarker(marker.Position);
			NavigationMarker = marker.Tag as NavigationMarker;
			DisplayOnForm(NavigationMarker);
			this.Settings = Settings.Global;
		}

		private void InitializeComboBox()
		{
			cmbMessage.DataSource = Translate.NavigationMessages;
			//cmbMessage_SelectedIndexChanged(null, null);
		}
		#endregion
		#region PROPERTIES
		public NavigationMarker NavigationMarker { get; set; }
		public Settings Settings { get; private set; }
		#endregion
		#region METHODES
		private void DisplayOnForm(NavigationMarker marker)
		{
			cmbMessage.SelectedIndex = -1; // Allow custom message.
			cmbMessage.Text = marker.Message;
			txtSoundFile.Text = marker.SoundFile;
		}
		private void GetFromForm()
		{
			NavigationMarker.Message = cmbMessage.Text;
			NavigationMarker.SoundFile = txtSoundFile.Text;
		}
		private void btnSave_Click(object sender, EventArgs e)
		{
			GetFromForm();
			IsButton = true;
			this.Close();
		}

		private void btnDelete_Click(object sender, EventArgs e)
		{
			marker.Tag = null;
			IsButton = true;
			this.Close();
		}
		private void FormEditChangeMarker_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (!IsButton) marker.Tag = originalTag;
		}

		private void btnPlay_Click(object sender, EventArgs e)
		{
			My.PlaySound(cmbMessage.Text);
		}

		private void cmbMessage_Validated(object sender, EventArgs e)
		{
			if (Settings.SpeechSyntesizer)
			{
				var wavFile = My.CheckPath(Settings.RoutePath , "VoiceFiles", My.ValidateFilename(cmbMessage.Text) + ".wav");
				Speech.SaveWav(cmbMessage.Text, wavFile);
			}
			cmbMessage_SelectedIndexChanged(null, null);
		}
		#endregion

		private void cmbMessage_SelectedIndexChanged(object sender, EventArgs e)
		{
			txtSoundFile.Text = My.ValidateFilename(cmbMessage.Text) + ".wav";
		}
	}
}
