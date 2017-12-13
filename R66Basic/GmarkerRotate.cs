using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GMap.NET;
using GMap.NET.WindowsForms;
using R66Basic.Properties;
using System.Drawing;

namespace R66Basic
{
	class GmarkerRotate : GMapMarker
	{
		private Bitmap bitmap;
		public float Rotation { get; set; }

		public GmarkerRotate(PointLatLng pos, Bitmap bitmap) : base(pos)
		{
			this.bitmap = bitmap;
			Size = new Size(30, 30);
			Offset = new Point(-Size.Width / 2 + 1, -Size.Height / 2 + 1);
			ToolTipText = $"hallo";
		}
		public override void OnRender(Graphics g)
		{
			Console.WriteLine($"local={LocalPosition}, clip={g.ClipBounds}, transform={g.Transform.OffsetX},{g.Transform.OffsetY}, rotation ={Rotation}");
			var sav = g.Transform;
			//g.TranslateTransform(0, 0);
			g.DrawEllipse(Pens.Red, -5, -5, 10, 10);
			//
			// Rotate bitmap.
			//
			//g.RotateTransform(Rotation);
			//g.TranslateTransform(-LocalPosition.X, -LocalPosition.Y);
			//g.TranslateTransform(LocalPosition.X, LocalPosition.Y);
			//g.TranslateTransform(15, 15);
			//g.TranslateTransform(-15, -15);
			//g.DrawImage(bitmap, 0, 0, Size.Width, Size.Height);
			//g.RenderingOrigin = new Point(0, 0);
			//g.Transform = sav;
			g.DrawImage(RotateImage( bitmap,Rotation), LocalPosition.X, LocalPosition.Y, Size.Width, Size.Height);
			//g.Transform = sav;
			//g.RotateTransform(-Rotation);
			//base.OnRender(g);
		}
		public override void Dispose()
		{
			if (bitmap != null)
			{
				bitmap.Dispose();
				bitmap = null;
			}

			base.Dispose();
		}
		private Bitmap RotateImage(Bitmap bmp, float angle)
		{
			Bitmap rotatedImage = new Bitmap(bmp.Width, bmp.Height);
			using (Graphics g = Graphics.FromImage(rotatedImage))
			{
				// Set the rotation point to the center in the matrix
				g.TranslateTransform(bmp.Width / 2, bmp.Height / 2);
				// Rotate
				g.RotateTransform(angle);
				// Restore rotation point in the matrix
				g.TranslateTransform(-bmp.Width / 2, -bmp.Height / 2);
				// Draw the image on the bitmap
				g.DrawImage(bmp, new Point(0, 0));
			}

			return rotatedImage;
		}
	}
}
