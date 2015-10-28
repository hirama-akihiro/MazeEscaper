using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MazeEscaper.Collidable;
using MazeEscaper.Collidable.Enemy;
using MazeEscaper.MazeMap;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace MazeEscaper.Scene
{
	/// <summary>
	/// ゲーム中のシーンインターフェース
	/// </summary>
    class GameIn : SceneInterface
    {
        private ContentManager content;
        private GraphicsDevice graphicsDevice;

        private Player player;

		private EnemyManager enemyManager;

        private GoalObject goalobj;

        private Maze maze;

		private BackGround backmodel;

        private Camera camera;

		private MiniMap miniMap;

		private Score score;

		private Random rand;

		private FadeOut fadeout;

		private Cue cueBGM;

		private CountDown countdown;

		/// <summary>
		/// ステージサイズ
		/// </summary>
		private StageSelect.SelectStage selectStage;

        /// <summary>
        /// メインゲームシーン
        /// </summary>
        /// <param name="content"></param>
        /// <param name="graphicsDevice"></param>
        /// <param name="input"></param>
		public GameIn(ContentManager content, GraphicsDevice graphicsDevice, GameTime gameTime, StageSelect.SelectStage selectStage, StageSelect.SelectEnemy selectEnemy)
		{
			this.content = content;
			this.graphicsDevice = graphicsDevice;

			this.selectStage = selectStage;

			// 乱数の初期化
			rand = new Random();

            // 迷路初期化。難易度によって大きさを変える
			maze = new Maze(content, selectStage);

            Point playerPosition;
            Point goalPosition;

            while (true) 
            {
                playerPosition = maze.RandomPoint();
                goalPosition = maze.RandomPoint();

                // 迷路の斜辺の長さを求める。三平方の定理 c^2 = sqrt(a^2 + b^2)
                double hypotenuse = Math.Sqrt(maze.Width * maze.Width + maze.Height * maze.Height);

                // 0.5斜辺 < 距離 < 0.6斜辺ならOK
				double distance = Math.Sqrt((playerPosition.X - goalPosition.X) * (playerPosition.X - goalPosition.X) + (playerPosition.Y - goalPosition.Y) * (playerPosition.Y - goalPosition.Y));
				if (hypotenuse * 0.5 < distance && distance < hypotenuse * 0.6)
                {
                    break;
                }
            }

			//プレイヤー初期化
			player = new Player(content, playerPosition, Collidable.Collidable.Orientation.East, maze);

			//ゴール初期化
			goalobj = new GoalObject(content, goalPosition, Collidable.Collidable.Orientation.East, maze);

			// Enemyマネージャ初期化
			enemyManager = new EnemyManager(content, selectEnemy, selectStage, maze, player);

			//背景モデルの宣言
			backmodel = new BackGround(content);

			//カメラの初期設定
			camera = new Camera(player);

			// ミニマップの初期化
			miniMap = new MiniMap(content, graphicsDevice);

			// スコアの初期化
            score = new Score(content, graphicsDevice, gameTime, selectStage);

			//フェードアウト処理
			fadeout = new FadeOut(content, graphicsDevice, FadeOut.SceneType.InGame);

			// カウントダウン
			countdown = new CountDown(content, graphicsDevice, gameTime);

			// BGMの再生
			cueBGM = SoundManager.Instance.SoundBank.GetCue("game");
		}


		/// <summary>
        /// 描画
        /// </summary>
        /// <param name="spriteBatch">スプライトバッチ</param>
        /// <param name="graphicsDevice">グラフィックデバイス</param>
        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            //奥行設定をON
            graphicsDevice.DepthStencilState = DepthStencilState.Default;

			//ゴールオブジェクトの描画
			goalobj.Draw(camera);

			//プレイヤー描画
            player.Draw(camera);

            //エネミー描画
			enemyManager.Draw(camera);

			//背景描画
			//backmodel.Draw(camera);

			//迷路の描画
			maze.Draw(camera, player.Position2, player.DirecState, player.currentMoveState);

			// ミニマップの描画
			miniMap.Draw(spriteBatch);

			// スコアの描画
			score.Draw(spriteBatch);

			//フェードアウトの描画
			fadeout.Draw(spriteBatch);

			// カウントダウン描画開始
			countdown.Draw(spriteBatch);
        }

        /// <summary>
        /// 更新処理
        /// </summary>
        /// <returns>次状態</returns>
        public SceneInterface Update(GameTime gameTime)
        {
			if (countdown.Counting)
			{
				countdown.Update(gameTime);
				return this;
			}

			if (countdown.Finished)
			{
				countdown.Finished = false;
				cueBGM.Play();
			}

			//プレイヤーの更新
			if (!fadeout.m_isFadeOut)
				player.Update(gameTime, maze);

            //エネミーの更新
			enemyManager.Update(gameTime, goalobj);

			//カメラの更新
            camera.Update(player);

			// ミニマップの更新
			miniMap.Update(maze, player.Position3, player.Position2, player.Rotate, goalobj.Position3);

			if (!fadeout.m_isFadeOut)
				// スコアの更新
				score.Update(player, gameTime);

			//フェードアウトの更新
			fadeout.Update();

			//エネミーとプレイヤーの当たり判定
			if (enemyManager.Collision(player))
			{
				//ダメージアニメーション開始
				player.DamegedFlag = true;
			}

            // ダメージを受けたフレームで残機を減らす
            if (player.justDamaged)
            {
				score.penaltyTime++;
            }
           

			//ゲームオーバー処理
            if (score.RemaningTime <= TimeSpan.Zero) 
			{
				// BGMの停止
				cueBGM.Stop(AudioStopOptions.AsAuthored);
				//フェードアウト開始
				fadeout.StartFadeOut(false);
			}

			//クリア条件チェック
			if (goalobj.Collision(player))
			{
				//フェードアウト開始
				fadeout.StartFadeOut(true);
			}

			//デバッグゴール
			if (Input.Instance.PushKey(Keys.O))
			{
				//フェードアウト開始
				fadeout.StartFadeOut(true);
			}

			// ゲームオーバーボタン
			if (Input.Instance.PushKey(Keys.L))
			{
				// BGMの停止
				cueBGM.Stop(AudioStopOptions.AsAuthored);
				//フェードアウト開始
				fadeout.StartFadeOut(false);
			}

			// フェードアウト終了後処理
			if(fadeout.EndFadeOut && Input.Instance.PushKey(Keys.Space))
				if (fadeout.ClearFlag)
				{
					// BGMの停止
					cueBGM.Stop(AudioStopOptions.AsAuthored);
					return new Ranking(content, graphicsDevice, score.RemaningTime, selectStage);
				}
				else
				{
					return new Ranking(content, graphicsDevice, selectStage);
				}

            return this;
        }

        /// <summary>
        /// 終了処理
        /// </summary>
        public void Unload()
        {
        }
    }

}
