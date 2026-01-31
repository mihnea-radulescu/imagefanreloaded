using System;

namespace ImageFanReloaded.Core.RuntimeEnvironment.Implementation.Extensions;

public static class RuntimeEnvironmentTypeExtensions
{
	extension(RuntimeEnvironmentType runtimeEnvironmentType)
	{
		public StringComparer GetStringComparer()
		{
			if (runtimeEnvironmentType == RuntimeEnvironmentType.Windows)
			{
				return StringComparer.InvariantCultureIgnoreCase;
			}

			return StringComparer.InvariantCulture;
		}
	}
}
