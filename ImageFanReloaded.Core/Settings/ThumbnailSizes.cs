using System.Collections.Generic;

namespace ImageFanReloaded.Core.Settings;

public static class ThumbnailSizes
{
	public static readonly HashSet<int> Values = [
		100,
		150,
		200,
		250,
		300,
		350,
		400,
		450,
		500,
		550,
		600,
		650,
		700,
		750,
		800,
		850,
		900,
		950,
		1000,
		1050,
		1100,
		1150,
		1200
	];

	public const int DefaultValue = 250;

	public const int Increment = 50;

	public static bool IsValid(int thumbnailSize)
		=> Values.Contains(thumbnailSize);
}
