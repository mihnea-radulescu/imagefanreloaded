using System;
using System.Collections.Generic;

namespace ImageFanReloaded.Core.Settings;

public enum UpsizeFullScreenImagesUpToScreenSize
{
	Disabled = 100,
	UpTo1Dot25X = 125,
	UpTo1Dot50X = 150,
	UpTo1Dot75X = 175,
	UpTo2X = 200,
	UpTo3X = 300,
	UpTo4X = 400,
	UpTo5X = 500,
	UpTo6X = 600,
	UpTo7X = 700,
	UpTo8X = 800
}

public static class UpsizeFullScreenImagesUpToScreenSizeExtensions
{
	public static IReadOnlyList<UpsizeFullScreenImagesUpToScreenSize> UpsizeFullScreenImagesUpToScreenSizes
		=> Enum.GetValues<UpsizeFullScreenImagesUpToScreenSize>();

	extension(UpsizeFullScreenImagesUpToScreenSize upsizeFullScreenImagesUpToScreenSize)
	{
		public double Value
		{
			get
			{
				var value = upsizeFullScreenImagesUpToScreenSize switch
				{
					UpsizeFullScreenImagesUpToScreenSize.Disabled => 1,
					UpsizeFullScreenImagesUpToScreenSize.UpTo1Dot25X => 1.25,
					UpsizeFullScreenImagesUpToScreenSize.UpTo1Dot50X => 1.50,
					UpsizeFullScreenImagesUpToScreenSize.UpTo1Dot75X => 1.75,
					UpsizeFullScreenImagesUpToScreenSize.UpTo2X => 2,
					UpsizeFullScreenImagesUpToScreenSize.UpTo3X => 3,
					UpsizeFullScreenImagesUpToScreenSize.UpTo4X => 4,
					UpsizeFullScreenImagesUpToScreenSize.UpTo5X => 5,
					UpsizeFullScreenImagesUpToScreenSize.UpTo6X => 6,
					UpsizeFullScreenImagesUpToScreenSize.UpTo7X => 7,
					UpsizeFullScreenImagesUpToScreenSize.UpTo8X => 8,

					_ => throw new NotSupportedException(
						$"Enum value {upsizeFullScreenImagesUpToScreenSize} not supported.")
				};

				return value;
			}
		}

		public string Description
		{
			get
			{
				var description = upsizeFullScreenImagesUpToScreenSize switch
				{
					UpsizeFullScreenImagesUpToScreenSize.Disabled => "Disabled",
					UpsizeFullScreenImagesUpToScreenSize.UpTo1Dot25X => "Up to 1.25X",
					UpsizeFullScreenImagesUpToScreenSize.UpTo1Dot50X => "Up to 1.50X",
					UpsizeFullScreenImagesUpToScreenSize.UpTo1Dot75X => "Up to 1.75X",
					UpsizeFullScreenImagesUpToScreenSize.UpTo2X => "Up to 2X",
					UpsizeFullScreenImagesUpToScreenSize.UpTo3X => "Up to 3X",
					UpsizeFullScreenImagesUpToScreenSize.UpTo4X => "Up to 4X",
					UpsizeFullScreenImagesUpToScreenSize.UpTo5X => "Up to 5X",
					UpsizeFullScreenImagesUpToScreenSize.UpTo6X => "Up to 6X",
					UpsizeFullScreenImagesUpToScreenSize.UpTo7X => "Up to 7X",
					UpsizeFullScreenImagesUpToScreenSize.UpTo8X => "Up to 8X",

					_ => throw new NotSupportedException(
						$"Enum value {upsizeFullScreenImagesUpToScreenSize} not supported.")
				};

				return description;
			}
		}
	}
}
