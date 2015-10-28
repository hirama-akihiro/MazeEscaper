using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MazeEscaper.Scene;
using Microsoft.Xna.Framework.Content;
using MazeEscaper.MazeMap;
using Microsoft.Xna.Framework;

namespace MazeEscaper.Collidable.Enemy
{
	class EnemyManager
	{
		private ContentManager content;
		private Maze maze;
		private Player player;

		/// <summary>
		/// Enemyを保存するリスト
		/// </summary>
        private List<Enemy> enemies = new List<Enemy>();

        private Random rand = new Random();
		
		public EnemyManager(ContentManager content, StageSelect.SelectEnemy selectEnemy, StageSelect.SelectStage selectStage, Maze maze, Player player)
		{
			this.content = content;
			this.maze = maze;
			this.player = player;

            int chaseEnemyNum;
            int randomWalkEnemyNum;
			// 敵数を決める
			switch (selectEnemy)
			{
				case StageSelect.SelectEnemy.Few:
					chaseEnemyNum = 10;
					break;
				case StageSelect.SelectEnemy.Regular:
                    chaseEnemyNum = 15;
					break;
				case StageSelect.SelectEnemy.Much:
					chaseEnemyNum = 20;
					break;
                default:
                    throw new ArgumentException();
			}

            randomWalkEnemyNum = chaseEnemyNum;

            switch (selectStage)
            {
                case StageSelect.SelectStage.Small:
                    randomWalkEnemyNum *= 1;
                    break;
                case StageSelect.SelectStage.Regular:
                    randomWalkEnemyNum *= 2;
                    break;
                case StageSelect.SelectStage.Big:
                    randomWalkEnemyNum *= 3;
                    break;
                default:
                    throw new ArgumentException();
            }            

			// リストにChaseEnemyを突っ込む
			for (int i = 0; i < chaseEnemyNum; i++)
			{
                Point enemyPosition = GetEnemyInitialPosition(maze, player);
				enemies.Add(new ChaseEnemy(content, enemyPosition, Collidable.Orientation.North, maze, player));
			}

            // リストにRandomWalkEnemyNumを突っ込む
            for (int i = 0; i < randomWalkEnemyNum; i++)
            {
                Point enemyPosition = GetEnemyInitialPosition(maze, player);
                enemies.Add(new RandomWalkEnemy(content, enemyPosition, Collidable.Orientation.North, maze, player, rand.Next()));
            }
		}

        private static Point GetEnemyInitialPosition(Maze maze, Player player)
        {
            // プレイヤーから6ます以上離す
            Point enemyPosition;
            while (true)
            {
                enemyPosition = maze.RandomPoint();
                if (Math.Sqrt((player.Position2.X - enemyPosition.X) * (player.Position2.X - enemyPosition.X) + (player.Position2.Y - enemyPosition.Y) * (player.Position2.Y - enemyPosition.Y)) > 6)
                {
                    return enemyPosition;
                }
            }
         }


		public void Draw(Camera camera)
		{
			foreach (Enemy enemy in enemies)
			{
				enemy.Draw(camera);
			}
		}

        public void Update(GameTime gameTime, GoalObject goal)
        {
            const int deg = 45;

            // ゴールとプレイヤーの角度を求める
            double playerDeg = MathHelper.ToDegrees((float)Math.Atan2(goal.Position2.Y - player.Position2.Y, goal.Position2.X - player.Position2.X));

            // ゴールとプレイヤーの角度を求める
            double playerDistance = Math.Sqrt((goal.Position2.X - player.Position2.X) * (goal.Position2.X - player.Position2.X) + (goal.Position2.Y - player.Position2.Y) * (goal.Position2.Y - player.Position2.Y));

            foreach (Enemy enemy in enemies)
            {
                #region 後ろにいる敵はテレポート
                // ゴールとエネミーの角度を求める
                double enemyDeg = MathHelper.ToDegrees((float)Math.Atan2(goal.Position2.Y - enemy.Position2.Y, goal.Position2.X - enemy.Position2.X));

                // ゴールとプレイヤーの角度、ゴールとエネミーの角度がだいたい等しければフラグを立てる
                bool flag1 = Math.Abs(playerDeg - enemyDeg) < deg || Math.Abs((playerDeg + 180) % 360 - (enemyDeg + 180) % 360) < deg;

                // ゴールとエネミーの角度を求める
                double enemyDistance = Math.Sqrt((goal.Position2.X - enemy.Position2.X) * (goal.Position2.X - enemy.Position2.X) + (goal.Position2.Y - enemy.Position2.Y) * (goal.Position2.Y - enemy.Position2.Y));

                // プレイヤーよりエネミーのほうがゴールより遠ければフラグを立てる
                bool flag2 = playerDistance < enemyDistance;

                // エネミーがプレイヤーから見えていなければフラグを立てる
                bool flag3 = Math.Sqrt((player.Position2.X - enemy.Position2.X) * (player.Position2.X - enemy.Position2.X) + (player.Position2.Y - enemy.Position2.Y) * (player.Position2.Y - enemy.Position2.Y)) > 16;

                // ゴールがプレイヤーから見えていなければフラグを立てる
                bool flag4 = playerDistance > 16;

                // 全条件が揃ったらテレポートさせる
                if (flag1 && flag2 && flag3 && flag4)
                {
                    enemy.Teleport(goal.Position2);
                }


                //const int distance = 10;
                //bool ushiro = false;
                //if (player.DirecState == Collidable.Orientation.North)
                //{
                //    if (enemy.Position2.Y - player.Position2.Y > distance)
                //    {
                //        ushiro = true;
                //    }
                //}
                //else if (player.DirecState == Collidable.Orientation.South)
                //{
                //    if (player.Position2.Y - enemy.Position2.Y > distance)
                //    {
                //        ushiro = true;
                //    }
                //}
                //else if (player.DirecState == Collidable.Orientation.West)
                //{
                //    if (enemy.Position2.X - player.Position2.X > distance)
                //    {
                //        ushiro = true;
                //    }
                //}
                //else if (player.DirecState == Collidable.Orientation.East)
                //{
                //    if (player.Position2.X - enemy.Position2.X > distance)
                //    {
                //        ushiro = true;
                //    }
                //}
                //if (ushiro)
                //{
                //    enemy.Teleport(goal.Position2);
                //}


                #endregion

                enemy.Update(gameTime, maze);
            }
        }

		/// <summary>
		/// Enemyとプレイヤーの当たり判定
		/// </summary>
		/// <param name="player">プレイヤー</param>
		/// <returns>当たったか</returns>
		public bool Collision(Player player)
		{
			foreach (Enemy enemy in enemies)
			{
				// マンハッタン距離が4未満の敵のみ当たり判定
				if (Math.Abs(player.Position2.X - enemy.Position2.X) + Math.Abs(player.Position2.Y - enemy.Position2.Y) < 4)
				{
					if (enemy.Collision(player))
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
