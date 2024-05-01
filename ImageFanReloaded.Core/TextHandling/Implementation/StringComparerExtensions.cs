using System;

namespace ImageFanReloaded.Core.TextHandling.Implementation;

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
