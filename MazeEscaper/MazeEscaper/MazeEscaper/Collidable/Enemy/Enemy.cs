using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MazeEscaper.MazeMap;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace MazeEscaper.Collidable.Enemy
{
    abstract class Enemy : AnimationModel
    {
        /// <summary>
        /// プレイヤー
        /// </summary>
        protected Player player;

        /// <summary>
        /// 迷路
        /// </summary>
        private Maze maze;

        /// <summary>
        /// 経路をためておくキュー
        /// </summary>
        protected Queue<Point> pathQ = new Queue<Point>();

        /// <summary>
        /// 動作をためておくキュー。pathQから自動生成される
        /// </summary>
        protected Queue<MoveState> moveStateQ = new Queue<MoveState>();


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="content"></param>
        /// <param name="startPosition"></param>
        /// <param name="orient"></param>
        /// <param name="maze"></param>
        /// <param name="player"></param>
        public Enemy(ContentManager content, string assetName, Point startPosition, Orientation orient, Maze maze, Player player)
			: base(content, assetName, startPosition, orient, new Vector3(0.35f, 0.35f, 0.35f), maze)
        {
            //モデルの移動速度
            speed = 0.03;
            //モデルの回転速度
            degreespeed = 1.5f;
			//モデルの半径
			Radius = 1.2f;
            // プレイヤー
            this.player = player;
            // 迷路
            this.maze = maze;
        }


		/// <summary>
        /// 2位置間のユークリッド距離を計算する
        /// </summary>
        /// <param name="p1">位置1</param>
        /// <param name="p2">位置2</param>
		/// <returns>ユークリッド距離</returns>
		protected double Euclidean(Point p1, Point p2)
        {
			return Euclidean(p1.X, p1.Y, p2.X, p2.Y);
        }


        /// <summary>
		/// 2位置間のユークリッド距離を計算する
        /// </summary>
        /// <param name="x1">位置1のx座標</param>
        /// <param name="y1">位置1のy座標</param>
        /// <param name="x2">位置2のx座標</param>
        /// <param name="y2">位置2のy座標</param>
		/// <returns>ユークリッド距離</returns>
		protected double Euclidean(int x1, int y1, int x2, int y2)
        {
			return Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
        }

        /// <summary>
        /// 2位置間のマンハッタン距離を計算する
        /// </summary>
        /// <param name="p1">位置1</param>
        /// <param name="p2">位置2</param>
        /// <returns>マンハッタン距離</returns>
        protected int Manhattan(Point p1, Point p2)
        {
            return Manhattan(p1.X, p1.Y, p2.X, p2.Y);
        }


        /// <summary>
        /// 2位置間のマンハッタン距離を計算する
        /// </summary>
        /// <param name="x1">位置1のx座標</param>
        /// <param name="y1">位置1のy座標</param>
        /// <param name="x2">位置2のx座標</param>
        /// <param name="y2">位置2のy座標</param>
        /// <returns>マンハッタン距離</returns>
        protected int Manhattan(int x1, int y1, int x2, int y2)
        {
            return Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
        }

		/// <summary>
		/// たまっている動作と経路を削除する
		/// </summary>
		public void ClearQ()
		{
			moveStateQ.Clear();
			pathQ.Clear();
		}


        /// <summary>
        /// 指定した地点にテレポートする
        /// </summary>
        /// <param name="destintaion">目的地</param>
        public void Teleport(Point destintaion)
        {
            ClearQ();
            currentMoveState = MoveState.Stop;
            DirecState = Orientation.North;
            rotate = OrientationToRotate(DirecState);
            degree = OrientationToDegree(DirecState);

            nextposition2 = position2 = destintaion;
            nextposition3 = position3 = PointToVecto3(position2, maze);
        }


        /// <summary>
        /// 指定した地点にテレポートする
        /// </summary>
        /// <param name="x">x座標</param>
        /// <param name="y">y座標</param>
        protected void Teleport(int x, int y)
        {
            Teleport(new Point(x, y));
        }


        /// <summary>
        /// 現在位置から目的位置への方位を得る
        /// </summary>
        /// <param name="destination">目的位置</param>
        /// <returns>方位</returns>
        protected Orientation[] Orientations(Point destination)
        {
            return Orientations(position2, destination);
        }


        /// <summary>
        /// 目的位置への方位を得る
        /// </summary>
        /// <param name="origin">出発位置</param>
        /// <param name="destination">目的位置</param>
        /// <returns>方位</returns>
        protected Orientation[] Orientations(Point origin, Point destination)
        {
            // 目的地と現在位置の差を取る
            int dx = origin.X.CompareTo(destination.X);
            int dy = origin.Y.CompareTo(destination.Y);

            // 結果
            List<Orientation> result = new List<Orientation>();

            // X方向について調べる
            switch (dx)
            {
                case -1:
                    result.Add(Orientation.East);
                    break;
                case 1:
                    result.Add(Orientation.West);
                    break;
            }

            // Y方向について調べる
            switch (dy)
            {
                case -1:
                    result.Add(Orientation.South);
                    break;
                case 1:
                    result.Add(Orientation.North);
                    break;
            }

            return result.ToArray();
        }


        /// <summary>
        /// 現在位置から目的位置に行く経路を得る
        /// </summary>
        /// <param name="destination">目的位置</param>
        /// <returns>経路</returns>
        protected virtual Point[] Search(Point destination)
        {
            return Search(position2, destination);
        }


        /// <summary>
        /// 目的位置に行く経路を得る
        /// </summary>
        /// <param name="newDestination">目的位置</param>
        /// <returns>経路</returns>
        protected virtual Point[] Search(Point origin, Point destination)
        {
            // 現在位置と目的位置が同じだったら何もしないでおわり
            if (origin == destination)
            {
                return new Point[] { };
            }

            // 探索距離（2の倍数、マンハッタン距離）
            const int searchLimit = 16;

            // 1辺の探索距離
            const int searchLimitOfEachSide = searchLimit / 2;

            // 全方位
            Orientation[] orientations = new[] { Orientation.North, Orientation.East, Orientation.South, Orientation.West };

            Point newDestination = destination;

            // もし目的位置が遠すぎたら探索に時間がかかるので近くにする
            if (Manhattan(origin.X, origin.Y, newDestination.X, newDestination.Y) > searchLimit)
            {
                // x方向
                if (origin.X - newDestination.X > searchLimitOfEachSide)
                {
                    newDestination.X = origin.X - searchLimitOfEachSide;
                }
                else if (newDestination.X - origin.X > searchLimitOfEachSide)
                {
                    newDestination.X = origin.X + searchLimitOfEachSide;
                }
                // y方向
                if (origin.Y - newDestination.Y > searchLimitOfEachSide)
                {
                    newDestination.Y = origin.Y - searchLimitOfEachSide;
                }
                else if (newDestination.Y - origin.Y > searchLimitOfEachSide)
                {
                    newDestination.Y = origin.Y + searchLimitOfEachSide;
                }
            }

            // どのノードから来たのか
            Dictionary<Point, Point> passed = new Dictionary<Point, Point>();

            // 探索する位置を格納するキュー
            Queue<Point> q = new Queue<Point>();

            // 探索を開始する位置
            Point v = new Point(origin.X, origin.Y);
            passed.Add(v, v);
            q.Enqueue(v);

            // 幅優先探索
            while (q.Count > 0)
            {
                Point u = q.Dequeue();
                foreach (Orientation o in orientations)
                {
                    if (CanMove(o, maze, u))
                    {
                        // 次の位置vを求める
                        v.X = u.X;
                        v.Y = u.Y;
                        switch (o)
                        {
                            case Orientation.North:
                                v.Y -= 2;
                                break;
                            case Orientation.East:
                                v.X += 2;
                                break;
                            case Orientation.South:
                                v.Y += 2;
                                break;
                            case Orientation.West:
                                v.X -= 2;
                                break;
                        }

                        if (!passed.ContainsKey(v))
                        {
                            passed.Add(v, u);

                            // 目的位置を発見したら
                            if (v == newDestination)
                            {
                                LinkedList<Point> result = new LinkedList<Point>();
                                // vを使うと紛らわしいので別名をつける
                                Point d = v;

                                // 経路をresultに書いてreturn
                                while (passed[d] != d)
                                {
                                    result.AddFirst(d);
                                    d = passed[d];
                                }
                                return result.ToArray();
                            }
                            q.Enqueue(v);
                        }
                    }
                }
            }

            return new Point[] { };
        }


        /// <summary>
        /// 指定した方角に行くためにどんな動作が必要か調べてキューに入れる
        /// </summary>
        /// <param name="orientation">行きたい方角</param>
        protected virtual void StoreMovenents(Orientation orientation)
        {
            // 向いている方角と行きたい方角の差を求める
            int directionDiff = (DirecState - orientation + 4) % 4;

            switch (directionDiff)
            {
                case 0:
                    // 行きたい方向を向いているとき前進
                    moveStateQ.Enqueue(MoveState.Advance);
                    break;
                case 1:
                    // 行きたい方向が左のとき左回転，前進
                    moveStateQ.Enqueue(MoveState.TurnLeft);
                    moveStateQ.Enqueue(MoveState.Advance);
                    break;
                case 2:
                    // 行きたい方向が後ろのとき左回転，左回転，前進
                    moveStateQ.Enqueue(MoveState.TurnLeft);
                    moveStateQ.Enqueue(MoveState.TurnLeft);
                    moveStateQ.Enqueue(MoveState.Advance);
                    break;
                case 3:
                    // 行きたい方向が右のとき右回転，前進
                    moveStateQ.Enqueue(MoveState.TurnRight);
                    moveStateQ.Enqueue(MoveState.Advance);
                    break;
            }
        }
    }
}
