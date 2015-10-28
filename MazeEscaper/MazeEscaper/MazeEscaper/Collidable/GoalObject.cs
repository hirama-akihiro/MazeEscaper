using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using MazeEscaper.MazeMap;

namespace MazeEscaper.Collidable
{
	/// <summary>
	/// ゴールオブジェクトクラス
	/// </summary>
	class GoalObject : Collidable
	{
		public GoalObject(ContentManager content, Point startPosition, Orientation orient, Maze maze)
			: base(content, @"Objects\Goal\flag", startPosition, orient, new Vector3(1.0f, 1.0f, 1.0f), maze)
		{
		}

		/// <summary>
		/// 何も移動しない
		/// </summary>
		/// <param name="maze"></param>
		public override void Update(GameTime gameTime, Maze maze)
		{
		}

		public override void Draw(Camera camera)
		{
			rotate = (float)Math.Atan2(camera.Position.X - Position3.X, camera.Position.Z - Position3.Z) + MathHelper.Pi;
			base.Draw(camera);
		}

		/// <summary>
		/// 3次元座標
		/// </summary>
		public Vector3 Position3 { set { ;} get { return this.position3; } }
	}
}
