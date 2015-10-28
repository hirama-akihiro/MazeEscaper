using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.VisualBasic.FileIO;

namespace MazeEscaper.Scene
{
	class KeyBoardInput : SceneInterface
	{
		//シーンインターフェース共通変数
		private ContentManager content;
		private GraphicsDevice graphicsDevice;

		/// <summary>
		/// キー入力された名前
		/// </summary>
		private string nametext;

		/// <summary>
		/// 最大テキスト文字数
		/// </summary>
		private const int maxtextsize = 6;

		private int textsize = 0;

		/// <summary>
		/// キーボド文字配列
		/// </summary>
		private string[,] keyboardstr = 
		{
			{"あ","い","う","え","お","は","ひ","ふ","へ","ほ","+"},
			{"か","き","く","け","こ","ま","み","む","め","も","+"},
			{"さ","し","す","せ","そ","や","ゆ","よ","゛","゜","+"},
			{"た","ち","つ","て","と","ら","り","る","れ","ろ","-"},
			{"な","に","ぬ","ね","の","わ","を","ん","、","。","-"},
			{"ぁ","ぃ","ぅ","ぇ","ぉ","っ","ゃ","ゅ","ょ","ー","-"}
		};

		/// <summary>
		/// 濁点キーボード文字配列
		/// </summary>
		private string[,] keyboardstrD =
		{
			{"あ","い","う","え","お","ば","び","ぶ","べ","ぼ","+"},
			{"が","ぎ","ぐ","げ","ご","ま","み","む","め","も","+"},
			{"ざ","じ","ず","ぜ","ぞ","や","ゆ","よ","゛","゜","+"},
			{"だ","ぢ","づ","で","ど","ら","り","る","れ","ろ","-"},
			{"な","に","ぬ","ね","の","わ","を","ん","、","。","-"},
			{"ぁ","ぃ","ぅ","ぇ","ぉ","っ","ゃ","ゅ","ょ","ー","-"}
		};

		/// <summary>
		/// 半濁点キーボード文字配列
		/// </summary>
		private string[,] keyboardstrH =
		{
			{"あ","い","う","え","お","ぱ","ぴ","ぷ","ぺ","ぽ","+"},
			{"か","き","く","け","こ","ま","み","む","め","も","+"},
			{"さ","し","す","せ","そ","や","ゆ","よ","゛","゜","+"},
			{"た","ち","つ","て","と","ら","り","る","れ","ろ","-"},
			{"な","に","ぬ","ね","の","わ","を","ん","、","。","-"},
			{"ぁ","ぃ","ぅ","ぇ","ぉ","っ","ゃ","ゅ","ょ","ー","-"}
		};

		//ランキングのCSVファイル
		private string[,] csvstr = new string[3, 2];

		#region テクスチャ
		//キー入力画面
		private Texture2D background;
		private Texture2D keybackground;
		private Texture2D keybutton;
		private Texture2D choicekeybutton;
		private Texture2D keybutton2;
		private Texture2D choicekeybutton2;
		//名前確認画面
		private Texture2D confbackground;
		private Texture2D confbutton;
		private Texture2D confbutton2;
		#endregion

		private SpriteFont keyfont;
		private SpriteFont namefont;

        private Vector2 buttonoffset = new Vector2(50, 250);
        private Vector2 stroffset = new Vector2(65, 258);
		private Vector2 nameoffset = new Vector2(60, 130);

		/// <summary>
		/// 選択中の文字に対応する配列番号
		/// </summary>
		private Point mousecursor = new Point(0, 0);

		/// <summary>
		/// 入力された文字のキー配列番号
		/// </summary>
		private Point[] namecursor = new Point[6];

		/// <summary>
		/// 名前を決定したかどうかを判定するフラグ
		/// </summary>
		private bool DecideFlag { get; set; }

		/// <summary>
		/// 名前最終確認フラグ
		/// </summary>
		private bool FinalDecideFlag { get; set; }

		//更新するランキング順位
		int rankNumber;
		//更新された記録時間
		string recordtime;

		/// <summary>
		/// ステージサイズ
		/// </summary>
		private StageSelect.SelectStage selectStage;




		/// <summary>
		/// キー入力を行いランキング用CSVを更新するクラス
		/// </summary>
		/// <param name="content"></param>
		/// <param name="graphicsDevice"></param>
		/// <param name="ranknumber">更新されるランキング順位</param>
		/// <param name="scoretime">スコアタイム</param>
		/// <param name="csvfime">CSVファイルの中身</param>
		/// <param name="selectStage"></param>
		public KeyBoardInput(ContentManager content, GraphicsDevice graphicsDevice, int ranknumber, string recordtime, StageSelect.SelectStage selectStage)
		{
			this.content = content;
			this.graphicsDevice = graphicsDevice;

			this.rankNumber = ranknumber;
			this.recordtime = recordtime;
			this.selectStage = selectStage;

			LoadContent();

			DecideFlag = false;
			FinalDecideFlag = false;
			nametext = "";
		}

		/// <summary>
		/// コンテンツ読み込み
		/// </summary>
		private void LoadContent()
		{
			//キー入力画面
			background = content.Load<Texture2D>(@"KeyBoardInput\background");
			keybackground = content.Load<Texture2D>(@"KeyBoardInput\keybackground");
			keybutton = content.Load<Texture2D>(@"KeyBoardInput\keybutton");
			choicekeybutton = content.Load<Texture2D>(@"KeyBoardInput\choicekeybutton");
			keybutton2 = content.Load<Texture2D>(@"KeyBoardInput\keybutton2");
			choicekeybutton2 = content.Load<Texture2D>(@"KeyBoardInput\choicekeybutton2");
			//確認画面
			confbackground = content.Load<Texture2D>(@"KeyBoardInput\confbackground");
			confbutton = content.Load<Texture2D>(@"KeyBoardInput\confbutton");
			confbutton2 = content.Load<Texture2D>(@"KeyBoardInput\confbutton2");

			keyfont = content.Load<SpriteFont>(@"KeyBoardInput\KeyBoardFont");
			namefont = content.Load<SpriteFont>(@"KeyBoardInput\NameFont");

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
		/// 更新
		/// </summary>
		/// <returns>次シーン</returns>
		public SceneInterface Update(GameTime gameTime)
		{
			#region 名前確認画面操作
			if (DecideFlag)
			{
				if (Input.Instance.PushKey(Keys.Space))
				{
					if (FinalDecideFlag)
					{
						SoundManager.Instance.SoundBank.PlayCue("ok");
						//名前入力が行われない場合の処理
						if (nametext.Length == 0)
							nametext = "ななしさん";
						//CSVファイルの書き込み
						CSVWriter(rankNumber, csvstr, nametext, recordtime);
						return new Ranking(content, graphicsDevice, selectStage);
					}
					else
					{
						SoundManager.Instance.SoundBank.PlayCue("ok");
						DecideFlag = false;
					}
				}
				if (Input.Instance.PushKey(Keys.Left))
					if (!FinalDecideFlag)
					{
						SoundManager.Instance.SoundBank.PlayCue("select");
						FinalDecideFlag = true;
					}
				if (Input.Instance.PushKey(Keys.Right))
					if (FinalDecideFlag)
					{
						SoundManager.Instance.SoundBank.PlayCue("select");
						FinalDecideFlag = false;
					}
				return this;
			}
			#endregion

			#region キー入力画面操作
			//配列ないでのみ移動可能
			if (Input.Instance.PushKey(Keys.Up))
			{
				//もどる、おわりボタンの設定
				if (mousecursor.X == keyboardstr.GetLength(1) - 1)
				{
					switch (mousecursor.Y / 3)
					{
						case 0:
							break;
						case 1:
							SoundManager.Instance.SoundBank.PlayCue("select");
							mousecursor.Y -= 3;
							break;
					}
				}
				else if(mousecursor.Y > 0)
				{
					SoundManager.Instance.SoundBank.PlayCue("select");
					//通常ボタンの設定
					mousecursor.Y--;
				}
			}
			if (Input.Instance.PushKey(Keys.Down))
			{
				//もどる、おわりボタンの設定
				if (mousecursor.X == keyboardstr.GetLength(1) - 1)
				{
					switch (mousecursor.Y / 3)
					{
						case 0:
							SoundManager.Instance.SoundBank.PlayCue("select");
							mousecursor.Y += 3;
							break;
						case 1:
							break;
					}
				}
				else if (mousecursor.Y < keyboardstr.GetLength(0) - 1)
				{
					SoundManager.Instance.SoundBank.PlayCue("select");
					//通常ボタンの設定
					mousecursor.Y++;
				}
			}
			if (Input.Instance.PushKey(Keys.Left))
			{
				if (mousecursor.X > 0)
				{
					SoundManager.Instance.SoundBank.PlayCue("select");
					mousecursor.X--;
				}
			}
			if (Input.Instance.PushKey(Keys.Right))
			{
				if (mousecursor.X < keyboardstr.GetLength(1) - 1)
				{
					SoundManager.Instance.SoundBank.PlayCue("select");
					mousecursor.X++;
				}
			}

			//文字入力,最大文字数6文字
			if (Input.Instance.PushKey(Keys.Space))
			{
				//濁点処理
				if (keyboardstr[mousecursor.Y, mousecursor.X] == "゛")
				{
					if (textsize > 0)
					{
						//一つ前の文字に濁点文字が存在するならば
						if (keyboardstr[namecursor[textsize - 1].Y, namecursor[textsize - 1].X] != keyboardstrD[namecursor[textsize - 1].Y, namecursor[textsize - 1].X])
						{
							SoundManager.Instance.SoundBank.PlayCue("ok");
							nametext = nametext.Remove(textsize - 1, 1);
							nametext += keyboardstrD[namecursor[textsize - 1].Y, namecursor[textsize - 1].X];
						}
					}
				}
				//半濁点処理
				else if (keyboardstr[mousecursor.Y, mousecursor.X] == "゜")
				{
					if (textsize > 0)
					{
						//一つ前の文字に半濁点文字が存在するならば
						if (keyboardstr[namecursor[textsize - 1].Y, namecursor[textsize - 1].X] != keyboardstrH[namecursor[textsize - 1].Y, namecursor[textsize - 1].X])
						{
							SoundManager.Instance.SoundBank.PlayCue("ok");
							nametext = nametext.Remove(textsize - 1, 1);
							nametext += keyboardstrH[namecursor[textsize - 1].Y, namecursor[textsize - 1].X];
						}
					}
				}
				//もどるボタン処理
				else if (keyboardstr[mousecursor.Y, mousecursor.X] == "+")
				{
					//文字削除
					if (textsize > 0)
					{
						SoundManager.Instance.SoundBank.PlayCue("ok");
						textsize--;
						nametext = nametext.Remove(textsize, 1);
					}
				}
				//名前入力完了処理
				else if (keyboardstr[mousecursor.Y, mousecursor.X] == "-")
				{
					SoundManager.Instance.SoundBank.PlayCue("ok");
					DecideFlag = true;
				}
				//通常のボタン入力
				else if (textsize < maxtextsize)
				{
					SoundManager.Instance.SoundBank.PlayCue("ok");
					nametext += keyboardstr[mousecursor.Y, mousecursor.X];
					namecursor[textsize] = mousecursor;
					textsize++;
				}
			}
			//文字削除
			if (Input.Instance.PushKey(Keys.B) && textsize > 0)
			{
				SoundManager.Instance.SoundBank.PlayCue("ok");
				textsize--;
				nametext = nametext.Remove(textsize, 1);
			}
			#endregion

			return this;
		}

		/// <summary>
		/// 描画
		/// </summary>
		/// <param name="spriteBatch">スプライトバッチ</param>
		/// <param name="graphicsDevice">グラフィックデバイス</param>
		public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
		{
			//描画開始
			spriteBatch.Begin();

			#region キー入力画面描画
			//背景描画
			spriteBatch.Draw(background, Vector2.Zero, Color.White);
			spriteBatch.Draw(keybackground, Vector2.Zero, Color.White);

			//キーのボタン描画
			for(int i= 0;i< keyboardstr.GetLength(0);i++)
			{
				//あ～な行
				for (int j = 0; j < 5; j++)
				{
					spriteBatch.Draw(keybutton, buttonoffset + new Vector2(keybutton.Bounds.Width * j, keybutton.Bounds.Height * i), Color.White);
					spriteBatch.DrawString(keyfont, keyboardstr[i, j], stroffset + new Vector2(keybutton.Bounds.Width * j, keybutton.Bounds.Height * i), Color.White);
				}
				//は～わ行
				for (int j = 5; j < keyboardstr.GetLength(1) - 1; j++)
				{
					spriteBatch.Draw(keybutton, buttonoffset + new Vector2(keybutton.Bounds.Width * j + 5, keybutton.Bounds.Height * i), Color.White);
					spriteBatch.DrawString(keyfont, keyboardstr[i, j], stroffset + new Vector2(keybutton.Bounds.Width * j + 5, keybutton.Bounds.Height * i), Color.White);
				}
			}
			//もどる、おわりボタンの描画
			spriteBatch.Draw(keybutton2, buttonoffset + new Vector2(keybutton.Bounds.Width * 10 + 5, keybutton.Bounds.Height * 0), Color.White);
			spriteBatch.DrawString(keyfont, "もどる", buttonoffset + new Vector2(keybutton.Bounds.Width * 10 + 18, keybutton.Bounds.Height * 1.5f), Color.White);
			spriteBatch.Draw(keybutton2, buttonoffset + new Vector2(keybutton.Bounds.Width * 10 + 5, keybutton.Bounds.Height * 3), Color.White);
			spriteBatch.DrawString(keyfont, "おわり", buttonoffset + new Vector2(keybutton.Bounds.Width * 10 + 18, keybutton.Bounds.Height * 4.2f), Color.White);



			//現在選択中のボタンを再描画
			if (mousecursor.X < 5)
			{
				spriteBatch.Draw(choicekeybutton, buttonoffset + new Vector2(keybutton.Bounds.Width * mousecursor.X, keybutton.Bounds.Height * mousecursor.Y), Color.White);
				spriteBatch.DrawString(keyfont, keyboardstr[mousecursor.Y, mousecursor.X], stroffset + new Vector2(keybutton.Bounds.Width * mousecursor.X, keybutton.Bounds.Height * mousecursor.Y), Color.Black);
			}
			else if (mousecursor.X < 10)
			{
				spriteBatch.Draw(choicekeybutton, buttonoffset + new Vector2(keybutton.Bounds.Width * mousecursor.X + 5, keybutton.Bounds.Height * mousecursor.Y), Color.White);
				spriteBatch.DrawString(keyfont, keyboardstr[mousecursor.Y, mousecursor.X], stroffset + new Vector2(keybutton.Bounds.Width * mousecursor.X + 5, keybutton.Bounds.Height * mousecursor.Y), Color.Black);
			}
			else
			{
				switch (mousecursor.Y / 3)
				{
					//もどる。おわりボタン
					case 0:
						spriteBatch.Draw(choicekeybutton2, buttonoffset + new Vector2(keybutton.Bounds.Width * 10 + 5, keybutton.Bounds.Height * 0), Color.White);
						spriteBatch.DrawString(keyfont, "もどる", buttonoffset + new Vector2(keybutton.Bounds.Width * 10 + 18, keybutton.Bounds.Height * 1.5f), Color.Black);
						break;
					case 1:
						spriteBatch.Draw(choicekeybutton2, buttonoffset + new Vector2(keybutton.Bounds.Width * 10 + 5, keybutton.Bounds.Height * 3), Color.White);
						spriteBatch.DrawString(keyfont, "おわり", buttonoffset + new Vector2(keybutton.Bounds.Width * 10 + 18, keybutton.Bounds.Height * 4.2f), Color.Black);
						break;
				}
			}

			//名前描画
			spriteBatch.DrawString(namefont, nametext, nameoffset, Color.Black);
			#endregion

			#region 名前確認画面描画
			if (DecideFlag)
			{
				//背景描画
				spriteBatch.Draw(background, Vector2.Zero, new Color(0, 0, 0, 0.7f));
				spriteBatch.Draw(confbackground, new Vector2(200, 150), Color.White);

				//名前描画
				if (textsize == 0)
					spriteBatch.DrawString(namefont, "ななしさん", new Vector2(290, 270), Color.Black);
				else
					spriteBatch.DrawString(namefont, nametext, new Vector2(290, 270), Color.Black);	

				//けってい、もどるボタン描画
				if (FinalDecideFlag)
				{
					spriteBatch.Draw(confbutton2, new Vector2(260, 340), Color.White);
					spriteBatch.Draw(confbutton, new Vector2(410, 340), Color.White);
					spriteBatch.DrawString(keyfont, "けってい", new Vector2(270, 360), Color.Black);
					spriteBatch.DrawString(keyfont, "もどる", new Vector2(435, 360), Color.White);

				}
				else
				{
					spriteBatch.Draw(confbutton, new Vector2(260, 340), Color.White);
					spriteBatch.Draw(confbutton2, new Vector2(410, 340), Color.White);
					spriteBatch.DrawString(keyfont, "けってい", new Vector2(270, 360), Color.White);
					spriteBatch.DrawString(keyfont, "もどる", new Vector2(435, 360), Color.Black);
				}
			}
			#endregion

			//描画終了
			spriteBatch.End();
		}

		/// <summary>
		/// ゲーム終了処理
		/// </summary>
		public void Unload()
		{
		}

		/// <summary>
		/// CSVファイルの書き込み
		/// </summary>
		/// <param name="number"></param>
		/// <param name="csvfile"></param>
		private void CSVWriter(int number, string[,] csvfile, string uptext, string recordtime)
		{
			//CSVファイルのエンコーディング設定
			System.Text.Encoding enc = System.Text.Encoding.GetEncoding("Shift_JIS");
			//書き込むファイルを開く
			System.IO.StreamWriter sr;
			switch (selectStage)
			{
				case StageSelect.SelectStage.Small:
					sr = new System.IO.StreamWriter(@"../../../../MazeEscaperContent/Ranking/RankerS.csv", false, enc);
					break;
				case StageSelect.SelectStage.Regular:
					sr = new System.IO.StreamWriter(@"../../../../MazeEscaperContent/Ranking/RankerR.csv", false, enc);
					break;
				case StageSelect.SelectStage.Big:
					sr = new System.IO.StreamWriter(@"../../../../MazeEscaperContent/Ranking/RankerB.csv", false, enc);
					break;
				default:
					sr = new System.IO.StreamWriter(@"../../../../MazeEscaperContent/Ranking/Ranker.csv", false, enc);
					break;
			}

			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 2; j++)
				{
					if (number == i && j == 0)
					{
						sr.Write(uptext);
						sr.Write(',');
						continue;
					}

					if (number == i && j == 1)
					{
						sr.Write(recordtime);
						continue;
					}

					if (number < i)
						sr.Write(csvfile[i - 1, j]);
					else
						sr.Write(csvfile[i, j]);

					if (j == 0)
						sr.Write(',');
				}
				sr.Write("\r\n");
			}
			sr.Close();
		}

		/// <summary>
		/// 背景テクスチャの中央にテクスチャを貼るための中央座標
		/// </summary>
		/// <param name="texturemain">目的のテクスチャ</param>
		/// <param name="textureback">背景のテクスチャ</param>
		/// <returns></returns>
		public float DrawCenterX(Texture2D texturemain, Texture2D textureback)
		{
			float ansx = textureback.Bounds.Width / 2 - texturemain.Bounds.Width / 2;
			return ansx;
		}
	}
}
