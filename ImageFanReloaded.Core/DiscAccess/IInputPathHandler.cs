using System.Collections.Generic;
using System.Threading.Tasks;
using ImageFanReloaded.Core.DiscAccess.EntryInfo;

namespace ImageFanReloaded.Core.DiscAccess;

public interface IInputPathHandler
{
	InputPathType InputPathType { get; }

	string? FolderPath { get; }
	string? FilePath { get; }

	bool CanHandleInputPath();

	Task<IFileSystemEntryInfo?> GetMatchingFileSystemEntryInfo(
		IReadOnlyList<IFileSystemEntryInfo> folders);
}
