﻿using Avalonia;
using ImageFanReloaded.CommonTypes.ImageHandling;

namespace ImageFanReloaded.Views;

public record CoordinatesToImageSizeRatio
{
	static CoordinatesToImageSizeRatio()
	{
		ImageCenter = new CoordinatesToImageSizeRatio(0.5, 0.5);
	}
	
	public static CoordinatesToImageSizeRatio ImageCenter { get; }
	
	public CoordinatesToImageSizeRatio(Point coordinates, ImageSize imageSize)
    {
        RatioX = coordinates.X / imageSize.Width;
		RatioY = coordinates.Y / imageSize.Height;
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
