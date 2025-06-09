using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.DiscAccess.Implementation;

public abstract class DiscQueryEngineBase : IDiscQueryEngine
{
	static DiscQueryEngineBase()
	{
		EmptyFileSystemEntryInfoCollection = Enumerable
			.Empty<FileSystemEntryInfo>()
			.ToList();

		EmptyImageFileCollection = Enumerable
			.Empty<IImageFile>()
			.ToList();
	}

	protected DiscQueryEngineBase(
		IGlobalParameters globalParameters,
		IFileSizeEngine fileSizeEngine,
		IImageFileFactory imageFileFactory)
	{
		_globalParameters = globalParameters;
		_fileSizeEngine = fileSizeEngine;
		_imageFileFactory = imageFileFactory;
		
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

	public async Task<IReadOnlyList<FileSystemEntryInfo>> GetSubFolders(
		string folderPath, FileSystemEntryInfoOrdering fileSystemEntryInfoOrdering)
		=> await Task.Run(() => GetSubFoldersInternal(folderPath, fileSystemEntryInfoOrdering));

	public async Task<FileSystemEntryInfo> GetFileSystemEntryInfo(string folderPath)
		=> await Task.Run(() => GetFileSystemEntryInfoInternal(folderPath));
	
	public async Task<IReadOnlyList<IImageFile>> GetImageFiles(string folderPath, bool recursive)
		=> await Task.Run(() => GetImageFilesInternal(folderPath, recursive));
	
	#region Protected

	protected abstract bool IsSupportedDrive(string driveName);
	
	#endregion

	#region Private
	
    private static readonly IReadOnlyList<FileSystemEntryInfo> EmptyFileSystemEntryInfoCollection;
    private static readonly IReadOnlyList<IImageFile> EmptyImageFileCollection;

	private readonly IGlobalParameters _globalParameters;
	private readonly IFileSizeEngine _fileSizeEngine;
	private readonly IImageFileFactory _imageFileFactory;
	
	private readonly IReadOnlyDictionary<string, IImage> _specialFolderToIconMapping;

	private HashSet<string>? _skipRecursionFolderPaths;

	private void BuildSkipRecursionFolderPathsInternal()
	{
		var homePath = _globalParameters.UserProfilePath;

		var drivePaths = DriveInfo.GetDrives()
			.Select(aDriveInfo => aDriveInfo.Name)
			.Where(IsSupportedDrive)
			.OrderBy(aDriveName => aDriveName, _globalParameters.NameComparer)
			.ToList();

		_skipRecursionFolderPaths = new HashSet<string>(
			[homePath, .. drivePaths],
			_globalParameters.NameComparer);
	}
	
	private IReadOnlyList<FileSystemEntryInfo> GetUserFolders()
	{
		try
		{
			var homeFolder = new FileSystemEntryInfo(
				"Home",
				_globalParameters.UserProfilePath,
				HasSubFolders(_globalParameters.UserProfilePath),
				_globalParameters.HomeFolderIcon);

			var specialFolders = _globalParameters.SpecialFolders
				.Select(aSpecialFolder =>
					new
					{
						Name = aSpecialFolder,
						Path = Path.Combine(_globalParameters.UserProfilePath, aSpecialFolder)
					})
				.Where(aSpecialFolderWithPath => Path.Exists(aSpecialFolderWithPath.Path))
				.Select(aSpecialFolderWithPath =>
					new FileSystemEntryInfo(
						aSpecialFolderWithPath.Name,
						aSpecialFolderWithPath.Path,
						HasSubFolders(aSpecialFolderWithPath.Path),
						GetIcon(aSpecialFolderWithPath.Name)))
				.OrderBy(aSpecialFolderInfo => aSpecialFolderInfo.Name, _globalParameters.NameComparer)
				.ToList();

			IReadOnlyList<FileSystemEntryInfo> userFolders = [homeFolder, .. specialFolders];
			return userFolders;
		}
		catch
		{
			return EmptyFileSystemEntryInfoCollection;
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
				.OrderBy(aDriveInfo => aDriveInfo.Name, _globalParameters.NameComparer)
				.ToList();
		}
		catch
		{
			return EmptyFileSystemEntryInfoCollection;
		}
	}

	private IReadOnlyList<FileSystemEntryInfo> GetRootFoldersInternal()
	{
		var userFolders = GetUserFolders();
		var drives = GetDrives();
			
		IReadOnlyList<FileSystemEntryInfo> rootFolders = [..userFolders, ..drives];
		return rootFolders;
	}
	
	private IReadOnlyList<FileSystemEntryInfo> GetSubFoldersInternal(
		string folderPath, FileSystemEntryInfoOrdering fileSystemEntryInfoOrdering)
	{
		try
		{
			var subFolderInfoCollection = new DirectoryInfo(folderPath)
				.GetDirectories()
				.AsQueryable();

			var orderedSubFolderInfoCollection = GetOrderedFileSystemInfoCollection(
				subFolderInfoCollection, fileSystemEntryInfoOrdering);
			
			var subFolders = orderedSubFolderInfoCollection
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
			return EmptyFileSystemEntryInfoCollection;
		}
	}

	private FileSystemEntryInfo GetFileSystemEntryInfoInternal(string folderPath)
	{
		var folderInfo = new DirectoryInfo(folderPath);
		var folderName = folderInfo.Name;

		var hasSubFolders = HasSubFolders(folderPath);

		var fileSystemEntryInfo = new FileSystemEntryInfo(
			folderName, folderPath, hasSubFolders, _globalParameters.FolderIcon);

		return fileSystemEntryInfo;
	}
	
	private IReadOnlyList<IImageFile> GetImageFilesInternal(
		string folderPath, bool recursive, int currentDepth = 0)
	{
		try
		{
			var folderInfo = new DirectoryInfo(folderPath);

			var filesInfo = folderInfo
				.GetFiles("*", SearchOption.TopDirectoryOnly)
				.ToList();

			var imageFiles = filesInfo
				.Where(aFileInfo =>
					_globalParameters.ImageFileExtensions.Contains(aFileInfo.Extension))
				.Select(aFileInfo => _imageFileFactory.GetImageFile(
					aFileInfo.Name,
					aFileInfo.FullName,
					_fileSizeEngine.ConvertToKilobytes(aFileInfo.Length)))
				.OrderBy(aFileInfo => aFileInfo.ImageFileName, _globalParameters.NameComparer)
				.ToList();

			var shouldRecursivelySearchSubFolders =
				recursive &&
				currentDepth < _globalParameters.MaxRecursionDepth &&
				!_skipRecursionFolderPaths!.Contains(folderPath);

			if (shouldRecursivelySearchSubFolders)
			{
				var subFolderPaths = folderInfo
					.GetDirectories()
					.Select(aDirectoryInfo => aDirectoryInfo.FullName)
					.OrderBy(aSubFolderPath => aSubFolderPath)
					.ToList();

				foreach (var aSubFolderPath in subFolderPaths)
				{
					var imageFilesInSubFolder = GetImageFilesInternal(
						aSubFolderPath, recursive, currentDepth + 1);

					imageFiles.AddRange(imageFilesInSubFolder);
				}
			}

			return imageFiles;
		}
		catch
		{
			return EmptyImageFileCollection;
		}
	}
    
    private static bool HasSubFolders(string folderPath)
    {
	    try
	    {
		    var subFoldersAsEnumerable = Directory.EnumerateDirectories(folderPath);

		    using var subFoldersEnumerator = subFoldersAsEnumerable.GetEnumerator();
		    return subFoldersEnumerator.MoveNext();
	    }
	    catch
	    {
		    return false;
	    }
    }
    
    private IImage GetIcon(string aSpecialFolderName) => _specialFolderToIconMapping[aSpecialFolderName];

    private IQueryable<FileSystemInfo> GetOrderedFileSystemInfoCollection(
	    IQueryable<FileSystemInfo> fileSystemInfoCollection,
	    FileSystemEntryInfoOrdering fileSystemEntryInfoOrdering)
    {
	    var orderedFileSystemInfoCollection = fileSystemEntryInfoOrdering switch
	    {
		    FileSystemEntryInfoOrdering.LastModificationTimeDescending => fileSystemInfoCollection
			    .OrderByDescending(aFileSystemInfo => aFileSystemInfo.LastWriteTimeUtc),
		    
		    _ => fileSystemInfoCollection
			    .OrderBy(aFileSystemInfo => aFileSystemInfo.Name, _globalParameters.NameComparer)
	    };

	    return orderedFileSystemInfoCollection;
    }
    
    #endregion
}
