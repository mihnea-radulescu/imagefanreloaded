using System;
using System.Collections.Generic;
using System.Linq;
using ImageFanReloaded.Core.DiscAccess.EntryInfo;
using ImageFanReloaded.Core.DiscAccess.Implementation.Extensions;
using ImageFanReloaded.Core.ImageCore;
using ImageFanReloaded.Core.ImageHandling.ImageFileData;
using ImageFanReloaded.Core.ImageHandling.Implementation.ImageFileData;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.DiscAccess.Implementation.EntryInfo;

public class ZipArchiveFolderEntryInfo : ZipArchiveEntryInfoBase
{
	public ZipArchiveFolderEntryInfo(
		IFileSystemEntryInfo? parent,
		ZipArchiveEntryInfo parentArchive,
		string archiveFolderName,
		string archiveFolderPath,
		IImage archiveFolderIcon)
			: base(
				parent,
				archiveFolderName,
				archiveFolderPath,
				false,
				archiveFolderIcon)
	{
		_parentArchive = parentArchive;

		DriveOrFolder = _parentArchive.DriveOrFolder;
		HasSubFolders = HasArchiveSubFoldersUnderPath(
			_parentArchive.Entries, Path);
	}

	public override string QualifiedPath
	{
		get
		{
			var normalizedPath = Path[..^1].Replace(
				System.IO.Path.ZipArchiveDirectorySeparatorChar,
				System.IO.Path.DirectorySeparatorChar);

			return $"{_parentArchive.Path}{System.IO.Path.DirectorySeparatorChar}{normalizedPath}";
		}
	}

	public override IReadOnlyList<IFileSystemEntryInfo> GetSubFolders(
		IFileSystemEntryInfoFactory fileSystemEntryInfoFactory,
		FileSystemEntryInfoOrdering folderOrdering,
		FileSystemEntryInfoOrderingDirection folderOrderingDirection,
		bool zipArchivesEnabled,
		StringComparer nameComparer,
		Random randomShuffler)
	{
		try
		{
			var subFolderEntries = _parentArchive.Entries
				.Where(anEntry => anEntry.IsDirectSubFolderOf(Path))
				.ToList();

			var orderedSubFolderEntries = GetOrderedZipArchiveEntryList(
				subFolderEntries,
				folderOrdering,
				folderOrderingDirection,
				nameComparer,
				randomShuffler);

			var subFolders = orderedSubFolderEntries
				.Select(aSubFolderEntry =>
					fileSystemEntryInfoFactory.GetZipArchiveFolderEntryInfo(
						this,
						_parentArchive,
						aSubFolderEntry.FullName[Path.Length..^1],
						aSubFolderEntry.FullName))
				.ToList();

			return subFolders;
		}
		catch
		{
			return EmptyFileSystemEntryInfoList;
		}
	}

	public override IReadOnlyList<IImageFileData> GetImageFileDataList(
		HashSet<string> enabledImageFileExtensions)
	{
		try
		{
			var imageFileEntries = _parentArchive.Entries
				.Where(anEntry => anEntry.IsImageFileDirectlyUnderPath(
					Path, enabledImageFileExtensions))
				.ToList();

			var imageFileDataList = imageFileEntries
				.Select(anImageFileEntry =>
					(IImageFileData)new ZipArchiveImageFileData(
						anImageFileEntry.Name,
						anImageFileEntry.FullName,
						System.IO.Path.GetExtension(anImageFileEntry.Name),
						System.IO.Path.GetFileNameWithoutExtension(
							anImageFileEntry.Name),
						(int)anImageFileEntry.Length,
						anImageFileEntry.LastWriteTime.UtcDateTime,
						_parentArchive.Path))
				.ToList();

			return imageFileDataList;
		}
		catch
		{
			return EmptyImageFileDataList;
		}
	}

	private readonly ZipArchiveEntryInfo _parentArchive;
}
