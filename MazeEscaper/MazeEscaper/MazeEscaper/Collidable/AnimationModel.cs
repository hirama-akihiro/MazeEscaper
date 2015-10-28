using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MazeEscaper.MazeMap;
using MazeEscaper.Shader;
using MazeEscaper.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SkinnedModel;

namespace MazeEscaper.Collidable
{
    abstract class AnimationModel : Collidable
    {
        /// <summary>
        /// アニメーションプレイヤー
        /// </summary>
        private AnimationPlayer animationPlayer;
        /// <summary>
        /// アニメーションクリップ集
        /// </summary>
        protected Dictionary<string, AnimationClip> clips;

		/// <summary>
		/// ダメージ中か判定するフラグ
		/// </summary>
		public bool DamegedFlag { get; set; }

		/// <summary>
		/// 1FPSで1増加するダメージ用FPSカウンター
		/// </summary>
		public int damegeCounter;

		/// <summary>
		/// ダメージ中のカラー設定
		/// </summary>
		private float dColor;
        
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="contentManager"></param>
		/// <param name="assetName"></param>
		/// <param name="startPosition"></param>
		/// <param name="orient"></param>
		/// <param name="scale"></param>
		/// <param name="maze"></param>
        public AnimationModel(ContentManager contentManager, string assetName,Point startPosition, Orientation orient, Vector3 scale, Maze maze)
			:base(contentManager, null, startPosition, orient,scale, maze)
        {
            // モデルの読み込み
			model = contentManager.Load<Model>(assetName);

            // スキニングデータ
            SkinningData skinningData = model.Tag as SkinningData;

            if (skinningData == null)
                throw new InvalidOperationException("This model does not contain a SkinningData tag.");

            // アニメーションプレイヤーの初期化
            animationPlayer = new AnimationPlayer(skinningData);
            // クリップ集の取得
            clips = skinningData.AnimationClips;

			//ダメージカウンターの初期化
			DamegedFlag = false;
			damegeCounter = 0;
			dColor = 0.8f;
        }

        /// <summary>
        /// アニメーションの開始
        /// </summary>
        /// <param name="clipName">クリップ名</param>
        protected void AnimationStart(string clipName)
        {
            animationPlayer.StartClip(clips[clipName]);
        }

        /// <summary>
        /// アニメーションの更新
        /// （常に呼び続けて問題ない）
        /// </summary>
        /// <param name="gameTime">GameTime</param>
        protected void AnimationUpdate(GameTime gameTime)
        {
            animationPlayer.Update(gameTime.ElapsedGameTime, true, Matrix.Identity);
			//ダメージ中の時の色更新処理
			UpdateDamegeColor();
		}

		/// <summary>
		/// 描画
		/// </summary>
		/// <param name="camera">カメラ</param>
        public override void Draw(Camera camera)
        {
            // ボーンの取得
            Matrix[] bones = animationPlayer.GetSkinTransforms();
			// ワールド座標
			Matrix world = CalculateWorld(camera);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (SkinnedEffect effect in mesh.Effects)
                {
                    effect.SetBoneTransforms(bones);

					effect.World = world;
                    effect.View = camera.View;
                    effect.Projection = camera.Projection;

                    effect.EnableDefaultLighting();

					effect.DiffuseColor = new Vector3(effect.DiffuseColor.X, dColor, dColor);

                    effect.SpecularColor = new Vector3(0.25f);
                    effect.SpecularPower = 16;
                }
                mesh.Draw();
            }
        }

		/// <summary>
		/// ダメージ判定中のモデルの色を更新
		/// </summary>
		private void UpdateDamegeColor()
		{
			if (!DamegedFlag)
				return;

			if (damegeCounter > 90)
			{
				DamegedFlag = false;
				damegeCounter = 0;
				dColor = 0.8f;
				return;
			}

			//色更新
			if ((damegeCounter % 10) == 0)
				dColor = 0.8f - dColor;

			damegeCounter++;

		}
    }
}
