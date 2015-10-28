using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MazeEscaper
{
	/// <summary>
	/// カウントダウンクラス
	/// </summary>
	class CountDown
	{
		private ContentManager content;
		private GraphicsDevice graphicsDevice;

		#region テクスチャ
		private Texture2D background;
		private Texture2D three;
		private Texture2D two;
		private Texture2D one;
		private Texture2D start;
		#endregion

		/// <summary>
		/// ゲーム開始時刻
		/// </summary>
		private GameTime startGameTime;

		/// <summary>
		/// ゲーム開始からの時間
		/// </summary>
		public TimeSpan totalTime;

		/// <summary>
		/// タイムリミット
		/// </summary>
		private TimeSpan timeLimit;

		/// <summary>
		/// 残り時間
		/// </summary>
		private TimeSpan remaningTime;

		/// <summary>
		/// True:カウントダウン中,False:カウントダウン終了後
		/// </summary>
		public bool Counting { get; set; }

		public bool Finished { get; set; }

		private bool FirstThree { get; set; }
		private bool FirstTwo { get; set; }
		private bool FirstOne { get; set; }

		public CountDown(ContentManager content, GraphicsDevice graphicsDevice,GameTime gameTime)
		{
			this.content = content;
			this.graphicsDevice = graphicsDevice;

			startGameTime = new GameTime(gameTime.TotalGameTime, gameTime.ElapsedGameTime);

			Counting = true;
			Finished = false;

			timeLimit = new TimeSpan(0, 0, 4);

			FirstThree = true;
			FirstTwo = true;
			FirstOne = true;

			LoadContent();

			//音声鳴らす
			SoundManager.Instance.SoundBank.PlayCue("countdown");
		}

		/// <summary>
		/// コンテンツ読み込み
		/// </summary>
		private void LoadContent()
		{
			background = content.Load<Texture2D>("CountDown/background");
			three = content.Load<Texture2D>("CountDown/three");
			two = content.Load<Texture2D>("CountDown/two");
			one = content.Load<Texture2D>("CountDown/one");
			start = content.Load<Texture2D>("CountDown/Start");
		}

		/// <summary>
		/// 描画メソッド
		/// </summary>
		/// <param name="spriteBatch"></param>
		internal void Draw(SpriteBatch spriteBatch)
		{
			if (Counting)
			{
				spriteBatch.Begin();
				spriteBatch.Draw(background, Vector2.Zero, new Color(0, 0, 0, 0.7f));
				switch(remaningTime.Seconds)
				{
					case(3):
						spriteBatch.Draw(three, new Vector2(300, 180), Color.White);
						break;
					case(2):
						spriteBatch.Draw(two, new Vector2(300, 180), Color.White);
						break;
					case(1):
						spriteBatch.Draw(one, new Vector2(300, 180), Color.White);
						break;
					case(0):
						spriteBatch.Draw(start, Vector2.Zero, Color.White);
						break;
				}
				spriteBatch.End();
			}
		}

		/// <summary>
		/// 更新処理
		/// </summary>
		/// <param name="gameTime"></param>
		internal void Update(GameTime gameTime)
		{
			// 経過時間を計算
			totalTime = gameTime.TotalGameTime - startGameTime.TotalGameTime;

			// 残り時間を計算
			remaningTime = timeLimit - totalTime;
			if (remaningTime < TimeSpan.Zero)
			{
				// ゲーム開始の音鳴らす
				remaningTime = TimeSpan.Zero;
				//音声ストップ
				Finished = true;
				Counting = false;
			}
		}

	}
}
