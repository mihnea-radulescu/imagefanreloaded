using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.ImageHandling.Factories;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.DiscAccess.Implementation;

public abstract class DiscQueryEngineBase : IDiscQueryEngine
{
	static DiscQueryEngineBase()
	{
		EmptyFileInfoList = Enumerable.Empty<FileInfo>().ToList();
		EmptyFileSystemEntryInfoList = Enumerable
			.Empty<FileSystemEntryInfo>()
			.ToList();
		EmptyImageFileList = Enumerable.Empty<IImageFile>().ToList();

		RandomShuffler = new Random();
	}

	protected DiscQueryEngineBase(
		IGlobalParameters globalParameters,
		IImageFileFactory imageFileFactory,
		IFileSizeEngine fileSizeEngine)
	{
		_globalParameters = globalParameters;
		_imageFileFactory = imageFileFactory;
		_fileSizeEngine = fileSizeEngine;

		_specialFolderToIconMapping = new Dictionary<string, IImage>
		{
			{ "Desktop", _globalParameters.DesktopFolderIcon },
			{ "Documents", _globalParameters.DocumentsFolderIcon },
			{ "Downloads", _globalParameters.DownloadsFolderIcon },
			{ "Pictures", _globalParameters.PicturesFolderIcon },
		};
	}

	public async Task BuildSkipRecursionFolderPaths()
		=> await Task.Run(BuildSkipRecursionFolderPathsInternal);

	public async Task<IReadOnlyList<FileSystemEntryInfo>> GetRootFolders()
		=> await Task.Run(GetRootFoldersInternal);

	public async Task<FileSystemEntryInfo> GetFileSystemEntryInfo(
		string folderPath)
			=> await Task.Run(() => GetFileSystemEntryInfoInternal(folderPath));

	public async Task<IReadOnlyList<FileSystemEntryInfo>> GetSubFolders(
		string folderPath, ITabOptions tabOptions)
			=> await Task.Run(() => GetSubFoldersInternal(
				folderPath,
				tabOptions.FolderOrdering,
				tabOptions.FolderOrderingDirection));

	public async Task<IReadOnlyList<IImageFile>> GetImageFiles(
		string folderPath, ITabOptions tabOptions)
			=> await Task.Run(() => GetImageFilesInternal(
				folderPath,
				tabOptions.ImageFileOrdering,
				tabOptions.ImageFileOrderingDirection,
				tabOptions.RecursiveFolderBrowsing,
				tabOptions.GlobalOrderingForRecursiveFolderBrowsing));

	public async Task<IReadOnlyList<IImageFile>> GetImageFilesDefault(
		string folderPath)
			=> await Task.Run(() => GetImageFilesInternal(
				folderPath,
				FileSystemEntryInfoOrdering.Name,
				FileSystemEntryInfoOrderingDirection.Ascending,
				false,
				false));

	protected abstract bool IsSupportedDrive(string driveName);

	private static readonly IReadOnlyList<FileInfo> EmptyFileInfoList;
	private static readonly IReadOnlyList<FileSystemEntryInfo>
		EmptyFileSystemEntryInfoList;
	private static readonly IReadOnlyList<IImageFile> EmptyImageFileList;

	private static readonly Random RandomShuffler;

	private readonly IGlobalParameters _globalParameters;
	private readonly IImageFileFactory _imageFileFactory;
	private readonly IFileSizeEngine _fileSizeEngine;

	private readonly IReadOnlyDictionary<string, IImage>
		_specialFolderToIconMapping;

	private HashSet<string>? _skipRecursionFolderPaths;

	private void BuildSkipRecursionFolderPathsInternal()
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

	private IReadOnlyList<FileSystemEntryInfo> GetRootFoldersInternal()
	{
		var userFolders = GetUserFolders();
		var drives = GetDrives();

		IReadOnlyList<FileSystemEntryInfo> rootFolders =
			[..userFolders, ..drives];
		return rootFolders;
	}

	private FileSystemEntryInfo GetFileSystemEntryInfoInternal(
		string folderPath)
	{
		var folderInfo = new DirectoryInfo(folderPath);
		var folderName = folderInfo.Name;

		var hasSubFolders = HasSubFolders(folderPath);

		var fileSystemEntryInfo = new FileSystemEntryInfo(
			folderName,
			folderPath,
			hasSubFolders,
			_globalParameters.FolderIcon);

		return fileSystemEntryInfo;
	}

	private IReadOnlyList<FileSystemEntryInfo> GetSubFoldersInternal(
		string folderPath,
		FileSystemEntryInfoOrdering folderOrdering,
		FileSystemEntryInfoOrderingDirection folderOrderingDirection)
	{
		try
		{
			var subFolderInfoList = new DirectoryInfo(folderPath)
				.GetDirectories()
				.ToList();

			var orderedSubFolderInfoList = GetOrderedFileSystemInfoList(
				subFolderInfoList,
				folderOrdering,
				folderOrderingDirection);

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

	private IReadOnlyList<IImageFile> GetImageFilesInternal(
		string folderPath,
		FileSystemEntryInfoOrdering imageFileOrdering,
		FileSystemEntryInfoOrderingDirection imageFileOrderingDirection,
		bool recursiveFolderBrowsing,
		bool globalOrderingForRecursiveFolderBrowsing)
	{
		try
		{
			var imageFileInfoList = GetImageFileInfoList(
				folderPath,
				imageFileOrdering,
				imageFileOrderingDirection,
				recursiveFolderBrowsing,
				globalOrderingForRecursiveFolderBrowsing);

			var imageFiles = imageFileInfoList
				.Select(aFileInfo => _imageFileFactory.GetImageFile(
					new ImageFileData(
						aFileInfo.Name,
						aFileInfo.FullName,
						aFileInfo.Extension,
						Path.GetFileNameWithoutExtension(aFileInfo.Name),
						Path.GetDirectoryName(aFileInfo.FullName)!,
						_fileSizeEngine.ConvertToKilobytes(aFileInfo.Length),
						aFileInfo.LastWriteTime)))
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
				.Where(aFileInfo => _globalParameters
						.ImageFileExtensions
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

	private IImage GetIcon(string aSpecialFolderName)
		=> _specialFolderToIconMapping[aSpecialFolderName];
}
