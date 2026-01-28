using System;

namespace ImageFanReloaded.Core.TextHandling.Implementation;

public static class StringComparerExtensions
{
	extension(StringComparer stringComparer)
	{
		public StringComparison ToStringComparison()
		{
			if (stringComparer is
				IStringComparisonEnabled stringComparisonEnabled)
			{
				return stringComparisonEnabled.GetStringComparison();
			}

			return StringComparison.InvariantCulture;
		}
	}
}
