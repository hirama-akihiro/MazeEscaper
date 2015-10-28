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
    /// エネミー1クラス。うろうろする敵。
    /// </summary>
    class Enemy1 : Enemy
    {
        /// <summary>
        /// 乱数生成器
        /// </summary>
        Random rand = new Random();
    
        /// <summary>
        /// 次にとる行動
        /// </summary>
        Queue<MoveState> moveStateQ = new Queue<MoveState>();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="content"></param>
        /// <param name="startPosition"></param>
        /// <param name="orient"></param>
        public Enemy1(ContentManager content, Point startPosition, Orientation orient)
            : base(content,startPosition, orient)
        {
            //モデルの移動速度
            speed = 0.05;
            //モデルの回転速度
            degreespeed = 2.0f;

            //2D->3D
            //PointToVecto3(position2, ref position3, maze);
            position3 = new Vector3(2.8f, 0.0f, 2.8f);
            nextposition3 = new Vector3(2.8f, 0.0f, 2.8f);
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
            Orientation? orientation = null;
            while (true)
            {
                orientation = (Orientation)rand.Next(4);
                // 壁じゃない＆後ろじゃないならbreak
                if (CanMove(orientation.Value, maze) && orientation.Value != DirecState.Back())
                {
                    break;
                }
            }

            // そこに動くために必要な動きを得る
            MoveState[] movement = RequireMovement(orientation.Value, maze);

            // 動きをキューに入れる
            foreach (MoveState m in movement) {
                moveStateQ.Enqueue(m);
            }

            // 動く
            Move(moveStateQ.Dequeue(), maze);

			// アニメーションの開始
			AnimationStart("walk");
        }
    }
}
