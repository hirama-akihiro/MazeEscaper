using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using MazeEscaper.MazeMap;
using Microsoft.Xna.Framework.Graphics;

namespace MazeEscaper.Collidable
{
	/// <summary>
	/// 背景オブジェクトクラス
	/// </summary>
	class BackGround
	{
		private Model model;

		public BackGround(ContentManager content)
		{
			model = content.Load<Model>(@"Objects\BackGround\BackGround");
		}

		public void Draw(Camera camera)
		{
			//座標変換の初期化
			Matrix world = Matrix.Identity;

			model.Draw(world, camera.View, camera.Projection);
		}
	}
}
