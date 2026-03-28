using System.Collections.Generic;

namespace ImageFanReloaded.Core.Settings;

public static class KeyboardScrollThumbnailIncrements
{
	public static readonly HashSet<int> Values = [
		4,
		5,
		6,
		7,
		8,
		9,
		10,
		11,
		12,
		13,
		14,
		15,
		16,
		17,
		18,
		19,
		20,
		21,
		22,
		23,
		24
	];

	public const int DefaultValue = 12;

	public static bool IsValid(int keyboardScrollThumbnailIncrement)
		=> Values.Contains(keyboardScrollThumbnailIncrement);
}
