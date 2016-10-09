using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Speech.Synthesis;
using System.Text;

namespace ITTrade.IT
{
	public static class Sounds
	{
		public static void Play(TargetSound targetSound)
		{
			if (Settings.SoundsAllowed == false)
			{
				return;
			}

			switch (targetSound)
			{
				case TargetSound.UnknownProductBarcode:
					SystemSounds.Beep.Play();
					Console.Beep(400, 300);
					//using (var speach = new SpeechSynthesizer())
					//{
					//    speach.Speak("uups");
					//}
					break;
				case TargetSound.UnusedProductBarcode:
					Console.Beep(200, 300);
					SystemSounds.Beep.Play();
					//using (var speach = new SpeechSynthesizer())
					//{
					//    speach.Speak("uups");
					//}
					break;
				case TargetSound.QuantityEqualsOne:
					Console.Beep(2000, 400);
					break;
				case TargetSound.QuantityEqualsZero:
					Console.Beep(1000, 400);
					break;
				default:
					throw new NotSupportedException("Поддержка этого звука еще не реализована.");
			}
		}
	}
}
