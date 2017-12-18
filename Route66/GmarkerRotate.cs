using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GMap.NET;
using GMap.NET.WindowsForms;
using Route66.Properties;
using System.Drawing;

namespace Route66
{
	class GmarkerRotate : GMapMarker
	{
		private Bitmap bitmap;
		public float Angle { get; set; }

		public GmarkerRotate(PointLatLng pos, Bitmap bitmap) : base(pos)
		{
			this.bitmap = bitmap;
			Size = new Size(30, 30);
			Offset = new Point(-Size.Width / 2 , -Size.Height / 2 );
			//ToolTipText = $"hallo";
		}
		public override void OnRender(Graphics g)
		{
			g.DrawImage(RotateImage(bitmap, Angle), LocalPosition.X, LocalPosition.Y, Size.Width, Size.Height);
			//g.DrawEllipse(Pens.Red, -5, -5, 10, 10);
			//MyLib.My.Status($"g.clipbounds { g.ClipBounds.Location}");
			//Route66.Overlay.ClipBounds = g.ClipBounds;
		}
		private Bitmap RotateImage(Bitmap bmp, float angle)
		{
			//Console.WriteLine("rotate bitmap");
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
		public override void Dispose()
		{
			if (bitmap != null)
			{
				bitmap.Dispose();
				bitmap = null;
			}

			base.Dispose();
		}
	}
}
