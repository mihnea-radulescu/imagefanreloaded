using System;

namespace ImageFanReloaded.Core.Settings;

public enum ImageViewDisplayMode
{
	FullScreen = 0,
	Windowed = 1,
	WindowedMaximized = 2,
	WindowedMaximizedBorderless = 3
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
					ImageViewDisplayMode.WindowedMaximized
						=> "Windowed maximized",
					ImageViewDisplayMode.WindowedMaximizedBorderless
						=> "Windowed maximized borderless",

					_ => throw new NotSupportedException(
						$"Enum value {imageViewDisplayMode} not supported.")
				};

				return description;
			}
		}
	}
}
