using System;
using System.Collections.Generic;

namespace ImageFanReloaded.Core.Settings;

public enum ThumbnailSize
{
	OneHundredPixels = 100,
	OneHundredAndFiftyPixels = 150,
	TwoHundredPixels = 200,
	TwoHundredAndFixtyPixels = 250,
	ThreeHundredPixels = 300,
	ThreeHundredAndFixtyPixels = 350,
	FourHundredPixels = 400
}

public static class ThumbnailSizeExtensions
{
	public static IReadOnlyList<ThumbnailSize> ThumbnailSizes
		=> (Enum.GetValues(typeof(ThumbnailSize)) as IReadOnlyList<ThumbnailSize>)!;

	public static IReadOnlyList<int> ThumbnailSizesAsIntegers
		=> (ThumbnailSizes as IReadOnlyList<int>)!;

	public static int ThumbnailSizeIncrement => 50;

	public static int ToInt(this ThumbnailSize thumbnailSize) => (int)thumbnailSize;

	public static ThumbnailSize ToThumbnailSize(this int thumbnailSize)
		=> (ThumbnailSize)thumbnailSize;

	public static bool IsValidThumbnailSize(this int thumbnailSize)
		=> Enum.IsDefined(typeof(ThumbnailSize), thumbnailSize);
}
