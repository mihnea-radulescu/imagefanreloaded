using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using ImageFanReloaded.Core.DiscAccess.EntryInfo;
using ImageFanReloaded.Core.DiscAccess.Implementation.Extensions;
using ImageFanReloaded.Core.ImageCore;
using ImageFanReloaded.Core.ImageHandling.ImageFileData;
using ImageFanReloaded.Core.ImageHandling.Implementation.ImageFileData;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.DiscAccess.Implementation.EntryInfo;

public class ZipArchiveEntryInfo : FileSystemEntryInfoBase
{
	public ZipArchiveEntryInfo(
		IFileSystemEntryInfo? parent,
		string archiveName,
		string archivePath,
		IImage archiveIcon)
			: base(
				parent,
				archiveName,
				archivePath,
				HasArchiveSubFolders(archivePath),
				archiveIcon)
	{
		DriveOrFolder = parent;
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
			using var zipArchive = ZipFile.OpenRead(Path);

			var subFolderEntries = zipArchive.Entries
				.Where(anEntry => anEntry.IsFirstLevelSubFolder)
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
						this,
						aSubFolderEntry.FullName[..^1],
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
			using var zipArchive = ZipFile.OpenRead(Path);

			var imageFileEntries = zipArchive.Entries
				.Where(anEntry => anEntry.IsImageFileDirectlyUnderPath(
					string.Empty,
					enabledImageFileExtensions))
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
						Path))
				.ToList();

			return imageFileDataList;
		}
		catch
		{
			return EmptyImageFileDataList;
		}
	}

	private static bool HasArchiveSubFolders(string path)
	{
		try
		{
			using var zipArchive = ZipFile.OpenRead(path);

			var hasAnySubFolders = zipArchive.Entries
				.Any(anEntry => anEntry.IsDirectSubFolderOf(string.Empty));

			return hasAnySubFolders;
		}
		catch
		{
			return false;
		}
	}
}
