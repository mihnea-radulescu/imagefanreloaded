using System;
using System.Collections.Generic;

namespace ImageFanReloaded.Core.Settings;

public enum SlideshowInterval
{
	OneSecond = 1,
	TwoSeconds = 2,
	ThreeSeconds = 3,
	FourSeconds = 4,
	FiveSeconds = 5
}

public static class SlideshowIntervalExtensions
{
	public static IReadOnlyList<SlideshowInterval> SlideshowIntervals
		=> (Enum.GetValues(typeof(SlideshowInterval)) as IReadOnlyList<SlideshowInterval>)!;

	public static int ToInt(this SlideshowInterval slideshowInterval) => (int)slideshowInterval;
}
