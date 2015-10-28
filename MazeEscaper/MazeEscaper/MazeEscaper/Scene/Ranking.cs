using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.VisualBasic.FileIO;
using Microsoft.Xna.Framework.Audio;

namespace MazeEscaper.Scene
{
	/// <summary>
	/// ランキングインターフェースクラス
	/// </summary>
	class Ranking : SceneInterface
	{
		//シーンインターフェース共通変数
		private ContentManager content;
		private GraphicsDevice graphicsDevice;

		private float textureheight = 100;
		private float offsetwidth = 100;
		private float offsetheight = 20;

		//テクスチャ各種
		private Texture2D background;
		private Texture2D ranking;
		private Texture2D rankingS;
		private Texture2D rankingR;
		private Texture2D rankingB;
		private Texture2D rankbar1;
		private Texture2D rankbar2;
		private Texture2D rankbar3;
		private Texture2D rankbarnow;
		private Texture2D name;
		private Texture2D time;
		private Texture2D newrecord;

		//フォント
		private SpriteFont font;
		private SpriteFont fontmath;

		private string[,] csvstr = new string[3, 2];

		/// <summary>
		/// 記録が更新されたか判定するフラグ
		/// </summary>
		private bool NewRecordFlag { get; set; }

		/// <summary>
		/// ゲームクリアにかかった時間
		/// </summary>
		private TimeSpan totalTime;

		/// <summary>
		/// ゲームクリア時間のランキング順位用変数
		/// </summary>
		private int newRank;

		/// <summary>
		/// ゲームのクリアにかかった時間を描画するかどうかを判定するフラグ
		/// </summary>
		private bool DrawFlag { get; set; }

		/// <summary>
		/// ステージサイズ
		/// </summary>
		private StageSelect.SelectStage selectStage;

		/// <summary>
		/// 
		/// </summary>
		private Cue cueBGM;

		/// <summary>
		/// フェードアウト処理用変数
		/// </summary>
		private FadeOut fadeout;

		/// <summary>
		/// ランキングシーン
		/// </summary>
		/// <param name="content"></param>
		/// <param name="graphicsDevice"></param>
		/// <param name="selectStage"></param>
		public Ranking(ContentManager content, GraphicsDevice graphicsDevice, StageSelect.SelectStage selectStage)
		{
			this.content = content;
			this.graphicsDevice = graphicsDevice;

			//ステージサイズ取得
			this.selectStage = selectStage;

			LoadContent();

			NewRecordFlag = false;
			DrawFlag = false;

			//音声初期化
			cueBGM = SoundManager.Instance.SoundBank.GetCue("result");
			cueBGM.Play();

			//フェードアウト初期化
			fadeout = new FadeOut(content, graphicsDevice, FadeOut.SceneType.OutGame);
		}

		/// <summary>
		/// ランキングシーン
		/// </summary>
		/// <param name="content"></param>
		/// <param name="graphicsDevice"></param>
		/// <param name="totalTime"></param>
		/// <param name="selectStage"></param>
		public Ranking(ContentManager content, GraphicsDevice graphicsDevice, TimeSpan totalTime, StageSelect.SelectStage selectStage)
		{
			this.content = content;
			this.graphicsDevice = graphicsDevice;

			//ステージサイズ取得
			this.selectStage = selectStage;

			//クリア時間取得
			this.totalTime = totalTime;

			LoadContent();

			NewRecordFlag = false;
			DrawFlag = true;
			//ランキングチェック
			newRank = checkRancking(totalTime, csvstr);

			//レコード更新フラグ確認
			if (newRank >= 0 && newRank <= 2)
				NewRecordFlag = true;

			//音声初期化
			cueBGM = SoundManager.Instance.SoundBank.GetCue("result");
			cueBGM.Play();

			fadeout = new FadeOut(content, graphicsDevice, FadeOut.SceneType.OutGame);
		}

		/// <summary>
		/// テクスチャの読み込み
		/// </summary>
		private void LoadContent()
		{
			#region フォント読み込み
			font = content.Load<SpriteFont>("Ranking/RankFont");
			fontmath = content.Load<SpriteFont>("Ranking/RankFontMath");
			#endregion

			#region テクスチャ読み込み
			background = content.Load<Texture2D>("Ranking/background");
			ranking = content.Load<Texture2D>("Ranking/ranking");
			rankingS = content.Load<Texture2D>("Ranking/rankingS");
			rankingR = content.Load<Texture2D>("Ranking/rankingR");
			rankingB = content.Load<Texture2D>("Ranking/rankingB");
			rankbar1 = content.Load<Texture2D>("Ranking/rankbar1");
			rankbar2 = content.Load<Texture2D>("Ranking/rankbar2");
			rankbar3 = content.Load<Texture2D>("Ranking/rankbar3");
			rankbarnow = content.Load<Texture2D>("Ranking/rankbarnow");
			name = content.Load<Texture2D>("Ranking/name");
			time = content.Load<Texture2D>("Ranking/time");
			newrecord = content.Load<Texture2D>("Ranking/newrecord");
			#endregion

			#region CSVファイル読み込み
			TextFieldParser parser;
			switch (selectStage)
			{
				case StageSelect.SelectStage.Small:
					parser = new TextFieldParser(@"../../../../MazeEscaperContent/Ranking/RankerS.csv", System.Text.Encoding.GetEncoding("Shift_JIS"));
					break;
				case StageSelect.SelectStage.Regular:
					parser = new TextFieldParser(@"../../../../MazeEscaperContent/Ranking/RankerR.csv", System.Text.Encoding.GetEncoding("Shift_JIS"));
					break;
				case StageSelect.SelectStage.Big:
					parser = new TextFieldParser(@"../../../../MazeEscaperContent/Ranking/RankerB.csv", System.Text.Encoding.GetEncoding("Shift_JIS"));
					break;
				default:
					parser = new TextFieldParser(@"../../../../MazeEscaperContent/Ranking/Ranker.csv", System.Text.Encoding.GetEncoding("Shift_JIS"));
					break;
			}

			parser.TextFieldType = FieldType.Delimited;
			//区切り文字はコンマ
			parser.SetDelimiters(",");
			//文末まで読み込む
			while (!parser.EndOfData)
			{
				for (int i = 0; i < 3; i++)
				{
					string[] row = parser.ReadFields();
					for (int j = 0; j < 2; j++)
					{
						csvstr[i, j] = row[j];
					}
				}
			}
			parser.Close();
			#endregion
		}

		/// <summary>
		/// 描画処理
		/// </summary>
		/// <param name="spriteBatch"></param>
		/// <param name="graphicsDevice"></param>
		public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
		{
			//描画開始
			spriteBatch.Begin();

			//背景描画
			spriteBatch.Draw(background, Vector2.Zero, Color.White);
			//ランキング文字
			switch (selectStage)
			{
				case StageSelect.SelectStage.Small:
					spriteBatch.Draw(rankingS, new Vector2(DrawCenterX(rankingS, background), 0f), Color.White);
					break;
				case StageSelect.SelectStage.Regular:
					spriteBatch.Draw(rankingR, new Vector2(DrawCenterX(rankingS, background), 0f), Color.White);
					break;
				case StageSelect.SelectStage.Big:
					spriteBatch.Draw(rankingB, new Vector2(DrawCenterX(rankingS, background), 0f), Color.White);
					break;
			}
			//ランキングバー
			spriteBatch.Draw(rankbar1, new Vector2(0f, textureheight + offsetheight), Color.White);
			spriteBatch.Draw(rankbar2, new Vector2(0f, textureheight * 2 + offsetheight), Color.White);
			spriteBatch.Draw(rankbar3, new Vector2(0f, textureheight * 3 + offsetheight), Color.White);
			spriteBatch.Draw(rankbarnow, new Vector2(0f, textureheight * 4 + offsetheight * 3), Color.White);

			//文字
			spriteBatch.Draw(name, new Vector2(DrawCenterX(name, background) - offsetwidth, textureheight), Color.White);
			spriteBatch.Draw(time, new Vector2(DrawCenterX(name, background) + offsetwidth * 1.5f, textureheight), Color.White);
			//しんきろく文字描画　条件式次第で表示するように変更
			if (NewRecordFlag)
				spriteBatch.Draw(newrecord, new Vector2(DrawCenterX(newrecord, background) - offsetwidth, textureheight * 4 + offsetheight * 4f), Color.White);

			//フォント描画
			for (int i = 0; i < 3; i++)
			{
				spriteBatch.DrawString(font, csvstr[i, 0], new Vector2(DrawCenterX(name, background) - offsetwidth * 1.5f, textureheight * (i + 1) + offsetheight * 2.5f), Color.Black);
				spriteBatch.DrawString(fontmath, String.Format("{0:D2} : {1:D2}", TimetoMinutes(int.Parse(csvstr[i, 1])), TimetoSeconds(int.Parse(csvstr[i, 1]))),
					new Vector2(DrawCenterX(name, background) + offsetwidth * 2f, textureheight * (i + 1) + offsetheight * 2.5f), Color.Black);
			}

			//クリア時間描画
			if (DrawFlag)
			{
				spriteBatch.DrawString(fontmath, String.Format("{0:D2} : {1:D2}", totalTime.Minutes, totalTime.Seconds),
					new Vector2(DrawCenterX(name, background) + offsetwidth * 2f, textureheight * 4 + offsetheight * 4.5f), Color.Black);
			}

			//描画終了
			spriteBatch.End();

			//フェードアウト描画
			fadeout.Draw(spriteBatch);
		}

		/// <summary>
		/// 更新処理
		/// </summary>
		/// <returns></returns>
		public SceneInterface Update(GameTime gameTime)
		{
			if (Input.Instance.PushKey(Keys.Space))
			{
				SoundManager.Instance.SoundBank.PlayCue("ok");
				fadeout.StartFadeOut();
			}

			if (fadeout.EndFadeOut)
			{
				if (NewRecordFlag)
				{
					cueBGM.Stop(AudioStopOptions.AsAuthored);
					return new KeyBoardInput(content, graphicsDevice, newRank, TimeToString(totalTime), selectStage);
				}
				else
				{
					cueBGM.Stop(AudioStopOptions.AsAuthored);
					return new Title(content, graphicsDevice);
				}
			}

			//フェードアウトの更新
			fadeout.Update();

			return this;
		}

		/// <summary>
		/// 終了処理
		/// </summary>
		public void Unload()
		{
		}

		/// <summary>
		/// 背景テクスチャの中央にテクスチャを貼るための中央座標
		/// </summary>
		/// <param name="texturemain">目的のテクスチャ</param>
		/// <param name="textureback">背景のテクスチャ</param>
		/// <returns></returns>
		private float DrawCenterX(Texture2D texturemain, Texture2D textureback)
		{
			float ansx = textureback.Bounds.Width / 2 - texturemain.Bounds.Width / 2;
			return ansx;
		}

		/// <summary>
		/// double型の時間からstrin型の時間に変換する
		/// </summary>
		/// <param name="timer"></param>
		/// <returns></returns>
		private string TimeToString(int timer)
		{
			return TimetoMinutes(timer).ToString() + ":" + TimetoSeconds(timer).ToString() + ":" + TimeToMilliSeconds(timer).ToString();

		}

		/// <summary>
		/// クリア時間の分の部分を取得するメソッド
		/// </summary>
		/// <param name="timer"></param>
		/// <returns></returns>
		private int TimetoMinutes(int timer)
		{
			return timer / 10000;
		}

		/// <summary>
		/// クリア時間の秒の単位を取得するメソッド
		/// </summary>
		/// <param name="timer"></param>
		/// <returns></returns>
		private int TimetoSeconds(int timer)
		{
			return (timer % 10000) / 100;
		}

		/// <summary>
		/// クリア時間のミリ秒の単位を取得するメソッド
		/// </summary>
		/// <param name="timer"></param>
		/// <returns></returns>
		private int TimeToMilliSeconds(int timer)
		{
			return (timer % 10000) % 100;
		}

		/// <summary>
		/// TimeSpan型の時間をランキングで仕様するstring型に変換するメソッド
		/// </summary>
		/// <param name="totalTime"></param>
		/// <returns></returns>
		private string TimeToString(TimeSpan totalTime)
		{
			string tempsec, tempmin, tempmil, temp;

			if ((totalTime.Minutes / 10) == 0)
				tempmin = "0" + totalTime.Minutes.ToString();
			else
				tempmin = totalTime.Minutes.ToString();

			if ((totalTime.Seconds / 10) == 0)
				tempsec = "0" + totalTime.Seconds.ToString();
			else
				tempsec = totalTime.Seconds.ToString();

			if ((totalTime.Milliseconds / 100) == 0)
				tempmil = "0" + (totalTime.Milliseconds / 10).ToString();
			else
				tempmil = (totalTime.Milliseconds / 10).ToString();

			temp = tempmin + tempsec + tempmil;
			return temp;
		}

		/// <summary>
		/// ゲームクリア時間がランキングを更新するか判定するメソッド
		/// </summary>
		/// <param name="totalTime"></param>
		/// <param name="csvfile"></param>
		/// <returns></returns>
		private int checkRancking(TimeSpan totalTime, string[,] csvfile)
		{
			//ランキングを1位から順探索
			for (int i = 0; i < 3; i++)
			{
				if (totalTime.Minutes > TimetoMinutes(int.Parse(csvstr[i, 1])))
					return i;
				else if (totalTime.Minutes == TimetoMinutes(int.Parse(csvstr[i, 1])))
				{
					if (totalTime.Seconds > TimetoSeconds(int.Parse(csvstr[i, 1])))
						return i;
					else if (totalTime.Seconds == TimetoSeconds(int.Parse(csvstr[i, 1])))
						if (totalTime.Seconds > TimetoSeconds(int.Parse(csvstr[i, 1])))
							return i;
						else if (totalTime.Seconds > TimetoSeconds(int.Parse(csvstr[i, 1])))
							return i + 1;
				}
			}
			return -1;
		}
	}
}
