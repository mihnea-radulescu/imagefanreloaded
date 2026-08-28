namespace ImageFanReloaded.Core.DiscAccess.EntryInfo;

public interface IFileSystemEntryInfoFactory
{
	IFileSystemEntryInfo GetDriveEntryInfo(string path, bool zipArchivesEnabled);
	IFileSystemEntryInfo GetFolderEntryInfo(
		IFileSystemEntryInfo? parent, string path, bool zipArchivesEnabled);
	IFileSystemEntryInfo GetZipArchiveEntryInfo(
		IFileSystemEntryInfo? parent, string path);
	IFileSystemEntryInfo GetZipArchiveFolderEntryInfo(
		IFileSystemEntryInfo? parent,
		IFileSystemEntryInfo parentArchive,
		string name,
		string path);

	IFileSystemEntryInfo GetHomeFolderEntryInfo(
		string path, bool zipArchivesEnabled);
	IFileSystemEntryInfo GetSpecialFolderEntryInfo(
		string name, string path, bool zipArchivesEnabled);
}
