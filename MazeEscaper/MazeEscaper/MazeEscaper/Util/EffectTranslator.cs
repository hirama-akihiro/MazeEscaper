using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MazeEscaper.Shader;
using Microsoft.Xna.Framework.Content;

namespace MazeEscaper.Util
{
	/// <summary>
	/// エフェクトの変換とかを管理
	/// </summary>
	public class EffectTranslator
	{

		/// <summary>
		/// テクニック
		/// </summary>
		public enum RenderingState
		{
			SHADOWMAP, NORMAL
		};

		private static RenderingState currentState = RenderingState.NORMAL;
		private static Texture2D shadowmap;

		/// <summary>
		/// エフェクトの変換
		/// </summary>
		/// <param name="srcEffect">変換元</param>
		/// <returns>変換後</returns>
		public static Effect Translate(ContentManager content, Effect srcEffect)
		{
			Vector3 ambientLightColor = Vector3.One * 0.2f;
			Vector3 light0SpecularColor = Vector3.One * 0.5f;
			Vector3 light0DiffuseColor = Vector3.One * 0.5f;
			Vector3 light0Direction = Vector3.Normalize(new Vector3(0.1f, -1.0f, 0.1f));
			Vector3 light1DiffuseColor = Vector3.One * 0.5f;
			Vector3 light1Direction = Vector3.Normalize(new Vector3(-0.1f, -1.0f, -0.1f));

			Matrix shadowView = Matrix.CreateLookAt(new Vector3(5.0f, 100.0f, 3.0f), Vector3.Zero, Vector3.UnitX);
			Matrix shadowProj = Matrix.CreatePerspectiveFieldOfView(0.6f, 1.0f, 80.0f, 240.0f);

			if (!(srcEffect is BasicEffect))
			{
				return srcEffect;
			}

#if false
			BasicEffect basicEffect = srcEffect as BasicEffect;
			basicEffect.EnableDefaultLighting();
			basicEffect.PreferPerPixelLighting = true;
			basicEffect.AmbientLightColor = ambientLightColor;
			basicEffect.DirectionalLight0.Enabled = true;
			basicEffect.DirectionalLight0.DiffuseColor = light0DiffuseColor;
			basicEffect.DirectionalLight0.SpecularColor = light0SpecularColor;
			basicEffect.DirectionalLight0.Direction = light0Direction;
			basicEffect.DirectionalLight1.Enabled = true;
			basicEffect.DirectionalLight1.DiffuseColor = light1DiffuseColor;
			basicEffect.DirectionalLight1.Direction = light1Direction;
			return basicEffect;
#elif true
			ToonEffect toonEffect = new ToonEffect(content, srcEffect as BasicEffect);
			toonEffect.AmbientLightColor = ambientLightColor;
			toonEffect.Light0Settings.DiffuseColor = light0DiffuseColor;
			toonEffect.Light0Settings.SpecularColor = light0SpecularColor;
			toonEffect.Light0Settings.Direction = light0Direction;
			toonEffect.Light1Settings.DiffuseColor = light1DiffuseColor;
			toonEffect.Light1Settings.Direction = light1Direction;
			toonEffect.Light2Settings.DiffuseColor = Vector3.Zero;

			toonEffect.ShadowView = shadowView;
			toonEffect.ShadowProjection = shadowProj;

			return toonEffect;
#else
			FresnelEffect fresnelEffect = new FresnelEffect(content, srcEffect as BasicEffect);
			fresnelEffect.AmbientLightColor = ambientLightColor;
			fresnelEffect.Light0Settings.DiffuseColor = light0DiffuseColor;
			fresnelEffect.Light0Settings.SpecularColor = light0SpecularColor;
			fresnelEffect.Light0Settings.Direction = light0Direction;
			fresnelEffect.Light1Settings.DiffuseColor = light1DiffuseColor;
			fresnelEffect.Light1Settings.Direction = light1Direction;
			fresnelEffect.Light2Settings.DiffuseColor = Vector3.Zero;

			fresnelEffect.ShadowView = shadowView;
			fresnelEffect.ShadowProjection = shadowProj;

			return fresnelEffect;
#endif
		}

		/// <summary>
		/// 描画前の設定
		/// </summary>
		/// <param name="effect">設定するエフェクト</param>
		public static void Settings(Effect effect)
		{
			switch (currentState)
			{
				default:
				case RenderingState.NORMAL:
#if true
					ToonEffect spEffect = (ToonEffect)effect;
#else
					FresnelEffect spEffect = (FresnelEffect)effect;
#endif
					effect.CurrentTechnique = spEffect.NormalTechnique;
					spEffect.EnableDepthShadow = shadowmap != null;
					spEffect.Shadowmap = shadowmap;
					break;
				case RenderingState.SHADOWMAP:
					effect.CurrentTechnique = effect.Techniques["CreateShadowmap"];
					break;
			}
		}


		public static void SetRenderingState(RenderingState state)
		{
			currentState = state;
		}

		public static void SetShadowmap(Texture2D shadowmap)
		{
			EffectTranslator.shadowmap = shadowmap;
		}
	}
}
