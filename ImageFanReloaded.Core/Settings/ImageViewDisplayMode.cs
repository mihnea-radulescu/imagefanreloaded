using System;

namespace ImageFanReloaded.Core.Settings;

public enum ImageViewDisplayMode
{
	FullScreen = 0,
	Windowed = 1,
	WindowedMaximized = 2
}

public static class ImageViewDisplayModeExtensions
{
	extension(ImageViewDisplayMode imageViewDisplayMode)
	{
		public string Description
		{
			get
			{
				var description = imageViewDisplayMode switch
				{
					ImageViewDisplayMode.FullScreen => "Full-screen",
					ImageViewDisplayMode.Windowed => "Windowed",
					ImageViewDisplayMode.WindowedMaximized => "Windowed maximized",

					_ => throw new NotSupportedException(
						$"Enum value {imageViewDisplayMode} not supported.")
				};

				return description;
			}
		}
	}
}
