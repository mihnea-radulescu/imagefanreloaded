using System.Collections.Generic;
using System.Threading.Tasks;

namespace ImageFanReloaded.Core.DiscAccess;

public interface IInputPathContainer
{
	InputPathType InputPathType { get; }
	
	string? FolderPath { get; }
	string? FilePath { get; }

	bool ShouldProcessInputPath();
	void DisableProcessInputPath();

	Task<FileSystemEntryInfo> GetFileSystemEntryInfo();
	Task<FileSystemEntryInfo?> GetMatchingFileSystemEntryInfo(IReadOnlyCollection<FileSystemEntryInfo> folders);
}
