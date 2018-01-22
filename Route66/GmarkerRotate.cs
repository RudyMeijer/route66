using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GMap.NET;
using GMap.NET.WindowsForms;
using Route66.Properties;
using System.Drawing;
using static Route66.DataContracts;

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
			Offset = new Point(-Size.Width / 2, -Size.Height / 2);
		}
		public override void OnRender(Graphics g)
		{
			g.DrawImage(RotateImage(bitmap, Angle), LocalPosition.X, LocalPosition.Y, Size.Width, Size.Height);
			g.DrawRectangle(Pens.Red, -5, -5, 10, 10);
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
	public static class Extensions
	{
		/// <summary>
		/// This extension returns information about item parameter:
		/// Gps points 1 {X=-325,Y=-109} Tag=Dosage 20 g Left 1 m Right 1 m
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public static string Info(this GMapMarker item)
		{
			var tag = "-";
			var dis = "";
			if (item == null) return null;
			if (item.Overlay.Id == "Gps points")
				dis = $"Total distance={item.Overlay.Routes[0].Distance:f3} km ";
			if (item.Tag != null)
				tag = $"Tag={(item.Tag as GpsMarker).ToString().Replace('\n', ' ')}";
			var idx = item.Overlay.Markers.IndexOf(item);
			return $"{item.Overlay.Id} {idx} {item.ToolTipText?.Replace('\n', ' ')} {item.Position} {dis}{tag}";
		}
	}
}
