using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImageFanReloaded.Core.DiscAccess.DriveInfo;
using ImageFanReloaded.Core.DiscAccess.EntryInfo;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.ImageHandling.Factories;
using ImageFanReloaded.Core.ImageHandling.ImageFileData;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.DiscAccess.Implementation;

public class DiscQueryEngineFileSystem : IDiscQueryEngineFileSystem
{
	public DiscQueryEngineFileSystem(
		IGlobalParameters globalParameters,
		IFileSystemEntryInfoFactory fileSystemEntryInfoFactory,
		IDriveInfo driveInfo,
		IImageFileFactory imageFileFactory)
	{
		_globalParameters = globalParameters;
		_fileSystemEntryInfoFactory = fileSystemEntryInfoFactory;
		_driveInfo = driveInfo;
		_imageFileFactory = imageFileFactory;
	}

	public void BuildSkipRecursionFolderPaths()
	{
		var homePath = _globalParameters.UserHomePath;

		var drivePaths = System.IO.DriveInfo.GetDrives()
			.Select(aDriveInfo => aDriveInfo.Name)
			.Where(IsSupportedDrive)
			.OrderBy(aDriveName => aDriveName, _globalParameters.NameComparer)
			.ToList();

		_skipRecursionFolderPaths = new HashSet<string>(
			[homePath, ..drivePaths], _globalParameters.NameComparer);
	}

	public IReadOnlyList<IFileSystemEntryInfo> GetRootFolders(
		ITabOptions tabOptions)
	{
		var userFolders = GetUserFolders(tabOptions.ZipArchivesEnabled);
		var drives = GetDrives(tabOptions.ZipArchivesEnabled);

		IReadOnlyList<IFileSystemEntryInfo> rootFolders =
			[..userFolders, ..drives];
		return rootFolders;
	}

	public IReadOnlyList<IImageFile> GetImageFilesDefault(string folderPath)
	{
		var folderEntryInfo = _fileSystemEntryInfoFactory.GetFolderEntryInfo(
			null, folderPath, false);

		return GetImageFilesInternal(
			FileSystemEntryInfoOrdering.Name,
			FileSystemEntryInfoOrderingDirection.Ascending,
			folderEntryInfo,
			FileSystemEntryInfoOrdering.Name,
			FileSystemEntryInfoOrderingDirection.Ascending,
			_globalParameters.ImageFileExtensions,
			false,
			false,
			false);
	}

	public IReadOnlyList<IFileSystemEntryInfo> GetSubFolders(
		IFileSystemEntryInfo fileSystemEntryInfo, ITabOptions tabOptions)
	{
		var subFolders = fileSystemEntryInfo.GetSubFolders(
			_fileSystemEntryInfoFactory,
			tabOptions.FolderOrdering,
			tabOptions.FolderOrderingDirection,
			tabOptions.ZipArchivesEnabled,
			_globalParameters.NameComparer,
			RandomShuffler);

		return subFolders;
	}

	public IReadOnlyList<IImageFile> GetImageFiles(
		IFileSystemEntryInfo fileSystemEntryInfo, ITabOptions tabOptions)
	{
		return GetImageFilesInternal(
			tabOptions.FolderOrdering,
			tabOptions.FolderOrderingDirection,
			fileSystemEntryInfo,
			tabOptions.ImageFileOrdering,
			tabOptions.ImageFileOrderingDirection,
			tabOptions.EnabledImageFileExtensions,
			tabOptions.ZipArchivesEnabled,
			tabOptions.RecursiveFolderBrowsing,
			tabOptions.GlobalOrderingForRecursiveFolderBrowsing);
	}

	private static readonly IReadOnlyList<IFileSystemEntryInfo>
		EmptyFileSystemEntryInfoList = [];
	private static readonly IReadOnlyList<IImageFileData>
		EmptyImageFileDataList = [];

	private static readonly Random RandomShuffler = new();

	private readonly IGlobalParameters _globalParameters;
	private readonly IFileSystemEntryInfoFactory _fileSystemEntryInfoFactory;
	private readonly IDriveInfo _driveInfo;
	private readonly IImageFileFactory _imageFileFactory;

	private HashSet<string>? _skipRecursionFolderPaths;

	private IReadOnlyList<IFileSystemEntryInfo> GetUserFolders(
		bool zipArchivesEnabled)
	{
		try
		{
			var homeFolder = _fileSystemEntryInfoFactory
				.GetHomeFolderEntryInfo(
					_globalParameters.UserHomePath, zipArchivesEnabled);

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
					_fileSystemEntryInfoFactory.GetSpecialFolderEntryInfo(
						aSpecialFolderWithPath.Name,
						aSpecialFolderWithPath.Path,
						zipArchivesEnabled))
				.OrderBy(aSpecialFolderInfo =>
							aSpecialFolderInfo.Name,
							_globalParameters.NameComparer)
				.ToList();

			IReadOnlyList<IFileSystemEntryInfo> userFolders =
				[homeFolder, ..specialFolders];
			return userFolders;
		}
		catch
		{
			return EmptyFileSystemEntryInfoList;
		}
	}

	private IReadOnlyList<IFileSystemEntryInfo> GetDrives(
		bool zipArchivesEnabled)
	{
		try
		{
			return System.IO.DriveInfo.GetDrives()
				.Select(aDriveInfo => aDriveInfo.Name)
				.Where(IsSupportedDrive)
				.Select(aDriveName => _fileSystemEntryInfoFactory
					.GetDriveEntryInfo(aDriveName, zipArchivesEnabled))
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
		FileSystemEntryInfoOrdering folderOrdering,
		FileSystemEntryInfoOrderingDirection folderOrderingDirection,
		IFileSystemEntryInfo fileSystemEntryInfo,
		FileSystemEntryInfoOrdering imageFileOrdering,
		FileSystemEntryInfoOrderingDirection imageFileOrderingDirection,
		HashSet<string> enabledImageFileExtensions,
		bool zipArchivesEnabled,
		bool recursiveFolderBrowsing,
		bool globalOrderingForRecursiveFolderBrowsing)
	{
		var imageFileDataList = GetImageFileDataList(
			folderOrdering,
			folderOrderingDirection,
			fileSystemEntryInfo,
			imageFileOrdering,
			imageFileOrderingDirection,
			enabledImageFileExtensions,
			zipArchivesEnabled,
			recursiveFolderBrowsing,
			globalOrderingForRecursiveFolderBrowsing);

		var imageFiles = imageFileDataList
			.Select(anImageFileData =>
				_imageFileFactory.GetImageFile(anImageFileData))
			.ToList();

		fileSystemEntryInfo.ImageFilesTotalSizeInBytes =
			GetImageFilesTotalSizeInBytes(imageFiles);

		return imageFiles;
	}

	private IReadOnlyList<IImageFileData> GetImageFileDataList(
		FileSystemEntryInfoOrdering folderOrdering,
		FileSystemEntryInfoOrderingDirection folderOrderingDirection,
		IFileSystemEntryInfo fileSystemEntryInfo,
		FileSystemEntryInfoOrdering imageFileOrdering,
		FileSystemEntryInfoOrderingDirection imageFileOrderingDirection,
		HashSet<string> enabledImageFileExtensions,
		bool zipArchivesEnabled,
		bool recursiveFolderBrowsing,
		bool globalOrderingForRecursiveFolderBrowsing,
		int currentDepth = 0)
	{
		var shouldRecursivelySearchSubFolders =
			recursiveFolderBrowsing &&
			currentDepth < _globalParameters.MaxRecursionDepth &&
			!_skipRecursionFolderPaths!.Contains(fileSystemEntryInfo.Path);

		var shouldApplyLocalOrdering =
			!recursiveFolderBrowsing ||
			!globalOrderingForRecursiveFolderBrowsing;

		var shouldApplyGlobalOrdering =
			recursiveFolderBrowsing &&
			globalOrderingForRecursiveFolderBrowsing &&
			currentDepth == 0;

		try
		{
			var imageFileDataList = fileSystemEntryInfo
				.GetImageFileDataList(enabledImageFileExtensions);

			if (shouldApplyLocalOrdering)
			{
				imageFileDataList = GetOrderedImageFileDataList(
					imageFileDataList,
					imageFileOrdering,
					imageFileOrderingDirection);
			}

			if (shouldRecursivelySearchSubFolders)
			{
				var subFolders = fileSystemEntryInfo.GetSubFolders(
					_fileSystemEntryInfoFactory,
					folderOrdering,
					folderOrderingDirection,
					zipArchivesEnabled,
					_globalParameters.NameComparer,
					RandomShuffler);

				foreach (var aSubFolder in subFolders)
				{
					var subFolderImageFileDataList = GetImageFileDataList(
						folderOrdering,
						folderOrderingDirection,
						aSubFolder,
						imageFileOrdering,
						imageFileOrderingDirection,
						enabledImageFileExtensions,
						zipArchivesEnabled,
						recursiveFolderBrowsing,
						globalOrderingForRecursiveFolderBrowsing,
						currentDepth + 1);

					imageFileDataList =
					[
						..imageFileDataList,
						..subFolderImageFileDataList
					];
				}
			}

			if (shouldApplyGlobalOrdering)
			{
				imageFileDataList = GetOrderedImageFileDataList(
					imageFileDataList,
					imageFileOrdering,
					imageFileOrderingDirection);
			}

			return imageFileDataList;
		}
		catch
		{
			return EmptyImageFileDataList;
		}
	}

	private IReadOnlyList<IImageFileData> GetOrderedImageFileDataList(
		IReadOnlyList<IImageFileData> imageFileDataList,
		FileSystemEntryInfoOrdering fileSystemInfoOrdering,
		FileSystemEntryInfoOrderingDirection fileSystemInfoOrderingDirection)
	{
		IReadOnlyList<IImageFileData> orderedImageFileDataList =
			new List<IImageFileData>(imageFileDataList);

		if (fileSystemInfoOrdering == FileSystemEntryInfoOrdering.Name)
		{
			if (fileSystemInfoOrderingDirection ==
			    FileSystemEntryInfoOrderingDirection.Ascending)
			{
				orderedImageFileDataList = orderedImageFileDataList
					.OrderBy(anImageFileData =>
						anImageFileData.FileName,
						_globalParameters.NameComparer)
					.ToList();
			}
			else if (fileSystemInfoOrderingDirection ==
			         FileSystemEntryInfoOrderingDirection.Descending)
			{
				orderedImageFileDataList = orderedImageFileDataList
					.OrderByDescending(anImageFileData =>
						anImageFileData.FileName,
						_globalParameters.NameComparer)
					.ToList();
			}
		}
		else if (fileSystemInfoOrdering ==
		         FileSystemEntryInfoOrdering.LastModificationTime)
		{
			if (fileSystemInfoOrderingDirection ==
			    FileSystemEntryInfoOrderingDirection.Ascending)
			{
				orderedImageFileDataList = orderedImageFileDataList
					.OrderBy(anImageFileData =>
						anImageFileData.FileLastModificationTime)
					.ToList();
			}
			else if (fileSystemInfoOrderingDirection ==
			         FileSystemEntryInfoOrderingDirection.Descending)
			{
				orderedImageFileDataList = orderedImageFileDataList
					.OrderByDescending(anImageFileData =>
						anImageFileData.FileLastModificationTime)
					.ToList();
			}
		}
		else if (fileSystemInfoOrdering ==
		         FileSystemEntryInfoOrdering.RandomShuffle)
		{
			orderedImageFileDataList = orderedImageFileDataList
				.OrderBy(_ => RandomShuffler.Next())
				.ToList();
		}

		return orderedImageFileDataList;
	}

	private bool IsSupportedDrive(string driveName)
		=> _driveInfo.IsSupportedDrive(driveName);

	private static long GetImageFilesTotalSizeInBytes(
		IReadOnlyList<IImageFile> imageFiles)
	{
		var imageFilesTotalSizeInBytes = imageFiles
			.Sum(anImageFile
				=> (long)anImageFile.ImageFileData.FileSizeInBytes);

		return imageFilesTotalSizeInBytes;
	}
}
