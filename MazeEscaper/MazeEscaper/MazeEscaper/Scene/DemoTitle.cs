//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Content;
//using MazeEscaper.Collidable;
//using MazeEscaper.MazeMap;
//using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework;

//namespace MazeEscaper.Scene
//{
//    class DemoTitle : SceneInterface
//    {
//        private ContentManager content;
//        private GraphicsDevice graphicsDevice;

//        private DemoPlayer demoplayer;

//        private GoalObject obfix;

//        private Maze maze;

//        private Camera camera;

//        /// <summary>
//        /// タイトル画面のスケール
//        /// </summary>
//        private Vector2 backScale;

//        private Vector2 titleScale;
//        private Vector2 titleOffset;

//        private Vector2 explanscale;
//        private Vector2 explanoffset;

//        /// <summary>
//        /// タイトル文字
//        /// </summary>
//        private Texture2D titleStr;

//        /// <summary>
//        /// タイトル画面の背景
//        /// </summary>
//        private Texture2D background;

//        /// <summary>
//        /// 説明文字
//        /// </summary>
//        private Texture2D explanation;

//        // フェードアウト
//        private float m_alpha;
//        private float m_alphaIncAmout;
//        private bool m_isFadeOut = false;
//        private Rectangle screenBound;
//        private Color color;

//        /// <summary>
//        /// メインゲームシーン
//        /// </summary>
//        /// <param name="content"></param>
//        /// <param name="graphicsDevice"></param>
//        /// <param name="input"></param>
//        public DemoTitle(ContentManager content, GraphicsDevice graphicsDevice)
//        {
//            this.content = content;
//            this.graphicsDevice = graphicsDevice;

//            demoplayer = new DemoPlayer(content, graphicsDevice);
//            obfix = new GoalObject(content, graphicsDevice);

//            maze = new Maze(content, 4, 5);

//            //camera = new Camera(GameMain.ScreenWidth, GameMain.ScreenHeight);
//            camera = new Camera(demoplayer);

//            LoadContent();

//            //スケールの設定
//            backScale = new Vector2((float)GameMain.ScreenWidth / background.Width, (float)GameMain.ScreenHeight / background.Height);
//            titleScale = new Vector2(0.75f, 0.75f);
//            titleOffset = new Vector2(20, 50);

//            explanscale = new Vector2(0.5f, 0.5f);
//            explanoffset = new Vector2(100, 300);

//            //フェードアウトの初期化
//            m_alpha = 0.0f;
//            m_alphaIncAmout = 0.008f;
//            //フェードアウト描画サイズ
//            screenBound = new Rectangle(0, 0, GameMain.ScreenWidth, GameMain.ScreenHeight);
//            //黒色でフェードアウト
//            color = new Color(0.0f, 0.0f, 0.0f, m_alpha);
//        }

//        /// <summary>
//        /// コンテンツのロードメソッド
//        /// </summary>
//        public void LoadContent()
//        {
//            this.background = content.Load<Texture2D>("Title/TitleBack");
//            this.titleStr = content.Load<Texture2D>("Title/Title");
//            this.explanation = content.Load<Texture2D>("Title/explan");
//        }

//        /// <summary>
//        /// 描画
//        /// </summary>
//        /// <param name="spriteBatch">スプライトバッチ</param>
//        /// <param name="graphicsDevice">グラフィックデバイス</param>
//        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
//        {
//            //player.Draw(spriteBatch, graphicsDevice);
//            //obfix.Draw(spriteBatch, graphicsDevice);

//            spriteBatch.Begin();

//            //奥行設定をON
//            graphicsDevice.DepthStencilState = DepthStencilState.Default;

//            demoplayer.Draw(camera);

//            obfix.Draw(camera);

//            maze.Draw(camera);

//            //spriteBatch.Draw(background, Vector2.Zero, null, Color.White, 0.0f, Vector2.Zero, backScale, SpriteEffects.None, 0.0f);
//            spriteBatch.Draw(titleStr, titleOffset, null, Color.White, 0.0f, Vector2.Zero, titleScale, SpriteEffects.None, 0.0f);
//            spriteBatch.Draw(explanation, explanoffset, null, Color.White, 0.0f, Vector2.Zero, explanscale, SpriteEffects.None, 0.0f);

//            //フェードアウト画面描画
//            if (m_isFadeOut)
//            {
//                color = new Color(0.0f, 0.0f, 0.0f, m_alpha);
//                spriteBatch.Draw(background, screenBound, color);
//                updateFadeOut();
//            }

//            spriteBatch.End();

//        }

//        /// <summary>
//        /// 更新処理
//        /// </summary>
//        /// <returns>次状態</returns>
//        public SceneInterface Update()
//        {
//            demoplayer.Move();

//            //camera.Update();
//            camera.Update(demoplayer);

//            //スペースキー入力でゲーム画面へ移行
//            if (Input.Instance.PushKey(Keys.A))
//            {
//                m_isFadeOut = true;
//            }

//            //フェードアウトの更新
//            if (m_isFadeOut)
//            {
//                updateFadeOut();
//            }

//            //フェードアウト後新しい画面に更新
//            if (m_alpha == 1.0f)
//            {
//                return new StageSelect(content, graphicsDevice);
//            }

//            return this;
//        }

//        /// <summary>
//        /// 終了処理
//        /// </summary>
//        public void Unload()
//        {
//        }

//        /// <summary>
//        /// フェードアウト処理
//        /// </summary>
//        private void updateFadeOut()
//        {
//            m_alpha += m_alphaIncAmout;
//            if (m_alpha >= 1.0f)
//            {
//                m_alpha = 1.0f;
//            }
//        }

//    }

//}
