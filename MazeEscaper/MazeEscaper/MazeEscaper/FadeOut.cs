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
	class FadeOut
	{
		private ContentManager content;
		private GraphicsDevice graphicsDevice;

		private Texture2D background;
		private Texture2D gameOver;
		private Texture2D gameClear;

		// フェードアウト
		private float m_alpha;
		private float m_alphaIncAmout;

		/// <summary>
		/// フェードアウト処理が開始しているかどうか,True:開始中
		/// </summary>
		public bool m_isFadeOut = false;

		private Rectangle screenBound;
		private Color color;
		private float maxm_alphaIn = 0.7f;
		private float maxm_alphaOut = 1.0f;

		/// <summary>
		/// フェードアウト処理が終了したときTrue
		/// </summary>
		public bool EndFadeOut { get; set; }

		/// <summary>
		/// ゲームクリア:True,GameOver:False
		/// </summary>
		public bool ClearFlag { get; set; }

		/// <summary>
		/// フェードアウトが呼ばれるゲームの種類
		/// </summary>
		private SceneType scenetype;

		/// <summary>
		/// GameIn用のフェードアウトか否か
		/// </summary>
		public enum SceneType { InGame, OutGame }

		/// <summary>
		/// フェードアウト管理クラス
		/// </summary>
		/// <param name="conent"></param>
		/// <param name="graphicsDevice"></param>
		public FadeOut(ContentManager content,GraphicsDevice graphicsDevice,SceneType scenetype)
		{
			this.content = content;
			this.graphicsDevice = graphicsDevice;

			LoadContent();

			this.scenetype = scenetype;

			//フェードアウトの初期化
			m_alpha = 0.0f;

			switch (scenetype)
			{
				case SceneType.InGame:
					m_alphaIncAmout = 0.02f;
					break;
				case SceneType.OutGame:
					m_alphaIncAmout = 0.04f;
					break;
			}
			
			//フェードアウト描画サイズ
			screenBound = new Rectangle(0, 0, GameMain.ScreenWidth, GameMain.ScreenHeight);
			//黒色でフェードアウト
			color = new Color(0.0f, 0.0f, 0.0f, m_alpha);
			EndFadeOut = false;
		}

		/// <summary>
		/// コンテンツ読み込み
		/// </summary>
		private void LoadContent()
		{
			background = content.Load<Texture2D>("FadeOut/background");
			gameOver = content.Load<Texture2D>("FadeOut/GameOver");
			gameClear = content.Load<Texture2D>("FadeOut/GameClear");
		}

		internal void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Begin();
			//フェードアウト画面描画
			if (m_isFadeOut)
			{
				color = new Color(0.0f, 0.0f, 0.0f, m_alpha);
				spriteBatch.Draw(background, screenBound, color);

				switch (scenetype)
				{
					case SceneType.InGame:
						if (EndFadeOut && ClearFlag)
							spriteBatch.Draw(gameClear, Vector2.Zero, Color.White);
						else if (EndFadeOut && !ClearFlag)
							spriteBatch.Draw(gameOver, Vector2.Zero, Color.White);
						break;
					case SceneType.OutGame:
						break;
				}
			}
			spriteBatch.End();
		}

		internal void Update()
		{
			//フェードアウトの更新
			if (m_isFadeOut && !EndFadeOut)
			{
				updateFadeOut();
			}
		}

		/// <summary>
		/// ゲーム終了時に呼ばれるメソッド
		/// </summary>
		/// <param name="clearflag"></param>
		public void StartFadeOut(bool clearflag)
		{
			m_isFadeOut = true;
			ClearFlag = clearflag;
		}

		/// <summary>
		/// ゲーム終了時に呼ばれるメソッド
		/// </summary>
		public void StartFadeOut()
		{
			m_isFadeOut = true;
		}

		/// <summary>
		/// フェードアウト処理
		/// </summary>
		private void updateFadeOut()
		{
			m_alpha += m_alphaIncAmout;
			switch(scenetype)
			{
				case SceneType.InGame:
					if (m_alpha >= maxm_alphaIn)
					{
						m_alpha = maxm_alphaIn;
						EndFadeOut = true;
					}
					break;
				case SceneType.OutGame:
					if (m_alpha >= maxm_alphaOut)
					{
						m_alpha = maxm_alphaOut;
						EndFadeOut = true;
					}
					break;
			}
		}
	}
}
