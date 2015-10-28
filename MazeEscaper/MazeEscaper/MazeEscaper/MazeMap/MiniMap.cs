using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MazeEscaper.MazeMap
{
	/// <summary>
	/// ミニマップ
	/// 
	/// ※プレイヤーが2マスずつ進むことを前提に設計
	/// </summary>
	class MiniMap
	{
		/// <summary>
		/// フレーム
		/// </summary>
		private Texture2D miniMapFrame;
		/// <summary>
		/// 背景
		/// </summary>
		private Texture2D miniMapBack;
		/// <summary>
		/// マップ
		/// </summary>
		private Texture2D miniMap;
		/// <summary>
		/// マップを構築するための配列
		/// </summary>
		private Color[] miniMapColor;
		/// <summary>
		/// プレイヤー
		/// </summary>
		private Texture2D player;
		/// <summary>
		/// 旗
		/// </summary>
		private Texture2D flag;

		/// <summary>
		/// フレームの描画位置
		/// </summary>
		private Vector2 drawFramePosition;
		/// <summary>
		/// マップの描画位置
		/// </summary>
		private Vector2 drawMiniMapPosition;

		/// <summary>
		/// 壁の大きさ
		/// </summary>
		private Rectangle bigWall;

		/// <summary>
		/// テクスチャの中心から端までの間に何セット壁が入るか
		/// </summary>
		private int wallNumber;

		/// <summary>
		/// 壁の色
		/// </summary>
		private Color wallColor;

		/// <summary>
		/// プレイヤーの回転
		/// </summary>
		private float playerRotate;
		/// <summary>
		/// 旗の回転
		/// </summary>
		private float? flagRotate;

		/// <summary>
		/// 旗描画用フレーム半径
		/// </summary>
		private int miniMapFrameRadius;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="contentManager"></param>
		/// <param name="graphicsDevice"></param>
		public MiniMap(ContentManager contentManager, GraphicsDevice graphicsDevice)
		{
			miniMapFrame = contentManager.Load<Texture2D>(@"Maze\MiniMap\mini_map_frame");
			miniMapBack = contentManager.Load<Texture2D>(@"Maze\MiniMap\mini_map_back");

			miniMap = new Texture2D(graphicsDevice, miniMapFrame.Width, miniMapFrame.Height);

			player = contentManager.Load<Texture2D>(@"Maze\MiniMap\player");
			flag = contentManager.Load<Texture2D>(@"Maze\MiniMap\flag");

			// テクスチャと同じサイズの配列を用意
			miniMapColor = new Color[miniMap.Width * miniMap.Height];

			drawFramePosition = new Vector2(GameMain.ScreenWidth - miniMapFrame.Width, 0);
			drawMiniMapPosition = new Vector2(GameMain.ScreenWidth - miniMap.Width / 2, miniMap.Height / 2);

			bigWall = new Rectangle(0, 0, 10, 1);

			// テクスチャの中心から端までの間に何セット壁が入るか
			wallNumber = (int)((miniMap.Width / 2.0f - bigWall.Width / 2.0f) / (bigWall.Width + bigWall.Height)) - 1;

			// 壁の色
			wallColor = Color.Gray;

			// ミニマップフレームの半径
			miniMapFrameRadius = 70;
		}

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="maze"></param>
		/// <param name="playerPosition3"></param>
		/// <param name="playerPosition2"></param>
		/// <param name="playerRotate"></param>
		/// <param name="goalPosition"></param>
		public void Update(Maze maze, Vector3 playerPosition3, Point playerPosition2, float playerRotate, Vector3 goalPosition)
		{
			// ミニマップを全て透明に初期化
			for (int i = 0; i < miniMapColor.Length; i++)
				miniMapColor[i] = Color.Transparent;

			// 離散値
			Vector2 discrete = new Vector2(
				(float)((maze.WallWidth + maze.WallDepth) / 2 + (maze.WallWidth + maze.WallDepth) * (playerPosition2.X / 2)),
				(float)((maze.WallWidth + maze.WallDepth) / 2 + (maze.WallWidth + maze.WallDepth) * (playerPosition2.Y / 2))
			);

			// 連続値で本来いる位置と離散値でいる位置の差
			Vector2 difference = discrete - new Vector2(playerPosition3.X, playerPosition3.Z);

			// 2次元の連続値と離散値を同時に変化させる
			for (int i = (int)(miniMap.Height / 2.0f - bigWall.Width / 2.0f) - (bigWall.Width + bigWall.Height) * wallNumber + (int)(difference.Y * bigWall.Height / maze.WallDepth)
				, y = playerPosition2.Y - wallNumber * 2; y <= playerPosition2.Y + wallNumber * 2; i += y % 2 == 0 ? bigWall.Height : bigWall.Width, y++)
				for (int j = (int)(miniMap.Width / 2.0f - bigWall.Width / 2.0f) - (bigWall.Width + bigWall.Height) * wallNumber + (int)(difference.X * bigWall.Height / maze.WallDepth)
					, x = playerPosition2.X - wallNumber * 2; x <= playerPosition2.X + wallNumber * 2; j += x % 2 == 0 ? bigWall.Height : bigWall.Width, x++)
					if (x >= 0 && x < maze.Width && y >= 0 && y < maze.Height && !maze[y, x])
						if (x % 2 == 0 && y % 2 == 0)
							DrawWall(i, j, bigWall.Height, bigWall.Height); // 細い壁
						else
							if (x % 2 == 0)
								DrawWall(i, j, bigWall.Height, bigWall.Width); // 縦長
							else
								DrawWall(i, j, bigWall.Width, bigWall.Height); // 横長

			// ミニマップを円形にする(円外を透明にする)
			Color[] data = new Color[miniMapBack.Height * miniMapBack.Width];
			miniMapBack.GetData<Color>(data);
			for (int i = 0; i < miniMapBack.Height; i++)
				for (int j = 0; j < miniMapBack.Width; j++)
					if (data[i * miniMapBack.Width + j] == Color.Transparent)
						miniMapColor[i * miniMapBack.Width + j] = Color.Transparent;

			// プレイヤーの回転
			this.playerRotate = playerRotate;

			// ゴールの回転
			flagRotate = (float)Math.Atan2(goalPosition.Z - playerPosition3.Z, goalPosition.X - playerPosition3.X);
			flagRotate += playerRotate;
		}

		/// <summary>
		/// 壁の描画
		/// </summary>
		/// <param name="i"></param>
		/// <param name="j"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		private void DrawWall(int i, int j, int width, int height)
		{
			for (int m = 0; m < height; m++)
				for (int n = 0; n < width; n++)
					miniMapColor[(i + m) * miniMap.Width + j + n] = wallColor;
		}

		/// <summary>
		/// 描画
		/// </summary>
		/// <param name="spriteBatch"></param>
		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Begin();

			// 背景
			spriteBatch.Draw(miniMapBack, drawFramePosition, Color.White);

			// マップ
			miniMap.SetData<Color>(miniMapColor);
			spriteBatch.Draw(miniMap, drawMiniMapPosition, null, Color.White, playerRotate, new Vector2(miniMap.Width, miniMap.Height) / 2, 1.0f, SpriteEffects.None, 0.0f);

			// フレーム
			spriteBatch.Draw(miniMapFrame, drawFramePosition, Color.White);

			// プレイヤー
			spriteBatch.Draw(player, drawMiniMapPosition - new Vector2(player.Width, player.Height) / 2, Color.White);

			// 旗
            if (flagRotate.HasValue)
            {
                spriteBatch.Draw(flag, drawMiniMapPosition + miniMapFrameRadius * new Vector2((float)Math.Cos(flagRotate.Value), (float)Math.Sin(flagRotate.Value)) - new Vector2(flag.Width, flag.Height) / 2, Color.White);
            }

			spriteBatch.End();
		}
	}
}
