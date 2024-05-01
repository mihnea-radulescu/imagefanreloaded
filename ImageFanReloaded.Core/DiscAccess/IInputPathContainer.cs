using System.Collections.Generic;

namespace ImageFanReloaded.Core.DiscAccess;

public interface IInputPathContainer
{
	string? InputPath { get; }
	
	bool HasPopulatedInputPath { get; set; }

	bool ShouldPopulateInputPath();

	FileSystemEntryInfo? GetMatchingFileSystemEntryInfo(IReadOnlyCollection<FileSystemEntryInfo> folders);
}
