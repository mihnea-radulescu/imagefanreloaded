using System;
using System.Linq;
using ImageFanReloaded.Core.TextHandling.Implementation;

namespace ImageFanReloaded.Core.DiscAccess.Implementation.Extensions;

public static class PathStringExtensions
{
	extension(string longerPath)
	{
		public bool StartsWithPath(
			string shorterPath,
			string directorySeparator,
			StringComparison pathComparison)
		{
			bool startsWithPath;

			if (longerPath.Equals(shorterPath, pathComparison))
			{
				startsWithPath = true;
			}
			else if (longerPath.StartsWith(shorterPath, pathComparison))
			{
				if (shorterPath.EndsWith(directorySeparator, pathComparison))
				{
					startsWithPath = true;
				}
				else
				{
					var remainingStringAfterSubstringMatch = longerPath
						.GetRemainingStringAfterSubstringMatch(
							shorterPath, pathComparison)!;

					startsWithPath = remainingStringAfterSubstringMatch
						.StartsWith(directorySeparator, pathComparison);
				}
			}
			else
			{
				startsWithPath = false;
			}

			return startsWithPath;
		}
	}

	extension(string path)
	{
		public int GetDirectorySeparatorCharCountInPath(
			char directorySeparatorChar)
				=> path.Count(aPathChar => aPathChar == directorySeparatorChar);
	}
}
