using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImageFanReloaded.Core.ImageCore;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.ImageHandling.Factories;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.DiscAccess.Implementation;

public class DiscQueryEngineFileSystem : IDiscQueryEngineFileSystem
{
	static DiscQueryEngineFileSystem()
	{
		EmptyFileInfoList = Enumerable.Empty<FileInfo>().ToList();
		EmptyFileSystemEntryInfoList = Enumerable
			.Empty<FileSystemEntryInfo>()
			.ToList();
		EmptyImageFileList = Enumerable.Empty<IImageFile>().ToList();

		RandomShuffler = new Random();
	}

	public DiscQueryEngineFileSystem(
		IGlobalParameters globalParameters,
		IDriveInfo driveInfo,
		IImageFileFactory imageFileFactory)
	{
		_globalParameters = globalParameters;
		_driveInfo = driveInfo;
		_imageFileFactory = imageFileFactory;

		_specialFolderToIconMapping = new Dictionary<string, IImage>
		{
			{ "Desktop", _globalParameters.DesktopFolderIcon },
			{ "Documents", _globalParameters.DocumentsFolderIcon },
			{ "Downloads", _globalParameters.DownloadsFolderIcon },
			{ "Pictures", _globalParameters.PicturesFolderIcon },
		};
	}

	public void BuildSkipRecursionFolderPaths()
	{
		var homePath = _globalParameters.UserHomePath;

		var drivePaths = DriveInfo.GetDrives()
			.Select(aDriveInfo => aDriveInfo.Name)
			.Where(IsSupportedDrive)
			.OrderBy(aDriveName => aDriveName, _globalParameters.NameComparer)
			.ToList();

		_skipRecursionFolderPaths = new HashSet<string>(
			[homePath, ..drivePaths], _globalParameters.NameComparer);
	}

	public IReadOnlyList<FileSystemEntryInfo> GetRootFolders()
	{
		var userFolders = GetUserFolders();
		var drives = GetDrives();

		IReadOnlyList<FileSystemEntryInfo> rootFolders =
			[..userFolders, ..drives];
		return rootFolders;
	}

	public IReadOnlyList<IImageFile> GetImageFilesDefault(
		string folderPath)
	{
		return GetImageFilesInternal(
			folderPath,
			FileSystemEntryInfoOrdering.Name,
			FileSystemEntryInfoOrderingDirection.Ascending,
			_globalParameters.ImageFileExtensions,
			false,
			false);
	}

	public IReadOnlyList<FileSystemEntryInfo> GetSubFolders(
		string folderPath, ITabOptions tabOptions)
	{
		try
		{
			var subFolderInfoList = new DirectoryInfo(folderPath)
				.GetDirectories()
				.ToList();

			var orderedSubFolderInfoList = GetOrderedFileSystemInfoList(
				subFolderInfoList,
				tabOptions.FolderOrdering,
				tabOptions.FolderOrderingDirection);

			var subFolders = orderedSubFolderInfoList
				.Select(aDirectory =>
					new FileSystemEntryInfo(
						aDirectory.Name,
						aDirectory.FullName,
						HasSubFolders(aDirectory.FullName),
						_globalParameters.FolderIcon))
				.ToList();

			return subFolders;
		}
		catch
		{
			return EmptyFileSystemEntryInfoList;
		}
	}

	public IReadOnlyList<IImageFile> GetImageFiles(
		string folderPath, ITabOptions tabOptions)
	{
		return GetImageFilesInternal(
			folderPath,
			tabOptions.ImageFileOrdering,
			tabOptions.ImageFileOrderingDirection,
			tabOptions.EnabledImageFileExtensions,
			tabOptions.RecursiveFolderBrowsing,
			tabOptions.GlobalOrderingForRecursiveFolderBrowsing);
	}

	private static readonly IReadOnlyList<FileInfo> EmptyFileInfoList;
	private static readonly IReadOnlyList<FileSystemEntryInfo>
		EmptyFileSystemEntryInfoList;
	private static readonly IReadOnlyList<IImageFile> EmptyImageFileList;

	private static readonly Random RandomShuffler;

	private readonly IGlobalParameters _globalParameters;
	private readonly IDriveInfo _driveInfo;
	private readonly IImageFileFactory _imageFileFactory;

	private readonly IReadOnlyDictionary<string, IImage>
		_specialFolderToIconMapping;

	private HashSet<string>? _skipRecursionFolderPaths;

	private IReadOnlyList<FileSystemEntryInfo> GetUserFolders()
	{
		try
		{
			var homeFolder = new FileSystemEntryInfo(
				"Home",
				_globalParameters.UserHomePath,
				HasSubFolders(_globalParameters.UserHomePath),
				_globalParameters.HomeFolderIcon);

			var specialFolders = _globalParameters.SpecialFolders
				.Select(aSpecialFolder =>
					new
					{
						Name = aSpecialFolder,
						Path = Path.Combine(
							_globalParameters.UserHomePath, aSpecialFolder)
					})
				.Where(aSpecialFolderWithPath => Path.Exists(
						aSpecialFolderWithPath.Path))
				.Select(aSpecialFolderWithPath =>
					new FileSystemEntryInfo(
						aSpecialFolderWithPath.Name,
						aSpecialFolderWithPath.Path,
						HasSubFolders(aSpecialFolderWithPath.Path),
						GetIcon(aSpecialFolderWithPath.Name)))
				.OrderBy(aSpecialFolderInfo =>
							aSpecialFolderInfo.Name,
							_globalParameters.NameComparer)
				.ToList();

			IReadOnlyList<FileSystemEntryInfo> userFolders =
				[homeFolder, ..specialFolders];
			return userFolders;
		}
		catch
		{
			return EmptyFileSystemEntryInfoList;
		}
	}

	private IReadOnlyList<FileSystemEntryInfo> GetDrives()
	{
		try
		{
			return DriveInfo.GetDrives()
				.Select(aDriveInfo => aDriveInfo.Name)
				.Where(IsSupportedDrive)
				.Select(aDriveName =>
					new FileSystemEntryInfo(
						aDriveName,
						aDriveName,
						HasSubFolders(aDriveName),
						_globalParameters.DriveIcon))
				.OrderBy(aDriveInfo =>
							aDriveInfo.Name, _globalParameters.NameComparer)
				.ToList();
		}
		catch
		{
			return EmptyFileSystemEntryInfoList;
		}
	}

	private IReadOnlyList<IImageFile> GetImageFilesInternal(
		string folderPath,
		FileSystemEntryInfoOrdering imageFileOrdering,
		FileSystemEntryInfoOrderingDirection imageFileOrderingDirection,
		HashSet<string> enabledImageFileExtensions,
		bool recursiveFolderBrowsing,
		bool globalOrderingForRecursiveFolderBrowsing)
	{
		try
		{
			var imageFileInfoList = GetImageFileInfoList(
				folderPath,
				imageFileOrdering,
				imageFileOrderingDirection,
				enabledImageFileExtensions,
				recursiveFolderBrowsing,
				globalOrderingForRecursiveFolderBrowsing);

			var imageFiles = imageFileInfoList
				.Select(aFileInfo => _imageFileFactory.GetImageFile(
					new ImageFileData(
						aFileInfo.Name,
						aFileInfo.FullName,
						aFileInfo.Extension,
						Path.GetFileNameWithoutExtension(aFileInfo.Name),
						(int)aFileInfo.Length,
						aFileInfo.LastWriteTimeUtc,
						Path.GetDirectoryName(aFileInfo.FullName)!)))
				.ToList();

			return imageFiles;
		}
		catch
		{
			return EmptyImageFileList;
		}
	}

	private IReadOnlyList<FileInfo> GetImageFileInfoList(
		string folderPath,
		FileSystemEntryInfoOrdering imageFileOrdering,
		FileSystemEntryInfoOrderingDirection imageFileOrderingDirection,
		HashSet<string> enabledImageFileExtensions,
		bool recursiveFolderBrowsing,
		bool globalOrderingForRecursiveFolderBrowsing,
		int currentDepth = 0)
	{
		var shouldRecursivelySearchSubFolders =
			recursiveFolderBrowsing &&
			currentDepth < _globalParameters.MaxRecursionDepth &&
			!_skipRecursionFolderPaths!.Contains(folderPath);

		var shouldApplyLocalOrdering =
			!recursiveFolderBrowsing ||
			!globalOrderingForRecursiveFolderBrowsing;

		var shouldApplyGlobalOrdering =
			recursiveFolderBrowsing &&
			globalOrderingForRecursiveFolderBrowsing &&
			currentDepth == 0;

		try
		{
			var folderInfo = new DirectoryInfo(folderPath);

			var imageFileInfoList = folderInfo
				.GetFiles("*", SearchOption.TopDirectoryOnly)
				.Where(aFileInfo => enabledImageFileExtensions
						.Contains(aFileInfo.Extension))
				.ToList();

			if (shouldApplyLocalOrdering)
			{
				imageFileInfoList = GetOrderedFileSystemInfoList(
					imageFileInfoList,
					imageFileOrdering,
					imageFileOrderingDirection);
			}

			if (shouldRecursivelySearchSubFolders)
			{
				var subFolderPaths = folderInfo
					.GetDirectories()
					.Select(aDirectoryInfo => aDirectoryInfo.FullName)
					.OrderBy(aSubFolderPath => aSubFolderPath)
					.ToList();

				foreach (var aSubFolderPath in subFolderPaths)
				{
					var subFolderImageFileInfoList = GetImageFileInfoList(
						aSubFolderPath,
						imageFileOrdering,
						imageFileOrderingDirection,
						enabledImageFileExtensions,
						recursiveFolderBrowsing,
						globalOrderingForRecursiveFolderBrowsing,
						currentDepth + 1);

					imageFileInfoList.AddRange(subFolderImageFileInfoList);
				}
			}

			if (shouldApplyGlobalOrdering)
			{
				imageFileInfoList = GetOrderedFileSystemInfoList(
					imageFileInfoList,
					imageFileOrdering,
					imageFileOrderingDirection);
			}

			return imageFileInfoList;
		}
		catch
		{
			return EmptyFileInfoList;
		}
	}

	private List<TFileSystemInfo> GetOrderedFileSystemInfoList<TFileSystemInfo>(
		List<TFileSystemInfo> fileSystemInfoList,
		FileSystemEntryInfoOrdering fileSystemInfoOrdering,
		FileSystemEntryInfoOrderingDirection fileSystemInfoOrderingDirection)
		where TFileSystemInfo : FileSystemInfo
	{
		List<TFileSystemInfo> orderedFileSystemInfoList = fileSystemInfoList;

		if (fileSystemInfoOrdering == FileSystemEntryInfoOrdering.Name)
		{
			if (fileSystemInfoOrderingDirection ==
				FileSystemEntryInfoOrderingDirection.Ascending)
			{
				orderedFileSystemInfoList = orderedFileSystemInfoList
					.OrderBy(aFileSystemInfo =>
						aFileSystemInfo.Name, _globalParameters.NameComparer)
					.ToList();
			}
			else if (fileSystemInfoOrderingDirection ==
					 FileSystemEntryInfoOrderingDirection.Descending)
			{
				orderedFileSystemInfoList = orderedFileSystemInfoList
					.OrderByDescending(aFileSystemInfo =>
						aFileSystemInfo.Name, _globalParameters.NameComparer)
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
				.OrderBy(_ => RandomShuffler.Next())
				.ToList();
		}

		return orderedFileSystemInfoList;
	}

	private static bool HasSubFolders(string folderPath)
	{
		try
		{
			var subFoldersAsEnumerable = Directory.EnumerateDirectories(
				folderPath);

			using var subFoldersEnumerator = subFoldersAsEnumerable
				.GetEnumerator();
			return subFoldersEnumerator.MoveNext();
		}
		catch
		{
			return false;
		}
	}

	private bool IsSupportedDrive(string driveName)
		=> _driveInfo.IsSupportedDrive(driveName);

	private IImage GetIcon(string aSpecialFolderName)
		=> _specialFolderToIconMapping[aSpecialFolderName];
}
