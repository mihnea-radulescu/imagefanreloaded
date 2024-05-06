using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImageFanReloaded.Core.Global;
using ImageFanReloaded.Core.ImageHandling;

namespace ImageFanReloaded.Core.DiscAccess.Implementation;

public abstract class DiscQueryEngineBase : IDiscQueryEngine
{
	static DiscQueryEngineBase()
	{
		EmptyFileSystemEntryInfoCollection = Enumerable
			.Empty<FileSystemEntryInfo>()
			.ToArray();

		EmptyImageFileCollection = Enumerable
			.Empty<IImageFile>()
			.ToArray();
	}

	protected DiscQueryEngineBase(
		IGlobalParameters globalParameters,
		IFileSizeEngine fileSizeEngine,
		IImageFileFactory imageFileFactory)
	{
		_globalParameters = globalParameters;
		_fileSizeEngine = fileSizeEngine;
		_imageFileFactory = imageFileFactory;
	}

	public async Task<IReadOnlyList<FileSystemEntryInfo>> GetRootFolders()
		=> await Task.Run(() =>
			{
				var userFolders = GetUserFolders();
				var drives = GetDrives();
			
				IReadOnlyList<FileSystemEntryInfo> rootFolders = [..userFolders, ..drives];
				return rootFolders;
			});

	public async Task<IReadOnlyList<FileSystemEntryInfo>> GetSubFolders(string folderPath)
		=> await Task.Run(() => GetSubFoldersInternal(folderPath));
	
	public async Task<IReadOnlyList<IImageFile>> GetImageFiles(string folderPath)
		=> await Task.Run(() => GetImageFilesInternal(folderPath));

	public async Task<FileSystemEntryInfo> GetFileSystemEntryInfo(string folderPath)
		=> await Task.Run(() =>
			{
				var folder = new DirectoryInfo(folderPath);
				var folderName = folder.Name;

				var hasSubFolders = HasSubFolders(folderPath);

				var fileSystemEntryInfo = new FileSystemEntryInfo(
					folderName, folderPath, hasSubFolders, _globalParameters.FolderIcon);

				return fileSystemEntryInfo;
			});
	
	#region Protected

	protected abstract bool IsSupportedDrive(string driveName);
	
	#endregion

	#region Private
	
    private static readonly IReadOnlyList<FileSystemEntryInfo> EmptyFileSystemEntryInfoCollection;
    private static readonly IReadOnlyList<IImageFile> EmptyImageFileCollection;

	private readonly IGlobalParameters _globalParameters;
	private readonly IFileSizeEngine _fileSizeEngine;
	private readonly IImageFileFactory _imageFileFactory;
	
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
						_globalParameters.FolderIcon))
				.OrderBy(aSpecialFolderInfo => aSpecialFolderInfo.Name, _globalParameters.NameComparer)
				.ToArray();

			IReadOnlyList<FileSystemEntryInfo> userFolders = [homeFolder, ..specialFolders];
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
				.ToArray();
		}
		catch
		{
			return EmptyFileSystemEntryInfoCollection;
		}
	}
	
	private IReadOnlyList<FileSystemEntryInfo> GetSubFoldersInternal(string folderPath)
	{
		try
		{
			return new DirectoryInfo(folderPath)
				.GetDirectories()
				.Select(aDirectory =>
					new FileSystemEntryInfo(
						aDirectory.Name,
						aDirectory.FullName,
						HasSubFolders(aDirectory.FullName),
						_globalParameters.FolderIcon))
				.OrderBy(aDirectory => aDirectory.Name, _globalParameters.NameComparer)
				.ToArray();
		}
		catch
		{
			return EmptyFileSystemEntryInfoCollection;
		}
	}
	
	private IReadOnlyList<IImageFile> GetImageFilesInternal(string folderPath)
	{
		try
		{
			var filesInformation = new DirectoryInfo(folderPath)
				.GetFiles("*", SearchOption.TopDirectoryOnly)
				.ToArray();

			var imageFiles = filesInformation
				.Where(aFileInfo =>
					_globalParameters.ImageFileExtensions.Contains(aFileInfo.Extension))
				.Select(aFileInfo => _imageFileFactory.GetImageFile(
					aFileInfo.Name,
					aFileInfo.FullName,
					_fileSizeEngine.ConvertToKilobytes(aFileInfo.Length)))
				.OrderBy(aFileInfo => aFileInfo.ImageFileName, _globalParameters.NameComparer)
				.ToArray();

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
		    
		    using (var subFoldersEnumerator = subFoldersAsEnumerable.GetEnumerator())
		    {
			    return subFoldersEnumerator.MoveNext();
		    }
	    }
	    catch
	    {
		    return false;
	    }
    }
    
    #endregion
}
