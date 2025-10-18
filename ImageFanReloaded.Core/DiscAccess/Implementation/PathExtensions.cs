using System;
using ImageFanReloaded.Core.TextHandling.Implementation;

namespace ImageFanReloaded.Core.DiscAccess.Implementation;

public static class PathExtensions
{
	public static bool ContainsPath(
		this string longerPath,
		string shorterPath,
		string directorySeparator,
		StringComparison pathComparison)
	{
		bool containsPath;

		if (longerPath.Equals(shorterPath, pathComparison))
		{
			containsPath = true;
		}
		else if (longerPath.Contains(shorterPath, pathComparison))
		{
			if (shorterPath.EndsWith(directorySeparator, pathComparison))
			{
				containsPath = true;
			}
			else
			{
				var remainingStringAfterSubstringMatch = longerPath
					.GetRemainingStringAfterSubstringMatch(shorterPath, pathComparison)!;

				containsPath = remainingStringAfterSubstringMatch.StartsWith(
					directorySeparator, pathComparison);
			}
		}
		else
		{
			containsPath = false;
		}

		return containsPath;
	}
}
