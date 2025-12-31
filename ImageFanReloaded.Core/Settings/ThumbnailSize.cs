using System;
using System.Collections.Generic;

namespace ImageFanReloaded.Core.Settings;

public enum ThumbnailSize
{
	OneHundredPixels = 100,
	OneHundredFiftyPixels = 150,
	TwoHundredPixels = 200,
	TwoHundredFiftyPixels = 250,
	ThreeHundredPixels = 300,
	ThreeHundredFiftyPixels = 350,
	FourHundredPixels = 400,
	FourHundredFiftyPixels = 450,
	FiveHundredPixels = 500,
	FiveHundredFiftyPixels = 550,
	SixHundredPixels = 600,
	SixHundredFiftyPixels = 650,
	SevenHundredPixels = 700,
	SevenHundredFiftyPixels = 750,
	EightHundredPixels = 800,
	EightHundredFiftyPixels = 850,
	NineHundredPixels = 900,
	NineHundredFiftyPixels = 950,
	OneThousandPixels = 1000,
	OneThousandFiftyPixels = 1050,
	OneThousandOneHundredPixels = 1100,
	OneThousandOneHundredFiftyPixels = 1150,
	OneThousandTwoHundredPixels = 1200
}

public static class ThumbnailSizeExtensions
{
	public static IReadOnlyList<ThumbnailSize> ThumbnailSizes => Enum.GetValues<ThumbnailSize>();
	public static IReadOnlyList<int> ThumbnailSizesAsIntegers => (ThumbnailSizes as IReadOnlyList<int>)!;
	public static int ThumbnailSizeIncrement => 50;

	extension(ThumbnailSize thumbnailSize)
	{
		public int ToInt() => (int)thumbnailSize;
	}

	extension(int thumbnailSize)
	{
		public ThumbnailSize ToThumbnailSize() => (ThumbnailSize)thumbnailSize;
		public bool IsValidThumbnailSize => Enum.IsDefined(typeof(ThumbnailSize), thumbnailSize);
	}
}
