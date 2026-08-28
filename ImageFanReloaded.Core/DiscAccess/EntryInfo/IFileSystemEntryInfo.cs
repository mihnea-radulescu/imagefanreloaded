using System;
using System.Collections.Generic;
using ImageFanReloaded.Core.ImageCore;
using ImageFanReloaded.Core.ImageHandling.ImageFileData;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.DiscAccess.EntryInfo;

public interface IFileSystemEntryInfo
{
	IFileSystemEntryInfo? Parent { get; }
	IFileSystemEntryInfo? DriveOrFolder { get; }

	string Name { get; }
	string Path { get; }
	string QualifiedPath { get; }
	bool HasSubFolders { get; }

	IImage Icon { get; }

	IReadOnlyList<IFileSystemEntryInfo> GetSubFolders(
		IFileSystemEntryInfoFactory fileSystemEntryInfoFactory,
		FileSystemEntryInfoOrdering folderOrdering,
		FileSystemEntryInfoOrderingDirection folderOrderingDirection,
		bool zipArchivesEnabled,
		StringComparer nameComparer,
		Random randomShuffler);

	IReadOnlyList<IImageFileData> GetImageFileDataList(
		HashSet<string> enabledImageFileExtensions);
}
