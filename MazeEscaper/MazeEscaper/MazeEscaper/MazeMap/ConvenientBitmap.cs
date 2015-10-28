using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace MazeEscaper.MazeMap
{
	class ConvenientBitmap
	{
		/// <summary>
		/// 縦
		/// </summary>
		public int Height { get; private set; }

		/// <summary>
		/// 横
		/// </summary>
		public int Width { get; private set; }

		/// <summary>
		/// ビットマップのデータ
		/// </summary>
		private byte[] data;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="bmp">bitmap</param>
		public ConvenientBitmap(Bitmap bmp)
		{
			Width = bmp.Width;
			Height = bmp.Height;

			BitmapData bitmapData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
			IntPtr bitmapPtr = bitmapData.Scan0;

			data = new byte[Width * Height * 4];

			Marshal.Copy(bitmapPtr, data, 0, data.Length);

			bmp.UnlockBits(bitmapData);
		}

		/// <summary>
		/// 赤
		/// </summary>
		/// <param name="x">x座標</param>
		/// <param name="y">y座標</param>
		/// <returns></returns>
		public byte GetR(int x, int y)
		{
			return data[(y * Width + x) * 4 + 2];
		}

		/// <summary>
		/// 緑
		/// </summary>
		/// <param name="x">x座標</param>
		/// <param name="y">y座標</param>
		/// <returns></returns>
		public byte GetG(int x, int y)
		{
			return data[(y * Width + x) * 4 + 1];
		}

		/// <summary>
		/// 青
		/// </summary>
		/// <param name="x">x座標</param>
		/// <param name="y">y座標</param>
		/// <returns></returns>
		public byte GetB(int x, int y)
		{
			return data[(y * Width + x) * 4];
		}
	}
}
