using ImageFanReloaded.Core.ImageHandling;

namespace ImageFanReloaded.Core.Controls;

public record CoordinatesToImageSizeRatio
{
	static CoordinatesToImageSizeRatio()
	{
		ImageCenter = new CoordinatesToImageSizeRatio(0.5, 0.5);
	}
	
	public static CoordinatesToImageSizeRatio ImageCenter { get; }
	
	public CoordinatesToImageSizeRatio(ImagePoint coordinates, ImageSize imageSize)
    {
        RatioX = (double)coordinates.X / (double)imageSize.Width;
		RatioY = (double)coordinates.Y / (double)imageSize.Height;
	}

	public double RatioX { get; }
	public double RatioY { get; }

	#region Private
	
	private CoordinatesToImageSizeRatio(double ratioX, double ratioY)
	{
		RatioX = ratioX;
		RatioY = ratioY;
	}
	
	#endregion
}
