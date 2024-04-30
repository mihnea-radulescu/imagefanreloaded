using System;
using ImageFanReloaded.Core.TextHandling;

namespace ImageFanReloaded.Core.Extensions;

public static class StringComparerExtensions
{
	public static StringComparison ToStringComparison(this StringComparer stringComparer)
	{
		if (stringComparer is IStringComparisonEnabled stringComparisonEnabled)
		{
			return stringComparisonEnabled.GetStringComparison();
		}

		return StringComparison.InvariantCulture;
	}
}
