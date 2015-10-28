using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MazeEscaper.Collidable;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MazeEscaper.Scene;

namespace MazeEscaper
{

    class Score
    {
        private ContentManager content;
        private GraphicsDevice graphicsDevice;

        /// <summary>
        /// ダメージを受けた回数
        /// </summary>
        public int penaltyTime = 0;

        /// <summary>
        /// ゲーム開始時刻
        /// </summary>
        private GameTime startGameTime;

        /// <summary>
        /// ゲーム開始からの時間
        /// </summary>
        private TimeSpan totalTime;

        /// <summary>
        /// タイムリミット
        /// </summary>
        private TimeSpan timeLimit;

        /// <summary>
        /// 残り時間
        /// </summary>
        private TimeSpan remaningTime;

        /// <summary>
        /// 数字の画像
        /// </summary>
        private Texture2D[] numbers;

        private Texture2D black;

        /// <summary>
        /// スプライトでテキストを描画するためのフォント
        /// </summary>
        private SpriteFont font;

        public Score(ContentManager content, GraphicsDevice graphicsDevice, GameTime gameTime, StageSelect.SelectStage selectStage)
        {
            // スタート時のゲーム時間を保存
            startGameTime = new GameTime(gameTime.TotalGameTime, gameTime.ElapsedGameTime);

            this.content = content;
            this.graphicsDevice = graphicsDevice;
            LoadContent();

            switch (selectStage)
            {
                case StageSelect.SelectStage.Small:
                    timeLimit = new TimeSpan(0, 3, 5);
                    break;
                case StageSelect.SelectStage.Regular:
                    timeLimit = new TimeSpan(0, 3, 35);
                    break;
                case StageSelect.SelectStage.Big:
					timeLimit = new TimeSpan(0, 4, 5);
                    break;
            }

        }

        /// <summary>
        /// コンテンツのロードメソッド
        /// </summary>
        public void LoadContent()
        {
            this.font = this.content.Load<SpriteFont>("Font");

            const string color = "Orange";

            // 数字の読み込み
            numbers = new Texture2D[11];
            for (int i = 0; i < 10; i++)
            {
                numbers[i] = content.Load<Texture2D>("Score/" + color + "/" + i);
            }

            numbers[10] = content.Load<Texture2D>("Score/" + color + "/colon");

            black = content.Load<Texture2D>("Score/black");
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // スプライトの描画準備
            spriteBatch.Begin();

            // 時間を表示
            if (totalTime != null)
            {
                spriteBatch.Draw(black, new Vector2(8, 8), null, Color.White * 0.5f, 0.0f, Vector2.Zero, new Vector2(96, 48), SpriteEffects.None, 0.0f);
                string time = String.Format("{0:D2}:{1:D2}", remaningTime.Minutes, remaningTime.Seconds);
                for (int i = 0; i < 5; i++)
                {
                    spriteBatch.Draw(numbers[time[i] - 0x30], new Vector2(i * 16 + 16, 16), Color.White);
                }
                //spriteBatch.DrawString(this.font, String.Format("{0:D2}:{1:D2}", remaningTime.Minutes, remaningTime.Seconds), Vector2.Zero, Color.White);
            }

            // スプライトの一括描画
            spriteBatch.End();
        }

        public void Update(Player player, GameTime gameTime)
        {
            // 経過時間を計算
            totalTime = gameTime.TotalGameTime - startGameTime.TotalGameTime;

            // ペナルティを受けた回数だけ経過時間を10秒足す
            for (int i = 0; i < penaltyTime; i++)
            {
                totalTime += new TimeSpan(0, 0, 10);
            }

            // 残り時間を計算
            remaningTime = timeLimit - totalTime;
            if (remaningTime < TimeSpan.Zero)
            {
                remaningTime = TimeSpan.Zero;
            }
        }

        #region プロパティ
        public TimeSpan RemaningTime { get { return remaningTime; } }
        #endregion
    }
}
