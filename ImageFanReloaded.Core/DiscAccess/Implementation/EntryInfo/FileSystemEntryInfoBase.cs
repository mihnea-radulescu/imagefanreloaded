using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using ImageFanReloaded.Core.DiscAccess.EntryInfo;
using ImageFanReloaded.Core.ImageCore;
using ImageFanReloaded.Core.ImageHandling.ImageFileData;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.DiscAccess.Implementation.EntryInfo;

public abstract class FileSystemEntryInfoBase : IFileSystemEntryInfo
{
	protected FileSystemEntryInfoBase(
		IFileSystemEntryInfo? parent,
		string name,
		string path,
		bool hasSubFolders,
		IImage icon)
	{
		Parent = parent;

		Name = name;
		Path = path;
		HasSubFolders = hasSubFolders;

		ImageFilesTotalSizeInBytes = 0;

		Icon = icon;
	}

	public IFileSystemEntryInfo? Parent { get; }
	public IFileSystemEntryInfo? DriveOrFolder { get; protected set; }

	public string Name { get; }
	public string Path { get; }
	public virtual string QualifiedPath => Path;
	public bool HasSubFolders { get; }

	public long ImageFilesTotalSizeInBytes { get; set; }

	public IImage Icon { get; }

	public abstract IReadOnlyList<IFileSystemEntryInfo> GetSubFolders(
		IFileSystemEntryInfoFactory fileSystemEntryInfoFactory,
		FileSystemEntryInfoOrdering folderOrdering,
		FileSystemEntryInfoOrderingDirection folderOrderingDirection,
		bool zipArchivesEnabled,
		StringComparer nameComparer,
		Random randomShuffler);

	public abstract IReadOnlyList<IImageFileData> GetImageFileDataList(
		HashSet<string> enabledImageFileExtensions);

	protected static readonly IReadOnlyList<IFileSystemEntryInfo>
		EmptyFileSystemEntryInfoList = [];
	protected static readonly IReadOnlyList<IImageFileData>
		EmptyImageFileDataList = [];

	protected static IReadOnlyList<FileSystemInfo> GetOrderedFileSystemInfoList(
		IReadOnlyList<FileSystemInfo> fileSystemInfoList,
		FileSystemEntryInfoOrdering fileSystemInfoOrdering,
		FileSystemEntryInfoOrderingDirection fileSystemInfoOrderingDirection,
		StringComparer nameComparer,
		Random randomShuffler)
	{
		var orderedFileSystemInfoList = fileSystemInfoList;

		if (fileSystemInfoOrdering == FileSystemEntryInfoOrdering.Name)
		{
			if (fileSystemInfoOrderingDirection ==
			    FileSystemEntryInfoOrderingDirection.Ascending)
			{
				orderedFileSystemInfoList = orderedFileSystemInfoList
					.OrderBy(aFileSystemInfo =>
						aFileSystemInfo.Name, nameComparer)
					.ToList();
			}
			else if (fileSystemInfoOrderingDirection ==
			         FileSystemEntryInfoOrderingDirection.Descending)
			{
				orderedFileSystemInfoList = orderedFileSystemInfoList
					.OrderByDescending(aFileSystemInfo =>
						aFileSystemInfo.Name, nameComparer)
					.ToList();
			}
		}
		else if (fileSystemInfoOrdering ==
		         FileSystemEntryInfoOrdering.LastModificationTime)
		{
			if (fileSystemInfoOrderingDirection ==
			    FileSystemEntryInfoOrderingDirection.Ascending)
			{
				orderedFileSystemInfoList = orderedFileSystemInfoList
					.OrderBy(aFileSystemInfo =>
						aFileSystemInfo.LastWriteTimeUtc)
					.ToList();
			}
			else if (fileSystemInfoOrderingDirection ==
			         FileSystemEntryInfoOrderingDirection.Descending)
			{
				orderedFileSystemInfoList = orderedFileSystemInfoList
					.OrderByDescending(aFileSystemInfo =>
						aFileSystemInfo.LastWriteTimeUtc)
					.ToList();
			}
		}
		else if (fileSystemInfoOrdering ==
		         FileSystemEntryInfoOrdering.RandomShuffle)
		{
			orderedFileSystemInfoList = orderedFileSystemInfoList
				.OrderBy(_ => randomShuffler.Next())
				.ToList();
		}

		return orderedFileSystemInfoList;
	}

	protected static IReadOnlyList<ZipArchiveEntry>
		GetOrderedZipArchiveEntryList(
			IReadOnlyList<ZipArchiveEntry> zipArchiveEntryList,
			FileSystemEntryInfoOrdering fileSystemInfoOrdering,
			FileSystemEntryInfoOrderingDirection
				fileSystemInfoOrderingDirection,
			StringComparer nameComparer,
			Random randomShuffler)
	{
		var orderedZipArchiveEntryList = zipArchiveEntryList;

		if (fileSystemInfoOrdering == FileSystemEntryInfoOrdering.Name)
		{
			if (fileSystemInfoOrderingDirection ==
			    FileSystemEntryInfoOrderingDirection.Ascending)
			{
				orderedZipArchiveEntryList = orderedZipArchiveEntryList
					.OrderBy(aZipArchiveEntry =>
						aZipArchiveEntry.FullName, nameComparer)
					.ToList();
			}
			else if (fileSystemInfoOrderingDirection ==
			         FileSystemEntryInfoOrderingDirection.Descending)
			{
				orderedZipArchiveEntryList = orderedZipArchiveEntryList
					.OrderByDescending(aZipArchiveEntry =>
						aZipArchiveEntry.FullName, nameComparer)
					.ToList();
			}
		}
		else if (fileSystemInfoOrdering ==
		         FileSystemEntryInfoOrdering.LastModificationTime)
		{
			if (fileSystemInfoOrderingDirection ==
			    FileSystemEntryInfoOrderingDirection.Ascending)
			{
				orderedZipArchiveEntryList = orderedZipArchiveEntryList
					.OrderBy(aZipArchiveEntry =>
						aZipArchiveEntry.LastWriteTime)
					.ToList();
			}
			else if (fileSystemInfoOrderingDirection ==
			         FileSystemEntryInfoOrderingDirection.Descending)
			{
				orderedZipArchiveEntryList = orderedZipArchiveEntryList
					.OrderByDescending(aZipArchiveEntry =>
						aZipArchiveEntry.LastWriteTime)
					.ToList();
			}
		}
		else if (fileSystemInfoOrdering ==
		         FileSystemEntryInfoOrdering.RandomShuffle)
		{
			orderedZipArchiveEntryList = orderedZipArchiveEntryList
				.OrderBy(_ => randomShuffler.Next())
				.ToList();
		}

		return orderedZipArchiveEntryList;
	}
}
