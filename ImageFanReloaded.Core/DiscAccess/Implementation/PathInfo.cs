using System;
using ImageFanReloaded.Core.DiscAccess.Implementation.Extensions;

namespace ImageFanReloaded.Core.DiscAccess.Implementation;

public class PathInfo : IPathInfo
{
	public bool IsFileInFolder(
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

		var isFileInFolder = 
			startsFilePathWithFolderPath &&
		    ((isFolderRootFolder && isFileDirectlyUnderRootFolder) ||
		    (!isFolderRootFolder && isFileDirectlyUnderNonRootFolder));

		return isFileInFolder;
	}
}
