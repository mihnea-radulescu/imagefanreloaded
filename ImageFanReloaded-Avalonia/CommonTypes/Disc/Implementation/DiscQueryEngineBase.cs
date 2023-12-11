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

	public IReadOnlyCollection<FileSystemEntryInfo> GetSpecialFoldersWithPaths()
	{
	    if (_specialFoldersWithPaths == null)
        {
			_specialFoldersWithPaths = SpecialFolders
									    .Select(aSpecialFolder =>
										    new FileSystemEntryInfo(
											    aSpecialFolder,
											    Path.Combine(UserProfilePath, aSpecialFolder),
											    GlobalData.FolderIcon))
									    .OrderBy(aSpecialFolder => aSpecialFolder.Name)
									    .ToArray();
		}

        return _specialFoldersWithPaths;
	}

	public IReadOnlyCollection<FileSystemEntryInfo> GetDrives()
    {
        if (_drives == null)
        {
			_drives = DriveInfo.GetDrives()
						.Select(aDriveInfo => aDriveInfo.Name)
						.Where(IsSupportedDrive)
						.Select(aDriveName =>
							new FileSystemEntryInfo(aDriveName,
													aDriveName,
													GlobalData.DriveIcon))
						.OrderBy(aDriveInfo => aDriveInfo.Name)
						.ToArray();
		}

        return _drives;
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
                                                GlobalData.FolderIcon))
                                        .OrderBy(aDirectory => aDirectory.Name)
                                        .ToArray();
        }
        catch
        {
            return EmptyFileSystemEntryInfoCollection;
        }
    }

    public IReadOnlyCollection<IImageFile> GetImageFiles(string folderPath)
    {
		IReadOnlyCollection<FileInfo> filesInformation;
        try
        {
            filesInformation = new DirectoryInfo(folderPath)
                                        .GetFiles("*", SearchOption.TopDirectoryOnly)
                                        .ToArray();
        }
        catch
        {
            return EmptyImageFileCollection;
        }

        var imageFiles = filesInformation
                            .Where(aFileInfo =>
                                   ImageFileExtensions.Contains(aFileInfo.Extension))
                            .OrderBy(aFileInfo => aFileInfo.Name)
                            .Select(aFileInfo => _imageFileFactory.GetImageFile(aFileInfo.FullName))
                            .OrderBy(aFileInfo => aFileInfo.FileName)
                            .ToArray();

        return imageFiles;
    }

	protected abstract bool IsSupportedDrive(string driveName);

	#region Private

    private static readonly IReadOnlyCollection<FileSystemEntryInfo> EmptyFileSystemEntryInfoCollection;
    private static readonly IReadOnlyCollection<IImageFile> EmptyImageFileCollection;

    private static readonly string UserProfilePath;

    private static readonly IReadOnlyCollection<string> SpecialFolders;

	private static readonly HashSet<string> ImageFileExtensions;

    private readonly IImageFileFactory _imageFileFactory;

	private IReadOnlyCollection<FileSystemEntryInfo> _specialFoldersWithPaths;
	private IReadOnlyCollection<FileSystemEntryInfo> _drives;

	#endregion
}
