using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MazeEscaper
{
	/// <summary>
	/// IUpdateable インターフェイスを実装したゲーム コンポーネントです。
	/// </summary>
	public class Input : Microsoft.Xna.Framework.GameComponent
	{
		private static Input input = new Input();

		/// <summary>
		/// 現在のコントローラの状態
		/// </summary>
		private GamePadState[] nowGamePad = new GamePadState[4];
		/// <summary>
		/// 1フレーム前のコントローラの状態
		/// </summary>
		private GamePadState[] previousGamePad = new GamePadState[4];
		/// <summary>
		/// 現在のキーボードの状態
		/// </summary>
		private KeyboardState nowKeyboard;
		/// <summary>
		/// 1フレーム前のキーボードの状態
		/// </summary>
		private KeyboardState previousKeyboard;

		private Input()
			: base(null)
		{
		}

		/// <summary>
		/// ゲーム コンポーネントの初期化を行います。
		/// ここで、必要なサービスを照会して、使用するコンテンツを読み込むことができます。
		/// </summary>
		public override void Initialize()
		{
			// TODO: ここに初期化のコードを追加します。
			for (int i = 0; i < 4; i++)
				nowGamePad[i] = GamePad.GetState(PlayerIndex.One + i);

			nowKeyboard = Keyboard.GetState();

			base.Initialize();
		}

		/// <summary>
		/// ゲーム コンポーネントが自身を更新するためのメソッドです。
		/// </summary>
		/// <param name="gameTime">ゲームの瞬間的なタイミング情報</param>
		public override void Update(GameTime gameTime)
		{
			// TODO: ここにアップデートのコードを追加します。
			for (int i = 0; i < 4; i++)
			{
				previousGamePad[i] = nowGamePad[i];
				nowGamePad[i] = GamePad.GetState(PlayerIndex.One + i);
			}

			previousKeyboard = nowKeyboard;
			nowKeyboard = Keyboard.GetState();

			base.Update(gameTime);
		}

		/// <summary>
		/// 設定画面での左スティックの値
		/// </summary>
		public Vector2 LeftStickConfig
		{
			get
			{
				if (!nowGamePad[0].IsConnected || !previousGamePad[0].IsConnected)
					return Vector2.Zero;

				float x = 0, y = 0;
				if (nowGamePad[0].ThumbSticks.Left.X > 0 && previousGamePad[0].ThumbSticks.Left.X <= 0)
					x = 1;
				else if (nowGamePad[0].ThumbSticks.Left.X < 0 && previousGamePad[0].ThumbSticks.Left.X >= 0)
					x = -1;
				if (nowGamePad[0].ThumbSticks.Left.Y > 0 && previousGamePad[0].ThumbSticks.Left.Y <= 0)
					y = 1;
				else if (nowGamePad[0].ThumbSticks.Left.Y < 0 && previousGamePad[0].ThumbSticks.Left.Y >= 0)
					y = -1;

				return new Vector2(x, y);
			}
		}

		/// <summary>
		/// 設定画面での左スティック
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public Vector2 LeftStickConfigPlayer(int index)
		{
			float x = 0, y = 0;
			if (nowGamePad[index].ThumbSticks.Left.X > 0 && previousGamePad[index].ThumbSticks.Left.X <= 0)
				x = 1;
			else if (nowGamePad[index].ThumbSticks.Left.X < 0 && previousGamePad[index].ThumbSticks.Left.X >= 0)
				x = -1;
			if (nowGamePad[index].ThumbSticks.Left.Y > 0 && previousGamePad[index].ThumbSticks.Left.Y <= 0)
				y = 1;
			else if (nowGamePad[index].ThumbSticks.Left.Y < 0 && previousGamePad[index].ThumbSticks.Left.Y >= 0)
				y = -1;

			return new Vector2(x, y);
		}

		/// <summary>
		/// 左スティック値を返す(Y)
		/// </summary>
		/// <param name="index">プレイヤーのインデッスク</param>
		/// <returns>左スティックの値</returns>
		public Vector2 LeftStick(PlayerIndex index)
		{
			if (index < PlayerIndex.One || index > PlayerIndex.Four)
				return Vector2.Zero;

			int i = index - PlayerIndex.One;
			if (!nowGamePad[i].IsConnected)
				return Vector2.Zero;

			return nowGamePad[i].ThumbSticks.Left;

		}

		/// <summary>
		/// 右スティック値を返す(Y)
		/// </summary>
		/// <param name="index">プレイヤーのインデッスク</param>
		/// <returns>右スティックの値</returns>
		public double RightStick(PlayerIndex index)
		{
			if (index < PlayerIndex.One || index > PlayerIndex.Four)
				return 0;

			int i = index - PlayerIndex.One;
			if (!nowGamePad[i].IsConnected)
				return 0;

			return nowGamePad[i].ThumbSticks.Right.Y;

		}

		/// <summary>
		/// 左トリガーが押されたか
		/// </summary>
		/// <param name="index">プレイヤーのインデックス</param>
		/// <returns>押されたらtrue、押されていないまたは押しっぱなしならfalse</returns>
		public bool LeftTrigger(PlayerIndex index)
		{
			if (index < PlayerIndex.One || index > PlayerIndex.Four)
				return false;

			int i = index - PlayerIndex.One;
			return nowGamePad[i].IsConnected && previousGamePad[i].IsConnected
				&& nowGamePad[i].Triggers.Left > 0 && previousGamePad[i].Triggers.Left == 0;
		}

		/// <summary>
		/// 右トリガーが押されたか
		/// </summary>
		/// <param name="index">プレイヤーのインデックス</param>
		/// <returns>押されたらtrue、押されていないまたは押しっぱなしならfalse</returns>
		public bool RightTrigger(PlayerIndex index)
		{
			if (index < PlayerIndex.One || index > PlayerIndex.Four)
				return false;

			int i = index - PlayerIndex.One;
			return nowGamePad[i].IsConnected && previousGamePad[i].IsConnected
				&& nowGamePad[i].Triggers.Right > 0 && previousGamePad[i].Triggers.Right == 0;
		}

		public bool A(PlayerIndex index)
		{
			if (index < PlayerIndex.One || index > PlayerIndex.Four)
				return false;

			int i = index - PlayerIndex.One;
			return nowGamePad[i].IsConnected && nowGamePad[i].Buttons.A == ButtonState.Pressed;
		}

		/// <summary>
		/// Aボタンが押されたか
		/// </summary>
		/// <param name="index">プレイヤーのインデックス</param>
		/// <returns>押されたらtrue、押されていないまたは押しっぱなしならfalse</returns>
		public bool PushA(PlayerIndex index)
		{
			if (index < PlayerIndex.One || index > PlayerIndex.Four)
				return false;

			int i = index - PlayerIndex.One;
			return nowGamePad[i].IsConnected && previousGamePad[i].IsConnected
				&& nowGamePad[i].Buttons.A == ButtonState.Pressed && previousGamePad[i].Buttons.A != ButtonState.Pressed;
		}

		/// <summary>
		/// STARTボタンが押されたか
		/// </summary>
		/// <param name="index">プレイヤーのインデックス</param>
		/// <returns>押されたらtrue、押されていないまたは押しっぱなしならfalse</returns>
		public bool PushStart(PlayerIndex index)
		{
			if (index < PlayerIndex.One || index > PlayerIndex.Four)
				return false;

			int i = index - PlayerIndex.One;
			return nowGamePad[i].IsConnected && previousGamePad[i].IsConnected
				&& nowGamePad[i].Buttons.Start == ButtonState.Pressed && previousGamePad[i].Buttons.Start != ButtonState.Pressed;
		}

		/// <summary>
		/// 指定したキーが押されたか
		/// </summary>
		/// <param name="key">キーの種類</param>
		/// <returns>押されたらtrue、押されていないまたはおしっぱなしならfalse</returns>
		public bool PushKey(Keys key)
		{
			return nowKeyboard.IsKeyDown(key) && !previousKeyboard.IsKeyDown(key);
		}

		/// <summary>
		/// 指定したキーが押されているか
		/// </summary>
		/// <param name="key">キーの種類</param>
		/// <returns>押されたらtrue、押されていなかったらfalse</returns>
		public bool DownKey(Keys key)
		{
			return nowKeyboard.IsKeyDown(key);
		}

		public static Input Instance
		{
			get
			{
				return input;
			}
		}
	}
}
