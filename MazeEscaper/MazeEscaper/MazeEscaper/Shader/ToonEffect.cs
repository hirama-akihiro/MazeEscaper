using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using MazeEscaper.Util;

namespace MazeEscaper.Shader
{
	public class ToonEffect : Effect, IEffectMatrices
	{

		private Texture2D toneTexture;

		private EffectParameter ambientLightColor;
		private LightSettings[] lightSettings;

		private EffectParameter worldMatrix;
		private EffectParameter viewMatrix;
		private EffectParameter projectionMatrix;

		private EffectParameter enableDepthShadow;
		private EffectParameter shadowmap;
		private EffectParameter shadowViewMatrix;
		private EffectParameter shadowProjectionMatrix;

		public ToonEffect(ContentManager content, BasicEffect effect)
			: base(content.Load<Effect>("Effect/toon"))
		{

			// EffectParamterの準備
			ambientLightColor = Parameters["AmbientLightColor"];
			lightSettings = new LightSettings[3];
			for (int i = 0; i < 3; i++)
			{
				lightSettings[i] = new LightSettings(Parameters, "Light" + i);
			}

			worldMatrix = Parameters["World"];
			viewMatrix = Parameters["View"];
			projectionMatrix = Parameters["Projection"];

			enableDepthShadow = Parameters["EnableDepthShadow"];
			shadowmap = Parameters["Shadowmap"];
			shadowViewMatrix = Parameters["ShadowView"];
			shadowProjectionMatrix = Parameters["ShadowProjection"];

			NormalTechnique = effect.TextureEnabled ? Techniques["Toon"] : Techniques["ToonWithoutTexture"];
			CurrentTechnique = NormalTechnique;

			// 値を送る
			toneTexture = content.Load<Texture2D>("Effect/tone");
			Parameters["ToneTexture"].SetValue(toneTexture);
			Parameters["AlbedTexture"].SetValue(effect.Texture);

			Parameters["DiffuseColor"].SetValue(effect.DiffuseColor);
			Parameters["SpecularColor"].SetValue(effect.SpecularColor);
			Parameters["EmissiveColor"].SetValue(effect.EmissiveColor);
			Parameters["SpecularPower"].SetValue(effect.SpecularPower * 0.2f);
			Parameters["Alpha"].SetValue(effect.Alpha);

			Vector2[] kernelStep = new Vector2[25];
			for (int i = 0; i < 25; i++)
			{
				kernelStep[i] = new Vector2(1.0f / 4096 * ((i % 5) - 2), 1.0f / 4096 * ((i / 5) - 2));
			}
			Parameters["KernelStep"].SetValue(kernelStep);
		}

		/// <summary>環境光色</summary>
		public Vector3 AmbientLightColor
		{
			get
			{
				return ambientLightColor.GetValueVector3();
			}
			set
			{
				ambientLightColor.SetValue(value);
			}
		}
		/// <summary>ライト0セッティング</summary>
		public LightSettings Light0Settings
		{
			get
			{
				return lightSettings[0];
			}
		}
		/// <summary>ライト1セッティング</summary>
		public LightSettings Light1Settings
		{
			get
			{
				return lightSettings[1];
			}
		}
		/// <summary>ライト2セッティング</summary>
		public LightSettings Light2Settings
		{
			get
			{
				return lightSettings[2];
			}
		}

		/// <summary>通常レンダリング時のテクニック</summary>
		public EffectTechnique NormalTechnique
		{
			get;
			set;
		}

		#region imprimentation of IEffectMatrices
		/*		ここからIEffectMatricesの実装		*/
		/// <summary>プロジェクション行列</summary>
		public Microsoft.Xna.Framework.Matrix Projection
		{
			get
			{
				return projectionMatrix.GetValueMatrix();
			}
			set
			{
				projectionMatrix.SetValue(value);
			}
		}
		/// <summary>ビュー行列</summary>
		public Microsoft.Xna.Framework.Matrix View
		{
			get
			{
				return viewMatrix.GetValueMatrix();
			}
			set
			{
				viewMatrix.SetValue(value);
			}
		}
		/// <summary>ワールド行列</summary>
		public Microsoft.Xna.Framework.Matrix World
		{
			get
			{
				return worldMatrix.GetValueMatrix();
			}
			set
			{
				worldMatrix.SetValue(value);
			}
		}
		#endregion

		/// <summary>シャドウのビュー行列</summary>
		public Matrix ShadowView
		{
			get
			{
				return shadowViewMatrix.GetValueMatrix();
			}
			set
			{
				shadowViewMatrix.SetValue(value);
			}
		}
		/// <summary>シャドウのプロジェクション行列</summary>
		public Matrix ShadowProjection
		{
			get
			{
				return shadowProjectionMatrix.GetValueMatrix();
			}
			set
			{
				shadowProjectionMatrix.SetValue(value);
			}
		}
		/// <summary>シャドウマップテクスチャ</summary>
		public Texture2D Shadowmap
		{
			set
			{
				shadowmap.SetValue(value);
			}
		}
		/// <summary>デプスシャドウが有効か</summary>
		public bool EnableDepthShadow
		{
			set
			{
				enableDepthShadow.SetValue(value);
			}
		}
	}
}
