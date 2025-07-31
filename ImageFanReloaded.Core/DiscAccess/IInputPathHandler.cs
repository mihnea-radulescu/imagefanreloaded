using System.Collections.Generic;
using System.Threading.Tasks;

namespace ImageFanReloaded.Core.DiscAccess;

public interface IInputPathHandler
{
	InputPathType InputPathType { get; }

	string? FolderPath { get; }
	string? FilePath { get; }

	bool CanProcessInputPath();

	Task<FileSystemEntryInfo> GetFileSystemEntryInfo();
	Task<FileSystemEntryInfo?> GetMatchingFileSystemEntryInfo(IReadOnlyList<FileSystemEntryInfo> folders);
}
