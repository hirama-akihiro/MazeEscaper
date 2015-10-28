using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace MazeEscaper.Scene
{
	/// <summary>
	/// ゲーム情報のインターフェース
	/// </summary>
	class Infomation : SceneInterface
	{
		//シーンインターフェース共通変数
		private ContentManager content;
		private GraphicsDevice graphicsDevice;

		#region テクスチャ
		private Texture2D background;
		#endregion

		public Infomation(ContentManager content, GraphicsDevice graphicsDevice)
		{
			this.content = content;
			this.graphicsDevice = graphicsDevice;

			LoadContent();
		}

		private void LoadContent()
		{
			background = content.Load<Texture2D>("Infomation/background");
		}

		public SceneInterface Update(GameTime gameTime)
		{
			if (Input.Instance.PushKey(Keys.Space))
			{
				SoundManager.Instance.SoundBank.PlayCue("ok");
				return new Manual(content, graphicsDevice);
			}
			else
				return this;
		}

		public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
		{
			spriteBatch.Begin();
			spriteBatch.Draw(background, Vector2.Zero, Color.White);
			spriteBatch.End();
		}

		public void Unload()
		{
			throw new NotImplementedException();
		}
	}
}
