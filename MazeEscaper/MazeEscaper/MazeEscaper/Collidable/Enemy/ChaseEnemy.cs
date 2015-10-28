using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MazeEscaper.MazeMap;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;


namespace MazeEscaper.Collidable.Enemy
{
    /// <summary>
    /// 追いかける敵（未完成）
    /// </summary>
    class ChaseEnemy : Enemy
    {
        /// <summary>
        /// 乱数生成器
        /// </summary>
        Random rand = new Random();

        /// <summary>
        /// Search時のプレイヤーの場所
        /// </summary>
        Point playerPosition;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="content"></param>
        /// <param name="startPosition"></param>
        /// <param name="orient"></param>
        /// <param name="maze"></param>
        /// <param name="player"></param>
		public ChaseEnemy(ContentManager content, Point startPosition, Orientation orient, Maze maze, Player player)
			: base(content, @"Objects\Enemy2\enemy2", startPosition, orient, maze, player)
		{
			// 乱数調整
			for (int i = 0; i < 20000; i++)
			{
				rand.Next();
			}
		}

        /// <summary>
        /// 更新処理
        /// </summary>
        /// <param name="maze"></param>
        public override void Update(GameTime gameTime, Maze maze)
        {
            // アニメーションの更新
            AnimationUpdate(gameTime);

			// プレイヤーから見えない場所にいたらこっそり高速移動する
			if (Euclidean(position2, player.Position2) > 20)
			{
				speed = 0.3;
				degreespeed = 15f;
			}
			else
			{
                speed = 0.03;
                degreespeed = 1.5f;
			}

            // 動作中ならなにもしないで終了
            if (currentMoveState != MoveState.Stop)
            {
                Move(currentMoveState, maze);

                return;
            }

            // Search時からプレイヤーが5マス離れたら再計算
            if (Manhattan(playerPosition, player.Position2) > 2 * 5)
            {
                ClearQ();
            }

            // たまっている動作があればそれを実行して終了
            if (moveStateQ.Count > 0)
            {
                Move(moveStateQ.Dequeue(), maze);
                // アニメーションの開始
                AnimationStart("walk");
                return;
            }

            // たまっている経路があれば動作を求めて終了
            if (pathQ.Count > 0)
            {
                StoreMovenents(Orientations(pathQ.Dequeue())[0]);
                return;
            }

            // プレイヤーの場所を保存しておく
            playerPosition = player.Position2;

            // プレイヤーへの経路を得る
            Point[] path = Search(player.Position2);

            // 経路をキューに溜める
            foreach (Point p in path)
            {
                pathQ.Enqueue(p);
            }
        }
    }
}
