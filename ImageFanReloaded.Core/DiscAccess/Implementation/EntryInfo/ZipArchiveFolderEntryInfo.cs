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

public class ZipArchiveFolderEntryInfo : FileSystemEntryInfoBase
{
	public ZipArchiveFolderEntryInfo(
		IFileSystemEntryInfo? parent,
		IFileSystemEntryInfo parentArchive,
		string archiveFolderName,
		string archiveFolderPath,
		IImage archiveFolderIcon)
			: base(
				parent,
				archiveFolderName,
				archiveFolderPath,
				HasArchiveSubFoldersUnderPath(
					parentArchive.Path, archiveFolderPath),
				archiveFolderIcon)
	{
		_parentArchive = parentArchive;

		DriveOrFolder = parentArchive.DriveOrFolder;
	}

	public override string QualifiedPath
		=> $"{_parentArchive.Path}{System.IO.Path.DirectorySeparatorChar}{Path[..^1]}";

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
			using var zipArchive = ZipFile.OpenRead(_parentArchive.Path);

			var subFolderEntries = zipArchive.Entries
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
			using var zipArchive = ZipFile.OpenRead(_parentArchive.Path);

			var imageFileEntries = zipArchive.Entries
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

	private readonly IFileSystemEntryInfo _parentArchive;

	private static bool HasArchiveSubFoldersUnderPath(
		string archivePath, string path)
	{
		try
		{
			using var zipArchive = ZipFile.OpenRead(archivePath);

			var hasAnySubFolders = zipArchive.Entries
				.Any(anEntry => anEntry.IsDirectSubFolderOf(path));

			return hasAnySubFolders;
		}
		catch
		{
			return false;
		}
	}
}
