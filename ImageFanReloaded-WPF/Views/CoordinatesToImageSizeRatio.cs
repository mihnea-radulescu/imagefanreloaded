using System.Windows;
using ImageFanReloaded.CommonTypes.ImageHandling;

namespace ImageFanReloaded.Views
{
	public class CoordinatesToImageSizeRatio
	{
		static CoordinatesToImageSizeRatio()
		{
			ImageCenter = new CoordinatesToImageSizeRatio(0.5, 0.5);
		}
		
		public CoordinatesToImageSizeRatio(Point coordinates, ImageSize imageSize)
        {
            RatioX = coordinates.X / imageSize.Width;
			RatioY = coordinates.Y / imageSize.Height;
		}

		public CoordinatesToImageSizeRatio(double ratioX, double ratioY)
		{
			RatioX = ratioX;
			RatioY = ratioY;
		}

		public double RatioX { get; }
		public double RatioY { get; }

		public static CoordinatesToImageSizeRatio ImageCenter { get; }
	}
}
