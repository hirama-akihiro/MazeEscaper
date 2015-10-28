using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MazeEscaper.Shader
{

	/// <summary>
	/// 平行光源
	/// </summary>
	public class LightSettings
	{
		/// <summary>
		/// ライトセッティングの生成
		/// </summary>
		/// <param name="parameters">パラメータコレクション</param>
		/// <param name="prefix">変数名の接頭辞</param>
		public LightSettings(EffectParameterCollection parameters, string prefix)
		{
			diffuseColor = parameters[prefix + "DiffuseColor"];
			specularColor = parameters[prefix + "SpecularColor"];
			direction = parameters[prefix + "Direction"];
			enabled = parameters[prefix + "Enabled"];

			diffuseColor.SetValue(Vector3.One * 0.7f);
			specularColor.SetValue(Vector3.One * 0.8f);
			direction.SetValue(-Vector3.Normalize(Vector3.One));
			enabled.SetValue(false);
		}

		/// <summary>拡散光色</summary>
		public Vector3 DiffuseColor
		{
			get
			{
				return diffuseColor.GetValueVector3();
			}
			set
			{
				diffuseColor.SetValue(value);
			}
		}

		/// <summary>鏡面光色</summary>
		public Vector3 SpecularColor
		{
			get
			{
				return specularColor.GetValueVector3();
			}
			set
			{
				specularColor.SetValue(value);
			}
		}

		/// <summary>ライト方向</summary>
		public Vector3 Direction
		{
			get
			{
				return direction.GetValueVector3();
			}
			set
			{
				direction.SetValue(value);
			}
		}

		/// <summary>ライトは有効</summary>
		public bool Enabled
		{
			get
			{
				return enabled.GetValueBoolean();
			}
			set
			{
				enabled.SetValue(value);
			}
		}

		private EffectParameter diffuseColor;	// 拡散光色
		private EffectParameter specularColor;	// 鏡面光色
		private EffectParameter direction;		// ライト方向
		private EffectParameter enabled;		// ライトが有効か
	}
}
