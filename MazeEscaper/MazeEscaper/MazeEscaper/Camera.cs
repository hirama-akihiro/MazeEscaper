using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MazeEscaper.Collidable;
using MazeEscaper.MazeMap;

namespace MazeEscaper
{
	class Camera
	{
		public Matrix View { get; private set; }
		public Matrix Projection { get; private set; }
		public Vector3 Position { get { return position; } }

		//カメラの位置、
		private Vector3 position;

		//プレイヤーからの距離
		private double distance = 5.64;

		//地面を基準とした時のプレイヤーとカメラの角度(Radian)
		private double angle = 1.13;

		//カメラ注視点とプレイヤー位置の差分
		private Vector3 diffDistance = new Vector3(0, 0, -0.8f);

		// デバッグ用カメラを使うか
		private bool debugCamera = false;

		//カメラから注視点への方向
		private Vector3 direction;

		//カメラの高さ
		private float cameraheight;

		//カメラの奥行
		private float cameradepth;

		private float camerarotate = 0;
		private Vector3 offset = new Vector3(0, 0, 0);

		/// <summary>
		/// プレイヤーの位置を指定してカメラ位置設定
		/// </summary>
		/// <param name="player"></param>
		public Camera(Player player)
		{
			//カメラ距離設定
			cameraheight = (float)(distance * Math.Sin(angle));
			cameradepth = (float)(distance * Math.Cos(angle));
			direction = new Vector3(0, -cameraheight, -cameradepth) + diffDistance;

			//カメラ位置設定
			position = player.Position3;
			position.Y += cameraheight;

			//ローテーション用Matrix
			Matrix rotationMatrix = Matrix.CreateRotationY(player.Rotate);

			//カメラポイントを回転
			Vector3 transformedRefeence = Vector3.Transform(direction, rotationMatrix);

			//プレイヤーの角度に合わせてカメラ位置移動
			position.Z += cameradepth * (float)Math.Cos((double)player.Rotate);
			position.X += cameradepth * (float)Math.Sin((double)player.Rotate);

			View = Matrix.CreateLookAt(position, position + transformedRefeence, new Vector3(0, 1, 0));

			//π/4
			float viewAngle = MathHelper.PiOver4;
			float aspectRatio = (float)GameMain.ScreenWidth / GameMain.ScreenHeight;
			float nearClip = 1.0f;
			float farClip = 2000.0f;
			Projection = Matrix.CreatePerspectiveFieldOfView(viewAngle, aspectRatio, nearClip, farClip);
		}

		public void Update(Player player)
		{
			// デバッグ用カメラ切り替え
			if (Input.Instance.PushKey(Microsoft.Xna.Framework.Input.Keys.F))
			{
				debugCamera = !debugCamera;
			}

			if (debugCamera)
			{
				// 真上（高さ50）からプレイヤーを見下ろす視点
				position = new Vector3(player.Position3.X, 50, player.Position3.Z);
				View = Matrix.CreateLookAt(position, position + Vector3.Down, new Vector3(0, 0, -1));
			}
			else
			{
				//カメラ位置をプレイヤーに合わせる
				position = player.Position3;
				position += offset;
				position.Y += cameraheight;

				//ローテーション用Matrix
				Matrix rotationMatrix = Matrix.CreateRotationY(player.Rotate + camerarotate);

				//カメラポイントを回転
				Vector3 transformedRefeence = Vector3.Transform(direction, rotationMatrix);

				//プレイヤーの角度に合わせてカメラ位置移動
				position.Z += cameradepth * (float)Math.Cos((double)player.Rotate);
				position.X += cameradepth * (float)Math.Sin((double)player.Rotate);

				View = Matrix.CreateLookAt(position, position + transformedRefeence, new Vector3(0, 1, 0));
			}
		}

		public void Update()
		{
			float speed = 0.1f;
			if (Input.Instance.DownKey(Microsoft.Xna.Framework.Input.Keys.Left))
				position.X -= speed;
			if (Input.Instance.DownKey(Microsoft.Xna.Framework.Input.Keys.Right))
				position.X += speed;
			if (Input.Instance.DownKey(Microsoft.Xna.Framework.Input.Keys.Up))
				position.Z -= speed;
			if (Input.Instance.DownKey(Microsoft.Xna.Framework.Input.Keys.Down))
				position.Z += speed;
			if (Input.Instance.DownKey(Microsoft.Xna.Framework.Input.Keys.Z))
				position.Y -= speed;
			if (Input.Instance.DownKey(Microsoft.Xna.Framework.Input.Keys.X))
				position.Y += speed;

			View = Matrix.CreateLookAt(position, position + direction, new Vector3(0, 0, -1));
		}
	}
}
