using System.Collections.Generic;

namespace ImageFanReloaded.Core.Settings;

public class SlideshowIntervalValues
{
	public static readonly HashSet<decimal> ValuesInSeconds = [
		0.125M,
		0.25M,
		0.50M,
		0.75M,
		1,
		1.25M,
		1.50M,
		1.75M,
		2,
		3,
		4,
		5,
		6,
		7,
		8,
		9,
		10
	];

	public static readonly decimal OneSecond = 1;
}
