using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImageFanReloaded.Core.DiscAccess.EntryInfo;
using ImageFanReloaded.Core.ImageCore;
using ImageFanReloaded.Core.ImageHandling.ImageFileData;
using ImageFanReloaded.Core.ImageHandling.Implementation.ImageFileData;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.DiscAccess.Implementation.EntryInfo;

public abstract class DriveOrFolderEntryInfoBase : FileSystemEntryInfoBase
{
	protected DriveOrFolderEntryInfoBase(
		IFileSystemEntryInfo? parent,
		string name,
		string path,
		bool zipArchivesEnabled,
		IImage icon)
			: base(
				parent,
				name,
				path,
				HasFileSystemSubFolders(path, zipArchivesEnabled),
				icon)
	{
		DriveOrFolder = this;
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
			var folderInfo = new DirectoryInfo(Path);

			var subFolderEntries = GetSubFolderEntries(
				fileSystemEntryInfoFactory,
				folderOrdering,
				folderOrderingDirection,
				zipArchivesEnabled,
				nameComparer,
				randomShuffler,
				folderInfo);

			IReadOnlyList<IFileSystemEntryInfo> subFolders;

			if (zipArchivesEnabled)
			{
				var zipArchiveEntries = GetZipArchiveEntries(
					fileSystemEntryInfoFactory,
					folderOrdering,
					folderOrderingDirection,
					nameComparer,
					randomShuffler,
					folderInfo);

				subFolders = [..subFolderEntries, ..zipArchiveEntries];
			}
			else
			{
				subFolders = subFolderEntries;
			}

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
			var folderInfo = new DirectoryInfo(Path);

			var imageFileDataList = folderInfo
				.GetFiles("*", SearchOption.TopDirectoryOnly)
				.Where(aFileInfo => enabledImageFileExtensions
					.Contains(aFileInfo.Extension))
				.Select(aFileInfo =>
					(IImageFileData)new FileSystemImageFileData(
						aFileInfo.Name,
						aFileInfo.FullName,
						aFileInfo.Extension,
						System.IO.Path.GetFileNameWithoutExtension(
							aFileInfo.Name),
						(int)aFileInfo.Length,
						aFileInfo.LastWriteTimeUtc,
						Path))
				.ToList();

			return imageFileDataList;
		}
		catch
		{
			return EmptyImageFileDataList;
		}
	}

	private const string ZipFilesSearchPattern = "*.zip";

	private static readonly EnumerationOptions FilesInFolderEnumerationOptions =
		new() { MatchCasing = MatchCasing.CaseInsensitive };

	private static bool HasFileSystemSubFolders(
		string path, bool zipArchivesEnabled)
	{
		try
		{
			var subFoldersEnumerable = Directory.EnumerateDirectories(path);
			using var subFoldersEnumerator = subFoldersEnumerable
				.GetEnumerator();
			var hasSubFolders = subFoldersEnumerator.MoveNext();

			if (hasSubFolders)
			{
				return true;
			}

			bool hasFileSystemSubFolders;

			if (zipArchivesEnabled)
			{
				var currentFolder = new DirectoryInfo(path);
				var filesInFolderEnumerable = currentFolder.EnumerateFiles(
					ZipFilesSearchPattern, FilesInFolderEnumerationOptions);
				using var filesInFolderEnumerator = filesInFolderEnumerable
					.GetEnumerator();
				var hasZipArchives = filesInFolderEnumerator.MoveNext();

				hasFileSystemSubFolders = hasZipArchives;
			}
			else
			{
				hasFileSystemSubFolders = false;
			}

			return hasFileSystemSubFolders;
		}
		catch
		{
			return false;
		}
	}

	private IReadOnlyList<IFileSystemEntryInfo> GetSubFolderEntries(
		IFileSystemEntryInfoFactory fileSystemEntryInfoFactory,
		FileSystemEntryInfoOrdering folderOrdering,
		FileSystemEntryInfoOrderingDirection folderOrderingDirection,
		bool zipArchivesEnabled,
		StringComparer nameComparer,
		Random randomShuffler,
		DirectoryInfo folderInfo)
	{
		var subFolderInfoList = folderInfo
			.GetDirectories()
			.ToList();

		var orderedSubFolderInfoList = GetOrderedFileSystemInfoList(
			subFolderInfoList,
			folderOrdering,
			folderOrderingDirection,
			nameComparer,
			randomShuffler);

		var subFolderEntries = orderedSubFolderInfoList
			.Select(aFileSystemInfo =>
				fileSystemEntryInfoFactory.GetFolderEntryInfo(
					this, aFileSystemInfo.FullName, zipArchivesEnabled))
			.ToList();

		return subFolderEntries;
	}

	private IReadOnlyList<IFileSystemEntryInfo> GetZipArchiveEntries(
		IFileSystemEntryInfoFactory fileSystemEntryInfoFactory,
		FileSystemEntryInfoOrdering folderOrdering,
		FileSystemEntryInfoOrderingDirection folderOrderingDirection,
		StringComparer nameComparer, Random randomShuffler,
		DirectoryInfo folderInfo)
	{
		var zipArchiveInfoList = folderInfo
			.GetFiles(ZipFilesSearchPattern, FilesInFolderEnumerationOptions)
			.ToList();

		var orderedZipArchiveInfoList = GetOrderedFileSystemInfoList(
			zipArchiveInfoList,
			folderOrdering,
			folderOrderingDirection,
			nameComparer,
			randomShuffler);

		var zipArchiveEntries = orderedZipArchiveInfoList
			.Select(aFileSystemInfo =>
				fileSystemEntryInfoFactory.GetZipArchiveEntryInfo(
					this, aFileSystemInfo.FullName))
			.ToList();

		return zipArchiveEntries;
	}
}
