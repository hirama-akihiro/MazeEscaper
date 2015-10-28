using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MazeEscaper.Scene;

namespace MazeEscaper.MazeMap
{
	class Maze
	{
		private const string xmlFilename = @"Content\Maze\Field\FieldChip.xml";

		private bool[,] movableArea;

		public int Width { get; private set; }
		public int Height { get; private set; }

		private Model wallBig;
		private Model wallSmall;
		private Model groundBig;
		private Model groundSmall;
		private double wallWidth = 1.0;
		private double wallDepth = 0.1;

		private Random rand = new Random();

		/// <summary>
		/// ブロック間の通り抜け可能な数(16*16の場合は、4以下を指定できる)
		/// </summary>
		private int hallNumber = 4;

		/// <summary>
		/// プレイヤーを中心とした壁の描画範囲の半径
		/// </summary>
		private readonly int drawAreaRadius;

		public Maze(ContentManager contentManager, StageSelect.SelectStage selectStage)
		{
			// 迷路初期化。難易度によって大きさを変える
			int width = 0;
			int height = 0;
			switch (selectStage)
			{
				case StageSelect.SelectStage.Small:
					width = 3;
					height = 2;
					break;
				case StageSelect.SelectStage.Regular:
					width = 4;
					height = 3;
					break;
				case StageSelect.SelectStage.Big:
					width = 5;
					height = 4;
					break;
			}

			// XMLの読み込み
			XElement xmlDoc = XElement.Load(xmlFilename);
			List<string> fieldChipNameList = new List<string>();
			foreach (var elem in xmlDoc.Descendants("chip"))
				fieldChipNameList.Add(elem.Value);

			// ランダムにチップを選択
			List<string> necessaryFieldChipList = new List<string>(width * height);
			Random random = new Random();
			for (int i = 0; i < width * height; i++)
				necessaryFieldChipList.Add(fieldChipNameList[random.Next(fieldChipNameList.Count)]);

			// チップの読み込み
			int[,][, ,] fieldChips = new int[height, width][, ,];
			foreach (var fieldChipName in necessaryFieldChipList.Select((v, i) => new { v, i }))
				fieldChips[fieldChipName.i / width, fieldChipName.i % width] = LoadFieldChip(@"Content\Maze\Field\" + fieldChipName.v);

			int[, ,] field = ConbineFieldChip(fieldChips);
			movableArea = new bool[field.GetLength(0), field.GetLength(1)];
			Width = movableArea.GetLength(1);
			Height = movableArea.GetLength(0);
			for (int i = 0; i < field.GetLength(0); i++)
				for (int j = 0; j < field.GetLength(1); j++)
					movableArea[i, j] = field[i, j, 0] == 0;

			wallBig = contentManager.Load<Model>(@"Maze\BigWall\BigWallbrick");
			wallSmall = contentManager.Load<Model>(@"Maze\SmallWall\SmallWallbrick");
			groundBig = contentManager.Load<Model>(@"Maze\BigGround\TileGroundB");
			groundSmall = contentManager.Load<Model>(@"Maze\SmallGround\TileGroundS");

			drawAreaRadius = 17;

			#region ToonEffect
			//wallBig.ReplaceAllEffects(effect => EffectTranslator.Translate(contentManager, effect));
			//wallSmall.ReplaceAllEffects(effect => EffectTranslator.Translate(contentManager, effect));
			#endregion

			#region BasicEffect
			
            foreach (ModelMesh mesh in this.wallBig.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    //デフォルトのライト適用
                    effect.EnableDefaultLighting();
					effect.DirectionalLight0.Direction = new Vector3(0, 10, 0);
					effect.DirectionalLight1.Direction = new Vector3(0, -10, 0);
					//effect.DirectionalLight2.Direction = new Vector3(0, -5, 0);
                }
            }
            foreach (ModelMesh mesh in this.wallSmall.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    //デフォルトのライト適用
                    effect.EnableDefaultLighting();
					//effect.DirectionalLight0.Direction = new Vector3(0, 10, 0);
					//effect.DirectionalLight1.Direction = new Vector3(0, -5, 0);
                }
            }
			#endregion
		}
		
		private int[, ,] LoadFieldChip(string filename)
		{
			int[, ,] field;

			using (Bitmap bitmap = new Bitmap(filename))
			{
				ConvenientBitmap convenientBitmap = new ConvenientBitmap(bitmap);
				field = new int[convenientBitmap.Height, convenientBitmap.Width, 3];
				for (int i = 0; i < convenientBitmap.Height; i++)
					for (int j = 0; j < convenientBitmap.Width; j++)
					{
						field[i, j, 0] = convenientBitmap.GetR(j, i);
						field[i, j, 1] = convenientBitmap.GetG(j, i);
						field[i, j, 2] = convenientBitmap.GetB(j, i);
					}
			}

			return field;
		}

		private int[, ,] ConbineFieldChip(int[,][, ,] fieldChips)
		{
			int height = fieldChips.GetLength(0) * (fieldChips[0, 0].GetLength(0) - 1) + 1;
			int width = fieldChips.GetLength(1) * (fieldChips[0, 0].GetLength(1) - 1) + 1;
			int[, ,] field = new int[height, width, 3];

			#region 枠
			// 上下
			for (int j = 0; j < width; j++)
				field[0, j, 0] = field[height - 1, j, 0] = 255;
			// 左右
			for (int i = 0; i < height; i++)
				field[i, 0, 0] = field[i, width - 1, 0] = 255;
			// マップチップの区切り（点）
			for (int i = fieldChips[0, 0].GetLength(0) - 1; i < height - 1; i += fieldChips[0, 0].GetLength(0) - 1)
				for (int j = fieldChips[0, 0].GetLength(1) - 1; j < width - 1; j += fieldChips[0, 0].GetLength(1) - 1)
					field[i, j, 0] = 255;
			// マップチップの区切り（横）
			for (int i = fieldChips[0, 0].GetLength(0) - 1; i < height - 1; i += fieldChips[0, 0].GetLength(0) - 1)
			{
				Random random = new Random();
				foreach (var j in Enumerable.Range(0, fieldChips.GetLength(1)))
				{
					int[] hall = new int[hallNumber];
					foreach (var k in Enumerable.Range(0, hall.Length))
					{
						for(bool loopFlag = true; loopFlag;)
						{
							loopFlag = false;
							// ランダムに穴の位置を決定
							hall[k] = random.Next(fieldChips[0, 0].GetLength(1) / 2 - 2) + 1;
							foreach (var l in Enumerable.Range(0, k))
								// 新しい穴が既存の穴と隣接していたら
								if (Math.Abs(hall[l] - hall[k]) <= 1)
								{
									loopFlag = true;
									break;
								}
						}
					}
					for (int k = 1; k < fieldChips[0, 0].GetLength(1) - 1; k++)
					{
						bool hallFlag = false;
						foreach (var l in Enumerable.Range(0, hall.Length))
							// 穴の位置だったら
							if (k / 2 == hall[l] && k % 2 == 1)
							{
								hallFlag = true;
								break;
							}
						// 穴の位置ではなかったら
						if (!hallFlag)
							field[i, j * (fieldChips[0, 0].GetLength(1) - 1) + k, 0] = 255;
					}
				}
			}
			// マップチップの区切り（縦）
 			for (int j = fieldChips[0, 0].GetLength(1) - 1; j < width- 1; j += fieldChips[0, 0].GetLength(1) - 1)
			{
				Random random = new Random();
				foreach (var i in Enumerable.Range(0, fieldChips.GetLength(0)))
				{
					int[] hall = new int[hallNumber];
					foreach (var k in Enumerable.Range(0, hall.Length))
					{
						for (bool loopFlag = true; loopFlag; )
						{
							loopFlag = false;
							// ランダムに穴の位置を決定
							hall[k] = random.Next(fieldChips[0, 0].GetLength(0) / 2 - 2) + 1;
							foreach (var l in Enumerable.Range(0, k))
								// 新しい穴が既存の穴と隣接していたら
								if (Math.Abs(hall[l] - hall[k]) <= 1)
								{
									loopFlag = true;
									break;
								}
						}
					}
					for (int k = 1; k < fieldChips[0, 0].GetLength(0) - 1; k++)
					{
						bool hallFlag = false;
						foreach (var l in Enumerable.Range(0, hall.Length))
							// 穴の位置だったら
							if (k / 2 == hall[l] && k % 2 == 1)
							{
								hallFlag = true;
								break;
							}
						// 穴の位置ではなかったら
						if (!hallFlag)
							field[i * (fieldChips[0, 0].GetLength(0) - 1) + k, j, 0] = 255;

					}
				}
			}
			#endregion

			#region 中身
			for (int i = 0; i < fieldChips.GetLength(0); i++)
				for (int j = 0; j < fieldChips.GetLength(1); j++)
					for (int k = 1; k < fieldChips[i, j].GetLength(0) - 1; k++)
						for (int l = 1; l < fieldChips[i, j].GetLength(1) - 1; l++)
						{
							int y = i * (fieldChips[0, 0].GetLength(0) - 1) + k;
							int x = j * (fieldChips[0, 0].GetLength(1) - 1) + l;
							field[y, x, 0] = fieldChips[i, j][k, l, 0];
							field[y, x, 1] = fieldChips[i, j][k, l, 1];
							field[y, x, 2] = fieldChips[i, j][k, l, 2];
						}
			#endregion

			return field;
		}

		public void Draw(Camera camera, Microsoft.Xna.Framework.Point playerPosition, Collidable.Collidable.Orientation orientation, Collidable.Collidable.MoveState moveState)
		{
			#region Toon
			//wallBig.ForEachEffect(EffectTranslator.Settings);
			//wallSmall.ForEachEffect(EffectTranslator.Settings);
			#endregion

			for (int i = 0; i < movableArea.GetLength(0); i++)
				for (int j = 0; j < movableArea.GetLength(1); j++)
				{
					// 壁の描画領域を制限する（Gキーを押している間は制限しない）
					if (!Input.Instance.DownKey(Microsoft.Xna.Framework.Input.Keys.G))
					{
						if ((i - playerPosition.Y) * (i - playerPosition.Y) + (j - playerPosition.X) * (j - playerPosition.X) > drawAreaRadius * drawAreaRadius)
						{
							continue;
						}
						else if (moveState != Collidable.Collidable.MoveState.TurnLeft && moveState != Collidable.Collidable.MoveState.TurnRight)
						{
							#region 後ろを描画制限
							if (orientation == Collidable.Collidable.Orientation.North)
							{
								if (i - playerPosition.Y > 4)
								{
									continue;
								}
							}
							else if (orientation == Collidable.Collidable.Orientation.South)
							{
								if (playerPosition.Y - i > 4)
								{
									continue;
								}
							}
							else if (orientation == Collidable.Collidable.Orientation.West)
							{
								if (j - playerPosition.X > 4)
								{
									continue;
								}
							}
							else if (orientation == Collidable.Collidable.Orientation.East)
							{
								if (playerPosition.X - j > 4)
								{
									continue;
								}
							}
							#endregion
						}
					}
					Matrix world = Matrix.CreateTranslation((float)(j / 2 * (wallDepth + wallWidth) + j % 2 * ((wallDepth + wallWidth) / 2)), 0, (float)(i / 2 * (wallDepth + wallWidth) + i % 2 * ((wallDepth + wallWidth) / 2)));
					if (movableArea[i, j])
					{
						if ((i % 2) == 1 && (j % 2) == 1)
						{
							groundBig.Draw(world, camera.View, camera.Projection);
						}
						else if ((i % 2) == 1)
							groundSmall.Draw(Matrix.CreateRotationY(MathHelper.PiOver2) * world, camera.View, camera.Projection);
						else
							groundSmall.Draw(world, camera.View, camera.Projection);

						continue;
					}

					world = Matrix.CreateTranslation((float)(j / 2 * (wallDepth + wallWidth) + j % 2 * ((wallDepth + wallWidth) / 2)), 0, (float)(i / 2 * (wallDepth + wallWidth) + i % 2 * ((wallDepth + wallWidth) / 2)));
					if (i % 2 == 0 && j % 2 == 0)
						wallSmall.Draw(world, camera.View, camera.Projection);
					else
						if (i % 2 == 1)
							wallBig.Draw(Matrix.CreateRotationY(MathHelper.PiOver2) * world, camera.View, camera.Projection);
						else
							wallBig.Draw(world, camera.View, camera.Projection);
				}
		}

		public bool this[int row, int col]
		{
			get
			{
				if (movableArea == null)
					return false;
				if (row < 0 || movableArea.GetLength(0) <= row)
					return false;
				if (col < 0 || movableArea.GetLength(1) <= col)
					return false;
				return movableArea[row, col];
			}
		}

		/// <summary>
		/// ランダムな位置を返します。
		/// </summary>
		/// <returns>ランダムな位置</returns>
		public Microsoft.Xna.Framework.Point RandomPoint()
		{
			return new Microsoft.Xna.Framework.Point(rand.Next(Width / 2) * 2 + 1, rand.Next(Height / 2) * 2 + 1);
		}

		#region プロパティ
		public double WallWidth { get { return this.wallWidth; } }
        public double WallDepth { get { return this.wallDepth; } }
		#endregion
	}
}
