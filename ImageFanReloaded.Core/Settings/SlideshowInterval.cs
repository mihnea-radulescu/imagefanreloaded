using System;
using System.Collections.Generic;

namespace ImageFanReloaded.Core.Settings;

public enum SlideshowInterval
{
	OneSecond = 1,
	TwoSeconds = 2,
	ThreeSeconds = 3,
	FourSeconds = 4,
	FiveSeconds = 5,
	SixSeconds = 6,
	SevenSeconds = 7,
	EightSeconds = 8,
	NineSeconds = 9,
	TenSeconds = 10
}

public static class SlideshowIntervalExtensions
{
	public static IReadOnlyList<SlideshowInterval> SlideshowIntervals
		=> Enum.GetValues<SlideshowInterval>();

	extension(SlideshowInterval slideshowInterval)
	{
		public int ToInt() => (int)slideshowInterval;
	}
}
