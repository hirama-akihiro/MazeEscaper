using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using MazeEscaper.Scene;


namespace MazeEscaper
{
    /// <summary>
    /// ゲームメインクラス
    /// </summary>
    class GameMain
    {

        //画面解像度
        private const int screenWidth = 800;
        private const int screenHeight = 600;

        //シーンインターフェース
        private SceneInterface scene;

        private static GameTime maingametime;

        /// <summary>
        /// ゲームメインの生成
        /// </summary>
        /// <param name="content"></param>
        /// <param name="graphicsdevice"></param>
        /// <param name="game"></param>
        public GameMain(ContentManager content, GraphicsDevice graphicsDevice, Game game)
        {
            game.Components.Add(Input.Instance);
            scene = new Title(content, graphicsDevice);
        }

        /// <summary>
        /// 更新
        /// </summary>
        public void Update(GameTime gameTime)
        {
            maingametime = gameTime;
            scene = scene.Update(gameTime);
        }

        /// <summary>
        /// 描画
        /// </summary>
        /// <param name="spritebatch"></param>
        /// <param name="graphicsdevice"></param>
        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            scene.Draw(spriteBatch, graphicsDevice);
        }

        /// <summary>
        /// アンロード
        /// </summary>
        public void Unload()
        {

        }

        #region //プロパティ

        /// <summary>
        /// 画面の高さ
        /// </summary>
        public static int ScreenWidth
        {
            get { return screenWidth; }
        }

        /// <summary>
        /// 画面の横幅
        /// </summary>
        public static int ScreenHeight
        {
            get { return screenHeight; }  
        }

        public static GameTime MainGameTime
        {
            get { return maingametime; }
        }

        #endregion

    }
}
