using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MazeEscaper.Util
{
	/// <summary>
	/// エフェクト関連のユーティリティ
	/// </summary>
	public static class EffectExtensions
	{

		/// <summary>
		/// モデルの全てのEffectをfuncによって置き換える
		/// </summary>
		/// <param name="model"></param>
		/// <param name="func"></param>
		public static void ReplaceAllEffects(this Model model, Func<Effect, Effect> func)
		{
			foreach (ModelMesh mesh in model.Meshes)
			{
				foreach (ModelMeshPart part in mesh.MeshParts)
				{
					part.Effect = func(part.Effect);
				}
			}
		}

		/// <summary>
		/// モデルの全てのEffectにactionを実行する
		/// </summary>
		/// <param name="model"></param>
		/// <param name="action"></param>
		public static void ForEachEffect(this Model model, Action<Effect> action)
		{
			foreach (ModelMesh mesh in model.Meshes)
			{
				foreach (Effect effect in mesh.Effects)
				{
					action(effect);
				}
			}
		}

		/// <summary>
		/// モデルの全てのIEffectLightsにactionを実行する
		/// </summary>
		/// <param name="model"></param>
		/// <param name="action"></param>
		public static void ForEachEffectLights(this Model model, Action<IEffectLights> action)
		{
			foreach (ModelMesh mesh in model.Meshes)
			{
				foreach (IEffectLights effect in mesh.Effects)
				{
					action(effect);
				}
			}
		}

		/*		ここから IEffectLightsのラッパ		*/
		public static void SetAmbientLightColor(this Model model, Vector3 ambient)
		{
			model.ForEachEffectLights(effect => effect.AmbientLightColor = ambient);
		}
		public static void SetLightingEnabled(this Model model, bool enabled)
		{
			model.ForEachEffectLights(effect => effect.LightingEnabled = enabled);
		}

		// DirectionalLight0
		public static void SetEnableLight0(this Model model, bool enabled)
		{
			model.ForEachEffectLights(effect => effect.DirectionalLight0.Enabled = enabled);
		}
		public static void SetDiffuseLightColor0(this Model model, Vector3 diffuse)
		{
			model.ForEachEffectLights(effect => effect.DirectionalLight0.DiffuseColor = diffuse);
		}
		public static void SetSpecularLightColor0(this Model model, Vector3 specular)
		{
			model.ForEachEffectLights(effect => effect.DirectionalLight0.SpecularColor = specular);
		}
		public static void SetLightDirection0(this Model model, Vector3 direction)
		{
			model.ForEachEffectLights(effect => effect.DirectionalLight0.Direction = direction);
		}

		// DirectionalLight1
		public static void SetEnableLight1(this Model model, bool enabled)
		{
			model.ForEachEffectLights(effect => effect.DirectionalLight1.Enabled = enabled);
		}
		public static void SetDiffuseLightColor1(this Model model, Vector3 diffuse)
		{
			model.ForEachEffectLights(effect => effect.DirectionalLight1.DiffuseColor = diffuse);
		}
		public static void SetSpecularLightColor1(this Model model, Vector3 specular)
		{
			model.ForEachEffectLights(effect => effect.DirectionalLight1.SpecularColor = specular);
		}
		public static void SetLightDirection1(this Model model, Vector3 direction)
		{
			model.ForEachEffectLights(effect => effect.DirectionalLight1.Direction = direction);
		}

		// DirectionalLight2
		public static void SetEnableLight2(this Model model, bool enabled)
		{
			model.ForEachEffectLights(effect => effect.DirectionalLight2.Enabled = enabled);
		}
		public static void SetDiffuseLightColor2(this Model model, Vector3 diffuse)
		{
			model.ForEachEffectLights(effect => effect.DirectionalLight2.DiffuseColor = diffuse);
		}
		public static void SetSpecularLightColor2(this Model model, Vector3 specular)
		{
			model.ForEachEffectLights(effect => effect.DirectionalLight2.SpecularColor = specular);
		}
		public static void SetLightDirection2(this Model model, Vector3 direction)
		{
			model.ForEachEffectLights(effect => effect.DirectionalLight2.Direction = direction);
		}
		/*		ここまでIEffectLightsのラッパ			*/

	}
}
