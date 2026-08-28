using System.Collections.Generic;
using System.IO;
using ImageFanReloaded.Core.DiscAccess.EntryInfo;
using ImageFanReloaded.Core.ImageCore;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.DiscAccess.Implementation.EntryInfo;

public class FileSystemEntryInfoFactory : IFileSystemEntryInfoFactory
{
	public FileSystemEntryInfoFactory(IGlobalParameters globalParameters)
	{
		_driveIcon = globalParameters.DriveIcon;
		_folderIcon = globalParameters.FolderIcon;
		_zipArchiveIcon = globalParameters.ZipArchiveIcon;

		_homeFolderIcon = globalParameters.HomeFolderIcon;

		_specialFolderToIconMapping = new Dictionary<string, IImage>
		{
			{ "Desktop", globalParameters.DesktopFolderIcon },
			{ "Documents", globalParameters.DocumentsFolderIcon },
			{ "Downloads", globalParameters.DownloadsFolderIcon },
			{ "Pictures", globalParameters.PicturesFolderIcon }
		};
	}

	public IFileSystemEntryInfo GetDriveEntryInfo(
		string path, bool zipArchivesEnabled)
		=> new DriveEntryInfo(null, path, path, zipArchivesEnabled, _driveIcon);

	public IFileSystemEntryInfo GetFolderEntryInfo(
		IFileSystemEntryInfo? parent, string path, bool zipArchivesEnabled)
		=> new FolderEntryInfo(
			parent,
			Path.GetFileName(path),
			path,
			zipArchivesEnabled,
			_folderIcon);

	public IFileSystemEntryInfo GetZipArchiveEntryInfo(
		IFileSystemEntryInfo? parent, string path)
		=> new ZipArchiveEntryInfo(
			parent, Path.GetFileName(path), path, _zipArchiveIcon);

	public IFileSystemEntryInfo GetZipArchiveFolderEntryInfo(
		IFileSystemEntryInfo? parent,
		IFileSystemEntryInfo parentArchive,
		string name,
		string path)
			=> new ZipArchiveFolderEntryInfo(
				parent, parentArchive, name, path, _zipArchiveIcon);

	public IFileSystemEntryInfo GetHomeFolderEntryInfo(
		string path, bool zipArchivesEnabled)
		=> new FolderEntryInfo(
			null, HomeFolderName, path, zipArchivesEnabled, _homeFolderIcon);

	public IFileSystemEntryInfo GetSpecialFolderEntryInfo(
		string name, string path, bool zipArchivesEnabled)
			=> new FolderEntryInfo(
				null, name, path, zipArchivesEnabled, GetIcon(name));

	private const string HomeFolderName = "Home";

	private readonly IImage _driveIcon;
	private readonly IImage _folderIcon;
	private readonly IImage _zipArchiveIcon;

	private readonly IImage _homeFolderIcon;

	private readonly IReadOnlyDictionary<string, IImage>
		_specialFolderToIconMapping;

	private IImage GetIcon(string aSpecialFolderName)
		=> _specialFolderToIconMapping[aSpecialFolderName];
}
