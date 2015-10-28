using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MazeEscaper.Collidable;
using Microsoft.Xna.Framework;

namespace MazeEscaper
{

	class HUD
	{
		private ContentManager content;
		private GraphicsDevice graphicsDevice;
		private int stock;

		/// <summary>
		/// スプライトでテキストを描画するためのフォント
		/// </summary>
		private SpriteFont font;

		public HUD(ContentManager content, GraphicsDevice graphicsDevice)
		{
			this.content = content;
			this.graphicsDevice = graphicsDevice;
			LoadContent();
		}

		/// <summary>
		/// コンテンツのロードメソッド
		/// </summary>
		public void LoadContent()
		{
			this.font = this.content.Load<SpriteFont>("Font");
		}

		internal void Draw(SpriteBatch spriteBatch)
		{
			// スプライトの描画準備
			spriteBatch.Begin();

			// テキストをスプライトとして描画する
			spriteBatch.DrawString(this.font, "" + stock, Vector2.Zero, Color.White);

			// スプライトの一括描画
			spriteBatch.End();
		}

		internal void Update(Player player)
		{
			stock = player.stock;
		}
	}
}
