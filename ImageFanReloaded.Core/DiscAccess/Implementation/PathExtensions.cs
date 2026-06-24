using System;
using System.Linq;
using ImageFanReloaded.Core.TextHandling.Implementation;

namespace ImageFanReloaded.Core.DiscAccess.Implementation;

public static class PathExtensions
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

	public static bool IsFileInFolder(
		string filePath,
		string folderPath,
		char directorySeparatorChar,
		StringComparison pathComparison)
	{
		var directorySeparator = directorySeparatorChar.ToString();

		var startsFilePathWithFolderPath = filePath
			.StartsWithPath(folderPath, directorySeparator, pathComparison);

		var folderPathDirectorySeparatorCharCount = folderPath
			.GetDirectorySeparatorCharCountInPath(directorySeparatorChar);
		var filePathDirectorySeparatorCharCount = filePath
			.GetDirectorySeparatorCharCountInPath(directorySeparatorChar);

		var isFolderRootFolder = folderPath.EndsWith(
			directorySeparator, pathComparison);
		var isFileDirectlyUnderRootFolder =
			folderPathDirectorySeparatorCharCount ==
			filePathDirectorySeparatorCharCount;

		var isFileDirectlyUnderNonRootFolder =
			filePathDirectorySeparatorCharCount ==
			folderPathDirectorySeparatorCharCount + 1;

		var isFileInFolder = startsFilePathWithFolderPath &&
			((isFolderRootFolder && isFileDirectlyUnderRootFolder) ||
			 (!isFolderRootFolder && isFileDirectlyUnderNonRootFolder));

		return isFileInFolder;
	}

	extension(string path)
	{
		private int GetDirectorySeparatorCharCountInPath(
			char directorySeparatorChar)
				=> path.Count(aPathChar => aPathChar == directorySeparatorChar);
	}
}
