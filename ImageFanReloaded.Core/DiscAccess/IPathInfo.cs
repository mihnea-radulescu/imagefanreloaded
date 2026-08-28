using System;

namespace ImageFanReloaded.Core.DiscAccess;

public interface IPathInfo
{
	bool IsFileInFolder(
		string filePath,
		string folderPath,
		char directorySeparatorChar,
		StringComparison pathComparison);
}
