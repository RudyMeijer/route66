using System;
using System.Speech.Synthesis;

namespace Route66
{
	internal class Speech
	{
		private static SpeechSynthesizer synthesizer;
		public Speech()
		{

		}
		internal static void Play(string text)
		{
			if (synthesizer == null)
			{
				synthesizer = new SpeechSynthesizer
				{
					Volume = 100,  // 0...100
					Rate = -2     // -10...10
				};
				synthesizer.SelectVoiceByHints(VoiceGender.Female);
				//synthesizer.SpeakCompleted += Synthesizer_SpeakCompleted;
			}
			synthesizer.SpeakAsync(text);
		}

		internal static void SaveWav(string text, string wavFile)
		{
			if (synthesizer == null)
			{
				synthesizer = new SpeechSynthesizer();
				synthesizer.Volume = 100;  // 0...100
				synthesizer.Rate = -2;     // -10...10
				synthesizer.SelectVoiceByHints(VoiceGender.Female);
				//synthesizer.SpeakCompleted += Synthesizer_SpeakCompleted;
			}
			synthesizer.SetOutputToWaveFile(wavFile);
			synthesizer.SpeakAsync(text);
		}

		//private static void Synthesizer_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
		//{
		//	synthesizer.SetOutputToDefaultAudioDevice();
		//}
	}
}