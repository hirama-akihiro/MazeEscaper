using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;

namespace MazeEscaper
{
	class SoundManager
	{
		private static SoundManager soundManager = new SoundManager();

		public AudioEngine AudioEngine { get; private set; }
		public WaveBank WaveBank { get; private set; }
		public SoundBank SoundBank { get; private set; }

		private SoundManager()
		{
			AudioEngine = new AudioEngine(@"Content/Sound/Sound.xgs");
			WaveBank = new WaveBank(AudioEngine, @"Content/Sound/Wave Bank.xwb");
			SoundBank = new SoundBank(AudioEngine, @"Content/Sound/Sound Bank.xsb");

			// ボリュームの設定
			AudioEngine.GetCategory("BGM").SetVolume(0.8f);
			AudioEngine.GetCategory("SE").SetVolume(1.0f);
		}

		public static SoundManager Instance
		{
			get
			{
				return soundManager;
			}
		}
	}
}
