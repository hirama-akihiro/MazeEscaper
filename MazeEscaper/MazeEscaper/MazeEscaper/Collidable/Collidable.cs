using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MazeEscaper.MazeMap;
using MazeEscaper.Shader;
using MazeEscaper.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MazeEscaper.Collidable
{
	abstract class Collidable
	{
		/// <summary>
		/// モデル
		/// </summary>
		protected Model model;

		/// <summary>
		/// オブジェクトの3次元座標
		/// </summary>
		protected Vector3 position3;

		/// <summary>
		/// オブジェクトの移動後の3次元座標
		/// </summary>
		protected Vector3 nextposition3;

		/// <summary>
		/// オブジェクトの2次元座標(X=X,Y=Z)
		/// </summary>
		protected Point position2;

		/// <summary>
		/// 移動後の2次元座標(X=X,Y=Z)
		/// </summary>
		protected Point nextposition2;

		/// <summary>
		/// オブジェクトのスケール
		/// </summary>
		protected Vector3 scale;

		/// <summary>
		/// オブジェクトの移動速度
		/// </summary>
		protected double speed;

		/// <summary>
		/// オブジェクトの回転角度(radian)
		/// </summary>
		protected float rotate;

		/// <summary>
		/// オブジェクトの回転角度(degree)
		/// </summary>
		protected float degree;

		/// <summary>
		/// オブジェクトの回転後の角度
		/// </summary>
		protected float nextdegree;

		/// <summary>
		/// オブジェクトの回転速度
		/// </summary>
		protected float degreespeed;

		/// <summary>
		/// オブジェクトの現在の状態
		/// </summary>
		public MoveState currentMoveState;

		/// <summary>
		/// モデルの向いている方向
		/// </summary>
		public Orientation DirecState { get; set; }

		/// <summary>
		/// モデルの半径
		/// </summary>
		public float Radius { get; set; }

		/// <summary>
		/// オブジェクトの現在の状態
		/// </summary>
		public enum MoveState
		{
			Stop, Advance, Back, TurnLeft, TurnRight
		}

		/// <summary>
		/// 方向の状態
		/// </summary>
		public enum Orientation
		{
			// Northから時計回り
			North, East, South, West
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="content"></param>
		/// <param name="assetname"></param>
		/// <param name="startPosition"></param>
		/// <param name="orient"></param>
		/// <param name="scale"></param>
		/// <param name="maze"></param>
		public Collidable(ContentManager content, string assetname, Point startPosition, Orientation orient, Vector3 scale, Maze maze)
		{
			// nullの場合はAnimationModelなどでロードしておく
			if (assetname != null)
				model = content.Load<Model>(assetname);

			position2 = startPosition;
			nextposition2 = startPosition;

			//2DPos -> 3DPos
			position3 = PointToVecto3(position2, maze);
			nextposition3 = position3;

			//方向設定
			DirecState = orient;
			this.scale = scale;

			//停止状態
			currentMoveState = MoveState.Stop;

			//オリエントからrotate,degreeを計算
			rotate = OrientationToRotate(DirecState);
			degree = OrientationToDegree(DirecState);
		}

		/// <summary>
		/// 更新処理
		/// </summary>
		/// <param name="maze"></param>
		public virtual void Update(GameTime gameTime, Maze maze)
		{
		}


		/// <summary>
		/// 指定した方角に動けるかどうか
		/// </summary>
		/// <param name="orientation"></param>
		/// <param name="maze"></param>
		/// <returns></returns>
		protected bool CanMove(Orientation orientation, Maze maze, Point position)
		{
			//モデルの方向によって移動判定を行う
			switch (orientation)
			{
				case Orientation.North:
					return maze[position.Y - 1, position.X];
				case Orientation.West:
					return maze[position.Y, position.X - 1];
				case Orientation.East:
					return maze[position.Y, position.X + 1];
				case Orientation.South:
					return maze[position.Y + 1, position.X];
			}
			throw new ArgumentOutOfRangeException("orientation");
		}


		/// <summary>
		/// 指定した方角に動けるかどうか
		/// </summary>
		/// <param name="orientation"></param>
		/// <param name="maze"></param>
		/// <returns></returns>
		protected bool CanMove(Orientation orientation, Maze maze)
		{
			return CanMove(orientation, maze, position2);
		}


		/// <summary>
		/// 移動更新
		/// </summary>
		/// <param name="moveState"></param>
		/// <param name="maze"></param>
		protected virtual void Move(MoveState moveState, Maze maze)
		{
			//停止状態のとき対応する初期動作を行う
			if (currentMoveState == MoveState.Stop)
			{
				switch (moveState)
				{
					case MoveState.Advance:
						StartAdvance(maze);
						break;
					case MoveState.Back:
						StartBack(maze);
						break;
					case MoveState.Stop:
						break;
					case MoveState.TurnLeft:
						StartTurnLeft();
						break;
					case MoveState.TurnRight:
						StartTurnRight();
						break;
				}
			}
			//状態に対応する動作を行う,動作終了後Stop状態に遷移
			else
			{
				switch (moveState)
				{
					case MoveState.Advance:
						Advancing();
						break;
					case MoveState.Back:
						Backing();
						break;
					case MoveState.Stop:
						break;
					case MoveState.TurnLeft:
						TurningLeft();
						break;
					case MoveState.TurnRight:
						TurningRight();
						break;
				}
			}
		}


		/// <summary>
		/// モデルの描画
		/// </summary>
		/// <param name="camera"></param>
		public virtual void Draw(Camera camera)
		{
			model.Draw(CalculateWorld(camera), camera.View, camera.Projection);
		}

		/// <summary>
		/// ワールド行列の計算
		/// </summary>
		/// <param name="camera">カメラ</param>
		/// <returns>ワールド行列</returns>
		protected Matrix CalculateWorld(Camera camera)
		{
			//座標変換の初期化
			Matrix transform = Matrix.Identity;

			//回転
			transform *= Matrix.CreateRotationY(this.rotate + MathHelper.Pi);

			//拡大縮小
			transform *= Matrix.CreateScale(this.scale);

			//移動
			transform *= Matrix.CreateTranslation(position3);

			return transform;
		}

		#region 移動アニメーション

		/// <summary>
		/// 前進開始
		/// </summary>
		protected void StartAdvance(Maze maze)
		{
			// 前方に動けないなら何もしないで終わる
			if (!CanMove(DirecState, maze))
			{
				return;
			}


			//モデルの方向によって移動判定を行う
			switch (DirecState)
			{
				case Orientation.North:
					currentMoveState = MoveState.Advance;
					nextposition2.Y -= 2;
					nextposition3 = PointToVecto3(nextposition2, maze);
					break;
				case Orientation.West:
					currentMoveState = MoveState.Advance;
					nextposition2.X -= 2;
					nextposition3 = PointToVecto3(nextposition2, maze);
					break;
				case Orientation.East:
					currentMoveState = MoveState.Advance;
					nextposition2.X += 2;
					nextposition3 = PointToVecto3(nextposition2, maze);
					break;
				case Orientation.South:
					currentMoveState = MoveState.Advance;
					nextposition2.Y += 2;
					nextposition3 = PointToVecto3(nextposition2, maze);
					break;
			}
		}

		/// <summary>
		/// 前進中
		/// </summary>
		protected void Advancing()
		{
			//モデルの方向によって前進方向を決定
			switch (DirecState)
			{
				case Orientation.North:
					position3.Z -= (float)speed;
					if (position3.X == nextposition3.X && position3.Z < nextposition3.Z)
					{
						position3.X = nextposition3.X;
						position3.Z = nextposition3.Z;
						position2.X = nextposition2.X;
						position2.Y = nextposition2.Y;
						currentMoveState = MoveState.Stop;
					}
					break;
				case Orientation.West:
					position3.X -= (float)speed;
					if (position3.X < nextposition3.X && position3.Z == nextposition3.Z)
					{
						position3.X = nextposition3.X;
						position3.Z = nextposition3.Z;
						position2.X = nextposition2.X;
						position2.Y = nextposition2.Y;
						currentMoveState = MoveState.Stop;
					}
					break;
				case Orientation.East:
					position3.X += (float)speed;
					if (position3.X > nextposition3.X && position3.Z == nextposition3.Z)
					{
						position3.X = nextposition3.X;
						position3.Z = nextposition3.Z;
						position2.X = nextposition2.X;
						position2.Y = nextposition2.Y;
						currentMoveState = MoveState.Stop;
					}
					break;
				case Orientation.South:
					position3.Z += (float)speed;
					if (position3.X == nextposition3.X && position3.Z > nextposition3.Z)
					{
						position3.X = nextposition3.X;
						position3.Z = nextposition3.Z;
						position2.X = nextposition2.X;
						position2.Y = nextposition2.Y;
						currentMoveState = MoveState.Stop;
					}
					break;
			}
		}

		/// <summary>
		/// 後退開始
		/// </summary>
		protected void StartBack(Maze maze)
		{
			// 後ろに動けないなら何もしないで終わる
			if (!CanMove(DirecState.Back(), maze))
			{
				return;
			}

			//モデルの方向によって移動判定を行う
			switch (DirecState)
			{
				case Orientation.North:
					currentMoveState = MoveState.Back;
					nextposition2.Y += 2;
					nextposition3 = PointToVecto3(nextposition2, maze);
					break;
				case Orientation.West:
					currentMoveState = MoveState.Back;
					nextposition2.X += 2;
					nextposition3 = PointToVecto3(nextposition2, maze);
					break;
				case Orientation.East:
					currentMoveState = MoveState.Back;
					nextposition2.X -= 2;
					nextposition3 = PointToVecto3(nextposition2, maze);
					break;
				case Orientation.South:
					currentMoveState = MoveState.Back;
					nextposition2.Y -= 2;
					nextposition3 = PointToVecto3(nextposition2, maze);
					break;
			}
		}

		/// <summary>
		/// 後退中
		/// </summary>
		protected void Backing()
		{
			//モデルの方向によって前進方向を決定
			switch (DirecState)
			{
				case Orientation.North:
					position3.Z += (float)speed;
					if (position3.X == nextposition3.X && position3.Z > nextposition3.Z)
					{
						position3.X = nextposition3.X;
						position3.Z = nextposition3.Z;
						position2.X = nextposition2.X;
						position2.Y = nextposition2.Y;
						currentMoveState = MoveState.Stop;
					}
					break;
				case Orientation.West:
					position3.X += (float)speed;
					if (position3.X > nextposition3.X && position3.Z == nextposition3.Z)
					{
						position3.X = nextposition3.X;
						position3.Z = nextposition3.Z;
						position2.X = nextposition2.X;
						position2.Y = nextposition2.Y;
						currentMoveState = MoveState.Stop;
					}
					break;
				case Orientation.East:
					position3.X -= (float)speed;
					if (position3.X < nextposition3.X && position3.Z == nextposition3.Z)
					{
						position3.X = nextposition3.X;
						position3.Z = nextposition3.Z;
						position2.X = nextposition2.X;
						position2.Y = nextposition2.Y;
						currentMoveState = MoveState.Stop;
					}
					break;
				case Orientation.South:
					position3.Z -= (float)speed;
					if (position3.X == nextposition3.X && position3.Z < nextposition3.Z)
					{
						position3.X = nextposition3.X;
						position3.Z = nextposition3.Z;
						position2.X = nextposition2.X;
						position2.Y = nextposition2.Y;
						currentMoveState = MoveState.Stop;
					}
					break;
			}
		}

		/// <summary>
		/// 左回転開始
		/// </summary>
		protected void StartTurnLeft()
		{
			currentMoveState = MoveState.TurnLeft;
			//Orientationの更新
			DirecState = DirecState.Left();
			nextdegree = degree + 90f;
		}

		/// <summary>
		/// 左回転中
		/// </summary>
		protected void TurningLeft()
		{
			degree += degreespeed;
			rotate = MathHelper.ToRadians(degree);
			if (degree > nextdegree)
			{
				degree = nextdegree;
				rotate = MathHelper.ToRadians(degree);
				currentMoveState = MoveState.Stop;
			}
		}

		/// <summary>
		/// 右回転開始
		/// </summary>
		protected void StartTurnRight()
		{
			currentMoveState = MoveState.TurnRight;
			//Orientationの更新
			DirecState = DirecState.Right();
			nextdegree = degree - 90f;
		}

		/// <summary>
		/// 右回転中
		/// </summary>
		protected void TurningRight()
		{
			degree -= degreespeed;
			rotate = MathHelper.ToRadians(degree);
			if (degree < nextdegree)
			{
				degree = nextdegree;
				rotate = MathHelper.ToRadians(degree);
				currentMoveState = MoveState.Stop;
			}
		}

		#endregion

		/// <summary>
		/// オブジェクト同士の衝突判定
		/// </summary>
		/// <param name="coll"></param>
		/// <returns></returns>
		public bool Collision(Collidable coll)
		{
			/*
			//モデルの基本包括球
			BoundingSphere baseBoundingSphere1 = new BoundingSphere();
			BoundingSphere baseBoundingSphere2 = new BoundingSphere();

			//モデルの包括球
			BoundingSphere sphere1BoundingSphere = new BoundingSphere();
			BoundingSphere sphere2BoundingSphere = new BoundingSphere();

			//包括球取得
			baseBoundingSphere1 = this.model.Meshes[0].BoundingSphere;
			baseBoundingSphere2 = coll.model.Meshes[0].BoundingSphere;

			//各モデル用の包括球半径設定
			sphere1BoundingSphere.Radius = baseBoundingSphere1.Radius * this.scale.X * 0.8f;
			sphere2BoundingSphere.Radius = baseBoundingSphere2.Radius * coll.scale.X * 0.8f;

			//衝突判定用の球を設定
			sphere1BoundingSphere.Center =
				this.position3 + baseBoundingSphere1.Center;
			sphere2BoundingSphere.Center =
				coll.position3 + baseBoundingSphere2.Center;

			//衝突判定
			return sphere1BoundingSphere.Intersects(sphere2BoundingSphere);

			 */

			double diff = Math.Pow(this.position3.X - coll.position3.X, 2) + Math.Pow((this.position3.Y + this.Radius * this.scale.Y) - (coll.position3.Y + this.Radius * this.scale.Y), 2) + Math.Pow(this.position3.Z - coll.position3.Z, 2);
			return diff <= Math.Pow(this.Radius * this.scale.X * 0.8f + coll.Radius * coll.scale.X * 0.8f, 2);
			
		}

		/// <summary>
		/// 2次元座標を3次元座標に変換するメソッド
		/// </summary>
		/// <param name="maze"></param>
		protected Vector3 PointToVecto3(Point pos2, Maze maze)
		{
			Vector3 pos3 = new Vector3();
			//モデル座標の更新(2D->3D)
			pos3.X = (float)(0.5 * maze.WallWidth + (pos2.X / 2) * maze.WallWidth + ((pos2.X / 2) + 0.5) * maze.WallDepth);
			pos3.Z = (float)(0.5 * maze.WallWidth + (pos2.Y / 2) * maze.WallWidth + ((pos2.Y / 2) + 0.5) * maze.WallDepth);
			pos3.Y = position3.Y;
			return pos3;
		}

		/// <summary>
		/// OrientationからRotateを計算するメソッド
		/// </summary>
		/// <param name="orient"></param>
		/// <returns></returns>
		public float OrientationToRotate(Orientation orient)
		{
			return MathHelper.ToRadians(OrientationToDegree(orient));
		}

		/// <summary>
		/// OrientationからDegreeを計算するメソッド
		/// </summary>
		/// <param name="orient"></param>
		/// <returns></returns>
		public float OrientationToDegree(Orientation orient)
		{
			float ansdegree = 0.0f;
			switch (orient)
			{
				case Orientation.North:
					ansdegree = 0.0f;
					break;
				case Orientation.West:
					ansdegree = 90.0f;
					break;
				case Orientation.South:
					ansdegree = 180.0f;
					break;
				case Orientation.East:
					ansdegree = 270.0f;
					break;
			}
			return ansdegree;
		}

		/// <summary>
		/// 2次元座標
		/// </summary>
		public Point Position2 { get { return position2; } }
	}

	static class OrientationExtention
	{
		/// <summary>
		/// 右の方位を返す
		/// </summary>
		/// <param name="orientation"></param>
		/// <returns>右の方位</returns>
		public static Collidable.Orientation Right(this Collidable.Orientation orientation)
		{
			return (Collidable.Orientation)((int)(orientation + 1) % 4);
		}


		/// <summary>
		/// 後ろの方位を返す
		/// </summary>
		/// <param name="orientation"></param>
		/// <returns>後ろの方位</returns>
		public static Collidable.Orientation Back(this Collidable.Orientation orientation)
		{
			return (Collidable.Orientation)((int)(orientation + 2) % 4);
		}


		/// <summary>
		/// 左の方位を返す
		/// </summary>
		/// <param name="orientation"></param>
		/// <returns>左の方位</returns>
		public static Collidable.Orientation Left(this Collidable.Orientation orientation)
		{
			return (Collidable.Orientation)((int)(orientation + 3) % 4);
		}
	}
}