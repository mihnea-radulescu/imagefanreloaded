using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImageFanReloaded.CommonTypes.ImageHandling;
using ImageFanReloaded.CommonTypes.Info;
using ImageFanReloaded.Factories;

namespace ImageFanReloaded.CommonTypes.Disc.Implementation;

public abstract class DiscQueryEngineBase
    : IDiscQueryEngine
{
	static DiscQueryEngineBase()
	{
		NameComparer = new NaturalSortingComparer();
		
		EmptyFileSystemEntryInfoCollection = Enumerable
			.Empty<FileSystemEntryInfo>()
			.ToArray();

		EmptyImageFileCollection = Enumerable
			.Empty<IImageFile>()
			.ToArray();

		UserProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

		SpecialFolders = new List<string>
		{
			"Desktop",
			"Documents",
			"Downloads",
			"Pictures"
		};

		ImageFileExtensions = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)
		{
			".bmp",
			".gif",
			".ico",
			".jpg", ".jpe", ".jpeg",
			".png",
			".tif", ".tiff",
            ".webp"
		};
	}

	protected DiscQueryEngineBase(IImageFileFactory imageFileFactory)
    {
        _imageFileFactory = imageFileFactory;
    }

	public IReadOnlyCollection<FileSystemEntryInfo> GetSpecialFolders()
	{
		try
		{
			return SpecialFolders
				.Select(aSpecialFolder =>
					new
					{
						Name = aSpecialFolder,
						Path = Path.Combine(UserProfilePath, aSpecialFolder)
					})
				.Select(aSpecialFolderWithPath =>
					new FileSystemEntryInfo(
						aSpecialFolderWithPath.Name,
						aSpecialFolderWithPath.Path,
						HasSubFolders(aSpecialFolderWithPath.Path),
						GlobalData.FolderIcon))
				.OrderBy(aSpecialFolderInfo => aSpecialFolderInfo.Name, NameComparer)
				.ToArray();
		}
		catch
		{
			return EmptyFileSystemEntryInfoCollection;
		}
	}

	public IReadOnlyCollection<FileSystemEntryInfo> GetDrives()
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
						GlobalData.DriveIcon))
				.OrderBy(aDriveInfo => aDriveInfo.Name, NameComparer)
				.ToArray();
		}
		catch
		{
			return EmptyFileSystemEntryInfoCollection;
		}
    }

    public IReadOnlyCollection<FileSystemEntryInfo> GetSubFolders(string folderPath)
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
                        GlobalData.FolderIcon))
                .OrderBy(aDirectory => aDirectory.Name, NameComparer)
                .ToArray();
        }
        catch
        {
            return EmptyFileSystemEntryInfoCollection;
        }
    }

    public IReadOnlyCollection<IImageFile> GetImageFiles(string folderPath)
    {
        try
        {
            var filesInformation = new DirectoryInfo(folderPath)
                .GetFiles("*", SearchOption.TopDirectoryOnly)
                .ToArray();

			var imageFiles = filesInformation
				.Where(aFileInfo =>
					   ImageFileExtensions.Contains(aFileInfo.Extension))
				.OrderBy(aFileInfo => aFileInfo.Name)
				.Select(aFileInfo => _imageFileFactory.GetImageFile(aFileInfo.FullName))
				.OrderBy(aFileInfo => aFileInfo.FileName, NameComparer)
				.ToArray();

			return imageFiles;
		}
        catch
        {
            return EmptyImageFileCollection;
        }
    }

	protected abstract bool IsSupportedDrive(string driveName);

	#region Private

	private static readonly IComparer<string> NameComparer;

    private static readonly IReadOnlyCollection<FileSystemEntryInfo> EmptyFileSystemEntryInfoCollection;
    private static readonly IReadOnlyCollection<IImageFile> EmptyImageFileCollection;

    private static readonly string UserProfilePath;
    private static readonly IReadOnlyCollection<string> SpecialFolders;

	private static readonly HashSet<string> ImageFileExtensions;

    private readonly IImageFileFactory _imageFileFactory;
    
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
