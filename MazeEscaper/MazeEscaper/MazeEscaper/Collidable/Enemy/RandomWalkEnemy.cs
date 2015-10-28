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
    /// うろうろする敵
    /// </summary>
    class RandomWalkEnemy : Enemy
    {
        /// <summary>
        /// 乱数生成器
        /// </summary>
        Random rand;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="content"></param>
        /// <param name="startPosition"></param>
        /// <param name="orient"></param>
		public RandomWalkEnemy(ContentManager content, Point startPosition, Orientation orient, Maze maze, Player player)
			: base(content, @"Objects\Enemy1\enemy1", startPosition, orient, maze, player)
        {
            rand = new Random();
        }

        public RandomWalkEnemy(ContentManager content, Point startPosition, Orientation orient, Maze maze, Player player, int seed)
			: base(content, @"Objects\Enemy1\enemy1", startPosition, orient, maze, player)
        {
            rand = new Random(seed);
        }

        /// <summary>
        /// 更新処理
        /// </summary>
        /// <param name="maze"></param>
        public override void Update(GameTime gameTime, Maze maze)
        {
            // アニメーションの更新
            AnimationUpdate(gameTime);

            // 動作中ならなにもしないで終了
            if (currentMoveState != MoveState.Stop)
            {
                Move(currentMoveState, maze);
                return;
            }

            // たまっている動作があればそれを実行して終了
            if (moveStateQ.Count > 0)
            {
                Move(moveStateQ.Dequeue(), maze);
                return;
            }

            // 動ける方角を探す
            Orientation orientation;
			int tries = 0;
            while (true)
            {
                orientation = (Orientation)rand.Next(4);
                // 後ろ向きなら前向きに変更
                if (orientation == DirecState.Back())
                {
                    orientation = DirecState;
                }

                // 壁じゃないならbreak
                if (CanMove(orientation, maze))
                {
                    break;
                }

				// 20回以上リトライしても決まらなかったらUターン（無限ループ対策）
				if (tries > 20)
				{
					orientation = DirecState.Back();
					break;
				}
				tries++;
            }

			// そこに動くために必要な動きを得る
			StoreMovenents(orientation);

            // 動く
            Move(moveStateQ.Dequeue(), maze);

            // アニメーションの開始
            AnimationStart("walk");
        }
    }
}
