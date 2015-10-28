using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using MazeEscaper.MazeMap;


namespace MazeEscaper.Collidable
{
	/// <summary>
	/// プレイヤークラス
	/// </summary>
	class Player : AnimationModel
	{
		//重力加速度(高さ/(フレーム数*(フレーム数+1)*0.5))
		private const float GRAVITY = 0.0027f;

		//ジャンプ力(重力加速度*フレーム数)
		private const float JUMPPOWER = 0.082f;

		//加速度
		private float Acc = 0.0f;

		/// <summary>
		/// ジャンプフラグ
		/// </summary>
		private bool JumpFlag { get; set; }

		/// <summary>
		/// ダメージを受けた瞬間だけtrueになるフラグ
		/// </summary>
		public bool justDamaged = false;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="content"></param>
		/// <param name="startPosition"></param>
		/// <param name="orient"></param>
		public Player(ContentManager content, Point startPosition, Orientation orient, Maze maze)
			: base(content, @"Objects\Chick\chick", startPosition, orient, new Vector3(0.8f, 0.8f, 0.8f), maze)
		{
			//モデルの移動速度
			speed = 0.05;
			//モデルの回転速度
			degreespeed = 2.0f;

			Radius = 1.0f;

			JumpFlag = false;
		}

		/// <summary>
		/// 更新処理
		/// </summary>
		/// <param name="maze"></param>
		public override void Update(GameTime gameTime, Maze maze)
		{
			// ダメージを受けた瞬間か
			justDamaged = damegeCounter == 1;
			if (justDamaged)
				SoundManager.Instance.SoundBank.PlayCue("damage");

			#region ジャンプ処理
			if (Input.Instance.PushKey(Microsoft.Xna.Framework.Input.Keys.Space))
				StartJump();

			Jumping();
			#endregion

			//停止状態でのみキー入力を可能にする
			if (currentMoveState == MoveState.Stop)
			{
				//左向きに回転
				if (Input.Instance.DownKey(Microsoft.Xna.Framework.Input.Keys.Left))
					Move(MoveState.TurnLeft, maze);
				//右向きに回転
				if (Input.Instance.DownKey(Microsoft.Xna.Framework.Input.Keys.Right))
					Move(MoveState.TurnRight, maze);
				//前進
				if (Input.Instance.DownKey(Microsoft.Xna.Framework.Input.Keys.Up))
				{
					Move(MoveState.Advance, maze);
					// アニメーションの開始
					AnimationStart("walk");
				}
				//後退
				if (Input.Instance.DownKey(Microsoft.Xna.Framework.Input.Keys.Down))
				{
					Move(MoveState.Back, maze);
					// アニメーションの開始
					AnimationStart("walk");
				}
			}
			else
			{
				//移動更新
				Move(currentMoveState, maze);
			}
			// アニメーションの更新
			AnimationUpdate(gameTime);
		}

		/// <summary>
		/// 移動更新
		/// </summary>
		/// <param name="moveState"></param>
		/// <param name="maze"></param>
		protected override void Move(Collidable.MoveState moveState, Maze maze)
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
				//switch (moveState)
				switch (currentMoveState)
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
		/// ジャンプ開始
		/// </summary>
		private void StartJump()
		{
			if (JumpFlag)
				return;

			JumpFlag = true;
			Acc = JUMPPOWER;
			SoundManager.Instance.SoundBank.PlayCue("jump");
		}

		/// <summary>
		/// ジャンプ中
		/// </summary>
		private void Jumping()
		{
			if (!JumpFlag)
				return;

			Acc -= GRAVITY;
			position3.Y += Acc;

			//着地
			if (position3.Y < 0.0f)
			{
				position3.Y = 0.0f;
				//ジャンプ終了
				JumpFlag = false;
			}
		}

		#region プロパティ

		/// <summary>
		/// 3次元座標
		/// </summary>
		public Vector3 Position3 { set { ;} get { return this.position3; } }

		/// <summary>
		/// モデル角度
		/// </summary>
		public float Rotate { set { ;} get { return this.rotate; } }



		#endregion
	}
}
