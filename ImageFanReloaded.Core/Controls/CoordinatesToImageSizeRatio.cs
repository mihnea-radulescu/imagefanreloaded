using ImageFanReloaded.Core.ImageHandling;

namespace ImageFanReloaded.Core.Controls;

public record CoordinatesToImageSizeRatio
{
	public static readonly CoordinatesToImageSizeRatio ImageCenter =
		new(0.5, 0.5);

	public CoordinatesToImageSizeRatio(
		ImagePoint coordinates, ImageSize imageSize)
	{
		RatioX = coordinates.X / (double)imageSize.Width;
		RatioY = coordinates.Y / (double)imageSize.Height;
	}

	public double RatioX { get; }
	public double RatioY { get; }

	private CoordinatesToImageSizeRatio(double ratioX, double ratioY)
	{
		RatioX = ratioX;
		RatioY = ratioY;
	}
}
