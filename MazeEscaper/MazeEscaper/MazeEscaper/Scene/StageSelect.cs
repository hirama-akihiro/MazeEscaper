using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace MazeEscaper.Scene
{
    class StageSelect:SceneInterface
    {
        //シーンインターフェース共通変数
        private ContentManager content;
        private GraphicsDevice graphicsDevice;

		private Texture2D background;
		private Texture2D frame;
		private Texture2D itemBox;
		private Texture2D enter;
		private Texture2D config;
		private Texture2D stage;
		private Texture2D enemy;
		private Texture2D start;
		private Texture2D small;
		private Texture2D regular;
		private Texture2D big;
		private Texture2D few;
		private Texture2D much;
		private Texture2D selectLine;
		private Texture2D selectItem;

		private readonly Vector2 configPosition;
		private readonly Vector2 stagePosition;
		private readonly Vector2 stageSelectPosition;
		private readonly Vector2 enemyPosition;
		private readonly Vector2 enemySelectPosition;
		private readonly Vector2 startPosition;
		private readonly Vector2 targetPosition;

		private Cue cueBGM;

		private Select select;
		private SelectStage selectStage;
		private SelectEnemy selectEnemy;

		public enum Select { Stage, Enemy, Start }
		public enum SelectStage { Small, Regular, Big }
		public enum SelectEnemy { Few, Regular, Much }

		public StageSelect(ContentManager content, GraphicsDevice graphicsDevice)
		{
			this.content = content;
			this.graphicsDevice = graphicsDevice;

			background = content.Load<Texture2D>("Config/background");
			frame = content.Load<Texture2D>("Config/frame");
			itemBox = content.Load<Texture2D>("Config/item_box");
			enter = content.Load<Texture2D>("Config/enter");
			config = content.Load<Texture2D>("Config/config");
			stage = content.Load<Texture2D>("Config/stage");
			enemy = content.Load<Texture2D>("Config/enemy");
			start = content.Load<Texture2D>("Config/start");
			small = content.Load<Texture2D>("Config/small");
			regular = content.Load<Texture2D>("Config/regular");
			big = content.Load<Texture2D>("Config/big");
			few = content.Load<Texture2D>("Config/few");
			much = content.Load<Texture2D>("Config/much");
			selectLine = content.Load<Texture2D>("Config/select_line");
			selectItem = content.Load<Texture2D>("Config/select_item");

			configPosition = new Vector2(GameMain.ScreenWidth / 2.0f, 70);
			stagePosition = new Vector2(GameMain.ScreenWidth / 2.0f, 170);
			stageSelectPosition = new Vector2(GameMain.ScreenWidth / 2.0f, 230);
			enemyPosition = new Vector2(GameMain.ScreenWidth / 2.0f, 320);
			enemySelectPosition = new Vector2(GameMain.ScreenWidth / 2.0f, 380);
			startPosition = new Vector2(GameMain.ScreenWidth / 2.0f, 490);
			targetPosition = new Vector2(200, 0);

			select = Select.Stage;
			selectStage = SelectStage.Regular;
			selectEnemy = SelectEnemy.Regular;

			cueBGM = SoundManager.Instance.SoundBank.GetCue("config");
			cueBGM.Play();
		}

		public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
		{
			spriteBatch.Begin();

			spriteBatch.Draw(background, Vector2.Zero, Color.White);
			spriteBatch.Draw(frame, Vector2.Zero, Color.White);
			spriteBatch.Draw(config, configPosition - new Vector2(config.Width, config.Height) / 2, Color.White);
			switch (select)
			{
				case Select.Stage:
					spriteBatch.Draw(selectLine, stageSelectPosition - new Vector2(0, itemBox.Height) / 2 - new Vector2(selectLine.Width, selectLine.Height) / 2, Color.White);
					break;
				case Select.Enemy:
					spriteBatch.Draw(selectLine, enemySelectPosition - new Vector2(0, itemBox.Height) / 2 - new Vector2(selectLine.Width, selectLine.Height) / 2, Color.White);
					break;
				case Select.Start:
					spriteBatch.Draw(selectLine, startPosition - new Vector2(selectLine.Width, selectLine.Height) / 2, Color.White);
					break;
			}

			spriteBatch.Draw(stage, stagePosition - new Vector2(stage.Width / 2.0f, stage.Height / 2.0f), Color.White);
			spriteBatch.Draw(itemBox, stageSelectPosition - new Vector2(itemBox.Width, itemBox.Height) / 2, Color.White);
			DrawSelect(spriteBatch, selectItem, stageSelectPosition, (int)selectStage);
			spriteBatch.Draw(small, stageSelectPosition - new Vector2(targetPosition.X + small.Width / 2.0f, small.Height / 2.0f), Color.White);
			spriteBatch.Draw(regular, stageSelectPosition - new Vector2(regular.Width / 2.0f, regular.Height / 2.0f), Color.White);
			spriteBatch.Draw(big, stageSelectPosition - new Vector2(-targetPosition.X + big.Width / 2.0f, big.Height / 2.0f), Color.White);

			spriteBatch.Draw(enemy, enemyPosition - new Vector2(enemy.Width / 2.0f, enemy.Height / 2.0f), Color.White);
			spriteBatch.Draw(itemBox, enemySelectPosition - new Vector2(itemBox.Width, itemBox.Height) / 2, Color.White);
			DrawSelect(spriteBatch, selectItem, enemySelectPosition, (int)selectEnemy);
			spriteBatch.Draw(few, enemySelectPosition - new Vector2(targetPosition.X + few.Width / 2.0f, few.Height / 2.0f), Color.White);
			spriteBatch.Draw(regular, enemySelectPosition - new Vector2(regular.Width / 2.0f, regular.Height / 2.0f), Color.White);
			spriteBatch.Draw(much, enemySelectPosition - new Vector2(-targetPosition.X + much.Width / 2.0f, much.Height / 2.0f), Color.White);

			spriteBatch.Draw(enter, startPosition - new Vector2(enter.Width, enter.Height) / 2, Color.White);
			spriteBatch.Draw(start, startPosition - new Vector2(start.Width / 2.0f, start.Height / 2.0f), Color.White);

			spriteBatch.End();
		}

		private void DrawSelect(SpriteBatch spriteBatch, Texture2D texture, Vector2 position, int target)
		{
			spriteBatch.Draw(texture, position - new Vector2(-targetPosition.X * (target - 1) + texture.Width / 2.0f, texture.Height / 2.0f), Color.White);
		}

		public SceneInterface Update(GameTime gameTime)
		{
			// Spaceキー入力でゲーム画面へ移行
			if (Input.Instance.PushKey(Keys.Space) && select == Select.Start)
			{
				SoundManager.Instance.SoundBank.PlayCue("ok");
				cueBGM.Stop(AudioStopOptions.AsAuthored);
				return new GameIn(content, graphicsDevice, gameTime, selectStage, selectEnemy);
			}
			// タイトルへ
			if (Input.Instance.PushKey(Keys.B))
			{
				SoundManager.Instance.SoundBank.PlayCue("cancel");
				cueBGM.Stop(AudioStopOptions.AsAuthored);
				return new Title(content, graphicsDevice);
			}

			// 前の状態を保存
			Select prevSelect = select;
			SelectStage prevSelectStage = selectStage;
			SelectEnemy prevSelectEnemy = selectEnemy;

			if (Input.Instance.PushKey(Keys.Up))
				select--;
			if (Input.Instance.PushKey(Keys.Down))
				select++;
			if (select < 0)
				select = 0;
			if ((int)select >= Enum.GetNames(typeof(Select)).Length)
				select = (Select)Enum.GetNames(typeof(Select)).Length - 1;

			if (Input.Instance.PushKey(Keys.Left))
				switch (select)
				{
					case Select.Stage:
						selectStage--;
						break;
					case Select.Enemy:
						selectEnemy--;
						break;
				}
			if (Input.Instance.PushKey(Keys.Right))
				switch (select)
				{
					case Select.Stage:
						selectStage++;
						break;
					case Select.Enemy:
						selectEnemy++;
						break;
				}
			if (selectStage < 0)
				selectStage = 0;
			if ((int)selectStage >= Enum.GetNames(typeof(SelectStage)).Length)
				selectStage = (SelectStage)Enum.GetNames(typeof(SelectStage)).Length - 1;
			if (selectEnemy < 0)
				selectEnemy = 0;
			if ((int)selectEnemy >= Enum.GetNames(typeof(SelectEnemy)).Length)
				selectEnemy = (SelectEnemy)Enum.GetNames(typeof(SelectEnemy)).Length - 1;

			// 前の状態から変わっていたら音を鳴らす
			if (select != prevSelect || selectStage != prevSelectStage || selectEnemy != prevSelectEnemy)
				SoundManager.Instance.SoundBank.PlayCue("select");

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
