using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MazeEscaper.Scene
{
    /// <summary>
    /// シーンインターフェース
    /// </summary>
    public interface SceneInterface
    {
		/// <summary>
		/// 更新
		/// </summary>
		/// <returns>次シーン</returns>
		SceneInterface Update(GameTime gameTime);

		/// <summary>
		/// 描画
		/// </summary>
		/// <param name="spriteBatch">スプライトバッチ</param>
		/// <param name="graphicsDevice">グラフィックデバイス</param>
		void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice);

		/// <summary>
		/// ゲーム終了処理
		/// </summary>
		void Unload();
    }
}
