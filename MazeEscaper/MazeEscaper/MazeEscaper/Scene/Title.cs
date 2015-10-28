using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace MazeEscaper.Scene
{
    /// <summary>
    /// タイトル画面のシーンインターフェース
    /// </summary>
    class Title:SceneInterface
    {
        //シーンインターフェース共通変数
        private ContentManager content;
        private GraphicsDevice graphicsDevice;

        /// <summary>
        /// タイトル文字
        /// </summary>
        private Texture2D titleStr;
        /// <summary>
        /// タイトル画面の背景
        /// </summary>
        private Texture2D background;
		/// <summary>
		/// ひよこ
		/// </summary>
		private Texture2D[] chickAnimation;

		private Texture2D selectLine;
		private Texture2D play;
		private Texture2D ranking;
		private Texture2D itemBox;
		private Texture2D selectItem;
		private Texture2D big;
		private Texture2D regular;
		private Texture2D small;

		/// <summary>
		/// クレジット
		/// </summary>
		private Texture2D credit;

		/// <summary>
		/// アニメーションのカウンタ
		/// </summary>
		private int animationCounter;

        /// <summary>
        /// タイトル画面のスケール
        /// </summary>
        private Vector2 backScale;

        private Vector2 titleScale;
        private Vector2 titleOffset;

		private Vector2 playOffset;
		private Vector2 rankingOffset;
		private Vector2 itemOffset;
		private Vector2 targetPosition;

		private Vector2 creditOffset1;
		private Vector2 creditOffset2;

        private Vector2 chickScale;
		private Vector2 chickOffset1;
		private Vector2 chickOffset2;
		private float chickSpeed;

        // フェードアウト
        private float m_alpha;
        private float m_alphaIncAmout;
        private bool m_isFadeOut = false;
        private Rectangle screenBound;
        private Color color;

		private Select select;
		private StageSelect.SelectStage selectRanking;

		private Cue cueBGM;

		public enum Select { Play, Ranking }

        /// <summary>
        /// タイトル画面シーン
        /// </summary>
        /// <param name="content"></param>
        /// <param name="graphicsDevice"></param>
        public Title(ContentManager content, GraphicsDevice graphicsDevice)
        {
            this.content = content;
            this.graphicsDevice = graphicsDevice;

            //コンテンツ読み込み
            LoadContent();

			animationCounter = 0;

            //スケールの設定
            backScale = new Vector2((float)GameMain.ScreenWidth / background.Width, (float)GameMain.ScreenHeight / background.Height);
			titleScale = Vector2.One;
			titleOffset = new Vector2(GameMain.ScreenWidth / 2.0f, 120);

			playOffset = new Vector2(GameMain.ScreenWidth / 2.0f, 260);
			rankingOffset = new Vector2(GameMain.ScreenWidth / 2.0f, 370);
			itemOffset = new Vector2(GameMain.ScreenWidth / 2.0f, 450);
			targetPosition = new Vector2(200, 0);
			
			chickScale = new Vector2(0.3f, 0.3f);
			chickOffset1 = new Vector2(800, 540);
			creditOffset1 = new Vector2(chickOffset1.X + 40 + credit.Width / 2.0f, chickOffset1.Y);
			chickOffset2 = new Vector2(creditOffset1.X + credit.Width / 2.0f + 60, chickOffset1.Y);
			creditOffset2 = new Vector2(chickOffset2.X + 40 + credit.Width / 2.0f, creditOffset1.Y);
			chickSpeed = 1.5f;

			//フェードアウトの初期化
            m_alpha = 0.0f;
            m_alphaIncAmout = 0.02f;
            //フェードアウト描画サイズ
            screenBound = new Rectangle(0, 0, GameMain.ScreenWidth, GameMain.ScreenHeight);
            //黒色でフェードアウト
            color = new Color(0.0f, 0.0f, 0.0f, m_alpha);

			select = Select.Play;
			selectRanking = StageSelect.SelectStage.Regular;

			cueBGM = SoundManager.Instance.SoundBank.GetCue("title");
			cueBGM.Play();
        }

        /// <summary>
        /// コンテンツのロードメソッド
        /// </summary>
		public void LoadContent()
		{
			//this.background = content.Load<Texture2D>("Title/TitleBack");
			background = content.Load<Texture2D>("Title/background");
			this.titleStr = content.Load<Texture2D>("Title/maze_escaper");

			selectLine = content.Load<Texture2D>("Title/select");
			play = content.Load<Texture2D>("Title/play");
			ranking = content.Load<Texture2D>("Title/ranking");
			itemBox = content.Load<Texture2D>("Title/item_box");
			selectItem = content.Load<Texture2D>("Title/select_item");
			big = content.Load<Texture2D>("Title/big");
			regular = content.Load<Texture2D>("Title/regular");
			small = content.Load<Texture2D>("Title/small");

			credit = content.Load<Texture2D>("Title/credit");

			chickAnimation = new Texture2D[60];
			foreach (var i in Enumerable.Range(0, chickAnimation.Length))
				chickAnimation[i] = content.Load<Texture2D>("Title/Chick/" + (i + 1).ToString().PadLeft(4, '0'));
		}


        /// <summary>
        /// タイトル画面の描画メソッド
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="graphicsDevice"></param>
		public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
		{
			spriteBatch.Begin();

			spriteBatch.Draw(background, Vector2.Zero, null, Color.White, 0.0f, Vector2.Zero, backScale, SpriteEffects.None, 0.0f);
			spriteBatch.Draw(titleStr, titleOffset - new Vector2(titleStr.Width, titleStr.Height) / 2, null, Color.White, 0.0f, Vector2.Zero, titleScale, SpriteEffects.None, 0.0f);

			switch (select)
			{
				case Select.Play:
					spriteBatch.Draw(selectLine, playOffset - new Vector2(selectLine.Width, selectLine.Height) / 2, Color.White);
					break;
				case Select.Ranking:
					spriteBatch.Draw(selectLine, rankingOffset - new Vector2(selectLine.Width, selectLine.Height) / 2, Color.White);
					break;
			}

			spriteBatch.Draw(play, playOffset - new Vector2(play.Width, play.Height) / 2, Color.White);
			spriteBatch.Draw(ranking, rankingOffset - new Vector2(ranking.Width, ranking.Height) / 2, Color.White);
			spriteBatch.Draw(itemBox, itemOffset - new Vector2(itemBox.Width, itemBox.Height) / 2, Color.White);
			spriteBatch.Draw(selectItem, itemOffset + new Vector2(targetPosition.X * ((int)selectRanking - 1), 0) - new Vector2(selectItem.Width, selectItem.Height) / 2, Color.White);
			spriteBatch.Draw(small, itemOffset - targetPosition - new Vector2(small.Width, small.Height) / 2, Color.White);
			spriteBatch.Draw(regular, itemOffset - new Vector2(regular.Width, regular.Height) / 2, Color.White);
			spriteBatch.Draw(big, itemOffset + targetPosition - new Vector2(big.Width, big.Height) / 2, Color.White);

			spriteBatch.Draw(credit, creditOffset1 - new Vector2(credit.Width, credit.Height) / 2, Color.White);
			spriteBatch.Draw(credit, creditOffset2 - new Vector2(credit.Width, credit.Height) / 2, Color.White);

			spriteBatch.Draw(chickAnimation[animationCounter], chickOffset1 - new Vector2(chickAnimation[animationCounter].Width * chickScale.X, chickAnimation[animationCounter].Height * chickScale.Y) / 2, null, Color.White, 0.0f, Vector2.Zero, chickScale, SpriteEffects.None, 0.0f);
			spriteBatch.Draw(chickAnimation[animationCounter], chickOffset2 - new Vector2(chickAnimation[animationCounter].Width * chickScale.X, chickAnimation[animationCounter].Height * chickScale.Y) / 2, null, Color.White, 0.0f, Vector2.Zero, chickScale, SpriteEffects.None, 0.0f);

			//フェードアウト画面描画
			if (m_isFadeOut)
			{
				color = new Color(0.0f, 0.0f, 0.0f, m_alpha);
				spriteBatch.Draw(background, screenBound, color);
				updateFadeOut();
			}

			spriteBatch.End();
		}

        /// <summary>
        /// タイトル画面の更新メソッド
        /// </summary>
		public SceneInterface Update(GameTime gameTime)
		{
			//フェードアウト後新しい画面に更新
			if (m_alpha == 1.0f)
				switch (select)
				{
					case Select.Play:
						//return new StageSelect(content, graphicsDevice);
						return new Infomation(content, graphicsDevice);
					case Select.Ranking:
						return new Ranking(content, graphicsDevice, selectRanking);
				}

			//フェードアウトの更新
			if (m_isFadeOut)
			{
				updateFadeOut();
				return this;
			}

			//スペースキー入力で次画面へ移行
			if (Input.Instance.PushKey(Keys.Space))
			{
				SoundManager.Instance.SoundBank.PlayCue("ok");
				cueBGM.Stop(AudioStopOptions.AsAuthored);
				m_isFadeOut = true;
			}

			// 一つ前の状態を保存
			Select prevSelect = select;
			StageSelect.SelectStage prevSelectRanking = selectRanking;
			// キー入力
			if (Input.Instance.PushKey(Keys.Up))
				select--;
			if (Input.Instance.PushKey(Keys.Down))
				select++;
			if (select < 0)
				select = 0;
			if ((int)select >= Enum.GetNames(typeof(Select)).Length)
				select = (Select)Enum.GetNames(typeof(Select)).Length - 1;

			if (select == Select.Ranking)
			{
				if (Input.Instance.PushKey(Keys.Left))
					selectRanking--;
				if (Input.Instance.PushKey(Keys.Right))
					selectRanking++;
				if (selectRanking < 0)
					selectRanking = 0;
				if ((int)selectRanking >= Enum.GetNames(typeof(StageSelect.SelectStage)).Length)
					selectRanking = (StageSelect.SelectStage)Enum.GetNames(typeof(StageSelect.SelectStage)).Length - 1;
			}
			// 前の状態と変わっていたら音を鳴らす
			if (select != prevSelect || selectRanking != prevSelectRanking)
				SoundManager.Instance.SoundBank.PlayCue("select");

			// アニメーションの更新
			animationCounter++;
			animationCounter %= chickAnimation.Length;
			//chickOffset1.X -= chickSpeed;
			//if (chickOffset1.X < -100)
			//    chickOffset1.X = GameMain.ScreenWidth * 2 - 100;
			//chickOffset2.X -= chickSpeed;
			//if (chickOffset2.X < -100)
			//    chickOffset2.X = GameMain.ScreenWidth * 2 - 100;

			//creditOffset1.X -= chickSpeed;
			//if (creditOffset1.X < -credit.Width / 2.0)
			//    creditOffset1.X = GameMain.ScreenWidth * 2 - 100 + credit.Width / 2.0f;
			//creditOffset2.X -= chickSpeed;
			//if (creditOffset2.X < -credit.Width / 2.0)
			//    creditOffset2.X = GameMain.ScreenWidth * 2 - 100 + credit.Width / 2.0f;
			chickOffset1.X -= chickSpeed;
			if (chickOffset1.X < -100)
				chickOffset1.X = creditOffset2.X + credit.Width / 2.0f + 60;
			creditOffset1.X -= chickSpeed;
			if (creditOffset1.X < -credit.Width / 2.0)
				creditOffset1.X = chickOffset1.X + 40 + credit.Width / 2.0f;
			chickOffset2.X -= chickSpeed;
			if (chickOffset2.X < -100)
				chickOffset2.X = creditOffset1.X + credit.Width / 2.0f + 60;
			creditOffset2.X -= chickSpeed;
			if (creditOffset2.X < -credit.Width / 2.0)
				creditOffset2.X = chickOffset2.X + 40 + credit.Width / 2.0f;
		
			return this;
		}

        /// <summary>
        /// 終了処理
        /// </summary>
        public void Unload()
        {
        }

        /// <summary>
        /// フェードアウト処理
        /// </summary>
        private void updateFadeOut()
        {
            m_alpha += m_alphaIncAmout;
            if (m_alpha >= 1.0f)
            {
                m_alpha = 1.0f;
            }
        }
    }
}
